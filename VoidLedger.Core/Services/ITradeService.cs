namespace VoidLedger.Core.Services
{
    public interface ITradeService
    {
        Task<OpResult> BuyAsync(string name, int qty);
        Task<OpResult> SellAsync(string name, int qty);
    }
}
