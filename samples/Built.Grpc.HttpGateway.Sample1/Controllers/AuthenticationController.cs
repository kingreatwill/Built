using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Built.Grpc.HttpGateway.Sample1.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public AuthenticationController(IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// 认证
        /// </summary>
        [HttpGet]
        public IActionResult Auth()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetValue<string>("JwtAuthorize:Secret");
            var key = Encoding.ASCII.GetBytes(secret);
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtClaimTypes.Audience,"api"),
                    //new Claim(JwtClaimTypes.Issuer,"http://localhost:5200"),
                    //new Claim(JwtClaimTypes.Id, "123"),
                    //new Claim(JwtClaimTypes.Name, "admin"),
                    //new Claim(JwtClaimTypes.Email, "350840291@qq.com"),
                    //new Claim(JwtClaimTypes.PhoneNumber, "18682128396")
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return new JsonResult(
            new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    sid = "123",
                    name = "admin",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
    }
}