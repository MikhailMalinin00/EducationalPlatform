using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EducationalPlatform.Web.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public string Email { get; set; }
        public string Role { get; set; }

        public void OnGet()
        {
            Email = User.FindFirst(ClaimTypes.Email)?.Value;
            Role = User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
