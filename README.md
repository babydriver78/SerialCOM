# SerialCOM — Serial Terminal

Lightweight, single-purpose **serial-only terminal** (a PuTTY / RealTerm alternative for COM ports only).
C# 12 · .NET 8 · WinForms · `System.IO.Ports.SerialPort` only — no third-party dependencies.

## Features

- Auto-enumerate COM ports on startup, **Refresh** button, `F5` shortcut, auto-refresh on USB plug/unplug (`WM_DEVICECHANGE`)
- Connect with baud (9600–921600, editable), data bits, parity, stop bits, handshake; live **DTR/RTS** toggles
- Terminal view (`RichTextBox`, Consolas 10pt) with **ASCII / HEX** display, timestamp, echo, auto-scroll, word wrap, 50k-line ring buffer
  (flicker-free `WM_SETREDRAW` batching, selection-preserving scroll/trim, dark theme + RX/TX colorize toggle).
  Every line is direction-tagged — `[TX]` sent (blue), `[RX]` received (green), `[SYS]` session events (gray):

  ```text
  [00:54:14.417] [TX] Hi
  [00:54:14.452] [RX] Hello back
  [00:55:01.003] [SYS] Disconnected.
  ```

  Timestamps are toggleable (off shows just `[TX] data`); HEX display packs 16 bytes per line, each line tagged.
  Sent input always renders once as `[TX]` (device echoes, if any, arrive separately as `[RX]` — no duplicates).
  `[SYS]` lines (connect/disconnect/clear) are terminal-only and are not written to the session log.
- Send bar with **ASCII / HEX** modes, line endings (None/LF/CR/CRLF), `Enter` to send, Up/Down history (50), **Send File** (raw, 4KB chunks)
- **Break signal** (500 ms), **Auto-reconnect** every 2 s
- Session logging to `./logs/SerialCOM_yyyyMMdd_HHmmss.log` in `[timestamp] [RX/TX]` format
- Menu (`File`/`View`/`Help`) + toolbar (Panel / Refresh / Clear / Break / Save log), collapsible connection panel
  (`SplitContainer`, `F9` toggle, splitter position remembered), **Always on top**, **Dark terminal**
- Settings persisted to `%AppData%/SerialCOM/settings.json` (Save / Load / Default, backward-compatible — old files load with defaults for `DarkTerminal`/`ColorizeTerminal`/`AlwaysOnTop`/`SplitterDistance`, case-insensitive keys)
- Encodings: ASCII / UTF-8 / Latin-1

## Build (Windows only, clean-first)

Windows-only: `net8.0-windows` in both projects, no Linux/macOS RIDs. Requires .NET 8 SDK on Windows.
Always cleans old outputs first (`dotnet clean` + `bin/`/`obj/` removal — logs and settings are preserved).

```bat
build-windows.bat         :: Release (default)
build-windows.bat Release
build-windows.bat Debug
```

Any other argument prints usage and exits with code 2; missing SDK exits with code 1.

Or manually (PowerShell, from the repo root):

```powershell
dotnet clean SerialCOM.sln -c Release
Remove-Item -Recurse -Force src\SerialCOM\bin, src\SerialCOM\obj, src\SerialCOM.Tests\bin, src\SerialCOM.Tests\obj -ErrorAction SilentlyContinue
dotnet build SerialCOM.sln -c Release
```

Replace `Release` with `Debug` for a debug build.

## Run (dev)

```powershell
dotnet run --project src/SerialCOM
```

## Test

```powershell
dotnet test SerialCOM.sln
```

80 MSTest cases, no hardware needed (FakeSerial loopback covers ASCII/HEX send, history, logging, settings roundtrip, ConnectionLost vs LineError, old-JSON backward compat, case-insensitive keys).

## Publish — single file (<15 MB framework-dependent)

```powershell
dotnet publish src/SerialCOM -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o publish
```

If the .NET 8 runtime is not guaranteed on the target machine, use self-contained instead:

```powershell
dotnet publish src/SerialCOM -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o publish
```

No installer, no admin rights, no background services.

## Keyboard shortcuts

| Keys | Action |
| ---- | ------ |
| `Enter` | Send |
| `Up` / `Down` | Command history |
| `Ctrl+L` | Clear terminal |
| `F5` | Refresh COM ports |
| `F9` | Show/hide connection panel |
| `Ctrl+S` | Save terminal buffer to file |

## Layout

- `SerialCOM.sln` — solution
- `src/SerialCOM/SerialCOM.csproj` — WinForms project (`net8.0-windows`, Nullable + ImplicitUsings)
- `src/SerialCOM/Program.cs` — entry point
- `src/SerialCOM/MainForm.cs` — behavior only (events, serial, logging, settings); no layout code
- `src/SerialCOM/MainForm.Designer.cs` — full 2-panel layout in `InitializeComponent`
  (`MenuStrip`/`ToolStrip`/`StatusStrip` + `SplitContainer` 280px left panel with
  Connection / TX options / View-log groups, right terminal + send bar; `TableLayoutPanel`
  label-75px/control-Fill, port-combo-Fill + Refresh-75px, `Dock.Fill/Top/Bottom`,
  `AutoScaleMode.Dpi`, `ClientSize 1000x650`, `MinimumSize 850x550`)
- `src/SerialCOM/FlickerFreeRichTextBox.cs` — double-buffered terminal control
- `src/SerialCOM/MainForm.resx` — form resources (open `MainForm.cs` in Visual Studio / VS Code
  WinForms Designer to edit the layout visually; `SerialCOM.csproj.user` holds designer prefs)
- `src/SerialCOM/Services/SerialService.cs` — `SerialPort` wrapper (open/close/send/break/events, enum mapping)
- `src/SerialCOM/Services/LogService.cs` — session file logger (`AutoFlush`, thread-safe)
- `src/SerialCOM/Services/SettingsService.cs` — JSON persistence
- `src/SerialCOM/Models/SerialSettings.cs`, `AppSettings.cs`
- `src/SerialCOM/Utils/HexConverter.cs` — HEX parse/format + validation regex
- `src/SerialCOM/Utils/HistoryManager.cs` — 50-entry command history
- `src/SerialCOM.Tests/` — MSTest suite (FakeSerial loopback, no hardware needed)
- `CHANGELOG.md` — release notes

## Logging format

Each line is `[yyyy-MM-dd HH:mm:ss.fff] [RX|TX] payload`, e.g. `[2026-10-02 12:00:00.123] [RX] Hi`. One file per session (`./logs/SerialCOM_yyyyMMdd_HHmmss.log` by default, override via Browse or settings). `StreamWriter.AutoFlush = true`, so entries survive cable-pull/disconnect. Logs are plaintext — they contain raw RX/TX, so treat them as sensitive.
Terminal and log share the same `[TX]` / `[RX]` tags (terminal stamps `[HH:mm:ss.fff]`, or omits the stamp when timestamps are toggled off); multi-line payloads are split so every line keeps its tag.
Parity: the same converted body feeds both outputs (`AppendRx` single conversion; `Send`/`SendFile` always emit one `[TX]` entry), except the terminal stamps short time + `[SYS]` events while the log stamps full date-time and records RX/TX only. A payload containing literal `[TX]` text cannot forge a tag — every split line is re-prefixed (e.g. `[RX] [TX] fake`).

## Security notes

- No secrets, tokens, or telemetry in code or settings (`%AppData%/SerialCOM/settings.json` holds only port params, view toggles, log path, history, window size). Corrupt JSON falls back to defaults, never crashes.
- `SerialPort` open/close/dispose is exception-safe (`UnauthorizedAccess`/`IOException`/`InvalidOperation`/`ArgumentException` surface as status-bar messages). RX callbacks marshal via `BeginInvoke`; terminal buffer capped at ~512k chars.
- HEX input is validated (`^([0-9A-Fa-f]{2}\s*)+$`, then per-pair `TryParse`); invalid input like `ZZ` shows a status error, never a crash. Baud is enforced 300–921600.
- Send File is read-only, 4 KB async chunks. Log path is user-chosen; invalid paths disable logging with a status error instead of crashing.
- Register safety: N/A — managed C# only, no direct registers, no `unsafe`, single P/Invoke
  (`user32.dll!SendMessage` for `WM_SETREDRAW` flicker-free batching only, no window subclassing).
  USB refresh via managed `WndProc`/`WM_DEVICECHANGE` override.

## Notes

- Loopback test: open two virtual COM ports (e.g. com0com) or a USB loopback plug, connect at `115200,8N1`, send `Hi` with CRLF — HEX send `48 69` decodes to `Hi`.
- Invalid HEX (e.g. `ZZ`) shows a status-bar error, never a crash.
- Cable unplug shows an error and closes the port; Auto-reconnect retries until replugged.
