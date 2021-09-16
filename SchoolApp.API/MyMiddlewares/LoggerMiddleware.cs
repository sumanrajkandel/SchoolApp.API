
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolApp.API.Data;
using SchoolApp.API.Models;
using System.Net;
using System.Text;

namespace SchoolApp.API.MyMiddlewares;
// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;  // using as a services with constr DI.

    // private readonly UserManager<Applicationuser> _userManager;
    //  private readonly RoleManager<IdentityRole> _roleManager;
    //private readonly AppDbContext _context;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger/*, UserManager<Applicationuser> userManager*/
                            /*, AppDbContext appDbContext*/) //ILoggerFactory loggerFactory
    {
        _next = next;
        _logger = logger; //loggerFactory.CreateLogger<LoggerMiddleware>();
                          // _userManager = userManager;
                          // _context = appDbContext;
    }

    public async Task InvokeAsync(HttpContext httpContext, AppDbContext _context)
    {
        LogInformation_Database(httpContext, APPLogInfo_Code.REQ, _context); //Request       
        await _next(httpContext);   //Move to next delegate/middleware in the pipleline
                                    // LogInformation_Database(httpContext, appDbContext, APPLogInfo_Code.RES); //Response
    }


    //Private method
    private string GetIp(HttpContext httpContext)
    {
        var IPAddress = httpContext.Connection.RemoteIpAddress;
        string BrowserName = httpContext.Request.Headers.UserAgent;
        if (IPAddress == null)
        {
            IPAddress = IPAddress.None;
        }
        return $"IP =  {IPAddress.ToString()} Browser =  {BrowserName}";

    }
    private async void LogInformation_Database(HttpContext httpContext, APPLogInfo_Code logInfo_Code, AppDbContext _context)
    {
        // Working CS code.
        // var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=school-api-db;Integrated Security=True;Pooling=False");
        // AppDbContext _context = new AppDbContext(optionsBuilder.Options);



        //Read Headers from Request/Response and log it
        var Headers = logInfo_Code == APPLogInfo_Code.REQ ? httpContext.Request.Headers : httpContext.Response.Headers;


        //Read body from the request and log it // uncomment and TODO
        // using var reader = new StreamReader(httpContext.Request.Body);       
        // var requestBody = await reader.ReadToEndAsync();


        StringBuilder sbb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(Headers.ToString()))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Headers)
            {
                sb.Append(item.Key + ":" + item.Value + ",");
            }
            //sbb = sb.Remove();
        }
        else
        {
            sbb.Append("N/A");
        }


        APPLogInfo _APPLoginInfo = new APPLogInfo();
        _APPLoginInfo.LogCode = logInfo_Code.ToString();
        _APPLoginInfo.LogDescription = "LoggerMiddleWare.cs";
        _APPLoginInfo.Headers = sbb.ToString();
        //  _APPLoginInfo.Body = _body; //httpContext.Request.Body.ToString(); //TODO body modifies here that's why no hit to controller A/M
        _APPLoginInfo.CreatedOn = DateTime.Now;
        _APPLoginInfo.UserAgent = GetIp(httpContext);

        try
        {
            await _context.APPLogInfo.AddAsync(_APPLoginInfo);
            int _Status = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}



enum APPLogInfo_Code
{
    REQ,
    RES
}


// Extension method used to add the middleware to the HTTP request pipeline.
public static class LoggerMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggerMiddleware>(); //.UseMiddleware<ClientConfigurationMiddleware>();
    }
}
