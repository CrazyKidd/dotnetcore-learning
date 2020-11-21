using System;
using Microsoft.AspNetCore.Http;
using NetNote.Models;
using System.Threading.Tasks;
using System.Text;
using System.Net;

namespace NetNote.Middleware
{
    public class BasicMiddleware
    {
        private readonly RequestDelegate _next;
        public const string AuthorizationHeader = "Authorization";

        public const string WWWAuthenticateHeader = "WWW-Authenticate";
        private BasicUser _user;

        public BasicMiddleware(RequestDelegate next, BasicUser user)
        {
            _next = next;
            _user = user;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var Request = httpContext.Request;
            string auth = Request.Headers[AuthorizationHeader];
            if (auth == null)
            {
                return BasicResult(httpContext);
            }
            string[] authParts = auth.Split(' ');
            if (authParts.Length != 2)
                return BasicResult(httpContext);
            string base64 = authParts[1];
            string authValue;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                authValue = Encoding.ASCII.GetString(bytes);
            }
            catch
            {
                authValue = null;
            }
            if (string.IsNullOrEmpty(authValue))
                return BasicResult(httpContext);

            string username;
            string password;
            int sepIndex = authValue.IndexOf(':');
            if (sepIndex == -1)
            {
                username = authValue;
                password = string.Empty;
            }
            else
            {
                username = authValue.Substring(0, sepIndex);
                password = authValue.Substring(sepIndex + 1);
            }
            if (_user.UserName.Equals(username) && _user.Password.Equals(password))
                return _next(httpContext);
            else
                return BasicResult(httpContext);
        }

        private Task BasicResult(HttpContext httpContext)
        {
            //throw new NotImplementedException();
            var host = httpContext.Request.Host;
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpContext.Response.Headers.Add(WWWAuthenticateHeader, string.Format("Basic realm=\"{0}\"", host));
            return Task.FromResult(httpContext);
        }
    }
}
