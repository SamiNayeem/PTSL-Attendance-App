using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserListController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserListController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserList")]
        [Authorize]  // Authorization via Bearer Token
        public async Task<IActionResult> GetUserList()
        {
            // Retrieve PtslId from the JWT token claims
            var ptslId = User.FindFirst("PtslId")?.Value;

            if (string.IsNullOrEmpty(ptslId))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token or role", data = (object)null });
            }

            // Retrieve the user's RoleId from the database
            var user = await _context.Users
                .Where(u => u.PtslId == ptslId)
                .Select(u => new { u.RoleId })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid token or role", data = (object)null });
            }

            long roleId = user.RoleId;

            // Check if the user has permission to view the list
            if (roleId >= 2 && roleId <= 6)
            {
                // If RoleId is 2, 3, 4, 5, or 6, return the list of users
                var usersList = await _context.Users
                    .Where(u => u.PtslId != "PTSL1111")
                    .Select(u => new
                    {
                        u.PtslId,
                        u.Name,
                        u.Phone,
                        u.Designation,
                        u.Email,
                        u.RoleId,
                        u.TeamId
                    })
                    .OrderBy(u => u.Name)
                    //.OrderBy(u=>u.RoleId)
                    .ToListAsync()

                    ;
                

                return Ok(new { statusCode = 200, message = "Success", data = usersList });
            }
            else if (roleId == 7)
            {
                // If RoleId is 7, return only the user's own information
                var userInfo = await _context.Users
                    .Where(u => u.PtslId == ptslId )
                    .Select(u => new
                    {
                        u.PtslId,
                        u.Name,
                        u.Phone,
                        u.Designation,
                        u.Email,
                        u.RoleId,
                        u.TeamId
                        
                    })
                    .FirstOrDefaultAsync();

                return Ok(new { statusCode = 200, message = "Success", data = userInfo });
            }
            else
            {
                // If RoleId is not authorized, return unauthorized response
                return Unauthorized(new { statusCode = 401, message = "Access denied", data = (object)null });
            }
        }
    }
}
