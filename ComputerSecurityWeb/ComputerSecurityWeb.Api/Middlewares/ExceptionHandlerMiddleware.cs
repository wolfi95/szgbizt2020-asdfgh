using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;
using ComputerSecurityWeb.Bll.Exceptions;

namespace ComputerSecurityWeb.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnvironment)
        {
            this._next = next;
            this._hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (NullReferenceException e)
            {
                await this.WriteAsJsonAsync(
                   context,
                   (int)HttpStatusCode.BadRequest,
                   new ErrorDto
                   {
                       Message = this._hostingEnvironment.IsDevelopment() ? e.Message : null,
                       StackTrace = null,
                   });
            }
            catch (AlreadyExistingException e)
            {
                await this.WriteAsJsonAsync(
                   context,
                   (int)HttpStatusCode.BadRequest,
                   new ErrorDto
                   {
                       Message = this._hostingEnvironment.IsDevelopment() ? e.Message : null,
                       StackTrace = null,
                   });
            }

            catch (Exception e)
            {
                await this.WriteAsJsonAsync(
                    context,
                    (int)HttpStatusCode.InternalServerError,
                    new ErrorDto
                    {
                        Message = this._hostingEnvironment.IsDevelopment() ? e.Message : "Internal error",
                        StackTrace = this._hostingEnvironment.IsDevelopment() ? e.StackTrace : null,
                    });
            }
        }

        private Task WriteAsJsonAsync(HttpContext context, int statusCode, ErrorDto payload)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            if (!this._hostingEnvironment.IsDevelopment())
            {
                payload.StackTrace = string.Empty;
            }

            var json = payload != null ? this.SerializeObject(payload) : string.Empty;

            return context.Response.WriteAsync(json);
        }

        private string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(
                obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        public class ErrorDto
        {
            public string Message { get; set; }

            public string StackTrace { get; set; }
        }
    }
}
