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
        readonly IConfiguration _config;
        readonly UserManager<User> _user;
        readonly SymmetricSecurityKey _key;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly SafiContext _context;
        public TokenService(UserManager<User> user, IConfiguration config, IHttpContextAccessor httpContextAccessor, SafiContext context)
        {
            _config = config;
            _user = user;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<string> CreateJWT(User user)
        {
           var Role = (await _user.GetRolesAsync(user)).FirstOrDefault();
           var claims = new List<Claim>{
           new Claim(JwtRegisteredClaimNames.Sub,user.Id),
           new Claim(JwtRegisteredClaimNames.GivenName,user.Name),
           new Claim(JwtRegisteredClaimNames.Email,user.Email),
           new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
           new Claim(ClaimTypes.Role,Role),
          };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Audience = _config["JWT:Audience"],
                Issuer = _config["JWT:Issuer"],
                Expires = DateTime.Now.AddMinutes(double.Parse(_config["JWT:ExpireTime"]))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(TokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            var randomNumber = new byte[64];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddMonths(2),
                CreatedOn = DateTime.UtcNow,
                UserId = user.Id
            };
        }
        public async Task<ResponseOfLogin> GenerateToken(User user)
        {
            var ResponseOfLogin = new ResponseOfLogin();
            var token = await CreateJWT(user);
            
            // Populate common response fields
            ResponseOfLogin.Username = user.Name;
            ResponseOfLogin.IsAuthenticated = true;
            ResponseOfLogin.Id = user.Id;
            ResponseOfLogin.Email = user.Email;
            ResponseOfLogin.Token = token;
            ResponseOfLogin.Custom_Id = user.Custome_Id;
            ResponseOfLogin.Role = (await _user.GetRolesAsync(user)).FirstOrDefault();
            
            // Handle refresh token (reuse existing or create new)
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var existingRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                ResponseOfLogin.RefreshToken = existingRefreshToken.Token;
                ResponseOfLogin.RefreshTokenExpiration = existingRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = await GenerateRefreshToken(user);
                
                // Save refresh token to database
                user.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();
                
                ResponseOfLogin.RefreshToken = refreshToken.Token;
                ResponseOfLogin.RefreshTokenExpiration = refreshToken.ExpiresOn;
            }
            
            return ResponseOfLogin;
        }
        public async Task<bool> RevokeRefreshToken()
        {
            var Token = getRefreshTokenFromCookies();
            if (Token == null)
                return false;

            var user =  _user.Users.SingleOrDefault(u => u.RefreshTokens!.Any(t => t.Token == Token));
            if (user == null)
                return false;
            var refreshToken = user.RefreshTokens!.Single(t => t.Token == Token);
            if (!refreshToken.IsActive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await _user.UpdateAsync(user);
            return true;
        }
        public void SetRefreshTokeninCookies(string token, DateTime expires)
        {

            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = expires.ToLocalTime(),
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", token, cookieOption);
        }
        public string? getRefreshTokenFromCookies()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
            return refreshToken;
        }
        public async Task<string> generateNewJWTTokenFromRefreshToken()
        {
            var refreshToken = getRefreshTokenFromCookies();
            var refreshToken2 = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (refreshToken2 == null || !refreshToken2.IsActive)
                return null;
            var user = await _user.FindByIdAsync(refreshToken2.UserId);
            var newJwtToken = await CreateJWT(user);
            return newJwtToken;
        }
    }
}
