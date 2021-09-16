
using Newtonsoft.Json;
using SchoolApp.API.Data.Helper;
using System.Net;

namespace SchoolApp.API.MyMiddlewares;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // context.Response.StatusCode = 200;
        try
        {
            //Request
            _logger.LogInformation("ExceptionHandlingMiddleware - Request");
            await _next(context);
            _logger.LogInformation("ExceptionHandlingMiddleware - Response");
            //Response
        }
        catch (Exception ex)
        {

            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception ex)
    {
        var errorMessageObject = new Error { Message = ex.Message, Code = "GE" };
        var statusCode = (int)HttpStatusCode.InternalServerError;

        switch (ex)
        {
            case InvalidSchoolAPIException:
                errorMessageObject.Code = "M001";
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                break;
        }



        var errorMessage = JsonConvert.SerializeObject(new { Message = ex.Message, Code = "GE" });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(errorMessage);
    }
}


// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlingMiddlewareExtention
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
