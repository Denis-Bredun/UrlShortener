using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using UrlShortener.API.Middlewares;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Validation;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Decorators;
using UrlShortener.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;

namespace UrlShortener.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureLogging();
            ConfigureServices(builder);
            ConfigureModelValidationErrorHandling(builder);

            var app = builder.Build();

            await SeedIdentityDataAsync(app);

            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UrlShortenerDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "UrlShortener API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevClient", builder =>
                {
                    builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateShortUrlDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateAboutDtoValidator>();

            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddTransient<IIdentitySeeder, IdentitySeeder>();
            builder.Services.Decorate<IIdentitySeeder, LoggingIdentitySeederDecorator>();

            builder.Services.AddScoped<IAboutInfoSeeder, AboutInfoSeeder>();
            builder.Services.Decorate<IAboutInfoSeeder, LoggingAboutInfoSeederDecorator>();

            builder.Services.AddScoped<IDemoUserSeeder, DemoUserSeeder>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.Decorate<IAuthService, LoggingAuthServiceDecorator>();

            builder.Services.AddScoped<IShortCodeGenerator, ShortCodeGenerator>();
            builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
            builder.Services.Decorate<IShortUrlService, LoggingShortUrlServiceDecorator>();

            builder.Services.AddScoped<IAboutService, AboutService>();
            builder.Services.Decorate<IAboutService, LoggingAboutServiceDecorator>();

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                });
            builder.Services.AddEndpointsApiExplorer();
        }

        private static void ConfigureModelValidationErrorHandling(WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(x => x.ErrorMessage)
                        });

                    return new BadRequestObjectResult(new { errors });
                };
            });

        }

        public static async Task SeedIdentityDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<UrlShortenerDbContext>();

            await dbContext.Database.EnsureDeletedAsync(); // you may want to comment this line so as not to create the DB and make migrations every time
            await dbContext.Database.MigrateAsync();

            const string adminEmail = "adminpanel@gmail.com";
            const string adminPassword = "sUpEr$ecret123";
            const string adminUsername = "Admin";

            var seeder = services.GetRequiredService<IIdentitySeeder>();
            await seeder.SeedRolesAsync();
            await seeder.SeedAdminUserAsync(adminEmail, adminPassword, adminUsername);

            var aboutSeeder = services.GetRequiredService<IAboutInfoSeeder>();
            await aboutSeeder.SeedAsync(adminEmail);

            var demoSeeder = services.GetRequiredService<IDemoUserSeeder>();
            await demoSeeder.SeedAsync();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<CustomExceptionMiddleware>();

            // We have CustomExceptionMiddleware, so General Exception Handler
            // is not necessary

            //app.UseExceptionHandler(errorApp =>
            //{
            //    errorApp.Run(async context =>
            //    {
            //        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            //        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");

            //        context.Response.StatusCode = 500;
            //        context.Response.ContentType = "application/json";

            //        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            //        if (exceptionHandlerFeature != null)
            //        {
            //            var exception = exceptionHandlerFeature.Error;

            //            logger.LogError(exception, "Unhandled exception occurred while processing the request");

            //            var response = new { error = exception.Message };
            //            await context.Response.WriteAsJsonAsync(response);
            //        }
            //    });
            //});

            app.UseHttpsRedirection();
            app.UseCors("AllowAngularDevClient");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
