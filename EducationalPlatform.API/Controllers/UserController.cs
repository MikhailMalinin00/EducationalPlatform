using EducationalPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Entities _context;

        public UserController(Entities context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Невалидный токен.");
            }

            var student = _context.Students.FirstOrDefault(s => s.Id == userId);
            if (student != null)
            {
                return Ok(new
                {
                    email = student.Email,
                    fullName = student.FullName,
                    role = "student"
                });
            }

            var teacher = _context.Teachers.Include(t => t.Role).FirstOrDefault(t => t.Id == userId);

            if (teacher != null)
            {
                return Ok(new
                {
                    email = teacher.Email,
                    fullName = teacher.FullName,
                    role = teacher.Role?.RoleName ?? "unknown"
                });
            }


            return NotFound("Пользователь не найден.");
        }
    }
}
