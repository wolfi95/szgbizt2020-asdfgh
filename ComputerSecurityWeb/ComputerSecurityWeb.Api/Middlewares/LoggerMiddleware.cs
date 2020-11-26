using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;



namespace ComputerSecurityWeb.Api.Middlewares
{
    public class LoggerMiddleware
    {
        private const string MessageTemplate =
          "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        //TODO: Can we inject that?
        private static readonly ILogger Log = Serilog.Log.ForContext<LoggerMiddleware>();

        private readonly RequestDelegate _next;

        public LoggerMiddleware(RequestDelegate next)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var bodyStream = httpContext.Response.Body;

            var responseBodyStream = new MemoryStream();
            httpContext.Response.Body = responseBodyStream;

            var sw = Stopwatch.StartNew();

            try
            {
                await this._next(httpContext);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw e;
            }
            finally
            {
                sw.Stop();

                //TODO: Not sure if we have response status codes here...
                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = LogForErrorContext(httpContext);

                if (httpContext.Response.ContentType != null && httpContext.Response.ContentType.Contains("json"))
                {
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(responseBodyStream);
                    log = log.ForContext("ResponseBody", reader.ReadToEnd());
                }

                responseBodyStream.Seek(0, SeekOrigin.Begin);

                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, sw.Elapsed.TotalMilliseconds);

                await responseBodyStream.CopyToAsync(bodyStream);
                httpContext.Response.Body = bodyStream;
            }
        }

        private static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol)
                .ForContext("Connection", new Dictionary<string, string>
                {
                    { "LocalIpAddress", httpContext.Connection.LocalIpAddress?.ToString() },
                    { "LocalPort", httpContext.Connection.LocalPort.ToString() },
                    { "RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString() },
                    { "RemotePort", httpContext.Connection.RemotePort.ToString() },
                });

            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                result = result.ForContext("User", new Dictionary<string, string>
                {
                    { "UserName", httpContext.User.Identity.Name },
                });
            }

            if (request.HasFormContentType)
            {
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));
            }

            if (request.ContentType != null && request.ContentType.Contains("json"))
            {
                var body = request.Body;
                request.EnableBuffering();
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(request.Body))
                {
                    result = result.ForContext("RequestBody", reader.ReadToEndAsync().GetAwaiter().GetResult());
                }

                request.Body = body;
            }

            return result;
        }
    }
}
