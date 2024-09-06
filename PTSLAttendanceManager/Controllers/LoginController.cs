using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const string OTP = "123456"; // Hardcoded OTP for now

    public LoginController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpModel model)
    {
        if (string.IsNullOrEmpty(model.PtslId))
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Message = "PtslId is required",
                Data = null
            });
        }

        var user = await _context.Users.FindAsync(model.PtslId);
        if (user == null)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = 404,
                Message = "User not found",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "OTP sent successfully",
            Data = new { OTP }
        });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestModel model)
    {
        if (string.IsNullOrEmpty(model.PtslId) || string.IsNullOrEmpty(model.OTP))
        {
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Message = "PtslId and OTP are required",
                Data = null
            });
        }

        if (model.OTP != OTP)
        {
            return Unauthorized(new ApiResponse<object>
            {
                StatusCode = 401,
                Message = "Invalid OTP",
                Data = null
            });
        }

        var user = _context.Users.Find(model.PtslId);
        if (user == null)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = 404,
                Message = "User not found",
                Data = null
            });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ca4e3bf2fd397790835c319a5b506b130cc0383d8e53028c63c61098681d3093"); // Replace with your actual key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, model.PtslId)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Explicitly set the PtslId in the Data response
        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Login successful",
            Data = new
            {
                PtslId = model.PtslId,  // Explicitly include the PtslId here
                Token = tokenString
            }
        });
    }

}
