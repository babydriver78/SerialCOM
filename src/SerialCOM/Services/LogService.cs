namespace SerialCOM.Services;

/// <summary>
/// Simple line-oriented session logger: "[timestamp] [RX/TX] payload".
/// One file per session, StreamWriter with AutoFlush so nothing is lost
/// on cable-pull or crash. Thread-safe for DataReceived callbacks.
/// </summary>
public sealed class LogService : IDisposable
{
    private readonly object _gate = new();
    private StreamWriter? _writer;
    private bool _disposed;

    public bool IsOpen => _writer is not null;
    public string CurrentPath { get; private set; } = "";

    /// <summary>Default path: ./logs/SerialCOM_yyyyMMdd_HHmmss.log</summary>
    public static string DefaultPathForNow()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "logs");
        return Path.Combine(dir, $"SerialCOM_{DateTime.Now:yyyyMMdd_HHmmss}.log");
    }

    public void Open(string? path)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Close();

        string target = string.IsNullOrWhiteSpace(path) ? DefaultPathForNow() : path.Trim();
        string? dir = Path.GetDirectoryName(Path.GetFullPath(target));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        lock (_gate)
        {
            _writer = new StreamWriter(target, append: false, System.Text.Encoding.UTF8)
            {
                AutoFlush = true,
            };
            CurrentPath = target;
        }
    }

    public void WriteRx(string text) => Write("RX", text);
    public void WriteTx(string text) => Write("TX", text);

    public void Write(string direction, string text)
    {
        lock (_gate)
        {
            _writer?.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{direction}] {text}");
        }
    }

    public void Close()
    {
        lock (_gate)
        {
            _writer?.Flush();
            _writer?.Dispose();
            _writer = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        Close();
    }
}
