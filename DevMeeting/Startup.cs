using System;
using System.Text;
using AspNetCore.Identity.Mongo;
using DevMeeting.Models.SettingModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using DevMeeting.Data.Entities.User;
using DevMeeting.Data.Repositories;
using DevMeeting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            
            #endregion

            #region Logging

            services.AddLogging();

            #endregion

            #region Mongo Identity
            
            services.AddIdentityMongoDbProvider<User, UserRole, string>(identity =>
            {
                identity.SignIn.RequireConfirmedEmail = true;
                identity.Password.RequireDigit = true;
                identity.Password.RequireLowercase = true;
                identity.Password.RequireNonAlphanumeric = true;
                identity.Password.RequireUppercase = true;
                identity.Password.RequiredLength = 8;
                identity.Password.RequiredUniqueChars = 1;

                identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                identity.Lockout.MaxFailedAccessAttempts = 5;
                identity.Lockout.AllowedForNewUsers = true;

                identity.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                identity.User.RequireUniqueEmail = true;
            }, mongo =>
            {
                mongo.ConnectionString = Configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()
                    .ConnectionString;
            });
            
            #endregion

            #region Authentication and Authorization

            var jwtSection = Configuration.GetSection(nameof(JwtBearerTokenSettings));
            var jwtBearerSettings = jwtSection.Get<JwtBearerTokenSettings>();
            var key = Encoding.ASCII.GetBytes(jwtBearerSettings.Key);
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtBearerSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtBearerSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/SignIn";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
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

            // After UseRouting, so that route information is available for authentication decisions.
            // Before UseEndpoints, so that users are authenticated before accessing the endpoints.
            // Resource: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-2.2
            
            app.UseAuthentication();

            app.UseAuthorization();
            
            #endregion

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}