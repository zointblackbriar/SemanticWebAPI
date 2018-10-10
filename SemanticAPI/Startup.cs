using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SemanticAPI.Helpers;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using SemanticAPI.Models.DataSet;
using SemanticAPI.Models.OPCUADataType;
using SemanticAPI.Models.AuthCredentials;
using SemanticAPI.Models.Options;
using SemanticAPI.MVC;
using SemanticAPI.OPCUAModel;
//using SemanticAPI.Auth;
using SemanticAPI.Services;

namespace SemanticAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("JwtOptions");
            services.Configure<AppSettings>(appSettingsSection);
            //Add service related to IOptions feature in Controllers



            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(options =>
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
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = Configuration["JwtOptions:Issuer"],
                    //ValidAudience = Configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["JwtOptions:SecurityKey"]))
                };
            });

            services.AddOptions();
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<ServerOptions>(Configuration.GetSection("ServerOptions"));

            //Register server specific for the platform
            //services.AddTransient<ITokenManager, JwtManager>();
            //Register server specific for the platform
            services.AddScoped<IUserService, UserService>();
            //Register a singleton service managing OPC UA interactions
            services.AddSingleton<IOPCUAClientSingleton, OPCUAClient>();

            //First two important
            services.AddCors();
            services.AddMvc();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            //app.CheckToken();

            app.UseMvc();
        }
    }
}
