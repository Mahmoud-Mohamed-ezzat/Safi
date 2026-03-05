using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Safi.Dto.Account;
using Safi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Safi.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _user;
        private readonly SymmetricSecurityKey _key;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SafiContext _context;

        public TokenService(
            UserManager<User> user,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            SafiContext context)
        {
            _config = config;
            _user = user;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }       
        /// Generates a full login response: JWT + rotated refresh token.
        /// Revokes all currently-active refresh tokens before issuing a new one.
        /// Preserves the original absolute-expiration so the 12-month hard limit is never reset.
  
        public async Task<ResponseOfLogin> GenerateToken(User user)
        {
            // Ensure RefreshTokens collection is loaded
            if (!_context.Entry(user).Collection(u => u.RefreshTokens).IsLoaded)
                await _context.Entry(user).Collection(u => u.RefreshTokens).LoadAsync();

            var jwtToken = await CreateJWT(user);

            var response = new ResponseOfLogin
            {
                Username = user.Name,
                IsAuthenticated = true,
                Id = user.Id,
                Email = user.Email,
                Token = jwtToken,
                Custom_Id = user.Custome_Id,
                Role = (await _user.GetRolesAsync(user)).FirstOrDefault()
            };

            // Preserve AbsoluteExpiration from the very first login (oldest token).
            var oldestAbsoluteExpiration = user.RefreshTokens
                .OrderBy(t => t.CreatedOn)
                .Select(t => (DateTime?)t.AbsoluteExpiration)
                .FirstOrDefault();

            // Revoke all currently active refresh tokens.
            foreach (var active in user.RefreshTokens.Where(t => t.IsActive))
                active.RevokedOn = DateTime.UtcNow;

            // Remove stale revoked tokens older than 30 days to keep the table clean.
            var staleTokens = user.RefreshTokens
                .Where(t => !t.IsActive && t.ExpiresOn < DateTime.UtcNow.AddDays(-30))
                .ToList();

            foreach (var stale in staleTokens)
                user.RefreshTokens.Remove(stale);

            // Issue a fresh refresh token.
            var newRefreshToken = GenerateRefreshToken(user.Id);

            // Keep the original absolute expiration if it is still in the future.
            if (oldestAbsoluteExpiration.HasValue && oldestAbsoluteExpiration.Value > DateTime.UtcNow)
                newRefreshToken.AbsoluteExpiration = oldestAbsoluteExpiration.Value;

            user.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return response;
        }

     
        public async Task<ResponseOfLogin?> RotateBothTokensAsync() // rotate both tokens
        {
            var refreshTokenValue = getRefreshTokenFromCookies();
            if (string.IsNullOrEmpty(refreshTokenValue)) return null;

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshTokenValue);

            if (refreshToken == null || !refreshToken.IsActive) return null;

            // Absolute expiration reached → force re-login.
            if (DateTime.UtcNow >= refreshToken.AbsoluteExpiration)
            {
                refreshToken.RevokedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return null;
            }

            // Load user with all refresh tokens so GenerateToken can
            // correctly revoke active tokens and inherit AbsoluteExpiration.
            var user = await _user.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);

            if (user == null) return null;

            return await GenerateToken(user);
        }
        public async Task<bool> RevokeRefreshToken()// Revokes the refresh token found in the request cookie.
        {
            var tokenValue = getRefreshTokenFromCookies();
            if (string.IsNullOrEmpty(tokenValue)) return false;

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == tokenValue);

            if (refreshToken == null || !refreshToken.IsActive) return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        // Appends the refresh token as a secure HttpOnly cookie to the response. 
        public void SetRefreshTokenInCookies(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Expires = expires.ToLocalTime()
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        //  PRIVATE Functions(
        // CreateJWT=>Create JWT Token
        // ,GenerateRefreshToken=>Generate Refresh Token
        // ,getRefreshTokenFromCookies=>Get Refresh Token From Cookies)
        private async Task<string> CreateJWT(User user)
        {
            var role = (await _user.GetRolesAsync(user)).FirstOrDefault();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,       user.Id    ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Name  ?? "User"),
                new Claim(JwtRegisteredClaimNames.Email,     user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti,       Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,                   role       ?? "Patient")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            if (!double.TryParse(_config["JWT:ExpireTime"], out var expireMinutes))
                expireMinutes = 15; // safe fallback

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Audience = _config["JWT:Audience"],
                Issuer = _config["JWT:Issuer"],
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private RefreshToken GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                AbsoluteExpiration = DateTime.UtcNow.AddMonths(12),
                CreatedOn = DateTime.UtcNow,
                UserId = userId
            };
        }
        private string? getRefreshTokenFromCookies()
            => _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
    }
}