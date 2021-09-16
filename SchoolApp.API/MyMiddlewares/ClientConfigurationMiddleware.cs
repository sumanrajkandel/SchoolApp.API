
using Microsoft.Extensions.Primitives; // StringValues presents 
using SchoolApp.API.Contracts;

namespace SchoolApp.API.MyMiddlewares;
// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ClientConfigurationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly ILogger<ClientConfigurationMiddleware> _logger;

    //public ClientConfigurationMiddleware(RequestDelegate next, IConfiguration config, ILogger<ClientConfigurationMiddleware> logger)
    //{
    //    _next = next;
    //    _config = config;
    //    _logger = logger;
    //}

    //public List<string> GetClientList()
    //{
    //    string ClientName = _config["ClientNameList"];
    //    string[] ClientNameList = ClientName.Split(',');

    //    return ClientNameList.ToList();
    //}

    //public async Task InvokeAsync(HttpContext httpContext, IClientConfiguration clientConfiguration)
    //{
    //    /// if (httpContext.Request.Headers.TryGetValue("CLIENTNAME", out StringValues clientName))
    //    /// {
    //    ///     bool IsClientNameExits = GetClientList().Contains(clientName);
    //    ///
    //    ///     if (IsClientNameExits)
    //    ///     {
    //    ///         clientConfiguration.ClientName = clientName.SingleOrDefault();
    //    ///
    //    ///         clientConfiguration.InvokedDateTime = DateTime.UtcNow;

    //    //Move to next delegate/middleware in the pipleline
    //    //Request
    //    _logger.LogInformation("LoggerMiddleware - Request");
    //    await _next(httpContext);
    //    _logger.LogInformation("LoggerMiddleware - Response");
    //    //Response
    //    /// }
    //    /// else
    //    /// {
    //    ///     await httpContext.Response.WriteAsync("Client Name not valid!!");
    //    ///     return;
    //    /// }
    //    /// }
    //    ///  else
    //    ///  {
    //    ///      //Here you can throw exception to force client to send the header
    //    ///      await httpContext.Response.WriteAsync("Client Name not valid!!");
    //    ///      await _next(httpContext);
    //    ///  }
    //}
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ClientConfigurationMiddlewareExtensions
{
//    public static IApplicationBuilder UseClientConfigurationMiddleware(this IApplicationBuilder builder)
//    {
//        return builder.UseMiddleware<ClientConfigurationMiddleware>();
//    }
}
