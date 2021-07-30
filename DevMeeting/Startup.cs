using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevMeeting.Models.SettingModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using AutoMapper;
using DevMeeting.Data.Repositories;
using DevMeeting.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DevMeeting
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
            services.AddControllers();
            
            #region Database Settings

            services.Configure<DatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            
            #endregion

            #region Automapper

            services.AddAutoMapper(typeof(AutoMapperProfile));

            #endregion

            #region Repositories

            services.AddScoped<IMeetupsRepository, MeetupsRepository>();

            #endregion

            #region Logging

            services.AddLogging();

            #endregion

            #region OpenID Connect
            //TODO: Add secret and client id from config (secret from environment variables or a secure way)
            //TODO: Add Authority dynamically (if possible).
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:5002";
                    options.ClientId = "devmeetupclient";
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.UsePkce = false;
                    options.Scope.Add(OpenIdConnectScope.OpenId);
                    //TODO: check if openid profile is ok.
                    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                    options.SaveTokens = true;
                    options.ClientSecret = "thisisasecret";
                });

            #endregion
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "DevMeeting", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevMeeting v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            #region Authentication & Authorization

            app.UseAuthentication();

            app.UseAuthorization();
            
            #endregion

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}