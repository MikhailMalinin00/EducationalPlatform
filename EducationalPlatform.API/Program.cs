using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EducationalPlatform.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Student Homework API",
                    Version = "v1"
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "MailRu";
            })
                .AddCookie()
                .AddOAuth("MailRu", options =>
                {
   
                    options.ClientId = "7d23e0108102457baca36ce4eb5d9f97";
                    options.ClientSecret = "1bd723fad7684e64ac95089bab3869c0";
                    options.CallbackPath = new PathString("/signin-mailru");
                    
                    options.AuthorizationEndpoint = "https://oauth.mail.ru/login";
                    options.TokenEndpoint = "https://oauth.mail.ru/token";
                    options.UserInformationEndpoint = "https://oauth.mail.ru/userinfo";
                    
                    options.Scope.Add("userinfo");
                    
                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            context.RunClaimActions(json.RootElement);
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Homework API v1");
                    c.RoutePrefix = string.Empty; // Swagger UI будет доступен по корню: http://localhost:xxxx/
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //Не думаю, что мне это пригодится...
            app.MapGet("/login", async context =>
            {
                await context.ChallengeAsync("MailRu", new AuthenticationProperties
                {
                    RedirectUri = "/profile"
                });
            });

            app.Run();
        }
    }
}
