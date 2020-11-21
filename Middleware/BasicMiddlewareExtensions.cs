using System;
using Microsoft.AspNetCore.Builder;
using NetNote.Models;

namespace NetNote.Middleware
{
    public static class BasicMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicMiddleware(this IApplicationBuilder builder, BasicUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("需设置Basic用户");
            }
            return builder.UseMiddleware<BasicMiddleware>(user);
        }
    }
}
