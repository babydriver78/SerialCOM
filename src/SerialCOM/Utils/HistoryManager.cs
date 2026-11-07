namespace SerialCOM.Utils;

/// <summary>
/// Up/Down-arrow command history, capped at 50 entries.
/// Navigation state resets whenever a new entry is added.
/// </summary>
public sealed class HistoryManager
{
    public const int MaxEntries = 50;

    private readonly List<string> _items = new();
    private int _navIndex = -1; // -1 = not navigating

    public IReadOnlyList<string> Items => _items;
    public int Count => _items.Count;

    public void Add(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return;

        // Avoid piling up the same command twice in a row.
        if (_items.Count > 0 && _items[^1] == command)
        {
            ResetNavigation();
            return;
        }

        _items.Add(command);
        if (_items.Count > MaxEntries)
            _items.RemoveAt(0);
        ResetNavigation();
    }

    /// <summary>Step back (Up arrow). Returns false when history is empty.</summary>
    public bool TryGetPrevious(out string command)
    {
        command = "";
        if (_items.Count == 0)
            return false;

        _navIndex = _navIndex < 0 ? _items.Count - 1 : Math.Max(0, _navIndex - 1);
        command = _items[_navIndex];
        return true;
    }

    /// <summary>Step forward (Down arrow). Returns false when at the newest entry.</summary>
    public bool TryGetNext(out string command)
    {
        command = "";
        if (_items.Count == 0 || _navIndex < 0)
            return false;

        if (_navIndex >= _items.Count - 1)
        {
            ResetNavigation();
            return false; // past the end -> caller clears the box
        }

        _navIndex++;
        command = _items[_navIndex];
        return true;
    }

    public void ResetNavigation() => _navIndex = -1;

    public void Load(IEnumerable<string>? saved)
    {
        _items.Clear();
        ResetNavigation();
        if (saved is null)
            return;
        foreach (var s in saved.TakeLast(MaxEntries))
            if (!string.IsNullOrWhiteSpace(s))
                _items.Add(s);
    }

    public List<string> ToList() => new(_items);

    public void Clear()
    {
        _items.Clear();
        ResetNavigation();
    }
}
