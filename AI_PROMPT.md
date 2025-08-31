## Project Prompt: csharp-hash (WPF GUI SHA-256 hasher)

### Overview

- Goal: Desktop utility to compute SHA-256 for files with a clean, responsive UI.
- Tech: C# 12, .NET 8, WPF; MVVM (no third-party libraries); Dark theme UI.
- Platforms: Windows primary.

### Key Features

- Drag-and-drop file hashing, browse dialog, and path input.
- Live progress in window title as percent during hashing.
- Buttons disabled while hashing; Cancel to stop and restore previous path.
- Outputs both HEX and Base64; toggle Uppercase HEX; auto-hash on selection.
- Shows meta (duration, size, throughput). Error feedback on failure.
- Window icon from env/paths with embedded fallback.
- GitHub Actions release pipeline with single-file publish.

### UI/UX Requirements

- Title: "C# Hash256" with dynamic progress suffix when hashing.
- Controls:
  - Path input: submit triggers Start.
  - Buttons: Browse, Clear, Cancel (Cancel visible only while hashing).
  - Toggles: Uppercase HEX, Auto hash on select.
  - Outputs: SHA-256 HEX and Base64 with Copy buttons.
  - Status: Shows path, error, or meta (elapsed, size, speed).
- Disable Browse/Clear/Copy while hashing.

### Behavior & State

- Start hashing when:
  - User presses Enter in path input; or
  - Auto-hash + new path selected/pasted/browsed/dropped.
- Progress: Read stream in chunks (e.g., 1–4 MB). Report bytes processed via IProgress<long> and update a ProgressPercent property; update window title via DispatcherTimer ~100ms.
- Cancel: Use CancellationTokenSource; stop worker; clear progress; restore previous path.
- Previous Path: Capture before path changes triggering hashing; restore on cancel.

### Architecture

- `App.xaml` / `MainWindow.xaml`: Single-window WPF app using MVVM.
- ViewModel: `MainViewModel` implements `INotifyPropertyChanged`; commands via a simple in-project `RelayCommand : ICommand`.
- Concurrency: Hashing runs on a background Task. Progress tracked via `IProgress<long>`; cancellation via `CancellationTokenSource`; result marshaled to UI thread with `Dispatcher`.
- File drop: Enable `AllowDrop=true`; handle `PreviewDragOver`/`Drop` to accept files.
- Tokening: `long token` to track current hash to ignore outdated results.

### Important Types

- Commands (`ICommand` via in-project `RelayCommand`): `BrowseCommand`, `ClearCommand`, `CancelCommand`, `CopyHexCommand`, `CopyBase64Command`, `StartHashCommand`.
- Properties: `PathInput`, `HexOutput`, `Base64Output`, `IsHashing`, `Uppercase`, `AutoHash`, `StartedAt`, `LastElapsed`, `LastBytes`, `LastPath`, `PreviousPathBeforeHash`, `ProgressTotalBytes`, `ProgressProcessedBytes`, `ProgressPercent`, `ErrorMessage`, `Token`.
- Result DTO: `HashResult` with `Hex`, `Base64`, `Elapsed`, `Bytes`, `Path`.

### Hashing Strategy

- Use `System.Security.Cryptography.SHA256`.
- Stream the file with `FileStream` and `IncrementalHash` or transform block approach to get progress updates.
- Buffer size tuned for throughput without UI starvation.

### Window Icon Loading Strategy

1. `APP_ICON`/`ICON` env var path.
2. `assets/app.ico` at CWD or exe-relative.
3. Embedded `assets/app.ico` as Resource; loaded via pack URI if not found on disk.

### Build & Release

- Local: `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained false`
- CI: `.github/workflows/release.yml` builds on tag push, publishes single-file artifact `csharp-hash.exe`, uploads artifact, creates release.

### Constraints & Style

- C# 12, .NET 8, MVVM with no third-party libraries.
- Implement `INotifyPropertyChanged` manually; provide a lightweight `RelayCommand` implementation in-project.
- Explicit, readable code; async/await with cancellation.
- Avoid deep nesting; prefer early returns.
- No inline comments for trivial code; XML docs for complex parts.
- Preserve indentation and formatting.

### Future Enhancements (Optional)

- Multiple hash algos (SHA-1/512, BLAKE3), multi-file queue, pause/resume.
- Theming options; localization; portable zip packaging; MSI installer (WiX/WinGet).
