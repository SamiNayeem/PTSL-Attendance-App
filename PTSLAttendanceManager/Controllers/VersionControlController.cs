using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTSLAttendanceManager.Data;
using System;
using System.Threading.Tasks;
using PTSLAttendanceManager.Models.Entity;
using PTSLAttendanceManager.Models;

namespace PTSLAttendanceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionControlController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VersionControlController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CheckForUpdate")]
        public async Task<IActionResult> CheckForUpdate([FromBody] VersionCheckRequest request)
        {
            
            var latestVersion = await _context.VersionControl
                .OrderByDescending(v => v.Date)
                .FirstOrDefaultAsync();

            if (latestVersion == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "No version information available",
                    data = new object() { }
                });
            }

            
            bool isUpdateAvailable = CompareVersions(latestVersion.Version, request.CurrentVersion);
            bool isForceUpdateAvailable = isUpdateAvailable && latestVersion.IsForceUpdateAvailable;

            return Ok(new
            {
                statusCode = 200,
                message = "Version check completed",
                data = new
                {
                    latestVersion = latestVersion.Version,
                    isUpdateAvailable,
                    isForceUpdateAvailable
                }
            });
        }

        
        private bool CompareVersions(string latestVersion, string currentVersion)
        {
            var latestVersionParts = latestVersion.Split('.');
            var currentVersionParts = currentVersion.Split('.');

            for (int i = 0; i < Math.Max(latestVersionParts.Length, currentVersionParts.Length); i++)
            {
                int latestPart = i < latestVersionParts.Length ? int.Parse(latestVersionParts[i]) : 0;
                int currentPart = i < currentVersionParts.Length ? int.Parse(currentVersionParts[i]) : 0;

                if (latestPart > currentPart) return true;
                if (latestPart < currentPart) return false;
            }
            return false;
        }
    }

    
}
