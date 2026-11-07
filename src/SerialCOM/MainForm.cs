using System.Drawing;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SerialCOM.Models;
using SerialCOM.Services;
using SerialCOM.Utils;

namespace SerialCOM;

/// <summary>
/// Main window: menu + toolbar + connection panel (left) +
/// terminal and send bar (right) + status strip.
/// Layout lives in <see cref="MainForm.Designer.cs"/>; this file holds behavior only.
/// </summary>
public partial class MainForm : Form
{
    private const int WM_DEVICECHANGE = 0x0219;
    private const int DBT_DEVICEARRIVAL = 0x8000;
    private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
    private const int WM_SETREDRAW = 0x000B;
    private const int MaxTerminalChars = 512_000; // ring-buffer cap (~50k lines)

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }

    private readonly SerialService _serial = new();
    private readonly LogService _log = new();
    private readonly SettingsService _settingsService = new();
    private readonly HistoryManager _history = new();
    private AppSettings _settings = new();

    private bool _manualDisconnect = true;
    private bool _loadingSettings;
    private bool _pendingPortRefresh;
    private long _rxBytes;
    private long _txBytes;

    public MainForm()
    {
        InitializeComponent();

        // Serial callbacks arrive on a background thread — marshal safely.
        _serial.DataReceived += (_, data) => SafeInvoke(() => AppendRx(data));
        _serial.ConnectionLost += (_, ex) => SafeInvoke(() => OnConnectionLost(ex));
        _serial.LineError += (_, msg) => SafeInvoke(() => ShowError(msg));

        _settings = _settingsService.Load();
        ApplySettingsToUi(_settings);
        RefreshPorts(selectSaved: true);

        // If logging was left enabled, start a fresh session file.
        if (chkLogging.Checked)
            StartLogging();
    }

    /// <summary>Marshal to the UI thread; no-op when the form is gone (fixes BeginInvoke dispose races).</summary>
    private void SafeInvoke(Action action)
    {
        if (IsDisposed || Disposing || !IsHandleCreated)
            return;
        try
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }
        catch (ObjectDisposedException)
        {
            // Form closed mid-callback — the update is no longer needed.
        }
        catch (InvalidOperationException)
        {
            // Handle destroyed while invoking — same story.
        }
    }

    // ================= Designer event handlers =================

    private void MiSaveLog_Click(object? sender, EventArgs e) => SaveLogAs();
    private void MiClearLog_Click(object? sender, EventArgs e) => ClearTerminal();
    private void MiExit_Click(object? sender, EventArgs e) => Close();
    private void MiAbout_Click(object? sender, EventArgs e) => ShowAbout();

    private void MnuSidePanel_Click(object? sender, EventArgs e) => ToggleSidePanel();
    private void MnuAlwaysOnTop_Click(object? sender, EventArgs e) => ApplyAlwaysOnTop(mnuAlwaysOnTop.Checked);
    private void MnuWordWrap_Click(object? sender, EventArgs e) => ApplyWordWrap(mnuWordWrap.Checked);
    private void MnuAutoScroll_Click(object? sender, EventArgs e) => chkAutoscroll.Checked = mnuAutoScroll.Checked;
    private void MnuTimestamps_Click(object? sender, EventArgs e) => chkTimestamp.Checked = mnuTimestamps.Checked;
    private void MnuDarkTerminal_Click(object? sender, EventArgs e) => ApplyDarkTerminal(mnuDarkTerminal.Checked);
    private void MnuColorize_Click(object? sender, EventArgs e) => ApplyColorize(mnuColorize.Checked);

    private void TbtnPanel_Click(object? sender, EventArgs e) => SetSidePanelVisible(tbtnPanel.Checked);
    private void TbtnRefresh_Click(object? sender, EventArgs e) => RefreshPorts();
    private void TbtnClear_Click(object? sender, EventArgs e) => ClearTerminal();
    private async void TbtnBreak_Click(object? sender, EventArgs e) => await OnBreakAsync();
    private void TbtnSaveLog_Click(object? sender, EventArgs e) => SaveLogAs();

    private void BtnPortRefresh_Click(object? sender, EventArgs e) => RefreshPorts();
    private void BtnConnect_Click(object? sender, EventArgs e) => ToggleConnection();
    private void BtnSend_Click(object? sender, EventArgs e) => SendCurrentLine();
    private async void BtnSendFile_Click(object? sender, EventArgs e) => await SendFileAsync();
    private void BtnBrowseLog_Click(object? sender, EventArgs e) => OnBrowseLogPath(sender, e);
    private void BtnSaveSettings_Click(object? sender, EventArgs e) => SaveSettings(showMessage: true);

    private void BtnLoadSettings_Click(object? sender, EventArgs e)
    {
        _settings = _settingsService.Load();
        ApplySettingsToUi(_settings);
        RefreshPorts(selectSaved: true);
    }

    private void BtnDefaultSettings_Click(object? sender, EventArgs e)
    {
        _settings = AppSettings.Default();
        ApplySettingsToUi(_settings);
        RefreshPorts(selectSaved: true);
    }

    private void ChkDtr_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_loadingSettings)
            _serial.SetDtr(chkDtr.Checked);
    }

    private void ChkRts_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_loadingSettings)
            _serial.SetRts(chkRts.Checked);
    }

    private void ChkWrap_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loadingSettings)
            return;
        ApplyWordWrap(chkWrap.Checked);
    }

    private void ChkAutoscroll_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_loadingSettings)
            mnuAutoScroll.Checked = chkAutoscroll.Checked;
    }

    private void ChkTimestamp_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_loadingSettings)
            mnuTimestamps.Checked = chkTimestamp.Checked;
    }

    private void ChkDarkTerminal_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loadingSettings)
            return;
        ApplyDarkTerminal(chkDarkTerminal.Checked);
    }

    private void ChkColorize_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loadingSettings)
            return;
        ApplyColorize(chkColorize.Checked);
    }

    private void ChkLogging_CheckedChanged(object? sender, EventArgs e)
    {
        if (_loadingSettings)
            return;
        if (chkLogging.Checked)
            StartLogging();
        else
            _log.Close();
    }

    private void CmbTxMode_SelectedIndexChanged(object? sender, EventArgs e) => UpdateHexValidation();
    private void CmbDisplay_SelectedIndexChanged(object? sender, EventArgs e) => ShowInfo($"Display: {cmbDisplay.Text}");

    private void CmbPort_DropDownClosed(object? sender, EventArgs e)
    {
        if (_pendingPortRefresh)
        {
            _pendingPortRefresh = false;
            RefreshPorts();
        }
    }

    private void TxtSend_KeyDown(object? sender, KeyEventArgs e) => OnSendBoxKeyDown(sender, e);
    private void TxtSend_TextChanged(object? sender, EventArgs e) => UpdateHexValidation();

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.L) { ClearTerminal(); e.SuppressKeyPress = true; }
        else if (e.KeyCode == Keys.F5) { RefreshPorts(); e.SuppressKeyPress = true; }
        else if (e.KeyCode == Keys.F9) { ToggleSidePanel(); e.SuppressKeyPress = true; }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e) => OnFormClosing(sender, e);
    private void ReconnectTimer_Tick(object? sender, EventArgs e) => TryAutoReconnect();

    private void StatusTimer_Tick(object? sender, EventArgs e)
    {
        toolError.Text = "";
        _statusTimer.Stop();
    }

    // ================= ports =================

    private void RefreshPorts(bool selectSaved = false)
    {
        // Don't yank the list out from under an open drop-down (focus steal).
        if (cmbPort.DroppedDown)
        {
            _pendingPortRefresh = true;
            return;
        }

        string keep = selectSaved ? _settings.Serial.PortName : (cmbPort.SelectedItem as string ?? cmbPort.Text);
        string[] ports;
        try
        {
            ports = SerialPort.GetPortNames().OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToArray();
        }
        catch (Exception ex)
        {
            ShowError($"Port scan failed: {ex.Message}");
            return;
        }

        cmbPort.BeginUpdate();
        try
        {
            cmbPort.Items.Clear();
            cmbPort.Items.AddRange(ports);
            if (!string.IsNullOrEmpty(keep) && ports.Contains(keep, StringComparer.OrdinalIgnoreCase))
                cmbPort.SelectedItem = ports.First(p => p.Equals(keep, StringComparison.OrdinalIgnoreCase));
            else if (ports.Length > 0)
                cmbPort.SelectedIndex = 0;
            else
                cmbPort.Text = "";
        }
        finally
        {
            cmbPort.EndUpdate();
        }

        bool none = ports.Length == 0;
        lblConnHint.Text = none ? "No COM ports found — plug in a device, then Refresh." : "";
        _tips.SetToolTip(cmbPort, none ? "No COM ports found" : "Select a COM port");
        if (!_serial.IsOpen)
            btnConnect.Enabled = !none;
        if (none)
            ShowInfo("No COM ports found.");
    }

    // USB plug/unplug auto-refresh (debounced, focus-safe).
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg != WM_DEVICECHANGE)
            return;
        int evt = m.WParam.ToInt32();
        if (evt is DBT_DEVICEARRIVAL or DBT_DEVICEREMOVECOMPLETE)
            SafeInvoke(() => RefreshPorts());
    }

    // ================= connect =================

    private void ToggleConnection()
    {
        if (_serial.IsOpen)
            Disconnect(manual: true);
        else
            Connect();
    }

    private void Connect(bool silent = false)
    {
        ClearStatus();
        SerialSettings s = CollectSerialSettings();
        if (string.IsNullOrWhiteSpace(s.PortName))
        {
            if (!silent) ShowError("Select a COM port first.");
            return;
        }
        if (s.BaudRate is < 300 or > 921600)
        {
            if (!silent)
            {
                ShowError("Baud rate must be 300–921600.");
                cmbBaud.BackColor = Color.MistyRose;
            }
            return;
        }
        cmbBaud.BackColor = SystemColors.Window;

        try
        {
            _serial.Open(s);
            _manualDisconnect = false;
            UpdateConnectUi(connected: true, s);
            ClearStatus();
            if (!silent)
            {
                string msg = $"Opened {s.PortName} @ {s.BaudRate},{s.DataFormatSummary()}.";
                ShowInfo(msg);
                AppendSystem(msg);
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or InvalidOperationException or ArgumentException)
        {
            if (!silent) ShowError($"Cannot open {s.PortName}: {ex.Message}");
        }
    }

    private void Disconnect(bool manual)
    {
        if (manual)
        {
            _manualDisconnect = true;
            _reconnectTimer.Stop();
        }
        _serial.Close();
        UpdateConnectUi(connected: false, null);
        if (manual)
        {
            ShowInfo("Disconnected.");
            AppendSystem("Disconnected.");
        }
    }

    private void UpdateConnectUi(bool connected, SerialSettings? s)
    {
        btnConnect.Text = connected ? "⏹  Disconnect" : "▶  Connect";
        lblLedDot.ForeColor = connected ? Color.SeaGreen : Color.Firebrick;
        lblLedText.Text = connected ? $"{s!.PortName} @ {s.BaudRate}" : "Disconnected";
        toolStatus.Text = connected ? $"{s!.PortName} @ {s.BaudRate},{s.DataFormatSummary()}" : "Disconnected";
        SetConnControlsEnabled(!connected);
    }

    private void OnConnectionLost(Exception ex)
    {
        if (!_serial.IsOpen && _manualDisconnect)
            return;
        _manualDisconnect = false;
        _serial.Close();
        UpdateConnectUi(connected: false, null);
        ShowError($"Connection lost: {ex.Message}");
        AppendSystem($"Connection lost: {ex.Message}");
        if (chkAutoReconnect.Checked)
            _reconnectTimer.Start();
    }

    private void TryAutoReconnect()
    {
        if (_manualDisconnect || _serial.IsOpen || !chkAutoReconnect.Checked)
        {
            _reconnectTimer.Stop();
            return;
        }
        RefreshPorts();
        Connect(silent: true);
        if (_serial.IsOpen)
        {
            _reconnectTimer.Stop();
            ShowInfo("Reconnected.");
            AppendSystem("Reconnected.");
        }
    }

    private void SetConnControlsEnabled(bool enabled)
    {
        // DTR/RTS stay live while connected — the driver supports it.
        cmbPort.Enabled = enabled;
        btnPortRefresh.Enabled = true; // rescan never disturbs the open port
        cmbBaud.Enabled = enabled;
        cmbDataBits.Enabled = enabled;
        cmbParity.Enabled = enabled;
        cmbStopBits.Enabled = enabled;
        cmbHandshake.Enabled = enabled;
        if (!enabled)
            _tips.SetToolTip(btnConnect, "Close the current connection");
        else
            _tips.SetToolTip(btnConnect, "Open the selected COM port");
    }

    // ================= RX =================

    private void AppendRx(byte[] data)
    {
        _rxBytes += data.Length;
        toolRx.Text = $"RX: {_rxBytes:N0}";

        bool hex = cmbDisplay.Text == "HEX";
        // Single conversion: HEX packs 16 bytes/line, ASCII decodes once.
        string body = hex ? HexConverter.ToHexString(data) : GetRxEncoding().GetString(data);
        AppendLines("RX", body, chkTimestamp.Checked);
        LogLines(_log.WriteRx, body);
    }

    private void AppendTx(string text) => AppendLines("TX", text, chkTimestamp.Checked);

    private void AppendSystem(string text) => AppendLines("SYS", text, chkTimestamp.Checked);

    /// <summary>
    /// Split a payload so every terminal/log line carries its own
    /// [timestamp] [TAG] prefix. Drops the single trailing empty piece
    /// from a trailing newline ("Hi\r\n" renders one line, not two).
    /// </summary>
    private static string[] SplitTerminalLines(string body)
    {
        if (body.Length == 0)
            return Array.Empty<string>();
        string[] parts = body.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        if (parts.Length > 1 && parts[^1].Length == 0)
            parts = parts[..^1];
        return parts;
    }

    /// <summary>Log one [timestamp] [TX/RX] entry per line so continuations stay tagged.</summary>
    private static void LogLines(Action<string> write, string body)
    {
        string trimmed = body.Trim();
        if (trimmed.Length == 0)
            return;
        foreach (string line in SplitTerminalLines(trimmed))
            write(line);
    }

    /// <summary>
    /// Flicker-free, selection-preserving append. Every line is prefixed
    /// "[HH:mm:ss.fff] [TAG] " (or "[TAG] " with timestamps off), where TAG
    /// is TX (blue), RX (green) or SYS (gray). Auto-scroll only fires when
    /// the caret was already at the bottom with no active selection, so reading
    /// or selecting history is never yanked away.
    /// </summary>
    private void AppendLines(string tag, string body, bool addTimestamp)
    {
        if (IsDisposed || Disposing || !rtbTerminal.IsHandleCreated)
            return;
        string[] lines = SplitTerminalLines(body);
        if (lines.Length == 0)
            return;

        bool stickToBottom = chkAutoscroll.Checked && IsCaretAtEnd();
        int savedStart = rtbTerminal.SelectionStart;
        int savedLen = rtbTerminal.SelectionLength;

        Color tagColor = tag switch
        {
            "TX" => TxColor(),
            "RX" => RxColor(),
            _ => SysColor(),
        };

        NativeMethods.SendMessage(rtbTerminal.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        try
        {
            rtbTerminal.SelectionStart = rtbTerminal.TextLength;
            rtbTerminal.SelectionLength = 0;

            foreach (string line in lines)
            {
                if (addTimestamp)
                {
                    rtbTerminal.SelectionColor = DimColor();
                    rtbTerminal.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] ");
                }

                rtbTerminal.SelectionColor = tagColor;
                rtbTerminal.AppendText($"[{tag}] {line}{Environment.NewLine}");
            }
            rtbTerminal.SelectionColor = rtbTerminal.ForeColor;
        }
        finally
        {
            NativeMethods.SendMessage(rtbTerminal.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            rtbTerminal.Invalidate();
        }

        TrimTerminal();

        if (stickToBottom)
        {
            rtbTerminal.SelectionStart = rtbTerminal.TextLength;
            rtbTerminal.SelectionLength = 0;
            rtbTerminal.ScrollToCaret();
        }
        else if (savedLen > 0)
        {
            // Restore the user's selection (shifted if the head was trimmed).
            rtbTerminal.SelectionStart = Math.Min(savedStart, rtbTerminal.TextLength);
            rtbTerminal.SelectionLength = Math.Min(savedLen, rtbTerminal.TextLength - rtbTerminal.SelectionStart);
        }
    }

    private bool IsCaretAtEnd()
    {
        if (rtbTerminal.SelectionLength > 0)
            return false; // user is selecting — never steal the view
        return rtbTerminal.SelectionStart + rtbTerminal.SelectionLength >= Math.Max(0, rtbTerminal.TextLength - 128);
    }

    private void TrimTerminal()
    {
        if (rtbTerminal.TextLength <= MaxTerminalChars)
            return;
        // Drop the oldest ~10% at a line boundary to keep memory bounded.
        int cut = rtbTerminal.TextLength / 10;
        int remove = cut;
        // Find the next newline without copying the whole buffer twice:
        // Text is the only public accessor; bounded search window keeps it cheap.
        string head = rtbTerminal.Text[..Math.Min(rtbTerminal.TextLength, cut + 512)];
        int nl = head.IndexOf('\n', cut);
        remove = nl >= 0 ? Math.Min(cut + 512, nl + 1) : cut;
        if (remove <= 0 || remove >= rtbTerminal.TextLength)
            remove = cut;

        int savedStart = rtbTerminal.SelectionStart;
        int savedLen = rtbTerminal.SelectionLength;
        rtbTerminal.Select(0, remove);
        rtbTerminal.SelectedText = "";
        rtbTerminal.SelectionStart = Math.Max(0, savedStart - remove);
        rtbTerminal.SelectionLength = savedLen;
    }

    private void ClearTerminal()
    {
        rtbTerminal.Clear();
        AppendSystem("Terminal cleared.");
        ShowInfo("Terminal cleared.");
    }

    // ================= TX =================

    private void OnSendBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter) { SendCurrentLine(); e.SuppressKeyPress = true; }
        else if (e.KeyCode == Keys.Up)
        {
            if (_history.TryGetPrevious(out string cmd)) { txtSend.Text = cmd; txtSend.SelectionStart = txtSend.TextLength; }
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Down)
        {
            txtSend.Text = _history.TryGetNext(out string cmd) ? cmd : "";
            txtSend.SelectionStart = txtSend.TextLength;
            e.SuppressKeyPress = true;
        }
    }

    private void SendCurrentLine()
    {
        string input = txtSend.Text;
        if (string.IsNullOrEmpty(input))
            return;
        if (!_serial.IsOpen)
        {
            ShowError("Not connected.");
            return;
        }

        try
        {
            byte[] payload = cmbTxMode.Text == "HEX"
                ? ParseHexOrThrow(input)
                : GetTxEncoding().GetBytes(input);

            byte[] framed = FrameWithLineEnding(payload);
            _serial.Send(framed);

            _txBytes += framed.Length;
            toolTx.Text = $"TX: {_txBytes:N0}";
            _history.Add(input);
            txtSend.Clear();
            ClearStatus();

            // Always render the sent line once as [TX], even with local echo
            // off. Echo ON adds no second copy — a device echo, if any,
            // arrives separately as [RX]. (chkEcho stays persisted for
            // settings backward-compat.)
            string shown = cmbTxMode.Text == "HEX" ? HexConverter.ToHexString(framed) : GetTxEncoding().GetString(framed);
            AppendTx(shown);
            LogLines(_log.WriteTx, shown);
        }
        catch (FormatException ex)
        {
            ShowError(ex.Message);
            UpdateHexValidation();
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or UnauthorizedAccessException or TimeoutException)
        {
            OnConnectionLost(ex);
        }
    }

    private static byte[] ParseHexOrThrow(string input)
    {
        if (!HexConverter.TryParseHex(input, out byte[] bytes))
            throw new FormatException("Invalid HEX — use byte pairs like “48 65 6C”.");
        return bytes;
    }

    private byte[] FrameWithLineEnding(byte[] payload) => cmbLineEnding.Text switch
    {
        "LF" => [.. payload, (byte)'\n'],
        "CR" => [.. payload, (byte)'\r'],
        "CRLF" => [.. payload, (byte)'\r', (byte)'\n'],
        _ => payload,
    };

    /// <summary>Live HEX validation: pink background + tooltip instead of a surprise error on Send.</summary>
    private void UpdateHexValidation()
    {
        if (txtSend is null)
            return;
        bool hexMode = cmbTxMode.Text == "HEX";
        string text = txtSend.Text;
        bool invalid = hexMode && !string.IsNullOrWhiteSpace(text) && !HexConverter.IsValidHex(text);
        txtSend.BackColor = invalid ? Color.MistyRose : SystemColors.Window;
        _tips.SetToolTip(txtSend, invalid
            ? "Invalid HEX — use byte pairs like “48 65 6C”."
            : "Enter sends · Up/Down recalls history · HEX mode needs pairs like “48 65”");
    }

    private async Task SendFileAsync()
    {
        if (!_serial.IsOpen)
        {
            ShowError("Not connected.");
            return;
        }
        using var dlg = new OpenFileDialog { Title = "Send file (raw bytes)" };
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        btnSendFile.Enabled = false;
        try
        {
            using var fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            var chunk = new byte[4096];
            int read;
            while ((read = await fs.ReadAsync(chunk, 0, chunk.Length)) > 0)
            {
                _serial.Send(chunk[..read]);
                _txBytes += read;
            }
            toolTx.Text = $"TX: {_txBytes:N0}";
            string sentNote = $"<sent file {Path.GetFileName(dlg.FileName)}>";
            _log.WriteTx(sentNote);
            AppendTx(sentNote);
            ShowInfo($"Sent {Path.GetFileName(dlg.FileName)}.");
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or UnauthorizedAccessException)
        {
            OnConnectionLost(ex);
        }
        finally
        {
            if (!IsDisposed)
                btnSendFile.Enabled = true;
        }
    }

    private async Task OnBreakAsync()
    {
        if (!_serial.IsOpen)
        {
            ShowError("Not connected.");
            return;
        }
        try
        {
            tbtnBreak.Enabled = false;
            await _serial.SendBreakAsync(500);
            ShowInfo("Break sent (500 ms).");
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or UnauthorizedAccessException)
        {
            OnConnectionLost(ex);
        }
        finally
        {
            if (!IsDisposed)
                tbtnBreak.Enabled = true;
        }
    }

    // ================= logging =================

    private void StartLogging()
    {
        try
        {
            string path = string.IsNullOrWhiteSpace(txtLogPath.Text) ? LogService.DefaultPathForNow() : txtLogPath.Text.Trim();
            _log.Open(path);
            txtLogPath.Text = _log.CurrentPath;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            _loadingSettings = true;
            chkLogging.Checked = false;
            _loadingSettings = false;
            ShowError($"Logging failed: {ex.Message}");
        }
    }

    private void OnBrowseLogPath(object? sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Title = "Log file",
            Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = Path.GetFileName(string.IsNullOrWhiteSpace(txtLogPath.Text) ? LogService.DefaultPathForNow() : txtLogPath.Text),
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
            txtLogPath.Text = dlg.FileName;
    }

    private void SaveLogAs()
    {
        using var dlg = new SaveFileDialog
        {
            Title = "Save terminal buffer",
            Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*",
            FileName = $"SerialCOM_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
        };
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            File.WriteAllText(dlg.FileName, rtbTerminal.Text);
            ShowInfo($"Terminal saved to {dlg.FileName}.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ShowError($"Save failed: {ex.Message}");
        }
    }

    // ================= view prefs =================

    private void ToggleSidePanel() => SetSidePanelVisible(split.Panel1Collapsed);

    private void SetSidePanelVisible(bool visible)
    {
        split.Panel1Collapsed = !visible;
        tbtnPanel.Checked = visible;
        mnuSidePanel.Checked = visible;
    }

    private void ApplyAlwaysOnTop(bool on)
    {
        TopMost = on;
        mnuAlwaysOnTop.Checked = on;
    }

    private void ApplyWordWrap(bool on)
    {
        chkWrap.Checked = on;
        mnuWordWrap.Checked = on;
        rtbTerminal.WordWrap = on;
    }

    private void ApplyDarkTerminal(bool on)
    {
        chkDarkTerminal.Checked = on;
        mnuDarkTerminal.Checked = on;
        ApplyTerminalTheme();
    }

    private void ApplyColorize(bool on)
    {
        chkColorize.Checked = on;
        mnuColorize.Checked = on;
    }

    private void ApplyTerminalTheme()
    {
        bool dark = chkDarkTerminal.Checked;
        rtbTerminal.BackColor = dark ? Color.FromArgb(30, 30, 30) : SystemColors.Window;
        rtbTerminal.ForeColor = dark ? Color.FromArgb(220, 220, 220) : SystemColors.WindowText;
    }

    private static Color DimColor() => Color.Gray;

    private Color RxColor()
    {
        if (!chkColorize.Checked)
            return SystemColor();
        return chkDarkTerminal.Checked ? Color.FromArgb(78, 201, 176) : Color.DarkGreen;
    }

    private Color TxColor()
    {
        if (!chkColorize.Checked)
            return SystemColor();
        return chkDarkTerminal.Checked ? Color.FromArgb(86, 156, 214) : Color.DarkBlue;
    }

    /// <summary>System lines render gray (plain default color when colorize is off).</summary>
    private Color SysColor() => chkColorize.Checked ? DimColor() : SystemColor();

    private Color SystemColor() => rtbTerminal.ForeColor;

    private static void ShowAbout() => MessageBox.Show(
        "SerialCOM — lightweight serial terminal.\n\nEnter = Send · ↑/↓ history · F5 refresh · Ctrl+L clear · F9 panel.",
        "About SerialCOM",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);

    // ================= settings =================

    private SerialSettings CollectSerialSettings() => new()
    {
        PortName = cmbPort.Text.Trim(),
        BaudRate = int.TryParse(cmbBaud.Text.Trim(), out int b) ? b : 115200,
        DataBits = int.TryParse(cmbDataBits.Text, out int d) ? d : 8,
        Parity = cmbParity.Text,
        StopBits = cmbStopBits.Text,
        Handshake = cmbHandshake.Text,
        DtrEnable = chkDtr.Checked,
        RtsEnable = chkRts.Checked,
    };

    private void ApplySettingsToUi(AppSettings s)
    {
        _loadingSettings = true;
        try
        {
            SetCombo(cmbBaud, s.Serial.BaudRate.ToString());
            SetCombo(cmbDataBits, s.Serial.DataBits.ToString());
            SetCombo(cmbParity, s.Serial.Parity);
            SetCombo(cmbStopBits, s.Serial.StopBits);
            SetCombo(cmbHandshake, s.Serial.Handshake);
            chkDtr.Checked = s.Serial.DtrEnable;
            chkRts.Checked = s.Serial.RtsEnable;

            SetCombo(cmbTxMode, s.TxMode);
            SetCombo(cmbLineEnding, s.LineEnding);
            SetCombo(cmbEncoding, s.EncodingName);
            SetCombo(cmbDisplay, s.DisplayMode);

            chkEcho.Checked = s.LocalEcho;
            chkTimestamp.Checked = s.ShowTimestamp;
            chkAutoscroll.Checked = s.AutoScroll;
            chkLogging.Checked = false; // enabled below if persisted
            txtLogPath.Text = s.LogPath;
            chkAutoReconnect.Checked = s.AutoReconnect;

            chkColorize.Checked = s.ColorizeTerminal;
            mnuColorize.Checked = s.ColorizeTerminal;
            chkDarkTerminal.Checked = s.DarkTerminal;
            mnuDarkTerminal.Checked = s.DarkTerminal;
            ApplyTerminalTheme();

            ApplyWordWrap(s.WordWrap);
            mnuAutoScroll.Checked = s.AutoScroll;
            mnuTimestamps.Checked = s.ShowTimestamp;

            ApplyAlwaysOnTop(s.AlwaysOnTop);
            if (s.SplitterDistance >= split.Panel1MinSize && s.SplitterDistance < ClientSize.Width - split.Panel2MinSize)
                split.SplitterDistance = s.SplitterDistance;

            _history.Load(s.History);
            ClientSize = new Size(Math.Max(850, s.WindowWidth), Math.Max(550, s.WindowHeight));

            if (s.EnableLogging)
                chkLogging.Checked = true; // handler opens the log
        }
        finally
        {
            _loadingSettings = false;
        }
        UpdateHexValidation();
    }

    private AppSettings CollectSettings() => new()
    {
        Serial = CollectSerialSettings(),
        TxMode = cmbTxMode.Text,
        LineEnding = cmbLineEnding.Text,
        EncodingName = cmbEncoding.Text,
        DisplayMode = cmbDisplay.Text,
        LocalEcho = chkEcho.Checked,
        ShowTimestamp = chkTimestamp.Checked,
        AutoScroll = chkAutoscroll.Checked,
        WordWrap = chkWrap.Checked,
        EnableLogging = chkLogging.Checked,
        LogPath = txtLogPath.Text,
        AutoReconnect = chkAutoReconnect.Checked,
        History = _history.ToList(),
        WindowWidth = ClientSize.Width,
        WindowHeight = ClientSize.Height,
        DarkTerminal = chkDarkTerminal.Checked,
        ColorizeTerminal = chkColorize.Checked,
        AlwaysOnTop = mnuAlwaysOnTop.Checked,
        SplitterDistance = split.Panel1Collapsed ? _settings.SplitterDistance : split.SplitterDistance,
    };

    private void SaveSettings(bool showMessage)
    {
        bool ok = _settingsService.Save(CollectSettings());
        if (showMessage)
        {
            if (ok) ShowInfo("Settings saved.");
            else ShowError("Could not save settings.");
        }
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        _reconnectTimer.Stop();
        _statusTimer.Stop();
        _settings = CollectSettings();
        _settingsService.Save(_settings);
        _log.Close();
        _serial.Dispose();
    }

    // ================= helpers =================

    private System.Text.Encoding GetTxEncoding() => cmbEncoding.Text switch
    {
        "ASCII" => System.Text.Encoding.ASCII,
        "Latin1" => System.Text.Encoding.Latin1,
        _ => System.Text.Encoding.UTF8,
    };

    private System.Text.Encoding GetRxEncoding() => GetTxEncoding();

    /// <summary>Sticky red error — stays until the next successful action.</summary>
    private void ShowError(string message)
    {
        if (IsDisposed || Disposing)
            return;
        toolError.ForeColor = Color.Firebrick;
        toolError.Text = message;
        _statusTimer.Stop(); // errors persist; info messages auto-clear
    }

    /// <summary>Transient confirmation — auto-clears after a few seconds.</summary>
    private void ShowInfo(string message)
    {
        if (IsDisposed || Disposing)
            return;
        if (!string.IsNullOrEmpty(toolError.Text) && toolError.ForeColor == Color.Firebrick && string.IsNullOrEmpty(message))
            return;
        toolError.ForeColor = SystemColors.GrayText;
        toolError.Text = message;
        _statusTimer.Stop();
        if (!string.IsNullOrEmpty(message))
            _statusTimer.Start();
    }

    private void ClearStatus()
    {
        toolError.Text = "";
        _statusTimer.Stop();
    }

    private static void SetCombo(ComboBox combo, string value)
    {
        int i = combo.Items.Cast<object>().ToList()
            .FindIndex(o => string.Equals(o.ToString(), value, StringComparison.OrdinalIgnoreCase));
        if (i >= 0) combo.SelectedIndex = i;
        else combo.Text = value;
    }
}
