using MantisWebshop.Server.Sql.Models;
using MantisWebshop.Server.Sql.Models.DTOs;

namespace MantisWebshop.Server.Sql.Extensions
{
    public static class ModelDtoExtensions
    {
        public static Product ToProduct(this ProductDto dto)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static void UpdateByDto(this Product product, ProductDto dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            ++product.Version;
        }

        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Creator = product.Creator?.Id.ToString()
            };
        }

        public static CartItemDto ToCartItemDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
                ProductId = cartItem.Product.Id.ToString(),
                ProductName = cartItem.Product.Name,
                ImageUrl = cartItem.Product.ImageUrl?.Split('\\').Last() ?? Constants.FALLBACK_PRODUCT_IMAGE,
                Quantity = cartItem.Quantity
            };
        }

        public static ProductSnapshot TakeSnapshot(this Product product)
        {
            return new ProductSnapshot
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Price = product.Price,
                SnapshotVersion = product.Version,
                ProductId = product.Id
            };
        }
    }
}
