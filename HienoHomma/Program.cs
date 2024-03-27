
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.ApplicationInsights.Log4NetAppender;

// Q: how to use log4net?
// A: Add the following line to the using directives
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using HienoHomma;

// Configure log4net.config file
XmlConfigurator.Configure(LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));



var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "HienoHomma",
            ValidAudience = "HienoHomma",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILog>(provider => LogManager.GetLogger("AuditLogger"));





// Configure log4net
//var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
//XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

//var logger = LogManager.GetLogger(typeof(Program));



var app = builder.Build();

// Use Swagger Authorize as part of the request
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        swagger.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        });
        swagger.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    // Example of logging
    //logger.Info($"Sent response: {context.Response.StatusCode}");

    Console.WriteLine("kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa kissa ");



    // Tarkista, onko Authorization-otsake asetettu
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        // Jos Authorization-otsakkeessa on Bearer-tokeni, tarkista se
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "HienoHomma",
                ValidAudience = "HienoHomma",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"))
            };
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var user = handler.ValidateToken(token, tokenValidationParameters, out _);
                // Voit esimerkiksi lisätä käyttäjän kontekstiin

                // Take Country claim from the token
                var country = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Country)?.Value;

                context.Items["User"] = user;
            }
            catch (Exception)
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
        }
        else
        {
            // Väärä muoto
            context.Response.StatusCode = 400; // Bad Request
            return;
        }
    }

    // Anna pyynnön jatkaa koodia
    await next();
});



app.MapControllers();

app.UseMiddleware<AuditMiddleware>();

log4net.Util.LogLog.InternalDebugging = true;

app.Run();

