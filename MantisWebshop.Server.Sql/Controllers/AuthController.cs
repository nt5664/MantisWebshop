using MantisWebshop.Server.Sql.Data;
using MantisWebshop.Server.Sql.Models;
using MantisWebshop.Server.Sql.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Crypt = BCrypt.Net.BCrypt;

namespace MantisWebshop.Server.Sql.Controllers
{
    [ApiController]
    public class AuthController : MantisShopControllerBase<AuthController>
    {
        public AuthController(ILogger<AuthController> logger, MantisWebshopDbContext dbContext)
            : base(logger, dbContext)
        { }

        [HttpPost]
        [Route("/signup")]
        public async Task<ActionResponse> PostSignup(SignupIntput signupInput)
        {
            if (DbContext.Users.Any(x => x.Email.Equals(signupInput.Email)))
            {
                Logger.LogInformation($"User with email [{signupInput.Email}] already exists");
                return GetResponse(403, "Invalid input");
            }

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = signupInput.Name!,
                    Email = signupInput.Email!,
                    Password = Crypt.HashPassword(signupInput.Password, 12)
                };

                await DbContext.Users.AddAsync(user);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation($"User [{user.Id}] has been created");
                return GetResponse(201, "Ok");
            }
            catch (Exception ex)
            {
                Logger.LogError("User creation failed", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ActionResponse> PostLogin(LoginInput loginInput)
        {
            try
            {
                var user = await DbContext.Users.SingleOrDefaultAsync(x => x.Email.Equals(loginInput.Email));
                if (user is null || !Crypt.Verify(loginInput.Password, user.Password))
                {
                    Logger.LogInformation("Unsuccessful login attempt");
                    return GetResponse(401, "Invalid credentials");
                }

                Logger.LogInformation($"User [{user.Id}] logged in");
                return GetResponse(200, "Ok", GetToken(user));
            }
            catch (Exception ex)
            {
                Logger.LogError("Login failed", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        private object GetToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("very secure key to prevent reverse engineering"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "localhost",
                "localhost",
                claims,
                null,
                DateTime.Now.AddMinutes(30),
                creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
