using System.Text.RegularExpressions;

namespace SerialCOM.Utils;

/// <summary>
/// Helpers to parse user-typed HEX ("48 65 6C") and to format
/// received bytes as HEX with 16 bytes per line.
/// </summary>
public static partial class HexConverter
{
    // Pairs of hex digits, whitespace between pairs is allowed.
    [GeneratedRegex(@"^([0-9A-Fa-f]{2}\s*)+$", RegexOptions.Compiled)]
    private static partial Regex HexPairRegex();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();

    /// <summary>True when text looks like space-separated hex byte pairs.</summary>
    public static bool IsValidHex(string? text)
        => !string.IsNullOrWhiteSpace(text) && HexPairRegex().IsMatch(text.Trim());

    /// <summary>
    /// Parse "48 65 6C6C 6F" into bytes. Returns false (and empty array)
    /// when the text is not valid hex.
    /// </summary>
    public static bool TryParseHex(string? text, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        if (string.IsNullOrWhiteSpace(text))
            return false;

        string compact = WhitespaceRegex().Replace(text.Trim(), "");
        if (compact.Length == 0 || compact.Length % 2 != 0)
            return false;

        var result = new byte[compact.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            string pair = compact.Substring(i * 2, 2);
            if (!byte.TryParse(pair, System.Globalization.NumberStyles.HexNumber, null, out byte b))
                return false;
            result[i] = b;
        }

        bytes = result;
        return true;
    }

    /// <summary>
    /// Format bytes as "48 65 6C …" with 16 bytes per line.
    /// Empty input yields an empty string.
    /// </summary>
    public static string ToHexString(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
            return string.Empty;

        // "XX " per byte + newline every 16 bytes: pre-size roughly.
        var sb = new System.Text.StringBuilder(data.Length * 3);
        for (int i = 0; i < data.Length; i++)
        {
            sb.Append(data[i].ToString("X2"));
            bool endOfLine = (i + 1) % 16 == 0;
            if (i < data.Length - 1)
                sb.Append(endOfLine ? Environment.NewLine : " ");
        }
        return sb.ToString();
    }

    public static string ToHexString(byte[]? data)
        => data is null || data.Length == 0 ? string.Empty : ToHexString((ReadOnlySpan<byte>)data);
}
