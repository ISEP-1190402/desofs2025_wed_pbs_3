using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Common.Interfaces;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Infrastructure.Services;
using LibraryOnlineRentalSystem.Repository.BookRepository;
using LibraryOnlineRentalSystem.Repository.Common;
using LibraryOnlineRentalSystem.Repository.RentalRepository;
using LibraryOnlineRentalSystem.Repository.UserRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace LibraryOnlineRentalSystem
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            // Create logger for startup
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<Startup>();

            _logger.LogInformation("Starting application in {Environment} environment", env.EnvironmentName);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => { loggingBuilder.AddConsole(); });

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = Environment.GetEnvironmentVariable("Telemetry_InstrumentationKey");
            });

            services.AddControllersWithViews();

            services.AddDbContext<LibraryDbContext>(opt =>
            {
                var serverVersion = ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("LibraryDatabase"));
                opt.UseMySql(
                    Environment.GetEnvironmentVariable("LibraryDatabase"),
                    serverVersion);

                opt.ReplaceService<IValueConverterSelector, StrongConverterOfIDValue>();
            });

            // Configure email options from appsettings and user secrets
            services.Configure<EmailOptions>(options =>
            {
                Configuration.GetSection("Email").Bind(options);

                // Override from environment variables if available
                var smtpUser = Configuration["EMAIL_USERNAME"];
                var smtpPass = Configuration["EMAIL_PASSWORD"];

                if (!string.IsNullOrEmpty(smtpUser)) options.SmtpUsername = smtpUser;
                if (!string.IsNullOrEmpty(smtpPass)) options.SmtpPassword = smtpPass;

                // Log the configuration (without password)
                _logger.LogInformation("Email Configuration - Server: {Server}:{Port}, From: {FromEmail}, DevEmail: {DevEmail}",
                    options.SmtpServer, options.SmtpPort, options.FromEmail, options.DevEmail);

                // Log if using environment variables
                if (!string.IsNullOrEmpty(smtpUser))
                {
                    _logger.LogInformation("Using SMTP username from environment variable");
                }
            });

            // Register email service
            services.AddScoped<IEmailService, DevelopmentEmailService>();

            ConfigureMyServices(services);
            ConfigureCors(services);

            // Clear default claim mappings
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            // Add Keycloak Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Environment.GetEnvironmentVariable("Keycloak__Authority");
                options.Audience = Environment.GetEnvironmentVariable("Keycloak__Audience");
                options.RequireHttpsMetadata = false; // Set to true in production

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "preferred_username",  // Map username from preferred_username claim
                    RoleClaimType = ClaimTypes.Role  // Will be mapped from realm_access.roles
                };

                // Handle Keycloak's claims mapping
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;

                        // Map roles from realm_access.roles
                        var realmAccessClaim = context.Principal.FindFirst("realm_access")?.Value;
                        if (!string.IsNullOrEmpty(realmAccessClaim))
                        {
                            try
                            {
                                var realmAccess = JsonDocument.Parse(realmAccessClaim);
                                if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement))
                                {
                                    foreach (var role in rolesElement.EnumerateArray())
                                    {
                                        var roleValue = role.GetString();
                                        if (!string.IsNullOrEmpty(roleValue))
                                        {
                                            identity?.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                context.Fail($"Failed to parse realm_access claim: {ex.Message}");
                                return Task.CompletedTask;
                            }
                        }

                        // Map email claim if present
                        var email = context.Principal.FindFirst("email")?.Value;
                        if (!string.IsNullOrEmpty(email))
                        {
                            identity?.AddClaim(new Claim(ClaimTypes.Email, email));
                        }

                        // Map name identifier (sub claim)
                        var sub = context.Principal.FindFirst("sub")?.Value;
                        if (!string.IsNullOrEmpty(sub) && !identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                        {
                            identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var errorMessage = $"Token validation failed: {context.Exception.Message}";
                        Console.WriteLine(errorMessage);
                        context.Response.Headers.Add("Token-Validation-Error", errorMessage);
                        return Task.CompletedTask;
                    }
                };
            });
            //services.UseAuthentication(); 
            services.AddAuthorization();
            Console.WriteLine(Environment.GetEnvironmentVariable("Keycloak__Username"));
            Console.WriteLine("there");
            services.AddHttpClient();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                options.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            // Register services
            services.AddTransient<IWorkUnity, WorkUnity>();
            services.AddTransient<BookService>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<RentalService>();
            services.AddTransient<IRentalRepository, RentalRepository>();

            // Register UserService with all its dependencies
            services.AddTransient<UserService>(sp =>
            {
                var userRepository = sp.GetRequiredService<IUserRepository>();
                var workUnit = sp.GetRequiredService<IWorkUnity>();
                var auditLogger = sp.GetRequiredService<IAuditLogger>();
                var httpClient = sp.GetRequiredService<HttpClient>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var emailService = sp.GetRequiredService<IEmailService>();
                var logger = sp.GetRequiredService<ILogger<UserService>>();

                return new UserService(
                    userRepository,
                    workUnit,
                    auditLogger,
                    httpClient,
                    configuration,
                    emailService,
                    logger);
            });

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuditLogger, AuditLogger>();
            services.AddTransient<AuthService>();
            services.AddHttpClient<AuthService>();

            // Configure ISBN validation with repository
            services.AddTransient(provider =>
            {
                var repository = provider.GetRequiredService<IBookRepository>();
                return new Action<string>(isbn =>
                {
                    var existingBook = repository.GetBookByIsbnAsync(isbn).Result;
                    if (existingBook != null)
                    {
                        throw new BusinessRulesException($"A book with ISBN {isbn} already exists.");
                    }
                });
            });

            // Configure ISBN validation
            services.AddTransient(provider =>
            {
                var validateAction = provider.GetRequiredService<Action<string>>();
                return new Action<IServiceProvider>(sp =>
                {
                    ISBN.Configure(validateAction);
                });
            });

            // Initialize ISBN validation
            services.BuildServiceProvider().GetService<Action<IServiceProvider>>()?.Invoke(services.BuildServiceProvider());
        }

        public void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("WWW-Authenticate");
                    });
            });
        }
    }
}