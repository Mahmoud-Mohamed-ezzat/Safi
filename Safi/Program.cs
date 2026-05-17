using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Safi.Configuration;
using Safi.Helpers;
using Safi.Hubs;
using System.ComponentModel;
using Safi.Interfaces;
using Safi.Models;
using Safi.Repositories;
using Safi.Services;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Safi.Services.AIService;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.IgnoreObsoleteActions();
    option.IgnoreObsoleteProperties();
    option.CustomSchemaIds(type => type.FullName);
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
// Add services to the container.
builder.Services.AddDbContext<SafiContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Defaultconn"));
}
);
// Add Email service to the container.
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters =
        "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 9;
})
.AddEntityFrameworkStores<SafiContext>()
.AddDefaultTokenProviders();


// 2. (Optional) Configure external cookie
builder.Services.ConfigureExternalCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultSignOutScheme = IdentityConstants.ExternalScheme;

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
}).AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:client_id"];
    options.ClientSecret = builder.Configuration["Google:client_secret"];
    options.SaveTokens = true;
    options.SignInScheme = IdentityConstants.ExternalScheme;

    options.UsePkce = true;

    // Change this to a DIFFERENT path from your controller endpoint
    //options.CallbackPath = "/signin-google"; // ? Different from your controller route

    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
});
//builder.Services.AddRateLimiter(options =>
//{
//    // Global Limiter (Limit requests by IP)
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    {
//        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

//        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
//        {
//            PermitLimit = 5,                         // Allow 5 requests
//            Window = TimeSpan.FromSeconds(10),       // Every 10 seconds
//            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//            QueueLimit = 0
//        });
//    });

//    // Login Policy (Limit by IP)
//    options.AddPolicy("LoginLimit", httpContext =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 3,                    // Allow 3 attempts
//                Window = TimeSpan.FromMinutes(3),   // Every 3 minutes
//                QueueLimit = 0,
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
//            }));

//    // Register Policy (Limit by IP)
//    options.AddPolicy("RegisterLimit", httpContext =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 3,                    // Allow 3 attempts
//                Window = TimeSpan.FromMinutes(1),   // Every 1 minute
//                QueueLimit = 0,
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
//            }));

//    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
//});
builder.Services.AddSignalR();

// Allow DateOnly to accept "MM/dd/yyyy" format from form data
TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));

builder.Services.AddControllers();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<HeartDiseasModel>();
builder.Services.AddTransient<LiverModel>();
builder.Services.AddScoped<MedicineModelService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PriceSegmentCalculator>();
builder.Services.AddScoped<IDepartment, DepartmentRepo>();
builder.Services.AddScoped<IPrices,PriceRepo>();
builder.Services.AddScoped<IReservation, ReservationRepo>();
builder.Services.AddScoped<IAvailableTimeOfDoctor, AvailableTimeOfDoctorRepo>();
builder.Services.AddScoped<IReportDoctorToPatient, ReportDoctorToPatientRepo>();
builder.Services.AddScoped<IRoom, RoomRepo>();
builder.Services.AddScoped<IICU, ICURepo>();
builder.Services.AddScoped<IEmergency, EmergencyRepo>();
builder.Services.AddScoped<IAssignWorks, AssignWorksRepo>();
builder.Services.AddScoped<IAppointmentToRoom, AppointmentToRoomRepo>();
builder.Services.AddScoped<IShift, ShiftRepo>();
builder.Services.AddScoped<IStatisticsRepo, StatisticsRepo>();
builder.Services.AddScoped<IEmailService, EmailRepository>();
builder.Services.AddScoped<IDoctor, DoctorRepo>();
builder.Services.AddScoped<IAnalysis, AnalysisRepo>();
builder.Services.AddScoped<IAccount, AccountRepo>();
builder.Services.AddScoped<IAttendance, AttendanceRepo>();
builder.Services.AddScoped<IBill, BillRepo>();
builder.Services.AddHostedService<BillBackgroundService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ReactPolicy");
// Only use HTTPS redirection in production or when HTTPS is available

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3001");
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800"); // Cache images 7 days
    }
});
//app.Use(async (context, next) =>
//{
//    var path = context.Request.Path.Value;

//    if (path != null && path.Contains("google"))
//    {
//        Console.WriteLine($"\n=== {context.Request.Method} {path} ===");
//        Console.WriteLine($"Has state query: {context.Request.Query.ContainsKey("state")}");
//        Console.WriteLine($"Has code query: {context.Request.Query.ContainsKey("code")}");
//        Console.WriteLine("Cookies present:");
//        foreach (var cookie in context.Request.Cookies)
//        {
//            Console.WriteLine($"  - {cookie.Key}");
//        }
//        Console.WriteLine("===================\n");
//    }

//    await next();
//});

//app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ReservationHub>("/reservationHub");
app.MapHub<AppointmentHub>("/appointmentHub");
app.MapHub<MessageHub>("/messageHub");
app.Run();