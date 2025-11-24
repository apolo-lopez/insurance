using Evaluation.API.Mapping;
using Evaluation.API.Middleware;
using Evaluation.API.Seed;
using Evaluation.Application.Interfaces;
using Evaluation.Application.Services;
using Evaluation.Infrastructure.Data;
using Evaluation.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// Logging (Serilog)
// ------------------------------------------------------
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console();
});

// ------------------------------------------------------
// Configuration
// ------------------------------------------------------
var configuration = builder.Configuration;

// ------------------------------------------------------
// Add Infrastructure (DbContext + Repositories + DI)
// ------------------------------------------------------
builder.Services.AddInfrastructure(configuration);

// ------------------------------------------------------
// Add Identity
// ------------------------------------------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ------------------------------------------------------
// JWT Authentication
// ------------------------------------------------------
var jwtSection = configuration.GetSection("JwtSettings");
var jwtKey = jwtSection.GetValue<string>("SecretKey")
    ?? throw new InvalidOperationException("JWT SecretKey is missing");
var jwtIssuer = jwtSection.GetValue<string?>("Issuer");
var jwtAudience = jwtSection.GetValue<string?>("Audience");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true
    };
});

// ------------------------------------------------------
// AutoMapper
// ------------------------------------------------------
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

// ------------------------------------------------------
// Controllers
// ------------------------------------------------------
builder.Services.AddControllers();

// ------------------------------------------------------
// Swagger + JWT support
// ------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Insurance API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese su token JWT así: Bearer {token}"
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, new string[] { } }
    });
});

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:5070", "http://82.197.94.78:5070")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// ------------------------------------------------------
// Build application
// ------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------
// Middlewares
// ------------------------------------------------------
app.UseSerilogRequestLogging();

// Middleware de excepciones personalizadas
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularDev");
// Important: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ------------------------------------------------------
// Seed inicial (Roles + Admin)
// ------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    if(context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate(); 
    }

    try
    {
        await IdentitySeed.SeedAsync(services, configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar roles y usuario admin.");
    }
}

app.Run();
