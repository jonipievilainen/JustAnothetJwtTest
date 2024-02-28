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
            // Check if user credentials are valid
            if (credentials.Username == "string" && credentials.Password == "string")
            {
                // Q: Can I add Username to the Claims?
                // A: Yes, you can add Username to the Claims
                // Q: How do I add Username to the Claims?
                // A: You can add Username to the Claims by creating a new Claim with the ClaimTypes.Name and the Username as the value
                // Q: How do I create a new Claim with the ClaimTypes.Name and the Username as the value?
                // A: You can create a new Claim with the ClaimTypes.Name and the Username as the value by creating a new Claim with the ClaimTypes.Name and the Username as the value
                // Q: How do I create a new Claim?
                // A: You can create a new Claim by creating a new Claim with the ClaimTypes.Name and the Username as the value

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
