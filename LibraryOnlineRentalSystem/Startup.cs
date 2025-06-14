using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Rentals;
using LibraryOnlineRentalSystem.Domain.User;
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

namespace LibraryOnlineRentalSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => { loggingBuilder.AddConsole(); });

            /*services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = Configuration["Telemetry_InstrumentionKey"];
            });*/

            services.AddControllersWithViews();

            services.AddDbContext<LibraryDbContext>(opt =>
            {
                var serverVersion = ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("LibraryDatabase"));
                opt.UseMySql(
                    Environment.GetEnvironmentVariable("LibraryDatabase"),
                    serverVersion);
                
                opt.ReplaceService<IValueConverterSelector, StrongConverterOfIDValue>();
            });

            ConfigureMyServices(services);
            ConfigureCors(services);

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
                        RoleClaimType = "realm_access.roles"
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
            services.AddTransient<UserService>();
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