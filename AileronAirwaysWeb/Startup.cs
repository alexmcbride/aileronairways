using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AileronAirwaysWeb
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            //allows temp data use
            services.AddMvc().AddSessionStateTempDataProvider();

            //services.AddSession();

            // Add timeline service.
            services.AddTransient<ITimelineService, TimelineService>((i) => new TimelineService(Configuration.GetValue<string>("BaseUrl"), 
                Configuration.GetValue<string>("AuthToken"), 
                Configuration.GetValue<string>("TenantId")));


             // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            //allow session
            services.AddSession(options =>
            {
                // Set session a short timeout for easy testing.
                //options.IdleTimeout = TimeSpan.FromSeconds(10);

                //prefent session's client side script manipulation to prevent a cross-site scripting (XSS) 
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDebugExceptionMiddleware();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            //allows temp data and session
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Timelines}/{action=Index}/{id?}");
            });
        }
    }
}
