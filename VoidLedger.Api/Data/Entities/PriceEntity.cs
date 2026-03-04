namespace VoidLedger.Api.Data.Entities
{
    public class PriceEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = "";
    }
}
