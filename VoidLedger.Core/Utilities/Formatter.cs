using System.Globalization;

namespace VoidLedger.Core;

public static class Formatter
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public static string Money(decimal value) =>
        value.ToString("F2", Culture);

    public static string UtcStamp(DateTime utc) =>
        utc.ToString("yyyy-MM-dd HH:mm 'UTC'", Culture);
}