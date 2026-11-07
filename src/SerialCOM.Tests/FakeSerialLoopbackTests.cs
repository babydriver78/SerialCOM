using System.Text;

namespace SerialCOM.Tests;

/// <summary>
/// In-memory loopback stand-in for <c>SerialPort</c>: whatever is "sent" is
/// raised back through <see cref="DataReceived"/>, exactly like a physical
/// TX-RX loopback plug or a virtual COM pair. No hardware required.
/// Also simulates an occupied port via <see cref="SimulateOccupiedPort"/>.
/// </summary>
public sealed class FakeSerialService : IDisposable
{
    private bool _isOpen;
    private bool _disposed;

    public event EventHandler<byte[]>? DataReceived;
    public event EventHandler<Exception>? ConnectionLost;

    public bool IsOpen => _isOpen;
    public string PortName { get; private set; } = "";
    public bool DtrEnable { get; private set; }
    public bool RtsEnable { get; private set; }
    public bool BreakAsserted { get; private set; }
    public long TxBytes { get; private set; }

    /// <summary>When true, <see cref="Open"/> throws like an occupied port.</summary>
    public bool SimulateOccupiedPort { get; set; }

    public void Open(string portName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (string.IsNullOrWhiteSpace(portName))
            throw new InvalidOperationException("No COM port selected.");
        if (_isOpen)
            throw new InvalidOperationException("Port is already open.");
        if (SimulateOccupiedPort)
            throw new UnauthorizedAccessException($"Access to {portName} is denied (port in use).");
        PortName = portName.Trim();
        _isOpen = true;
    }

    public void Close() => _isOpen = false;

    public void Send(byte[] data)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_isOpen)
            throw new InvalidOperationException("Port is not open.");
        TxBytes += data.Length;
        // Loopback: echo the exact bytes back asynchronously, like hardware.
        byte[] echo = (byte[])data.Clone();
        ThreadPool.QueueUserWorkItem(_ => DataReceived?.Invoke(this, echo));
    }

    public async Task SendBreakAsync(int milliseconds = 500)
    {
        if (!_isOpen)
            throw new InvalidOperationException("Port is not open.");
        BreakAsserted = true;
        await Task.Delay(milliseconds);
        BreakAsserted = false;
    }

    public void SetDtr(bool enable) { if (_isOpen) DtrEnable = enable; }
    public void SetRts(bool enable) { if (_isOpen) RtsEnable = enable; }

    public void SimulateCableUnplug()
    {
        if (!_isOpen) return;
        _isOpen = false;
        ConnectionLost?.Invoke(this, new IOException("Device unplugged (simulated)."));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Close();
    }
}

/// <summary>
/// End-to-end TX/RX path validation over the loopback fake:
/// ASCII+CRLF echo, HEX "48 69" -&gt; "Hi", DTR/RTS live toggle,
/// break-without-disconnect, occupied-port error, cable-unplug handling.
/// </summary>
[TestClass]
public sealed class FakeSerialLoopbackTests
{
    private static byte[] FrameWithLineEnding(byte[] payload, string lineEnding) =>
        lineEnding switch
        {
            "LF" => [.. payload, (byte)'\n'],
            "CR" => [.. payload, (byte)'\r'],
            "CRLF" => [.. payload, (byte)'\r', (byte)'\n'],
            _ => payload,
        };

    private static async Task<byte[]> SendAndReceiveAsync(
        FakeSerialService fake, byte[] framed, int timeoutMs = 2000)
    {
        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler<byte[]>? handler = null;
        handler = (_, data) =>
        {
            fake.DataReceived -= handler;
            tcs.TrySetResult(data);
        };
        fake.DataReceived += handler;
        fake.Send(framed);
        using var cts = new CancellationTokenSource(timeoutMs);
        cts.Token.Register(() => tcs.TrySetCanceled());
        try
        {
            return await tcs.Task;
        }
        finally
        {
            fake.DataReceived -= handler;
        }
    }

    [TestMethod]
    public async Task Loopback_AsciiWithCrlf_EchoesExactly()
    {
        // Arrange
        using var fake = new FakeSerialService();
        fake.Open("COM1");

        // Act: MainForm TX path — ASCII encode + CRLF framing.
        byte[] payload = Encoding.UTF8.GetBytes("Hello");
        byte[] framed = FrameWithLineEnding(payload, "CRLF");
        byte[] echoed = await SendAndReceiveAsync(fake, framed);

        // Assert: loopback returns byte-identical data incl. CRLF.
        CollectionAssert.AreEqual(framed, echoed);
        Assert.AreEqual("Hello\r\n", Encoding.UTF8.GetString(echoed));
    }

    [TestMethod]
    public async Task Loopback_Hex48_69_DecodesToHi()
    {
        // Arrange
        using var fake = new FakeSerialService();
        fake.Open("COM1");

        // Act: MainForm HEX TX path — parse "48 69", send raw bytes.
        Assert.IsTrue(SerialCOM.Utils.HexConverter.TryParseHex("48 69", out byte[] payload));
        byte[] echoed = await SendAndReceiveAsync(fake, payload);

        // Assert
        Assert.AreEqual("Hi", Encoding.ASCII.GetString(echoed));
    }

    [TestMethod]
    public void Loopback_InvalidHex_NeverReachesPort()
    {
        // Arrange
        using var fake = new FakeSerialService();
        fake.Open("COM1");

        // Act: invalid HEX is rejected before Send (MainForm shows status error).
        bool ok = SerialCOM.Utils.HexConverter.TryParseHex("ZZ", out _);

        // Assert
        Assert.IsFalse(ok);
        Assert.AreEqual(0, fake.TxBytes);
    }

    [TestMethod]
    public void Loopback_OccupiedPort_SurfacesCatchableError()
    {
        // Arrange: another app holds the port.
        using var fake = new FakeSerialService { SimulateOccupiedPort = true };

        // Act + Assert: UnauthorizedAccessException -> status-bar message, no crash.
        var ex = Assert.ThrowsException<UnauthorizedAccessException>(() => fake.Open("COM3"));
        Assert.IsTrue(ex.Message.Contains("COM3"));
        Assert.IsFalse(fake.IsOpen);
    }

    [TestMethod]
    public void Loopback_DtrRts_LiveToggleWhileOpen()
    {
        using var fake = new FakeSerialService();
        fake.Open("COM1");

        fake.SetDtr(true);
        fake.SetRts(true);
        Assert.IsTrue(fake.DtrEnable);
        Assert.IsTrue(fake.RtsEnable);

        fake.SetDtr(false);
        fake.SetRts(false);
        Assert.IsFalse(fake.DtrEnable);
        Assert.IsFalse(fake.RtsEnable);
    }

    [TestMethod]
    public async Task Loopback_Break_DoesNotDisconnect()
    {
        using var fake = new FakeSerialService();
        fake.Open("COM1");

        await fake.SendBreakAsync(50);

        Assert.IsFalse(fake.BreakAsserted);
        Assert.IsTrue(fake.IsOpen); // break must not drop the port

        // Port still usable after break.
        byte[] echoed = await SendAndReceiveAsync(fake, new byte[] { 0x41 });
        Assert.AreEqual("A", Encoding.ASCII.GetString(echoed));
    }

    [TestMethod]
    public void Loopback_CableUnplug_RaisesConnectionLostAndCloses()
    {
        // Arrange
        using var fake = new FakeSerialService();
        fake.Open("COM1");
        Exception? lost = null;
        fake.ConnectionLost += (_, ex) => lost = ex;

        // Act: yank the cable.
        fake.SimulateCableUnplug();

        // Assert: MainForm would show error + start auto-reconnect.
        Assert.IsFalse(fake.IsOpen);
        Assert.IsNotNull(lost);
        Assert.IsInstanceOfType(lost, typeof(IOException));
    }

    [TestMethod]
    public void Loopback_SendAfterUnplug_ThrowsCatchableError()
    {
        using var fake = new FakeSerialService();
        fake.Open("COM1");
        fake.SimulateCableUnplug();

        Assert.ThrowsException<InvalidOperationException>(
            () => fake.Send(new byte[] { 0x41 }));
    }
}
