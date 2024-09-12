using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models.Entity;
using PTSLAttendanceManager.Services; // Import FirebaseService
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FirebaseService _firebaseService;
        private readonly string _secretKey = "ca4e3bf2fd397790835c319a5b506b130cc0383d8e53028c63c61098681d3093";

        public AuthController(ApplicationDbContext context, FirebaseService firebaseService)
        {
            _context = context;
            _firebaseService = firebaseService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Find the user by PtslId in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == request.PtslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid PtslId" });
            }

            // Extract phone number from the user entity
            var phoneNumber = "+880" + user.Phone;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest(new { statusCode = 400, message = "Phone number not found for the given PtslId." });
            }

            // Send OTP using Firebase REST API
            string sessionInfo;
            try
            {
                sessionInfo = await _firebaseService.SendOtpAsync(phoneNumber);

                // Store the sessionInfo in the user's record for later use
                user.SessionInfo = sessionInfo;
                await _context.SaveChangesAsync();
            }
            catch (HttpRequestException ex)
            {
                // Log the detailed error
                var errorDetail = ex.Message;
                return StatusCode(500, new { statusCode = 500, message = "Error generating OTP", error = errorDetail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Unexpected error generating OTP", error = ex.Message });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "Login Successful. OTP sent.",
            });
        }

    //    [HttpPost("VerifyOtp")]
    //    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    //    {
    //        // Find the user by PtslId in the database
    //        var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == request.PtslId);

    //        if (user == null)
    //        {
    //            return Unauthorized(new { statusCode = 401, message = "Invalid PtslId" });
    //        }

    //        // Retrieve the sessionInfo from the user's record
    //        var sessionInfo = user.SessionInfo;

    //        if (string.IsNullOrEmpty(sessionInfo))
    //        {
    //            return BadRequest(new { statusCode = 400, message = "Session information is missing for the given PtslId." });
    //        }

    //        // Verify OTP using Firebase REST API
    //        bool isOtpValid;
    //        try
    //        {
    //            isOtpValid = await _firebaseService.VerifyOtpAsync(sessionInfo, request.Otp);
    //        }
    //        catch (HttpRequestException ex)
    //        {
    //            // Log the detailed error
    //            var errorDetail = ex.Message;
    //            return StatusCode(500, new { statusCode = 500, message = "Error verifying OTP", error = errorDetail });
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new { statusCode = 500, message = "Unexpected error verifying OTP", error = ex.Message });
    //        }

    //        if (!isOtpValid)
    //        {
    //            return Unauthorized(new { statusCode = 401, message = "Invalid OTP" });
    //        }

    //        // Generate JWT token
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = Encoding.ASCII.GetBytes(_secretKey);
    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new[]
    //            {
    //                new Claim(JwtRegisteredClaimNames.Sub, request.PtslId),
    //                new Claim("PtslId", request.PtslId)
    //            }),
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        var tokenString = tokenHandler.WriteToken(token);

    //        // Clear the session info after successful verification
    //        user.SessionInfo = null;
    //        await _context.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            statusCode = 200,
    //            message = "OTP Verified",
    //            data = new { token = tokenString }
    //        });
    //    }
    //}

//    public class LoginRequest
//    {
//        public string PtslId { get; set; } // The PtslId to log in
//    }

//    public class VerifyOtpRequest
//    {
//        public string PtslId { get; set; } // The PtslId used during login
//        public string Otp { get; set; } // OTP provided by the user
//    }
}
    }
