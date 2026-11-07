using System.IO.Ports;
using SerialCOM.Models;

namespace SerialCOM.Services;

/// <summary>
/// Thin wrapper around <see cref="SerialPort"/>: open/close/send/break,
/// raw-byte DataReceived event, live DTR/RTS. Never throws from Dispose.
/// </summary>
public sealed class SerialService : IDisposable
{
    private SerialPort? _port;
    private bool _disposed;

    /// <summary>Raised on a background thread with the bytes just received.</summary>
    public event EventHandler<byte[]>? DataReceived;

    /// <summary>Fired when the port drops unexpectedly (cable pulled, etc.).</summary>
    public event EventHandler<Exception>? ConnectionLost;

    /// <summary>Non-fatal line noise (framing/overrun). Port stays open.</summary>
    public event EventHandler<string>? LineError;

    public bool IsOpen => _port?.IsOpen ?? false;
    public string PortName => _port?.PortName ?? "";

    public void Open(SerialSettings settings)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (IsOpen)
            throw new InvalidOperationException("Port is already open.");

        if (string.IsNullOrWhiteSpace(settings.PortName))
            throw new InvalidOperationException("No COM port selected.");

        var port = new SerialPort
        {
            PortName = settings.PortName.Trim(),
            BaudRate = settings.BaudRate,
            DataBits = settings.DataBits,
            Parity = MapParity(settings.Parity),
            StopBits = MapStopBits(settings.StopBits),
            Handshake = MapHandshake(settings.Handshake),
            DtrEnable = settings.DtrEnable,
            RtsEnable = settings.RtsEnable,
            ReadTimeout = 500,
            WriteTimeout = 1000,
        };

        port.DataReceived += OnPortDataReceived;
        port.ErrorReceived += OnPortError;

        // Let UnauthorizedAccessException / IOException / ArgumentException
        // propagate — the form turns them into status-bar messages.
        port.Open();

        // Discard anything stale sitting in driver buffers.
        port.DiscardInBuffer();
        _port = port;
    }

    public void Close()
    {
        var port = Interlocked.Exchange(ref _port, null);
        if (port is null)
            return;

        try
        {
            port.DataReceived -= OnPortDataReceived;
            port.ErrorReceived -= OnPortError;
            if (port.IsOpen)
                port.Close();
        }
        catch (IOException)
        {
            // Port already gone (unplugged) — nothing to do.
        }
        finally
        {
            port.Dispose();
        }
    }

    public void Send(byte[] data)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var port = _port ?? throw new InvalidOperationException("Port is not open.");
        port.Write(data, 0, data.Length);
    }

    /// <summary>Hold the break condition for <paramref name="milliseconds"/>.</summary>
    public async Task SendBreakAsync(int milliseconds = 500, CancellationToken ct = default)
    {
        var port = _port;
        if (port is null || !port.IsOpen)
            throw new InvalidOperationException("Port is not open.");
        try
        {
            port.BreakState = true;
            await Task.Delay(milliseconds, ct);
        }
        finally
        {
            try { port.BreakState = false; } catch (IOException) { }
        }
    }

    public void SetDtr(bool enable)
    {
        if (_port?.IsOpen == true)
            _port.DtrEnable = enable;
    }

    public void SetRts(bool enable)
    {
        if (_port?.IsOpen == true)
            _port.RtsEnable = enable;
    }

    private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var port = (SerialPort)sender;
        try
        {
            int n = port.BytesToRead;
            if (n <= 0)
                return;
            var buf = new byte[n];
            int read = port.Read(buf, 0, n);
            if (read <= 0)
                return;
            if (read < n)
                Array.Resize(ref buf, read);
            DataReceived?.Invoke(this, buf);
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or UnauthorizedAccessException)
        {
            ConnectionLost?.Invoke(this, ex);
        }
    }

    private void OnPortError(object sender, SerialErrorReceivedEventArgs e)
    {
        // RXOver / Overrun / framing noise is common on real hardware;
        // report it but keep the port open.
        LineError?.Invoke(this, $"Serial line error: {e.EventType}");
    }

    // --- string -> enum mapping (combo box text to SerialPort enums) ---

    public static Parity MapParity(string? name) => name?.Trim().ToLowerInvariant() switch
    {
        "odd" => Parity.Odd,
        "even" => Parity.Even,
        "mark" => Parity.Mark,
        "space" => Parity.Space,
        _ => Parity.None,
    };

    public static StopBits MapStopBits(string? name) => name?.Trim().ToLowerInvariant() switch
    {
        "onepointfive" or "1.5" => StopBits.OnePointFive,
        "two" or "2" => StopBits.Two,
        "none" => StopBits.None,
        _ => StopBits.One,
    };

    public static Handshake MapHandshake(string? name) => name?.Trim().ToLowerInvariant() switch
    {
        "xonxoff" or "xonex" or "xon/xoff" => Handshake.XOnXOff,
        "requesttosend" or "rts" => Handshake.RequestToSend,
        "requesttosendxonxoff" or "rts+xon" => Handshake.RequestToSendXOnXOff,
        _ => Handshake.None,
    };

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        Close();
    }
}
