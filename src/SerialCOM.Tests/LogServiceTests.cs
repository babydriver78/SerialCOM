using System.Text.RegularExpressions;
using SerialCOM.Services;

namespace SerialCOM.Tests;

/// <summary>
/// Validates session logging. Spec: logging creates a timestamped file with
/// [RX]/[TX] lines and survives disconnect (no loss, no throw).
/// </summary>
[TestClass]
public sealed class LogServiceTests
{
    private static string NewTempLogPath()
        => Path.Combine(Path.GetTempPath(), "SerialCOM-tests",
            $"SerialCOM_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.log");

    [TestMethod]
    public void Open_WritesRxAndTxWithDirectionAndTimestamp()
    {
        // Arrange
        string path = NewTempLogPath();
        using var log = new LogService();

        // Act
        log.Open(path);
        Assert.IsTrue(log.IsOpen);
        Assert.AreEqual(path, log.CurrentPath);
        log.WriteRx("hello from device");
        log.WriteTx("hello to device");
        log.Close();

        // Assert
        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(2, lines.Length);
        Assert.IsTrue(lines[0].Contains("[RX]"), $"was: {lines[0]}");
        Assert.IsTrue(lines[0].Contains("hello from device"));
        Assert.IsTrue(lines[1].Contains("[TX]"), $"was: {lines[1]}");
        Assert.IsTrue(lines[1].Contains("hello to device"));

        // "[2026-10-02 12:00:00.123] [RX] data" timestamp shape.
        var stamp = new Regex(@"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\] \[(RX|TX)\] ");
        foreach (string line in lines)
            Assert.IsTrue(stamp.IsMatch(line), $"bad line shape: {line}");
    }

    [TestMethod]
    public void Write_AfterClose_DoesNotThrowOrLoseProcess()
    {
        // Arrange: simulate disconnect -> logger closed.
        string path = NewTempLogPath();
        var log = new LogService();
        log.Open(path);
        log.WriteTx("before disconnect");
        log.Close();
        Assert.IsFalse(log.IsOpen);

        // Act: stray RX arriving after close must be a safe no-op.
        log.WriteRx("late arrival after disconnect");
        log.Dispose();

        // Assert: only the pre-close line persisted; nothing thrown.
        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(1, lines.Length);
        Assert.IsTrue(lines[0].Contains("before disconnect"));
    }

    [TestMethod]
    public void Open_Twice_StartsFreshSessionFile()
    {
        string path = NewTempLogPath();
        using var log = new LogService();
        log.Open(path);
        log.WriteTx("session line");
        log.Close();

        // Re-open same path = new session, must not append stale content.
        log.Open(path);
        log.WriteRx("new session line");
        log.Close();

        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(1, lines.Length);
        Assert.IsTrue(lines[0].Contains("new session line"));
    }

    [TestMethod]
    public void DefaultPathForNow_HasTimestampedLogsShape()
    {
        // Arrange + Act
        string path = LogService.DefaultPathForNow();

        // Assert: ./logs/SerialCOM_yyyyMMdd_HHmmss.log
        Assert.IsTrue(path.EndsWith(".log"));
        Assert.IsTrue(path.Contains("logs"));
        StringAssert.Matches(
            Path.GetFileName(path)!,
            new Regex(@"^SerialCOM_\d{8}_\d{6}\.log$"));
    }

    [TestMethod]
    public void Open_CreatesMissingDirectories()
    {
        string path = Path.Combine(Path.GetTempPath(), "SerialCOM-tests",
            Guid.NewGuid().ToString("N"), "nested", "session.log");
        using var log = new LogService();

        log.Open(path);
        log.WriteTx("dir bootstrap");
        log.Close();

        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void AutoFlush_PersistsWithoutExplicitClose()
    {
        // Arrange: writer must flush per line so a cable-pull loses nothing.
        string path = NewTempLogPath();
        using var log = new LogService();
        log.Open(path);

        // Act: no Close() before reading.
        log.WriteTx("unflushed? no — AutoFlush=true");

        // Assert: open with shared access (writer holds an exclusive lock,
        // so File.ReadAllText would throw — that is expected StreamWriter
        // behavior, not a bug). Shared read proves the line was flushed.
        using var fs = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs);
        string content = reader.ReadToEnd();
        Assert.IsTrue(content.Contains("AutoFlush"));
    }
}
