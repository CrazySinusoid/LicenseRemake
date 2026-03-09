using LicenseRemake.Application.Interfaces;
using LicenseRemake.Application.Services;
using LicenseRemake.External;
using LicenseRemake.Infrastructure;
using LicenseRemake.Infrastructure.Configurations;
using LicenseRemake.Infrastructure.Filters;
using LicenseRemake.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// настройка jwt токена
ConfigureJwtAuthService(builder.Services, builder.Configuration);

// авторизация
builder.Services.AddAuthorization();

// регистрация сервисов
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IKassaService, MockKassaService>();
builder.Services.AddScoped<ILicensingService, LicensingService>();
builder.Services.AddScoped<ILicenseLogService, LicenseLogService>();

builder.Services.AddTransient<IDbTableRegistration, AppUserConfiguration>();
builder.Services.AddTransient<IDbTableRegistration, CashRegisterConfiguration>();
builder.Services.AddTransient<IDbTableRegistration, LicenseLogConfiguration>();
builder.Services.AddTransient<IDbTableRegistration, RefreshTokenConfiguration>();
builder.Services.AddTransient<IDbTableRegistration, RequestLogConfiguration>();

// регистрация фильтров
builder.Services.AddScoped<ApiExceptionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApiExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LicenseRemake API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// реквест логи что б видеть кто что делает и куда стучит
app.UseRequestLogging();

app.MapControllers();

// автоматическая миграция базы данных. что б каждый раз не писать руками database update
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataDbContext>();
    db.Database.Migrate();

    // дата сидер
    await DataSeeder.SeedAsync(db);
}

app.Run();

void ConfigureJwtAuthService(IServiceCollection services, IConfiguration configuration)
{
    var audienceConfig = configuration.GetSection("Audience");
    var secret = audienceConfig["Secret"]!;
    var keyByteArray = Encoding.ASCII.GetBytes(secret);
    var signingKey = new SymmetricSecurityKey(keyByteArray);

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidIssuer = audienceConfig["Issuer"],
        ValidAudience = audienceConfig["Audience"],
        ClockSkew = TimeSpan.Zero
    };

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = tokenValidationParameters;
    });
}
