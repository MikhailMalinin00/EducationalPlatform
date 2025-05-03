using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using EducationalPlatform.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EducationalPlatform.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<Entities>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowRazorPages",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7193") // Razor Pages origin
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // JSON-serialization
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

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

            // JWT authentication
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
            //    };
            //});

            // JWT and Mail.ru authentication
            builder.Services.AddAuthentication(options =>
            {
                //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = "MailRu";
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "MailRu";
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                    };
                })
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
                        //OnCreatingTicket = async context =>
                        //{
                        //    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        //    var response = await context.Backchannel.SendAsync(request);
                        //    response.EnsureSuccessStatusCode();

                        //    var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        //    context.RunClaimActions(json.RootElement);

                        //    // Получаем нужные данные
                        //    var email = json.RootElement.GetProperty("email").GetString();
                        //    var name = json.RootElement.GetProperty("name").GetString();

                        //    // Получаем scope сервиса
                        //    var dbContext = context.HttpContext.RequestServices.GetRequiredService<Entities>();

                        //    if (email != null)
                        //    {
                        //        if (email.EndsWith("@edu.fa.ru"))
                        //        {
                        //            var existing = dbContext.Students.FirstOrDefault(s => s.Email == email);
                        //            if (existing == null)
                        //            {
                        //                dbContext.Students.Add(new Student
                        //                {
                        //                    Email = email,
                        //                    FullName = name
                        //                });
                        //                await dbContext.SaveChangesAsync();
                        //            }
                        //        }
                        //        else if (email.EndsWith("@fa.ru"))
                        //        {
                        //            var existing = dbContext.Teachers.FirstOrDefault(t => t.Email == email);
                        //            if (existing == null)
                        //            {
                        //                dbContext.Teachers.Add(new Teacher
                        //                {
                        //                    Email = email,
                        //                    FullName = name
                        //                });
                        //                await dbContext.SaveChangesAsync();
                        //            }
                        //        }
                        //    }
                        //}

                        OnCreatingTicket = async context =>
                        {
                            var userInfoUrl = $"{context.Options.UserInformationEndpoint}?access_token={context.AccessToken}";
                            var request = new HttpRequestMessage(HttpMethod.Get, userInfoUrl);
                            var response = await context.Backchannel.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var jsonString = await response.Content.ReadAsStringAsync();
                            var json = JsonDocument.Parse(jsonString);
                            var root = json.RootElement;

                            var id = root.GetProperty("id").GetString();
                            var name = root.GetProperty("nickname").GetString();
                            var email = root.GetProperty("email").GetString();

                            // Присваиваем клеймы
                            context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                            context.Identity.AddClaim(new Claim(ClaimTypes.Name, name));
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email));

                            // Получаем доступ к DbContext
                            var dbContext = context.HttpContext.RequestServices.GetRequiredService<Entities>();

                            if (email.EndsWith("@edu.fa.ru", StringComparison.OrdinalIgnoreCase))
                            {
                                var exists = await dbContext.Students.AnyAsync(s => s.Email == email);
                                if (!exists)
                                {
                                    dbContext.Students.Add(new Student
                                    {
                                        Id = Guid.NewGuid(),
                                        FullName = name,
                                        Email = email,
                                        Visibility = true
                                    });
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                            else if (email.EndsWith("@fa.ru", StringComparison.OrdinalIgnoreCase))
                            {
                                var exists = await dbContext.Teachers.AnyAsync(t => t.Email == email);
                                if (!exists)
                                {
                                    dbContext.Teachers.Add(new Teacher
                                    {
                                        Id = Guid.NewGuid(),
                                        FullName = name,
                                        Email = email,
                                        RoleId = Guid.Parse("D93737E7-70CC-4463-8091-8EEE3877FE22"),
                                        Visibility = true
                                    });
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    };
                });

            var app = builder.Build();

            app.UseCors("AllowRazorPages");

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

            // Global error handling middleware
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new { error = "Произошла внутренняя ошибка сервера." };
                    var json = JsonSerializer.Serialize(errorResponse);
                    await context.Response.WriteAsync(json);
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
