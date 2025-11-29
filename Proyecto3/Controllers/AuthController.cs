using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Proyecto3.Core.Models;

namespace Proyecto3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Login de usuario - genera JWT token
        /// </summary>
        /// <param name="request">Usuario y contraseña</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // TODO: Validar contra BD (por ahora validación simple)
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Unauthorized(new { message = "Usuario o contraseña inválida" });
                }

                // Simulación: en producción validar contra BD
                if (request.Username != "admin" || request.Password != "admin123")
                {
                    return Unauthorized(new { message = "Credenciales incorrectas" });
                }

                var token = GenerateJwtToken(request.Username);

                return Ok(new { token, expiresIn = 3600 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Generar JWT token
        /// </summary>
        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings?.Issuer,
                audience: jwtSettings?.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// DTO para login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
