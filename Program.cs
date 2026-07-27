using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ubuntu_health_api.Repositories;
using ubuntu_health_api.Services;
using ubuntu_health_api.Data;
using ubuntu_health_api.Models;
using Microsoft.OpenApi.Models;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Middleware;
using System.Security.Claims;
using DotNetEnv;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["JWT:Secret"];
var issuer = builder.Configuration["JWT:ValidIssuer"];

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  options.Password.RequiredLength = 12;
  options.Password.RequireDigit = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequiredUniqueChars = 4;
  options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ??
          throw new InvalidOperationException("JWT:Secret is not configured")))
  };

  options.Events = new JwtBearerEvents
  {
    OnMessageReceived = context =>
    {
      if (!context.Request.Headers.ContainsKey("Authorization"))
      {
        context.Token = context.Request.Cookies[AuthCookie.Name];
      }

      return Task.CompletedTask;
    },

    OnTokenValidated = async context =>
    {
      var userManager = context.HttpContext.RequestServices
        .GetRequiredService<UserManager<ApplicationUser>>();

      var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var user = string.IsNullOrEmpty(userId) ? null : await userManager.FindByIdAsync(userId);

      if (user == null || !user.IsActive)
      {
        context.Fail("This account is no longer active");
        return;
      }

      if (!string.IsNullOrWhiteSpace(user.LicenseNumber))
      {
        context.Principal?.AddIdentity(new ClaimsIdentity(
          [new Claim(CurrentUser.LicenseNumberClaim, user.LicenseNumber)]));
      }
    }
  };
});
builder.Services.AddAuthorization();
// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Ubuntu Health API",
    Version = "v1"
  });
});
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured");

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowConfiguredOrigins",
    policy =>
    {
      policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IClinicalNoteService, ClinicalNoteService>();
builder.Services.AddScoped<IClinicalNoteRepository, ClinicalNoteRepository>();
builder.Services.AddScoped<IPracticeRepository, PracticeRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await db.Database.MigrateAsync();

  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  var roles = new[] { "admin", "doctor", "nurse", "receptionist" };

  foreach (var role in roles)
  {
    if (!await roleManager.RoleExistsAsync(role))
    {
      await roleManager.CreateAsync(new IdentityRole(role));
    }
  }
}

app.UseHttpsRedirection();
// Enable Swagger middleware
app.UseSwagger();
// Enable Swagger UI middleware
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ubuntu Health API v1"));
app.UseCors("AllowConfiguredOrigins");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();