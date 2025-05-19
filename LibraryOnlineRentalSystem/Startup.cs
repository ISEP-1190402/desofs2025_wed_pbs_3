using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Repository.BookRepository;
using LibraryOnlineRentalSystem.Repository.Common;
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
            services.AddDbContext<LibraryDbContext>(opt =>
                opt.UseMySql(
                    Configuration.GetConnectionString("LibraryDatabase"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("LibraryDatabase"))
                )
                .ReplaceService<IValueConverterSelector, StrongConverterOfIDValue>()
            );

            //ConfigureCors(services);
            ConfigureMyServices(services);

            // Add Keycloak Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["Keycloak:Authority"];
                options.Audience = Configuration["Keycloak:Audience"];
                options.RequireHttpsMetadata = false; // Set to true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddControllers().AddNewtonsoftJson();

            // Apenas Swashbuckle para Swagger
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

            // Swagger deve vir antes do endpoints
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
            services.AddTransient<IWorkUnity, WorkUnity>();
            services.AddTransient<BookService>();
            services.AddTransient<IBookRepository, BookRepository>();
            //User
            // User
            services.AddTransient<UserService>();
            services.AddTransient<IUserRepository, UserRepository>();

        }

        public void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }
    }
}
