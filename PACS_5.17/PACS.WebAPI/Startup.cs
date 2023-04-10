using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Data;
using PACS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.WebAPI
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Input the JWT like: Bearer {your token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                
            });

            

            // JWT
            var jwtSettings = Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecurityKey)),
                ClockSkew = TimeSpan.Zero,
            };
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; });


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICloudService, CloudService>();
            services.AddScoped<IAutoLableService, AutoLableService>();
            
            services.AddScoped<IAutoDiagnoseService, AutoDiagnoseService>();

            // 邮箱
            services.AddTransient<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region 设置数据库种子
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdentityContext>();
                context.Database.EnsureCreated();

                var _userManager =
                         serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
                var _roleManager =
                         serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (!context.Users.Any(usr => usr.UserName == "admin@test"))
                {
                    var user1 = new AppUser()
                    {
                        UserName = "admin@test",
                    };

                    var user2 = new AppUser()
                    {
                        UserName = "user1@test",
                    };

                    _userManager.CreateAsync(user1, "000000");
                    _userManager.CreateAsync(user2, "string");
                }

                if (!_roleManager.RoleExistsAsync("Admin").Result)
                {
                    _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                }

                var adminUser = _userManager.FindByNameAsync("admin@test").Result;
                 _userManager.AddToRolesAsync(adminUser, new string[] { "Admin" });
                adminUser.EmailConfirmed = true;

                var User2 = _userManager.FindByNameAsync("user1@test").Result;
                User2.EmailConfirmed = true;

                context.SaveChanges();
            }
            #endregion
        }
    }
}
