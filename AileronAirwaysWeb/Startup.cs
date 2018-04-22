using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using AileronAirwaysWeb.Extensions;

namespace AileronAirwaysWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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

            // Init middlewear that writes Exception message to Debug window.
            app.UseInitializeDatabaseMiddleware(Configuration, serviceProvider);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var env = serviceProvider.GetService<IHostingEnvironment>();

            // Init DB context.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            //options is for https connection allows temp data use
            services.AddMvc().AddSessionStateTempDataProvider();

            services.AddSession();

            // Add timeline service.
            services.AddSingleton<ITimelineService, TimelineService>((i) => new TimelineService(
                Configuration.GetValue<string>("BaseUrl"), 
                Configuration.GetValue<string>("AuthToken"), 
                Configuration.GetValue<string>("TenantId"),
                env.WebRootPath));

            // Add timeline repo
            services.AddTransient<TimelineRepository>();

            // Add service for handling flash temp messages.
            services.AddTransient<IFlashService, FlashService>();
        }
    }
}
