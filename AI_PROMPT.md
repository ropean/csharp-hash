## Project Prompt: csharp-hash (Professional WPF GUI SHA-256 hasher)

### Overview

- Goal: Professional desktop utility to compute SHA-256 for files with a clean, responsive UI and comprehensive features.
- Tech: C# 12, .NET 8, WPF; MVVM (no third-party libraries); Professional branding and packaging.
- Platforms: Windows primary (64-bit).

### Key Features

- Drag-and-drop file hashing, browse dialog, and path input with auto-completion.
- Live progress in window title as percent during hashing with smooth UI updates.
- Buttons disabled while hashing; Cancel to stop and restore previous path with proper state management.
- Outputs both HEX and Base64; toggle Uppercase HEX; auto-hash on selection.
- Professional unit formatting: file sizes (bytes/KB/MB/GB), elapsed time (ms/s/m/h), throughput (B/s/KB/s/MB/s/GB/s) with thousands separators and decimal precision.
- Shows comprehensive meta (formatted duration, size, throughput). Error feedback on failure.
- Professional application icon embedded in executable (no separate files needed).
- Complete GitHub Actions CI/CD pipeline with automated releases and dynamic versioning from git tags.
- Framework-dependent publishing for optimal file size (200KB vs 50MB self-contained).
- Bootstrapper scripts for automatic .NET runtime installation.
- Comprehensive assembly metadata, repository integration, and professional packaging.

### UI/UX Requirements

- Title: "C# Hash256" with dynamic progress suffix when hashing.
- Window: Centered on screen, professional icon embedded.
- Controls:
  - Path input: submit triggers Start with Enter key support.
  - Buttons: Browse, Clear, Cancel (Cancel visible only while hashing).
  - Toggles: Uppercase HEX, Auto hash on select.
  - Outputs: SHA-256 HEX and Base64 with Copy buttons.
  - Status: Shows formatted path, error, or comprehensive meta (formatted elapsed time, size, throughput).
  - Progress: Visual progress bar during hashing.
- Disable Browse/Clear/Copy while hashing.
- Professional formatting: All numeric values use appropriate units with thousands separators and decimal precision.

### Behavior & State

- Start hashing when:
  - User presses Enter in path input; or
  - Auto-hash + new path selected/pasted/browsed/dropped.
- Progress: Read stream in chunks (e.g., 1–4 MB). Report bytes processed via IProgress<long> and update a ProgressPercent property; update window title via DispatcherTimer ~100ms.
- Cancel: Use CancellationTokenSource; stop worker; clear progress; restore previous path and all previous results.
- Previous Path: Capture before path changes triggering hashing; restore on cancel with comprehensive state restoration (path, results, timing).
- State Management: Proper snapshot/restore of all UI state during cancel operations.

### Architecture

- `App.xaml` / `MainWindow.xaml`: Single-window WPF app using MVVM with professional branding.
- ViewModel: `MainViewModel` implements `INotifyPropertyChanged`; commands via a simple in-project `RelayCommand : ICommand`.
- Formatting: Comprehensive unit formatting methods for sizes (bytes/KB/MB/GB), time (ms/s/m/h), and throughput (B/s/KB/s/MB/s/GB/s).
- Concurrency: Hashing runs on a background Task. Progress tracked via `IProgress<long>`; cancellation via `CancellationTokenSource`; result marshaled to UI thread with `Dispatcher`.
- File drop: Enable `AllowDrop=true`; handle `PreviewDragOver`/`Drop` to accept files.
- Tokening: `long token` to track current hash to ignore outdated results.
- State Management: Comprehensive snapshot/restore system for cancel operations.
- Icon: Embedded professional icon with automatic WPF integration.

### Important Types

- Commands (`ICommand` via in-project `RelayCommand`): `BrowseCommand`, `ClearCommand`, `CancelCommand`, `CopyHexCommand`, `CopyBase64Command`, `StartHashCommand`.
- Properties: `PathInput`, `HexOutput`, `Base64Output`, `IsHashing`, `Uppercase`, `AutoHash`, `StartedAt`, `LastElapsed`, `LastBytes`, `LastPath`, `PreviousPathBeforeHash`, `ProgressTotalBytes`, `ProgressProcessedBytes`, `ProgressPercent`, `ErrorMessage`, `Token`.
- Formatted Properties: `LastBytesFormatted`, `LastElapsedFormatted`, `ThroughputFormatted` with professional unit formatting.
- Result DTO: `HashResult` with `Hex`, `Base64`, `Elapsed`, `Bytes`, `Path`.
- Formatting Methods: `FormatBytes()`, `FormatTimeSpan()`, `FormatThroughput()` with decimal units and thousands separators.

### Hashing Strategy

- Use `System.Security.Cryptography.SHA256`.
- Stream the file with `FileStream` and `IncrementalHash` or transform block approach to get progress updates.
- Buffer size tuned for throughput without UI starvation.

### Window Icon Strategy

- Icon embedded in executable via `<ApplicationIcon>` in .csproj
- WPF automatically uses embedded icon for taskbar, title bar, and file properties
- No separate icon file needed in distribution
- Professional branding maintained across all Windows UI contexts

### Build & Release

- Local: `scripts/build.cmd fd release` or `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false`
- Clean: `scripts/clean.cmd` removes all build artifacts and publish directory
- CI: `.github/workflows/release.yml` builds on tag push with dynamic versioning, publishes single-file `CSharpHash.exe`, creates automated GitHub release
- Bootstrapper: `scripts\CSharpHash.bat` and `scripts\CSharpHash.ps1` for automatic .NET runtime installation
- Distribution: Single 200KB executable with embedded icon and comprehensive metadata

### Constraints & Style

- C# 12, .NET 8, MVVM with no third-party libraries.
- Implement `INotifyPropertyChanged` manually; provide a lightweight `RelayCommand` implementation in-project.
- Explicit, readable code; async/await with cancellation.
- Avoid deep nesting; prefer early returns.
- No inline comments for trivial code; XML docs for complex parts.
- Preserve indentation and formatting.

### Current Implementation Status

✅ **Implemented Features:**
- Professional unit formatting (sizes, time, throughput with decimal units)
- Embedded application icon with WPF integration
- GitHub Actions CI/CD with dynamic versioning
- Framework-dependent publishing for optimal size
- Bootstrapper scripts for .NET installation
- Comprehensive state management and cancel recovery
- Professional assembly metadata and branding

### Future Enhancements (Optional)

- Multiple hash algorithms (SHA-1/512, BLAKE3), multi-file queue, pause/resume
- Dark/light theme options with system integration
- Localization and internationalization
- MSI installer (WiX/WinGet) for enterprise deployment
- Command-line interface for batch processing
- Hash verification against known checksums
- File integrity monitoring and change detection
