namespace VoidLedger.Core.Utilities
{
    public static class NameNormalizer
    {
        public static string Normalize(string? name)
        {
            return (name ?? "").Trim().ToUpperInvariant();
        }
    }
}
