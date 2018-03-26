using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AileronAirwaysWeb
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            ////to redirect connection to https
            //var options = new RewriteOptions()
            //.AddRedirectToHttps(StatusCodes.Status301MovedPermanently, 44346);

            //app.UseRewriter(options);
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

            //options is for https connection allows temp data use
            services.AddMvc(
                //options =>
                //{
                //    options.SslPort = 44346;
                //    options.Filters.Add(new RequireHttpsAttribute());
                //}
            ).AddSessionStateTempDataProvider();
            //for https connection
            //services.AddAntiforgery(
            // options =>
            //    {
            //    options.Cookie.Name = "_af";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //    options.HeaderName = "X-XSRF-TOKEN";
            //    }
            //);

            services.AddSession();

            // Add timeline service.
            services.AddSingleton<ITimelineService, TimelineService>((i) => new TimelineService(
                Configuration.GetValue<string>("BaseUrl"), 
                Configuration.GetValue<string>("AuthToken"), 
                Configuration.GetValue<string>("TenantId"),
                _env.WebRootPath));

            services.AddTransient<TimelineRepository>();

            // Add service for handling flash temp messages.
            services.AddTransient<IFlashService, FlashService>();
        }
    }
}
