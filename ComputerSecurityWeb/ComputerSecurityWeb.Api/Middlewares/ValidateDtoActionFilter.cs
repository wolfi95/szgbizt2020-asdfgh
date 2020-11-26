namespace ComputerSecurityWeb.Api.Middlewares
{
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Serilog;

    public class ValidateDtoActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var bodyStr = string.Empty;
                var req = context.HttpContext.Request;

                // Allows using several time the stream in ASP.Net Core
                req.EnableBuffering();

                // Arguments: Stream, Encoding, detect encoding, buffer size 
                // AND, the most important: keep stream opened
                using (StreamReader reader
                          = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEndAsync().GetAwaiter().GetResult();
                }

                Log.Information($"Invalid model body: {bodyStr}");

                // Rewind, so the core is not lost when it looks the body for the request
                req.Body.Position = 0;
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
