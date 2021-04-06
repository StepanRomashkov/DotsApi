using AutoMapper;
using DotsApi.Authorization;
using DotsApi.Helpers;
using DotsApi.Models;
using DotsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace DotsApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(key);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = symmetricSecurityKey,
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            /*            services.AddAuthentication(x =>
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
                                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                                        var userId = context.Principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                                        var user = userService.GetUserById(userId);
                                        if (user == null)
                                        {
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
                            });*/

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.Configure<DotsDatabaseSettings>(options => 
                {
                    options.ConnectionString = _configuration.GetSection("MongoConnection:ConnectionString").Value;
                    options.Database = _configuration.GetSection("MongoConnection:Database").Value;
                }
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoticesRepository, NoticesRepository>();
            services.AddScoped<IDotsSecurityTokenHandler, DotsSecurityTokenHandler>();
            services.AddScoped<IAuthorizationHandler, UserUDSelfAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
