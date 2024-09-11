// Controllers/OTPController.cs
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly string _connectionString = "DefaultConnectionn";

        public OTPController()
        {
            FirebaseAdminHelper.InitializeFirebase();
        }

        [HttpPost("sendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OTPRequest request)
        {
            try
            {
                // Generate OTP using Firebase Phone Auth
                var user = await FirebaseAuth.DefaultInstance.GetUserByPhoneNumberAsync(request.PhoneNumber);
                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

                // Store OTP in database using stored procedure
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PhoneNumber", request.PhoneNumber);
                    parameters.Add("@OTP", customToken);

                    db.Execute("sp_StoreOTP", parameters, commandType: CommandType.StoredProcedure);
                }

                return Ok(new { message = "OTP sent successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("verifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OTPVerifyRequest request)
        {
            try
            {
                // Verify OTP using Firebase Phone Auth
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.OTP);

                // You can now use the verified user's UID or other info as needed
                var uid = decodedToken.Uid;

                return Ok(new { message = "OTP verified successfully!", uid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }
    }

    public class OTPRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class OTPVerifyRequest
    {
        public string OTP { get; set; }
    }
}
