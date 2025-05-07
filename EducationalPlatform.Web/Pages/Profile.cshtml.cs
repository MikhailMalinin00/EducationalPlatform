using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EducationalPlatform.Web.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProfileModel> _logger;

        public string Email { get; set; }
        public string Role { get; set; }

        public ProfileModel(IHttpClientFactory httpClientFactory, ILogger<ProfileModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in cookies.");
                return;
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7203/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/user/profile");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonSerializer.Deserialize<UserProfileDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Email = userInfo?.Email;
                    Role = userInfo?.Role;
                }
                else
                {
                    _logger.LogError("Failed to fetch user profile: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching profile.");
            }
        }

        private class UserProfileDto
        {
            public string Email { get; set; }
            public string Role { get; set; }
        }
    }
}
