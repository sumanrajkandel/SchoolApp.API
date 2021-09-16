
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; // UseSqlServer() present on this //add nuget package :  Microsoft.EntityFrameworkCore.SqlServer
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey present
using Microsoft.OpenApi.Models;  //OpenApiInfo present
using SchoolApp.API.ContractImplementations;
using SchoolApp.API.Contracts;
using SchoolApp.API.Data;
using SchoolApp.API.Models;
using SchoolApp.API.MyMiddlewares;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Reading ConnectionString from appsettings.json file.
string ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Add services to the container.

builder.Services.AddControllers();

// MediaType Formatter : default is camelCase, below sets PascalCase formatting.
// .AddJsonOptions(options =>
//           options.JsonSerializerOptions.PropertyNamingPolicy = null);


// builder.Services.AddScoped<IClientConfiguration, ClientConfiguration>(); // TODO : uncomment this  and use later

// Adding Swagger
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolApp.API", Version = "v1" });
    });

// Adding/configuring EntityFrameworkCore DbContext with SQL to the WebApi project.
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));

builder.Services.AddDbContextPool<AppDbContext>(
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],

    ValidAudience = builder.Configuration["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);


// Adding Identity
// We don't create custom class for IdentityRole if u want u can create add here.
builder.Services.AddIdentity<Applicationuser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>() //added to work with Identity related tables with DBContxt
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Add JWT Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = tokenValidationParameters;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.


//Adding logger as the first component to execute in the pipeline
app.UseLoggerMiddleware();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolApp.API v1");
    });
}

// Custom middleware for API ExceptionHandling calls early in the middleware pipeline so to handel exceptions on later deligates
// app.UseMiddleware(typeof(ExceptionHandlingMiddleware))
// app.UseExceptionHandlingMiddleware(); //TODO : uncomment this code and implement it.



// Adding middleware for Custom header (CLIENTNAME) read
// app.UseClientConfigurationMiddleware();


app.UseRouting();

// Authentication & Autherization
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Seed the database
AppDbInitilizer.SeedRolesToDb(app).Wait();


app.Run();
