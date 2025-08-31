# C# Hash256

A simple WPF application for computing SHA256 hashes of files.

## Features

- Calculate SHA256 hashes for files
- Support for drag & drop
- Auto-hash when file is selected
- Copy hash results to clipboard
- Cancel long-running hash operations
- Professional icon and branding
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

**Features:**
- Professional icon embedded in executable
- Single-file deployment (no external dependencies needed)

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

1. Commit your changes
2. Create a git tag: `git tag v1.0.1` (version automatically set from tag name)
3. Push the tag: `git push origin v1.0.1`
4. GitHub Actions will automatically build and publish the release

The workflow will:

- Extract the version from the tag name (e.g., `v1.0.1` → version `1.0.1`)
- Build the executable with that version
- Publish the executable file to GitHub Releases

**Tag Format**: Use `v{major}.{minor}.{patch}` format (e.g., `v1.2.3`)

**Example**:

```bash
git add .
git commit -m "Add new feature"
git tag v1.0.1
git push origin main
git push origin v1.0.1  # This triggers the release build
```
