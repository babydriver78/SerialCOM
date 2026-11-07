// Whole-application settings persisted to %AppData%/SerialCOM/settings.json.
namespace SerialCOM.Models;

/// <summary>
/// Everything the app remembers between runs: connection, TX/RX view
/// options, logging, command history, window size and UI preferences.
/// New UI properties have defaults so old settings.json files
/// (without those keys) still load correctly.
/// </summary>
public sealed class AppSettings
{
    public SerialSettings Serial { get; set; } = new();

    // TX options
    public string TxMode { get; set; } = "ASCII";       // ASCII | HEX
    public string LineEnding { get; set; } = "CRLF";     // None | LF | CR | CRLF
    public string EncodingName { get; set; } = "UTF8";   // ASCII | UTF8 | Latin1

    // View / log options
    public string DisplayMode { get; set; } = "ASCII";   // ASCII | HEX
    public bool LocalEcho { get; set; } = true;
    public bool ShowTimestamp { get; set; }
    public bool AutoScroll { get; set; } = true;
    public bool WordWrap { get; set; } = true;
    public bool EnableLogging { get; set; }
    public string LogPath { get; set; } = "";
    public bool AutoReconnect { get; set; }

    public List<string> History { get; set; } = new();

    public int WindowWidth { get; set; } = 900;
    public int WindowHeight { get; set; } = 600;

    // UI preferences (added later; defaults keep old JSON files working).
    public bool DarkTerminal { get; set; }
    public bool ColorizeTerminal { get; set; } = true;
    public bool AlwaysOnTop { get; set; }
    public int SplitterDistance { get; set; } = 264;

    public static AppSettings Default() => new();
}
