using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentApi.DataSimulation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Secured_Student_API.Model;


namespace Secured_Student_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
            {
                return BadRequest("Email and password are required");
            }

            var student = StudentDataSimulation.StudentsList
                .FirstOrDefault(s => s.Email == req.Email);

            if (student == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // ✅ FIXED PASSWORD CHECK
            if (!BCrypt.Net.BCrypt.Verify(req.Password, student.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            // ✅ CORRECT CLAIMS
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Role, student.Role)
            };

            // ✅ MUST MATCH Program.cs / Startup.cs
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("MY_VERY_SECRET_KEY_IS_HERE_0000000000000")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "StudentAPI",
                audience: "StudentAPIUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
