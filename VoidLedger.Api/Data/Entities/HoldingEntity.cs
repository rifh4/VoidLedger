namespace VoidLedger.Api.Data.Entities
{
    public class HoldingEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
    }
}
