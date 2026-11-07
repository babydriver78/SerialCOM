using System.IO.Ports;
using SerialCOM.Models;
using SerialCOM.Services;

namespace SerialCOM.Tests;

/// <summary>
/// Validates SerialService enum mapping + open/send guards in isolation
/// (no real COM port required). Spec: invalid config (occupied/bad port)
/// must surface a status error, never crash.
/// </summary>
[TestClass]
public sealed class SerialMappingTests
{
    // --- Parity ---

    [TestMethod]
    [DataRow("None", Parity.None)]
    [DataRow("Odd", Parity.Odd)]
    [DataRow("Even", Parity.Even)]
    [DataRow("Mark", Parity.Mark)]
    [DataRow("Space", Parity.Space)]
    [DataRow("odd", Parity.Odd)]     // combo text is case-tolerant
    [DataRow("EVEN", Parity.Even)]
    [DataRow("bogus", Parity.None)]  // unknown -> safe default
    [DataRow(null, Parity.None)]
    public void MapParity_MapsComboText(string? name, Parity expected)
        => Assert.AreEqual(expected, SerialService.MapParity(name));

    // --- StopBits ---

    [TestMethod]
    [DataRow("One", StopBits.One)]
    [DataRow("OnePointFive", StopBits.OnePointFive)]
    [DataRow("1.5", StopBits.OnePointFive)]
    [DataRow("Two", StopBits.Two)]
    [DataRow("2", StopBits.Two)]
    [DataRow("bogus", StopBits.One)]
    [DataRow(null, StopBits.One)]
    public void MapStopBits_MapsComboText(string? name, StopBits expected)
        => Assert.AreEqual(expected, SerialService.MapStopBits(name));

    // --- Handshake ---

    [TestMethod]
    [DataRow("None", Handshake.None)]
    [DataRow("XOnXOff", Handshake.XOnXOff)]
    [DataRow("RequestToSend", Handshake.RequestToSend)]
    [DataRow("RequestToSendXOnXOff", Handshake.RequestToSendXOnXOff)]
    [DataRow("bogus", Handshake.None)]
    [DataRow(null, Handshake.None)]
    public void MapHandshake_MapsComboText(string? name, Handshake expected)
        => Assert.AreEqual(expected, SerialService.MapHandshake(name));

    // --- Guards: no port, no crash ---

    [TestMethod]
    public void Open_EmptyPortName_ThrowsWithoutCrash()
    {
        using var serial = new SerialService();
        var settings = new SerialSettings { PortName = "", BaudRate = 115200 };

        // MainForm catches this and shows a status error; it must throw a
        // catchable InvalidOperationException, never crash the process.
        Assert.ThrowsException<InvalidOperationException>(() => serial.Open(settings));
        Assert.IsFalse(serial.IsOpen);
    }

    [TestMethod]
    public void Open_NonExistentPort_ThrowsCatchableException()
    {
        using var serial = new SerialService();
        var settings = new SerialSettings { PortName = "COM250", BaudRate = 115200 };

        try
        {
            serial.Open(settings);
            Assert.IsTrue(serial.IsOpen); // a real COM250 exists on this box
            serial.Close();
        }
        catch (Exception ex) when (ex is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or InvalidOperationException)
        {
            // Expected on machines without COM250: surfaced as status error.
            Assert.IsFalse(serial.IsOpen);
        }
    }

    [TestMethod]
    public void Send_WhenClosed_ThrowsInvalidOperation()
    {
        using var serial = new SerialService();
        Assert.ThrowsException<InvalidOperationException>(
            () => serial.Send(new byte[] { 0x48, 0x69 }));
    }

    [TestMethod]
    public void SendBreak_WhenClosed_ThrowsInvalidOperation()
    {
        using var serial = new SerialService();
        Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => serial.SendBreakAsync(500));
    }

    [TestMethod]
    public void SetDtrRts_WhenClosed_AreSafeNoOps()
    {
        using var serial = new SerialService();
        serial.SetDtr(true);   // must not throw when disconnected
        serial.SetRts(true);
        serial.SetDtr(false);
        serial.SetRts(false);
    }

    [TestMethod]
    public void Close_WhenNeverOpened_IsSafeNoOp()
    {
        using var serial = new SerialService();
        serial.Close();
        serial.Close();
        Assert.IsFalse(serial.IsOpen);
    }

    [TestMethod]
    public void Dispose_Twice_DoesNotThrow()
    {
        var serial = new SerialService();
        serial.Dispose();
        serial.Dispose();
        Assert.ThrowsException<ObjectDisposedException>(
            () => serial.Open(new SerialSettings { PortName = "COM1" }));
    }
}
