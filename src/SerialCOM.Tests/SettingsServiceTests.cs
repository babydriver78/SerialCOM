using SerialCOM.Models;
using SerialCOM.Services;

namespace SerialCOM.Tests;

/// <summary>
/// Validates JSON settings persistence (%AppData%/SerialCOM/settings.json).
/// Spec: save/reload must restore ALL settings after restart.
/// </summary>
[TestClass]
public sealed class SettingsServiceTests
{
    private static string NewTempSettingsPath()
        => Path.Combine(Path.GetTempPath(), "SerialCOM-tests",
            $"settings_{Guid.NewGuid():N}.json");

    private static AppSettings FullSample() => new()
    {
        Serial = new SerialSettings
        {
            PortName = "COM7",
            BaudRate = 230400,
            DataBits = 7,
            Parity = "Even",
            StopBits = "Two",
            Handshake = "RequestToSend",
            DtrEnable = true,
            RtsEnable = true,
        },
        TxMode = "HEX",
        LineEnding = "LF",
        EncodingName = "Latin1",
        DisplayMode = "HEX",
        LocalEcho = false,
        ShowTimestamp = true,
        AutoScroll = false,
        WordWrap = false,
        EnableLogging = true,
        LogPath = "./logs/custom.log",
        AutoReconnect = true,
        History = new List<string> { "first", "second", "third" },
        WindowWidth = 1234,
        WindowHeight = 777,
        DarkTerminal = true,
        ColorizeTerminal = false,
        AlwaysOnTop = true,
        SplitterDistance = 300,
    };

    private static void AssertSame(AppSettings expected, AppSettings actual)
    {
        Assert.AreEqual(expected.Serial.PortName, actual.Serial.PortName);
        Assert.AreEqual(expected.Serial.BaudRate, actual.Serial.BaudRate);
        Assert.AreEqual(expected.Serial.DataBits, actual.Serial.DataBits);
        Assert.AreEqual(expected.Serial.Parity, actual.Serial.Parity);
        Assert.AreEqual(expected.Serial.StopBits, actual.Serial.StopBits);
        Assert.AreEqual(expected.Serial.Handshake, actual.Serial.Handshake);
        Assert.AreEqual(expected.Serial.DtrEnable, actual.Serial.DtrEnable);
        Assert.AreEqual(expected.Serial.RtsEnable, actual.Serial.RtsEnable);
        Assert.AreEqual(expected.TxMode, actual.TxMode);
        Assert.AreEqual(expected.LineEnding, actual.LineEnding);
        Assert.AreEqual(expected.EncodingName, actual.EncodingName);
        Assert.AreEqual(expected.DisplayMode, actual.DisplayMode);
        Assert.AreEqual(expected.LocalEcho, actual.LocalEcho);
        Assert.AreEqual(expected.ShowTimestamp, actual.ShowTimestamp);
        Assert.AreEqual(expected.AutoScroll, actual.AutoScroll);
        Assert.AreEqual(expected.WordWrap, actual.WordWrap);
        Assert.AreEqual(expected.EnableLogging, actual.EnableLogging);
        Assert.AreEqual(expected.LogPath, actual.LogPath);
        Assert.AreEqual(expected.AutoReconnect, actual.AutoReconnect);
        CollectionAssert.AreEqual(expected.History, actual.History);
        Assert.AreEqual(expected.WindowWidth, actual.WindowWidth);
        Assert.AreEqual(expected.WindowHeight, actual.WindowHeight);
        Assert.AreEqual(expected.DarkTerminal, actual.DarkTerminal);
        Assert.AreEqual(expected.ColorizeTerminal, actual.ColorizeTerminal);
        Assert.AreEqual(expected.AlwaysOnTop, actual.AlwaysOnTop);
        Assert.AreEqual(expected.SplitterDistance, actual.SplitterDistance);
    }

    [TestMethod]
    public void SaveLoad_FullSettings_RoundTrips()
    {
        // Arrange
        var service = new SettingsService(NewTempSettingsPath());
        var expected = FullSample();

        // Act
        Assert.IsTrue(service.Save(expected));
        AppSettings actual = service.Load();

        // Assert: every persisted field survives a restart.
        AssertSame(expected, actual);
    }

    [TestMethod]
    public void SaveLoad_HistoryWithFiftyEntries_RoundTrips()
    {
        var service = new SettingsService(NewTempSettingsPath());
        var settings = new AppSettings
        {
            History = Enumerable.Range(0, 50).Select(i => $"cmd{i}").ToList(),
        };

        Assert.IsTrue(service.Save(settings));
        AppSettings loaded = service.Load();

        Assert.AreEqual(50, loaded.History.Count);
        CollectionAssert.AreEqual(settings.History, loaded.History);
    }

    [TestMethod]
    public void Load_MissingFile_ReturnsDefaults()
    {
        // Arrange: path that was never written.
        var service = new SettingsService(NewTempSettingsPath());

        // Act
        AppSettings settings = service.Load();

        // Assert: defaults per AppSettings/SerialSettings.
        Assert.AreEqual(115200, settings.Serial.BaudRate);
        Assert.AreEqual(8, settings.Serial.DataBits);
        Assert.AreEqual("None", settings.Serial.Parity);
        Assert.AreEqual("One", settings.Serial.StopBits);
        Assert.AreEqual("ASCII", settings.TxMode);
        Assert.AreEqual("CRLF", settings.LineEnding);
        Assert.AreEqual("UTF8", settings.EncodingName);
        Assert.IsTrue(settings.LocalEcho);
        Assert.IsTrue(settings.AutoScroll);
    }

    [TestMethod]
    public void Load_CorruptJson_ReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange
        string path = NewTempSettingsPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "{ this is not valid json !!!");
        var service = new SettingsService(path);

        // Act (must not throw)
        AppSettings settings = service.Load();

        // Assert
        Assert.IsNotNull(settings);
        Assert.AreEqual(115200, settings.Serial.BaudRate);
    }

    [TestMethod]
    public void Load_OldJsonWithoutNewPrefs_LoadsWithDefaults()
    {
        // Arrange: settings.json written before DarkTerminal/ColorizeTerminal/
        // AlwaysOnTop/SplitterDistance existed — those keys are missing.
        string path = NewTempSettingsPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, """
            {
              "serial": { "portName": "COM3", "baudRate": 9600 },
              "txMode": "ASCII",
              "windowWidth": 900,
              "windowHeight": 600
            }
            """);
        var service = new SettingsService(path);

        // Act
        AppSettings settings = service.Load();

        // Assert: old values load, new prefs fall back to defaults (no throw).
        Assert.AreEqual("COM3", settings.Serial.PortName);
        Assert.AreEqual(9600, settings.Serial.BaudRate);
        Assert.IsFalse(settings.DarkTerminal);
        Assert.IsTrue(settings.ColorizeTerminal);
        Assert.IsFalse(settings.AlwaysOnTop);
        Assert.AreEqual(264, settings.SplitterDistance);
    }

    [TestMethod]
    public void Load_CaseInsensitiveJson_LoadsValues()
    {
        // Arrange: hand-edited PascalCase file (loader must accept any casing).
        string path = NewTempSettingsPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, """
            {
              "TxMode": "HEX",
              "DarkTerminal": true,
              "ColorizeTerminal": false,
              "AlwaysOnTop": true,
              "SplitterDistance": 320
            }
            """);
        var service = new SettingsService(path);

        // Act
        AppSettings settings = service.Load();

        // Assert
        Assert.AreEqual("HEX", settings.TxMode);
        Assert.IsTrue(settings.DarkTerminal);
        Assert.IsFalse(settings.ColorizeTerminal);
        Assert.IsTrue(settings.AlwaysOnTop);
        Assert.AreEqual(320, settings.SplitterDistance);
    }

    [TestMethod]
    public void DefaultSettings_MatchSpecDefaults()
    {
        var defaults = AppSettings.Default();
        Assert.AreEqual(115200, defaults.Serial.BaudRate);
        Assert.AreEqual(8, defaults.Serial.DataBits);
        Assert.AreEqual("None", defaults.Serial.Parity);
        Assert.AreEqual(900, defaults.WindowWidth);
        Assert.AreEqual(600, defaults.WindowHeight);
    }

    [TestMethod]
    public void SerialSettings_DataFormatSummary_Formats8N1()
    {
        Assert.AreEqual("8N1", new SerialSettings().DataFormatSummary());
        Assert.AreEqual("7E2", new SerialSettings
        {
            DataBits = 7, Parity = "Even", StopBits = "Two",
        }.DataFormatSummary());
        Assert.AreEqual("8N1.5", new SerialSettings
        {
            StopBits = "OnePointFive",
        }.DataFormatSummary());
    }
}
