//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SemanticAPI.Auth
//{
//    public class TokenMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public TokenMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public Task InvokeAsync(HttpContext context, ITokenManager tokenManager)
//        {
//            if (context.Request.Path.StartsWithSegments("/api"))
//            {
//                string authorization = context.Request.Headers["Authorization"];
//                authorization = (String.IsNullOrEmpty(authorization)) ? "Bearer " : authorization;
//                string token = "empty";

//                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//                {
//                    token = authorization.Substring("Bearer ".Length).Trim();
//                    try
//                    {
//                        if(tokenManager.isRefreshable(token))
//                        {
//                            token = tokenManager.GenerateTokenForUser(token);
//                            //todo: remove this print line
//                            Console.WriteLine("Token Refreshed");

//                        }
//                    }
//                    catch (Exception)
//                    {
//                        //todo: remove this print line
//                        Console.WriteLine("Unauthorized by exception");
//                        context.Response.StatusCode = 401;
//                    }
//                }

//                context.Response.Headers.Add("x-token", token); 
//            }

//            return this._next(context);
            
//        }
//    }

//    public static class TokenMiddlewareExtensions
//    {
//        public static IApplicationBuilder CheckToken(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<TokenMiddleware>();
//        }
//    }
//}
