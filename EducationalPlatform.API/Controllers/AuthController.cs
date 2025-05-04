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

            string role;
            if (email.EndsWith("@edu.fa.ru"))
            {
                if (!_context.Students.Any(s => s.Email == email))
                {
                    _context.Students.Add(new Student { Email = email, FullName = name });
                    await _context.SaveChangesAsync();
                }
                role = "student";
            }
            else if (email.EndsWith("@fa.ru"))
            {
                if (!_context.Teachers.Any(t => t.Email == email))
                {
                    _context.Teachers.Add(new Teacher { Email = email, FullName = name });
                    await _context.SaveChangesAsync();
                }
                role = "teacher";
            }
            else
            {
                return Redirect("https://localhost:7193/error/UnsupportedDomain");
            }

            var token = JwtTokenGenerator.GenerateToken(email, role, _config["Jwt:SecretKey"], _config["Jwt:Issuer"]);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Redirect("https://localhost:7193/profile");

            //return Ok(new { token, role });
        }
    }
}
