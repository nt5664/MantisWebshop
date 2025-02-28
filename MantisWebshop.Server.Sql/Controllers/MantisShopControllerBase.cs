using MantisWebshop.Server.Sql.Data;
using MantisWebshop.Server.Sql.Extensions;
using MantisWebshop.Server.Sql.Models;
using MantisWebshop.Server.Sql.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MantisWebshop.Server.Sql.Controllers
{
    public abstract class MantisShopControllerBase<T> : ControllerBase where T : MantisShopControllerBase<T>
    {
        public MantisShopControllerBase(ILogger<T> logger, MantisWebshopDbContext dbContext)
        {
            Logger = logger;
            DbContext = dbContext;
        }

        protected ILogger<T> Logger { get; }
        protected MantisWebshopDbContext DbContext { get; }

        protected ActionResponse GetResponse(int statusCode, string message, object? data = null)
        {
            Response.StatusCode = statusCode;
            return GetResponse(message, data);
        }

        protected ActionResponse GetResponse(string message, object? data = null)
        {
            Logger.LogInformation("Response generated", message, Response.StatusCode, data);
            return new ActionResponse
            {
                Message = message,
                Data = data
            };
        }

        protected async Task<User?> TryGetUserAsync()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                Logger.LogWarning("Invalid token, user ID cannot be obtained");
                return null;
            }

            try
            {
                var user = await DbContext.Users
                    .Include(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .Include(x => x.Products)
                    .Include(x => x.Orders)
                    .ThenInclude(y => y.ProductSnapshots)
                    .SingleOrDefaultAsync(x => x.Id.ToString().Equals(userId));
                Response.StatusCode = user is not null ? 200 : 401;
                return user;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, $"ID: {userId}", ex.StackTrace);
                Response.StatusCode = 500;
                return null;
            }
        }

        protected static int PageOffset(int page, int itemsPerPage)
        {
            return (page <= 1 ? 0 : page - 1) * itemsPerPage;
        }
    }
}
