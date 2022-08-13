//using FluentResults;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Core.Exceptions;
//using wallet.lib.logger;

//namespace Api.Middlewares
//{
//    public class LoggerMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILoggerManager _logger;
//        private IHttpContextAccessor _accessor;
//        private IConfiguration _configuration;
//        public LoggerMiddleware(RequestDelegate next, ILoggerManager logger, IHttpContextAccessor accessor, IConfiguration configuration)
//        {
//            _next = next;
//            _logger = logger;
//            _accessor = accessor;
//            _configuration = configuration;
//        }

//        public async Task InvokeAsync(HttpContext httpContext)
//        {
//            var watch = System.Diagnostics.Stopwatch.StartNew();
//            DateTime startDate = DateTime.Now;
//            string url = httpContext.Request.Path.Value;
//            UserRequestProperties userRequest = null;

//            if (new List<string>() { "/swagger", "/favicon.ico" }.Any(u => url.Contains(u.ToLower())))
//            {
//                await _next(httpContext);
//            }
//            else
//            {
//                Stream originalBody = httpContext.Response.Body;
//                try
//                {
//                    userRequest = new UserRequestProperties()
//                    {
//                        RegisterDate = startDate.ToString(),
//                        Params = await GetRequestBodyAsync(httpContext),
//                        Url = httpContext.Request.Host + url,
//                        Status = ERunningStatus.Running.ToString(),
//                        Code = Guid.NewGuid().ToString(),
//                        Headers = string.Join(',', httpContext.Request.Headers.Select(h => $" {h.Key}:{h.Value}").ToList()),
//                        ClientIP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
//                    };

//                    if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
//                    {
//                        userRequest.ClientIP = httpContext.Request.Headers["X-Forwarded-For"];
//                    }

//                    _logger.LogInfo(userRequest);

//                    using (var memStream = new MemoryStream())
//                    {
//                        httpContext.Response.Body = memStream;
//                        var headers = httpContext.Request.Headers;

//                        await _next(httpContext);

//                        memStream.Position = 0;
//                        string response = await new StreamReader(memStream).ReadToEndAsync();
//                        userRequest.Responce = response.Replace("\"", "'");
//                        memStream.Position = 0;

//                        await memStream.CopyToAsync(originalBody);
//                    }

//                    userRequest.Duration = watch.ElapsedMilliseconds.ToString();
//                    watch.Stop();
//                    userRequest.Status = ERunningStatus.Done.ToString();

//                    if (userRequest.Responce.Contains("'isSuccess':true"))
//                        _logger.LogInfo(userRequest);
//                    else
//                        _logger.LogWarn(userRequest);
//                }
//                catch (Exception ex)
//                {
//                    httpContext.Response.Body = originalBody;
//                    await HandleExceptionAsync(httpContext, userRequest, ex, startDate);
//                }
//            }
//        }

//        private Stream GenerateStreamFromString(string s)
//        {
//            var stream = new MemoryStream();
//            var writer = new StreamWriter(stream);
//            writer.Write(s);
//            writer.Flush();
//            stream.Position = 0;
//            return stream;
//        }

//        private async Task<string> GetRequestBodyAsync(HttpContext httpContext)
//        {
//            var req = httpContext.Request;

//            // Allows using several time the stream in ASP.Net Core
//            //req.EnableRewind();
//            req.EnableBuffering();

//            var bodyStr = "";
//            // Arguments: Stream, Encoding, detect encoding, buffer size 
//            // AND, the most important: keep stream opened
//            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
//            {
//                bodyStr = await reader.ReadToEndAsync();
//            }

//            // Rewind, so the core is not lost when it looks the body for the request
//            req.Body.Position = 0;

//            return bodyStr.Replace("\"", "'");
//        }

//        private async Task HandleExceptionAsync(HttpContext context, UserRequestProperties userRequest, Exception ex, DateTime startDate)
//        {
//            try
//            {
//                string code = Guid.NewGuid().ToString();
//                var errorLog = new
//                {
//                    Message = ex.ToString() + (ex.InnerException == null ? "" : "/n/n with Inner Exception: /n/n" + ex.InnerException.ToString()),
//                    StackTrace = ex.StackTrace + (ex.InnerException == null ? "" : "/n/n with Inner Exception StackTrace: /n/n" + ex.StackTrace),
//                    RegisterDate = DateTime.Now,
//                };

//                if (userRequest != null)
//                {
//                    userRequest.Error = JObject.FromObject(errorLog).ToString();
//                    userRequest.Duration = Convert.ToInt32((DateTime.Now - startDate).TotalMilliseconds).ToString();
//                    userRequest.Status = ERunningStatus.Done.ToString();
//                }
//                else
//                {
//                    userRequest = new UserRequestProperties()
//                    {
//                        Error = JObject.FromObject(errorLog).ToString(),
//                        Duration = Convert.ToInt32((DateTime.Now - startDate).TotalMilliseconds).ToString(),
//                        Status = ERunningStatus.Done.ToString(),
//                        Code = Guid.NewGuid().ToString()
//                    };
//                }

//                _logger.LogError(userRequest);
//            }
//            catch (Exception)
//            {
//                //
//            }

//            context.Response.StatusCode = 500;
//            context.Response.ContentType = "application/json";
//            string jsonstring = "";
//            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
//            if (ex.GetType() == typeof(BadRequestException))
//            {
//                context.Response.StatusCode = 400;
//                jsonstring = JObject.FromObject(new Result().WithError(ex.Message)).ToString();
//                await context.Response.WriteAsync(jsonstring, Encoding.UTF8);
//                return;
//            }
            
//            jsonstring = JObject.FromObject(new Result().WithError("متاسفانه خطایی در ارتباط با سرور رخ داده است. لطفا مدتی دیگر اقدام نمایید")).ToString();

//            if (ex.Message.Contains("System.InvalidOperationException: Unable to resolve service for type 'Swashbuckle.AspNetCore.Swagger.ISwaggerProvider' while attempting to Invoke middleware"))
//            {
//                context.Response.StatusCode = 404;
//                jsonstring = JObject.FromObject(new Result().WithError("No Service Available")).ToString();
//            }

//            await context.Response.WriteAsync(jsonstring, Encoding.UTF8);

//            // to stop futher pipeline execution 
//            return;
//        }
//    }
//}
