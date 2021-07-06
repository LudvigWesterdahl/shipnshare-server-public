using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using ShipWithMeCore.ExternalServices;
using ShipWithMeInfrastructure;
using ShipWithMeInfrastructure.Models;
using ShipWithMeWeb.Authentication;
using ShipWithMeWeb.Extensions;
using ShipWithMeWeb.Helpers;

namespace ShipWithMe
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!WebHostEnvironment.HasValidEnvironmentName())
            {
                throw new ArgumentException($"Invalid environment: {WebHostEnvironment.EnvironmentName}");
            }

            if (WebHostEnvironment.IsDevelop())
            {
                Console.WriteLine($"CertificatePath={CertConfig.CertificatePath}");
                Console.WriteLine($"CertificatePassword={CertConfig.CertificatePassword}");
            }

            services.AddLogging(options =>
            {
                options.AddSimpleConsole(c =>
                {
                    c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                });
            });

            services.AddMailKit(options =>
            {
                var email = Configuration.GetSection("Email");
                options.UseMailKit(new MailKitOptions()
                {
                    // options from sercets.json
                    Server = email["Server"],
                    Port = Convert.ToInt32(email["Port"]),
                    SenderName = email["SenderName"],
                    SenderEmail = email["SenderEmail"],

                    // can be optional with no authentication 
                    Account = email["SenderEmail"],
                    Password = email["SenderPassword"],
                    // enable ssl or tls
                    Security = true
                });
            });

            services.AddInfrastructure(Configuration.GetConnectionString("Main"));

            services.AddCore();

            services.AddControllers();

            services.AddRazorPages();

            services.AddRouting(routeOptions =>
            {
                // Configures linkgenerator to return lowercase urls even though camelcase will match.
                routeOptions.LowercaseUrls = true;
                routeOptions.LowercaseQueryStrings = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShipWithMe", Version = "v1" });
            });

            services.AddScoped(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<AuthenticationHelper>>();
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<long>>>();
                var mainDbContext = sp.GetRequiredService<MainDbContext>();
                var emailService = sp.GetRequiredService<IEmailService>();

                return new AuthenticationHelper(
                    logger,
                    userManager,
                    roleManager,
                    mainDbContext,
                    emailService,
                    Configuration["Jwt:Secret"],
                    Configuration["Jwt:ValidIssuer"],
                    Configuration["Jwt:ValidAudience"],
                    int.Parse(Configuration["Jwt:ValidMinutes"]));
            });

            services.AddSingleton(sp =>
            {
                return new ServerInfo
                {
                    WwwRootPath = WebHostEnvironment.WebRootPath,
                    Hostname = Configuration["Hostname"]
                };
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:ValidAudience"],
                    ValidIssuer = Configuration["Jwt:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])),
                    ClockSkew = TimeSpan.FromMinutes(0)
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthenticationHelper.AdminRights, policy =>
                {
                    policy.RequireRole(
                        AuthenticationHelper.AdminRole);
                });

                options.AddPolicy(AuthenticationHelper.ModeratorRights, policy =>
                {
                    policy.RequireRole(
                        AuthenticationHelper.AdminRole,
                        AuthenticationHelper.ModeratorRole);
                });

                options.AddPolicy(AuthenticationHelper.CustomerRights, policy =>
                {
                    policy.RequireRole(
                        AuthenticationHelper.AdminRole,
                        AuthenticationHelper.ModeratorRole,
                        AuthenticationHelper.CustomerRole);
                });
            });

            services.AddScoped<IAuthorizationHandler, UserNotBlockedHandler>();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-Version"),
                    new QueryStringApiVersionReader("ver", "version"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<User> userManager,
            RoleManager<IdentityRole<long>> roleManager)
        {
            Console.WriteLine($"Running environment: {env.EnvironmentName}");

            if (env.IsDevelop())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShipWithMe v1"));
                AuthenticationHelper.SeedUserAndRoles(userManager, roleManager);
            }

            if (env.IsProduction())
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
                    db.Database.Migrate();
                    db.Database.EnsureCreated();
                }

                AuthenticationHelper.SeedAdminUser(
                    Configuration["AdminUser:UserName"],
                    Configuration["AdminUser:Email"],
                    Configuration["AdminUser:Password"],
                    userManager,
                    roleManager);
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            /*
            using (var scope = app.ApplicationServices.CreateScope()) {
                var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
                db.Database.Migrate();
            }
            */
        }
    }
}
