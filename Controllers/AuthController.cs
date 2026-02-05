using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentApi.DataSimulation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Secured_Student_API.DTOs.Auth;
using RefreshRequest = Secured_Student_API.DTOs.Auth.RefreshRequest;

namespace StudentApi.Controllers
{
 
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
       
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var student = StudentDataSimulation.StudentsList
                .FirstOrDefault(s => s.Email == request.Email);

            if (student == null)
                return Unauthorized("Invalid credentials");

      
            bool isValidPassword =
                BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash);

            if (!isValidPassword)
                return Unauthorized("Invalid credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),

                new Claim(ClaimTypes.Email, student.Email),

                new Claim(ClaimTypes.Role, student.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));

           
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           
            var token = new JwtSecurityToken(
                issuer: "StudentApi",
                audience: "StudentApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();


            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            student.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            student.RefreshTokenRevokedAt = null;

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            var student = StudentDataSimulation.StudentsList
                .FirstOrDefault(s => s.Email == request.Email);

            if (student == null)
                return Unauthorized("Invalid refresh request");

            if (student.RefreshTokenRevokedAt != null)
                return Unauthorized("Refresh token is revoked");

            if (student.RefreshTokenExpiresAt == null || student.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expired");

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, student.RefreshTokenHash);
            if (!refreshValid)
                return Unauthorized("Invalid refresh token");

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                 new Claim(ClaimTypes.Email, student.Email),
                 new Claim(ClaimTypes.Role, student.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "StudentApi",
                audience: "StudentApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(10),
                signingCredentials: creds
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var newRefreshToken = GenerateRefreshToken();
            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            student.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            student.RefreshTokenRevokedAt = null;

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var student = StudentDataSimulation.StudentsList
                .FirstOrDefault(s => s.Email == request.Email);

            if (student == null)
                return Ok(); 

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, student.RefreshTokenHash);
            if (!refreshValid)
                return Ok();

            student.RefreshTokenRevokedAt = DateTime.UtcNow;
            return Ok("Logged out successfully");
        }
    }
}
