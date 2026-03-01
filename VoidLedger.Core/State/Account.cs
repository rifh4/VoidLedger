namespace VoidLedger.Core;

public sealed class Account
{
    public decimal Balance { get; private set; }

    public Account(decimal startingBalance)
    {
        if (startingBalance < 0) startingBalance = 0;
        Balance = startingBalance;
    }

    public bool Deposit(decimal amount)
    {
        if (amount <= 0) return false;
        Balance += amount;
        return true;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) return false;
        if (amount > Balance) return false;
        Balance -= amount;
        return true;
    }
}