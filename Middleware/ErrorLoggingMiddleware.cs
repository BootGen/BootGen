using System;
using System.Threading.Tasks;
using Editor.Services;
using Microsoft.AspNetCore.Http;

namespace Editor
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IErrorService errorService)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                errorService.LogException(e);
                throw;
            }
        }
    }
}