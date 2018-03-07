﻿using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AileronAirwaysWeb
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IMemoryCache _cache;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache)
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
                app.UseExceptionHandler("/Timelines/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            //allows temp data use
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Timelines}/{action=Index}/{id?}");
            });

            _env = env;
            _cache = cache;
        }

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

            services.AddSession();

            // We use this to cache API responses.
            services.AddMemoryCache();

            // Add timeline service.
            services.AddSingleton<ICachedTimelineService, CachedTimelineService>((i) => new CachedTimelineService(
                Configuration.GetValue<string>("BaseUrl"), 
                Configuration.GetValue<string>("AuthToken"), 
                Configuration.GetValue<string>("TenantId"),
                _env.WebRootPath,
                _cache));

            services.AddMvc();

            // Add service for handling flasg temp messages.
            services.AddTransient<IFlashService, FlashService>();
        }
    }
}
