using SerialCOM.Utils;

namespace SerialCOM.Tests;

/// <summary>
/// Validates Up/Down command history (cap 50, persisted via settings).
/// </summary>
[TestClass]
public sealed class HistoryManagerTests
{
    [TestMethod]
    public void Add_SingleCommand_RecallableViaPrevious()
    {
        // Arrange
        var history = new HistoryManager();

        // Act
        history.Add("COM1 test");

        // Assert
        Assert.AreEqual(1, history.Count);
        Assert.IsTrue(history.TryGetPrevious(out string cmd));
        Assert.AreEqual("COM1 test", cmd);
    }

    [TestMethod]
    public void Add_BlankCommands_Ignored()
    {
        var history = new HistoryManager();
        history.Add("");
        history.Add("   ");
        history.Add(null!);
        Assert.AreEqual(0, history.Count);
        Assert.IsFalse(history.TryGetPrevious(out _));
    }

    [TestMethod]
    public void Add_MoreThanFiftyEntries_KeepsNewestFifty()
    {
        // Arrange
        var history = new HistoryManager();

        // Act: push 60 distinct entries.
        for (int i = 0; i < 60; i++)
            history.Add($"cmd{i}");

        // Assert: oldest 10 evicted, newest 50 retained.
        Assert.AreEqual(HistoryManager.MaxEntries, history.Count);
        Assert.AreEqual(50, history.Count);
        Assert.AreEqual("cmd10", history.Items[0]);
        Assert.AreEqual("cmd59", history.Items[^1]);
    }

    [TestMethod]
    public void Navigation_UpDownWalksHistory()
    {
        // Arrange
        var history = new HistoryManager();
        history.Add("first");
        history.Add("second");
        history.Add("third");

        // Act + Assert: Up walks newest -> oldest.
        Assert.IsTrue(history.TryGetPrevious(out string c1));
        Assert.AreEqual("third", c1);
        Assert.IsTrue(history.TryGetPrevious(out string c2));
        Assert.AreEqual("second", c2);
        Assert.IsTrue(history.TryGetPrevious(out string c3));
        Assert.AreEqual("first", c3);
        // Clamped at oldest.
        Assert.IsTrue(history.TryGetPrevious(out string c4));
        Assert.AreEqual("first", c4);

        // Down walks back toward newest, then past-the-end returns false.
        Assert.IsTrue(history.TryGetNext(out string n1));
        Assert.AreEqual("second", n1);
        Assert.IsTrue(history.TryGetNext(out string n2));
        Assert.AreEqual("third", n2);
        Assert.IsFalse(history.TryGetNext(out _));
    }

    [TestMethod]
    public void Navigation_EmptyHistory_ReturnsFalse()
    {
        var history = new HistoryManager();
        Assert.IsFalse(history.TryGetPrevious(out _));
        Assert.IsFalse(history.TryGetNext(out _));
    }

    [TestMethod]
    public void Add_NewEntry_ResetsNavigation()
    {
        var history = new HistoryManager();
        history.Add("a");
        history.Add("b");
        Assert.IsTrue(history.TryGetPrevious(out _)); // navigating
        history.Add("c");                              // resets

        Assert.IsTrue(history.TryGetPrevious(out string cmd));
        Assert.AreEqual("c", cmd);
    }

    [TestMethod]
    public void Add_ConsecutiveDuplicate_NotDuplicated()
    {
        var history = new HistoryManager();
        history.Add("same");
        history.Add("same");
        Assert.AreEqual(1, history.Count);
    }

    [TestMethod]
    public void Load_MoreThanFiftySaved_KeepsLastFifty()
    {
        // Arrange
        var history = new HistoryManager();
        var saved = Enumerable.Range(0, 70).Select(i => $"saved{i}");

        // Act
        history.Load(saved);

        // Assert
        Assert.AreEqual(50, history.Count);
        Assert.AreEqual("saved20", history.Items[0]);
        Assert.AreEqual("saved69", history.Items[^1]);
    }

    [TestMethod]
    public void Load_NullOrBlank_ClearsAndSkips()
    {
        var history = new HistoryManager();
        history.Add("x");
        history.Load(null);
        Assert.AreEqual(0, history.Count);

        history.Load(new[] { "ok", "", "  ", "fine" });
        CollectionAssert.AreEqual(new[] { "ok", "fine" }, history.ToList());
    }

    [TestMethod]
    public void ToList_RoundTripsThroughLoad()
    {
        var history = new HistoryManager();
        history.Add("one");
        history.Add("two");

        var restored = new HistoryManager();
        restored.Load(history.ToList());

        CollectionAssert.AreEqual(history.ToList(), restored.ToList());
    }

    [TestMethod]
    public void Clear_EmptiesAndResetsNavigation()
    {
        var history = new HistoryManager();
        history.Add("a");
        history.TryGetPrevious(out _);
        history.Clear();
        Assert.AreEqual(0, history.Count);
        Assert.IsFalse(history.TryGetPrevious(out _));
    }
}
