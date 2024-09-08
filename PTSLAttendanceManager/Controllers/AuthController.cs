using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey = "ca4e3bf2fd397790835c319a5b506b130cc0383d8e53028c63c61098681d3093"; 

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == request.PtslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid PtslId" });
            }

            
            var otp = "123456"; 

            return Ok(new { statusCode = 200, message = "Login successful", otp = otp });
        }

        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            
            if (request.Otp != "123456") 
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid OTP" });
            }

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, request.PtslId),
                    new Claim("PtslId", request.PtslId) 
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { statusCode = 200, message = "OTP verified successfully", token = tokenString });
        }
    }

    public class LoginRequest
    {
        public string PtslId { get; set; } // The PtslId to log in
    }

    public class VerifyOtpRequest
    {
        public string PtslId { get; set; } // The PtslId used during login
        public string Otp { get; set; } // OTP provided by the user
    }
}
