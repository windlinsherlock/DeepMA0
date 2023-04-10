using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Data;

[assembly: HostingStartup(typeof(PACS.WebAPI.Areas.Identity.IdentityHostingStartup))]
namespace PACS.WebAPI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityContext>(options =>
                
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("IdentityContextConnection")));

                services.AddIdentity<AppUser, IdentityRole>(options =>
                 {
                     options.SignIn.RequireConfirmedAccount = true;

                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequireUppercase = false;
                     options.Password.RequireLowercase = false;
                     options.Password.RequireDigit = false;

                 }
                )
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<IdentityContext>();
                

               
            });
            
            
        }
    }
}