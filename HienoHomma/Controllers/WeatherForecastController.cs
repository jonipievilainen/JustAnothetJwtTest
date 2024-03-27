using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace HienoHomma.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        // Endpoint for generating JWT token from user credentials
        [HttpPost("token", Name = "GetToken")]
        public async Task<IActionResult> GetToken([FromBody] UserCredentials credentials)
        {
            Console.WriteLine("Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: Username: " + credentials.Username);
            // Check if user credentials are valid
            if (credentials.Username == "string" && credentials.Password == "string")
            {
                // Create JWT token
                var token = new JwtSecurityToken(
                    issuer: "HienoHomma",
                    audience: "HienoHomma",
                    claims: new[] { new Claim(ClaimTypes.Name, credentials.Username), new Claim(ClaimTypes.Country, "Suomi") },
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication")), SecurityAlgorithms.HmacSha256)
                );

                // Return token
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
