using MantisWebshop.Server.Models;
using MantisWebshop.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Crypt = BCrypt.Net.BCrypt;
using Const = MantisWebshop.Server.Constants;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ZstdSharp.Unsafe;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace MantisWebshop.Server.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private readonly IMongoCollection<User> _users;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
            _users = MongoDbService.Instance.Db.GetCollection<User>(Const.USER_COLLECTION);
        }

        [HttpPost]
        [Route("/signup")]
        public void PostSignUp(AuthData userData)
        {
            if (_users.CountDocuments(x => x.Email.Equals(userData.Email)) > 0)
            {
                Response.StatusCode = 422;
                return;
            }

            try
            {
                var newUser = new User
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    Email = userData.Email!,
                    Password = Crypt.HashPassword(userData.Password, 12),
                    Cart = new User.UserCart()
                };

                _users.InsertOne(newUser);
                Response.StatusCode = 201;
            }
            catch (Exception ex)
            {
                _logger.LogError("User creation failed", ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return;
            }
        }

        [HttpPost]
        [Route("/login")]
        public object PostLogin(AuthData userData)
        {
            object? retValue = null;
            try
            {
                var user = _users.Find(x => x.Email.Equals(userData.Email)).First();
                if (!Crypt.Verify(userData.Password, user.Password))
                    Response.StatusCode = 401;
                else
                {
                    Response.StatusCode = 200;
                    retValue = GetToken(user);
                }
            }
            catch (InvalidOperationException)
            {
                Response.StatusCode = 401;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
            }

            return retValue!;
        }

        private object GetToken(User userObj)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userObj.Email),
                new Claim(JwtRegisteredClaimNames.Name, userObj.Id.ToString()),
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

            return new {
                Message = "Ok",
                UserId = userObj.Id.ToString(),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
