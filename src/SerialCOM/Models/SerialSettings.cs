// Serial port connection settings (persisted as part of AppSettings).
namespace SerialCOM.Models;

/// <summary>
/// Connection parameters for a single serial port.
/// Stored as plain strings/ints so JSON stays human-editable;
/// <see cref="Services.SerialService"/> maps them to Parity/StopBits/Handshake enums.
/// </summary>
public sealed class SerialSettings
{
    public string PortName { get; set; } = "";
    public int BaudRate { get; set; } = 115200;
    public int DataBits { get; set; } = 8;
    public string Parity { get; set; } = "None";
    public string StopBits { get; set; } = "One";
    public string Handshake { get; set; } = "None";
    public bool DtrEnable { get; set; }
    public bool RtsEnable { get; set; }

    /// <summary>Short "8N1" style summary used in the status bar.</summary>
    public string DataFormatSummary()
    {
        char p = Parity.Length > 0 ? char.ToUpperInvariant(Parity[0]) : 'N';
        string s = StopBits switch
        {
            "One" => "1",
            "OnePointFive" => "1.5",
            "Two" => "2",
            _ => "1",
        };
        return $"{DataBits}{p}{s}";
    }
}
