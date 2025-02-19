namespace MantisWebshop.Server.Sql
{
    public static class Constants
    {
        public static readonly string[] AllowedImageTypes =
        {
            "image/png",
            "image/jpg",
            "image/jpeg",
            "image/gif",
            "image/webp"
        };

        public const string FALLBACK_PRODUCT_IMAGE = "/static/default/item_default.image";
    }
}
