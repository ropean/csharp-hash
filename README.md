# C# Hash256

A simple WPF application for computing SHA256 hashes of files.

## Features

- Calculate SHA256 hashes for files
- Support for drag & drop
- Auto-hash when file is selected
- Copy hash results to clipboard
- Cancel long-running hash operations
- Lightweight framework-dependent executable

## Usage

### Running from Source

```bash
dotnet run --project CSharpHash/CSharpHash.csproj
```

### Standalone Executable

Download the latest `CSharpHash.exe` from the [Releases](https://github.com/yourusername/csharp-hash/releases) page.

**Requirements:**
- Windows 10/11 (64-bit)
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) pre-installed

Simply download and run `CSharpHash.exe`!

## Building

### Development Build

```bash
dotnet build CSharpHash/CSharpHash.csproj
```

### Release Build (Framework-dependent)

```bash
scripts/build.cmd fd release
```

Build script arguments:
- `fd` or `framework-dependent`: Framework-dependent publishing (default)
- `sc` or `self-contained`: Self-contained publishing
- `debug`: Debug configuration
- `release`: Release configuration (default)

Or using individual dotnet commands:

```bash
dotnet publish CSharpHash/CSharpHash.csproj --configuration Release --self-contained false --runtime win-x64 --output ./publish
```

## Creating Releases

To create a new release:

1. Update version numbers in `CSharpHash/CSharpHash.csproj`
2. Commit your changes
3. Create a git tag: `git tag v1.0.1`
4. Push the tag: `git push origin v1.0.1`
5. GitHub Actions will automatically build and publish the release

The workflow will automatically publish the executable file to GitHub Releases.
