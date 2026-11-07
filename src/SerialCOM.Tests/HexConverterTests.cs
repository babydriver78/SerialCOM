using System.Text;
using SerialCOM.Utils;

namespace SerialCOM.Tests;

/// <summary>
/// Validates HEX parsing/formatting (MainForm TX-path dependency).
/// Spec: HEX send "48 69" decodes to "Hi"; bad HEX like "ZZ" shows a
/// status error and never crashes; HEX view uses 16 bytes per line.
/// </summary>
[TestClass]
public sealed class HexConverterTests
{
    [TestMethod]
    public void TryParseHex_ValidSpacedPair_DecodesToHi()
    {
        // Arrange
        const string input = "48 69";

        // Act
        bool ok = HexConverter.TryParseHex(input, out byte[] bytes);

        // Assert
        Assert.IsTrue(ok);
        CollectionAssert.AreEqual(new byte[] { 0x48, 0x69 }, bytes);
        Assert.AreEqual("Hi", Encoding.ASCII.GetString(bytes));
    }

    [TestMethod]
    public void TryParseHex_InvalidChars_Fails()
    {
        // Arrange + Act
        bool ok = HexConverter.TryParseHex("ZZ", out byte[] bytes);

        // Assert
        Assert.IsFalse(ok);
        Assert.AreEqual(0, bytes.Length);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("4")]        // odd nibble
    [DataRow("48 6")]     // odd nibble after space strip
    [DataRow("48 ZZ 41")]
    [DataRow("0x48")]     // prefixes not accepted
    [DataRow("48,69")]    // commas not accepted
    public void TryParseHex_MalformedInputs_Fail(string input)
    {
        Assert.IsFalse(HexConverter.TryParseHex(input, out _));
    }

    [TestMethod]
    public void TryParseHex_NullInput_Fails()
    {
        Assert.IsFalse(HexConverter.TryParseHex(null, out byte[] bytes));
        Assert.AreEqual(0, bytes.Length);
    }

    [TestMethod]
    public void TryParseHex_CompactAndMixedCase_Parses()
    {
        // "Hello" as compact + lowercase mix.
        Assert.IsTrue(HexConverter.TryParseHex("48656c6C6F", out byte[] compact));
        Assert.IsTrue(HexConverter.TryParseHex("48 65 6c 6c 6f", out byte[] spaced));
        CollectionAssert.AreEqual(compact, spaced);
        Assert.AreEqual("Hello", Encoding.ASCII.GetString(spaced));
    }

    [TestMethod]
    public void TryParseHex_TabAndNewlineWhitespace_Parses()
    {
        Assert.IsTrue(HexConverter.TryParseHex("48\t69\r\n41", out byte[] bytes));
        CollectionAssert.AreEqual(new byte[] { 0x48, 0x69, 0x41 }, bytes);
    }

    [TestMethod]
    public void IsValidHex_AcceptsSpecRegex()
    {
        Assert.IsTrue(HexConverter.IsValidHex("48 65 6C"));
        Assert.IsTrue(HexConverter.IsValidHex("48656C"));
        Assert.IsFalse(HexConverter.IsValidHex("ZZ"));
        Assert.IsFalse(HexConverter.IsValidHex(""));
        Assert.IsFalse(HexConverter.IsValidHex(null));
    }

    [TestMethod]
    public void ToHexString_ShortInput_SpaceSeparatedUppercase()
    {
        // Arrange
        byte[] data = Encoding.ASCII.GetBytes("He"); // 48 65

        // Act
        string hex = HexConverter.ToHexString(data);

        // Assert
        Assert.AreEqual("48 65", hex);
    }

    [TestMethod]
    public void ToHexString_SixteenBytesPerLine()
    {
        // Arrange: 32 bytes -> exactly 2 lines of 16.
        var data = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();

        // Act
        string hex = HexConverter.ToHexString(data);
        string[] lines = hex.Split(Environment.NewLine);

        // Assert
        Assert.AreEqual(2, lines.Length);
        foreach (string line in lines)
            Assert.AreEqual(16, line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        Assert.IsTrue(lines[0].StartsWith("00 01"));
        Assert.IsTrue(lines[1].StartsWith("10 11"));
    }

    [TestMethod]
    public void ToHexString_PartialLastLine_NoTrailingSeparator()
    {
        byte[] data = Enumerable.Range(0, 18).Select(i => (byte)i).ToArray();
        string hex = HexConverter.ToHexString(data);
        string[] lines = hex.Split(Environment.NewLine);
        Assert.AreEqual(2, lines.Length);
        Assert.AreEqual(16, lines[0].Split(' ').Length);
        Assert.AreEqual(2, lines[1].Split(' ').Length);
        Assert.IsFalse(hex.EndsWith(" "));
        Assert.IsFalse(hex.EndsWith(Environment.NewLine));
    }

    [TestMethod]
    public void ToHexString_EmptyOrNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, HexConverter.ToHexString(Array.Empty<byte>()));
        Assert.AreEqual(string.Empty, HexConverter.ToHexString((byte[]?)null));
        Assert.AreEqual(string.Empty, HexConverter.ToHexString(ReadOnlySpan<byte>.Empty));
    }

    [TestMethod]
    public void HexRoundTrip_ParseOfFormatted_AreEqual()
    {
        // Arrange
        var original = Enumerable.Range(0, 40).Select(i => (byte)i).ToArray();

        // Act
        string formatted = HexConverter.ToHexString(original);
        bool ok = HexConverter.TryParseHex(formatted, out byte[] parsed);

        // Assert
        Assert.IsTrue(ok);
        CollectionAssert.AreEqual(original, parsed);
    }
}
