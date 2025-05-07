using EducationalPlatform.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducationalPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Entities _context;
        private readonly IConfiguration _config;

        public AuthController(Entities context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "https://localhost:7203/api/auth/mailru-callback"
            };
            return Challenge(props, "MailRu");
        }

        [HttpGet("mailru-callback")]
        public async Task<IActionResult> MailRuCallback()
        {
            var result = await HttpContext.AuthenticateAsync("MailRu");

            if (!result.Succeeded)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (email == null || name == null)
                return BadRequest("Не удалось получить информацию о пользователе");

            Guid userId;

            if (email.EndsWith("@edu.fa.ru"))
            {
                var student = _context.Students.FirstOrDefault(s => s.Email == email);
                if (student == null)
                {
                    student = new Student
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        FullName = name,
                        GroupId = Guid.Parse("082FC3CB-0966-44FC-990E-6CAF0D2AE747"),
                        Visibility = true
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                }
                userId = student.Id;
            }
            else if (email.EndsWith("@fa.ru"))
            {
                var teacher = _context.Teachers.FirstOrDefault(t => t.Email == email);
                if (teacher == null)
                {
                    teacher = new Teacher
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        FullName = name,
                        RoleId = Guid.Parse("D93737E7-70CC-4463-8091-8EEE3877FE22"),
                        Visibility = true
                    };
                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();
                }
                userId = teacher.Id;
            }
            else
            {
                return Redirect("https://localhost:7193/error/UnsupportedDomain");
            }

            var token = JwtTokenGenerator.GenerateToken(userId, _config["Jwt:SecretKey"], _config["Jwt:Issuer"]);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Redirect("https://localhost:7193/profile");
        }

    }
}
