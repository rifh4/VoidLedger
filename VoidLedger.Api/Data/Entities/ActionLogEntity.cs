using VoidLedger.Core;

namespace VoidLedger.Api.Data.Entities
{
    public class ActionLogEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public ActionType Type { get; set; }
        public DateTime AtUtc { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Total { get; set; }
        public decimal? Amount { get; set; }
    }
}
