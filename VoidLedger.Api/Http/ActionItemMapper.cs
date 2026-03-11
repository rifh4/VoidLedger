using VoidLedger.Api.Contracts;
using VoidLedger.Core;

namespace VoidLedger.Api.Http
{
    internal static class ActionItemMapper
    {
        internal static ActionItemResponse Map(ActionRecordBase action)
        {
            string? name = null;
            int? quantity = null;
            decimal? amount = null;
            decimal? unitPrice = null;
            decimal? total = null;
            decimal? price = null;

            if (action is DepositAction deposit)
            {
                amount = deposit.Amount;
            }
            else if (action is SetPriceAction setPrice)
            {
                name = setPrice.Name;
                price = setPrice.Price;
            }
            else if (action is BuyAction buy)
            {
                name = buy.Name;
                quantity = buy.Qty;
                unitPrice = buy.UnitPrice;
                total = buy.Total;
            }
            else if (action is SellAction sell)
            {
                name = sell.Name;
                quantity = sell.Qty;
                unitPrice = sell.UnitPrice;
                total = sell.Total;
            }

            return new ActionItemResponse(
                Type: action.Type,
                At: action.At,
                Name: name,
                Quantity: quantity,
                Amount: amount,
                UnitPrice: unitPrice,
                Total: total,
                Price: price
            );
        }
    }
}
