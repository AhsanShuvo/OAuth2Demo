using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuth2Demo.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuth2Demo
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("defaultDB")));

            services.AddControllersWithViews();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    IConfigurationSection googleAuthSection = _config.GetSection("Authentication:Google");
                    googleOptions.ClientId = googleAuthSection["ClientId"];
                    googleOptions.ClientSecret = googleAuthSection["ClientSecret"];
                })
                .AddFacebook(facebookOptions =>
                {
                    IConfigurationSection fbAuthSection = _config.GetSection("Authentication:Facebook");
                    facebookOptions.AppId = fbAuthSection["AppId"];
                    facebookOptions.AppSecret = fbAuthSection["AppSecret"];
                })
                .AddGitHub(githubOptions => {
                    IConfigurationSection githubSection = _config.GetSection("Authentication:Github");
                    githubOptions.ClientId = githubSection["ClientId"];
                    githubOptions.ClientSecret = githubSection["ClientSecret"];
                    githubOptions.Scope.Add("user:email");
                    githubOptions.Events = new OAuthEvents
                    {
                        OnCreatingTicket = OnCreatingGitHubTicket()
                    };
                });
        }

        private static Func<OAuthCreatingTicketContext, Task> OnCreatingGitHubTicket()
        {
            return async context =>
            {
                var fullName = context.Identity.FindFirst("urn:github:name").Value;
                var email = context.Identity.FindFirst(ClaimTypes.Email).Value;
                //Todo: Add logic here to save info into database
                await Task.FromResult(true);
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
