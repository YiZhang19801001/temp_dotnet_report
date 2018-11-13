using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using demoBusinessReport.Helpers;
using demoBusinessReport.Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using demoBusinessReport.Entities;

namespace demoBusinessReport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddDbContext<MyDbContext>(x => x.UseInMemoryDatabase("TestDb"));
            services.AddDbContext<MyDbContext>();
            services.AddDbContext<ShopDbContext>();
            services.AddMvc();
            services.AddAutoMapper();

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IDataService<Shop>, DataService<Shop>>();
            services.AddScoped<IDataService<UserShop>, DataService<UserShop>>();
            services.AddScoped<IDataService<UserShop>, DataService<UserShop>>();
            services.AddScoped<IDataService<Docket>, ShopDataService<Docket>>();
            services.AddScoped<IDataService<Return>, ShopDataService<Return>>();
            services.AddScoped<IDataService<ReturnsLine>, ShopDataService<ReturnsLine>>();
            services.AddScoped<IDataService<DocketLine>, ShopDataService<DocketLine>>();
            services.AddScoped<IDataService<Stock>, ShopDataService<Stock>>();
            services.AddScoped<IDataService<Audit>, ShopDataService<Audit>>();
            services.AddScoped<IDataService<Staff>, ShopDataService<Staff>>();
            services.AddScoped<IDataService<SalesOrder>, ShopDataService<SalesOrder>>();
            services.AddScoped<IDataService<Payments>, ShopDataService<Payments>>();


            //add asp.net identity framework
            //add identity services
            services.AddIdentity<IdentityUser, IdentityRole>
                (
                   config =>
                   {
                       config.Password.RequireDigit = true;
                       config.Password.RequiredLength = 6;
                       config.Password.RequireNonAlphanumeric = false;
                       config.Password.RequireUppercase = false;
                   }
                ).AddEntityFrameworkStores<MyDbContext>();
            services.AddDbContext<MyDbContext>();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = context.Principal.Identity.Name;
                        var user = userService.GetById(userId);

                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseMvc();

            app.UseMvcWithDefaultRoute();

            //call seed method
            //SeedHelper.Seed(app.ApplicationServices).Wait();
        }
    }
}
