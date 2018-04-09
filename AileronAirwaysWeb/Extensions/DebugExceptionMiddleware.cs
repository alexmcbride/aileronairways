using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AileronAirwaysWeb
{
    /// <summary>
    /// Middlewear to log exceptions to the Debug output window.
    /// </summary>
    public class DebugExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public DebugExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log exception message to Debug output window.
                Debug.WriteLine("---- START EXCEPTION ----");
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine("---- END EXCEPTION ----");

                throw;
            }
        }
    }

    public static class DebugExceptionMiddlewareExtensionMethod
    {
        public static void UseDebugExceptionMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<DebugExceptionMiddleware>();
        }
    }
}
