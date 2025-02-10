namespace MantisWebshop.Server.Models
{
    public struct CartData
    {
        public string ProductId { get; set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return $"Id: {ProductId} | Amount: {Amount}";
        }
    }
}
