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
    public class Auth2Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey = "ca4e3bf2fd397790835c319a5b506b130cc0383d8e53028c63c61098681d3093";

        public Auth2Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Retrieve the user by PtslId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PtslId == request.PtslId);

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid PtslId" });
            }

            // Case 1: Check if the DeviceUId is already registered to another user
            var otherUserWithSameDevice = await _context.Users
                .FirstOrDefaultAsync(u => u.DeviceUId == request.DeviceUId && u.PtslId != request.PtslId);

            if (otherUserWithSameDevice != null)
            {
                // Log the proxy attempt in ProxyLogs table
                var proxyLog = new ProxyLogs
                {
                    Date = DateTime.Now,
                    ProxyGiverId = user.PtslId,
                    AbsentPersonId = otherUserWithSameDevice.PtslId,
                    Remarks = "Proxy login attempt detected",
                    ProxyGiver = otherUserWithSameDevice,
                    AbsentPerson = user
                };

                // Add the log to the database
                _context.ProxyLogs.Add(proxyLog);
                await _context.SaveChangesAsync();

                // Return an error message indicating a proxy attempt
                return Unauthorized(new
                {
                    statusCode = 401,
                    message = "This device is already registered with another user. You have been blacklisted for attempting proxy.",
                    data = new { conflictingUser = otherUserWithSameDevice.PtslId }
                });
            }

            // Case 2: If the user does not have a DeviceUId set, store the current DeviceUId and DeviceModel
            if (string.IsNullOrEmpty(user.DeviceUId))
            {
                // Store the new DeviceUId and DeviceModel
                user.DeviceUId = request.DeviceUId;
                user.DeviceModel = request.DeviceModel;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = 200,
                    message = "Login successful, device information stored.",
                    data = new { otp = "123456" } // Replace with OTP generation logic
                });
            }

            // Case 3: If the DeviceUId matches the current user, allow login
            if (user.DeviceUId == request.DeviceUId)
            {
                return Ok(new
                {
                    statusCode = 200,
                    message = "Login successful, device information matches.",
                    data = new { otp = "123456" } // Replace with OTP generation logic
                });
            }

            // Case 4: If the DeviceUId does not match the current user, deny login
            return Unauthorized(new
            {
                statusCode = 401,
                message = "Device does not match the current user.",
                data = new { }
            });
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

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddDays(365) // Expires in 1 year
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //return Ok(new { statusCode = 200, message = "OTP verified successfully", token = tokenString });
            return Ok(new
            {
                statusCode = 200,
                message = "OTP Verified",
                data = new
                {
                    token = tokenString
                }
            });
        }
    }

    public class LoginRequest
    {
        public string PtslId { get; set; } // The PtslId to log in
        public string DeviceUId { get; set; }  // Device Unique Identifier
        public string DeviceModel { get; set; }  // Device Model (optional)
    }

    public class VerifyOtpRequest
    {
        public string PtslId { get; set; } // The PtslId used during login
        public string Otp { get; set; } // OTP provided by the user
    }
}