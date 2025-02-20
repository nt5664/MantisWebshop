using MantisWebshop.Server.Sql.Data;
using MantisWebshop.Server.Sql.Extensions;
using MantisWebshop.Server.Sql.Models;
using MantisWebshop.Server.Sql.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MantisWebshop.Server.Sql.Controllers
{
    [ApiController]
    public class AdminController : MantisShopControllerBase<AdminController>
    {
        private readonly IWebHostEnvironment _environment;

        public AdminController(ILogger<AdminController> logger, MantisWebshopDbContext dbContext, IWebHostEnvironment environment)
            : base(logger, dbContext)
        {
            _environment = environment;
        }

        [HttpGet]
        [Route("[controller]/products")]
        [Authorize]
        public async Task<ActionResponse> GetProducts()
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                return GetResponse(200, "Ok", user.Products?.Select(ModelDtoExtensions.ToProductDto));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot fetch products of user {user.Id}", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpGet]
        [Route("[controller]/products/{id}")]
        [Authorize]
        public async Task<ActionResponse> GetProduct(string id)
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                var product = user.Products?.SingleOrDefault(x => x.Id.ToString().Equals(id));
                if (product is null)
                    return GetResponse(404, "Product does not exist");

                return GetResponse(200, "Ok", product.ToProductDto());
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot fetch product [{id}] for user [{user.Id}]", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpDelete]
        [Route("[controller]/products/{id}")]
        [Authorize]
        public async Task<ActionResponse> DeleteProduct(string id)
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                var product = user.Products?.SingleOrDefault(x => x.Id.ToString().Equals(id));
                if (product is null)
                    return GetResponse(404, "Product does not exist");

                DbContext.Remove(product);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation($"Product [{id}] has been deleted successfully by user [{user.Id}]");
                return GetResponse(200, "Ok", id);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot delete product [{id}] for user [{user.Id}]", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpPost]
        [Route("[controller]/products/edit")]
        [Authorize]
        public async Task<ActionResponse> PostEditProduct([FromForm] ProductInput dto)
        {
            if (dto is null)
                return GetResponse(400, "Invalid data");

            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            bool isNew = dto.Id is null;
            try
            {
                var status = 200;
                Product product = null!;
                string? imagePath = null;
                if (dto.Image is not null && !string.IsNullOrEmpty(dto.Image.FileName))
                {
                    if (!IsFileAllowed(dto.Image.ContentType))
                    {
                        Logger.LogWarning($"File [{dto.Image.FileName}] uploaded by user [{user.Id}] is not allowed!");
                        return GetResponse(403, "This file is not allowed", dto.Image.FileName);
                    }

                    imagePath = Path.Combine(_environment.WebRootPath, "images/", dto.Image.FileName);
                }

                if (isNew)
                {
                    product = dto.ToProduct();
                    product.Creator = user;
                    if (imagePath is not null)
                    { 
                        await SaveImage(product, dto.Image!, imagePath);
                        Logger.LogInformation($"Product image saved for product [{product.Id}]");
                    }

                    DbContext.Products.Add(product);
                    status = 201;
                }
                else
                {
                    if (!(user.Products?.Any(x => x.Id.ToString().Equals(dto.Id)) ?? false))
                    {
                        Logger.LogWarning($"User [{user.Id}] does not have authorization to edit product [{dto.Id}]");
                        return GetResponse(404, "Product does not exist"); 
                    }

                    product = await DbContext.Products.SingleAsync(x => x.Id.ToString().Equals(dto.Id));
                    product.UpdateByDto(dto);
                    if (imagePath is not null)
                    {
                        if (System.IO.File.Exists(product.ImageUrl))
                            System.IO.File.Delete(product.ImageUrl);

                        await SaveImage(product, dto.Image!, imagePath);
                        Logger.LogInformation($"Product image updated for product [{product.Id}]");
                    }
                }

                await DbContext.SaveChangesAsync();
                return GetResponse(status, "Ok", product.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot edit product {dto} for user {user.Id}", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        private static async Task SaveImage(Product product, IFormFile image, string imagePath)
        {
            using var fs = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(fs);
            product.ImageUrl = imagePath;
        }

        private static bool IsFileAllowed(string type)
        {
            return Constants.AllowedImageTypes.Contains(type);
        }
    }
}
