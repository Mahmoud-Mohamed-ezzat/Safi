using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Safi.Interfaces;
using Safi.Models;
using Safi.Repositories;
using Safi.Services;
using Safi.Hubs;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<SafiContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Defaultconn"));
}
);
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters =
        "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._@";
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 9;
})
.AddEntityFrameworkStores<SafiContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
    };
});
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IDepartment, DepartmentRepo>();
builder.Services.AddScoped<IReservation, ReservationRepo>();
builder.Services.AddScoped<IAvailableTimeOfDoctor, AvailableTimeOfDoctorRepo>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Only use HTTPS redirection in production or when HTTPS is available
if (!app.Environment.IsDevelopment() || builder.Configuration.GetValue<string>("ASPNETCORE_URLS")?.Contains("https") == true)
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ReservationHub>("/reservationHub");
app.Run();