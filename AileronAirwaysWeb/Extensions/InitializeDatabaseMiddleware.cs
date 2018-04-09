using AileronAirwaysWeb.Models;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AileronAirwaysWeb.Extensions
{
    public static class InitializeDatabaseMiddleware
    {
        public static void UseInitializeDatabaseMiddleware(this IApplicationBuilder builder, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            if (configuration.GetValue<bool>("RefreshDatabase"))
            {
                serviceProvider.GetService<TimelineRepository>().Initialize();
            }
        }
    }
}
