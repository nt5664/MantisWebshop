using Microsoft.AspNetCore.Mvc;

namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class ActionResponse
    {
        public required string Message { get; set; }
        public object? Data { get; set; }
    }
}
