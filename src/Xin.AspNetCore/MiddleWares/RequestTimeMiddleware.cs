using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Xin.AspNetCore.MiddleWares
{
    public class RequestTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var time = DateTime.Now;
            try
            {
                await _next(context);
            }
            finally
            {
                var r = (DateTime.Now - time).TotalMilliseconds;
                Console.WriteLine($"Url: {context.Request.Path} - {r} ms");
            }
        }
    }


    public static class RequestTimeMiddleWareExtensions
    {
        public static IApplicationBuilder UseRequestTime(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestTimeMiddleware>();
        }
    }
}
