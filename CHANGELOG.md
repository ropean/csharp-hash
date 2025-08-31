# Changelog

All notable changes to C# Hash256 will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-12-XX

### Added

- **Window Centering**: Application now starts centered on screen for better user experience
- **Framework-Dependent Publishing**: Configured for smaller executable size (~200KB vs ~50MB)
- **Bootstrapper Scripts**: Created `CSharpHash.bat` and `CSharpHash.ps1` for automatic .NET 8 installation
- **GitHub Actions CI/CD**: Automated release workflow that publishes executable to GitHub Releases
- **Optimized Build Script**: Enhanced `scripts/build.cmd` with argument parsing, better logging, and error handling
- **Repository Cleanup**: Added `publish/` directory to `.gitignore` and updated `clean.cmd`
- **Comprehensive Documentation**: Updated README with build instructions, release process, and usage examples
- **Unit Formatting**: Added proper formatting for file sizes (bytes/KB/MB/GB), elapsed time (ms/s/m/h), and throughput (B/s/KB/s/MB/s/GB/s) with thousands separators and decimal precision
- **Dynamic Versioning**: Wired git tag names into executable version numbers
- **Assembly Information**: Added comprehensive exe file metadata (company, copyright, description, etc.)

### Fixed

- **Cancel Operation File Path Recovery**: Fixed issue where canceling hash operation didn't properly restore the previous file path
- **Path Capture Timing**: Resolved timing issue in `PreviousPathBeforeHash` capture that was causing incorrect path restoration
- **KB Unit Support**: Added KB formatting for medium-sized files (e.g., 5,875 bytes → "5.88 KB")

### Changed

- **Publishing Strategy**: Switched from self-contained to framework-dependent publishing for smaller file sizes
- **Release Process**: Simplified to publish single executable file instead of ZIP archive
- **Build System**: Integrated optimized build script into GitHub Actions workflow
- **Size Display**: Enhanced file size display with KB units and proper decimal formatting
- **Performance Display**: Added throughput display with proper unit formatting
- **Repository URLs**: Updated to point to actual GitHub repository (https://github.com/ropean/csharp-hash)

### Technical Details

- **Framework**: .NET 8.0 Windows Desktop Runtime required
- **Architecture**: x64 (64-bit Windows)
- **Distribution**: Single-file executable via GitHub Releases
- **Build Tools**: Enhanced build and clean scripts in `scripts/` directory
- **Version Format**: Semantic versioning with git tag integration (e.g., `v1.2.3` → version `1.2.3`)
- **File Metadata**: Complete assembly information embedded in executable

### Breaking Changes

- **Publishing Mode**: Changed from self-contained to framework-dependent
- **Distribution**: Now requires .NET 8 Desktop Runtime installation on target machines
- **Release Format**: Single executable file instead of ZIP archive
- **File Size Units**: Now uses decimal units (1e9/1e6/1e3) instead of binary units

### Development

- **Build Script**: `scripts/build.cmd fd release` for framework-dependent release builds
- **Clean Script**: `scripts/clean.cmd` now includes publish directory cleanup
- **Git Workflow**: Automated releases triggered by version tags (e.g., `v1.0.0`)
- **Unit Formatting**: Decimal-based units with thousands separators and 2-decimal precision
- **Version Control**: Dynamic versioning from git tags with MSBuild parameter support

---

## Types of changes

- `Added` for new features
- `Changed` for changes in existing functionality
- `Deprecated` for soon-to-be removed features
- `Removed` for now removed features
- `Fixed` for any bug fixes
- `Security` in case of vulnerabilities

## Version History

- **1.0.0**: Initial release with complete packaging and distribution setup
- **0.x.x**: Development versions (not publicly released)

---

_This changelog was generated based on the development history and feature implementations._
