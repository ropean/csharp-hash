# C# Hash Application - TODO List

## UI/UX Improvements

- [x] **File Path Recovery**: When clicking Cancel, the file path in file selector should also be recovered
- [ ] **Window Positioning**: Start application in screen center
- [ ] **Drag & Drop Enhancement**: Improve visual feedback during drag operations
- [ ] **Dark/Light Theme**: Add theme switching capability
- [ ] **Accessibility**: Add keyboard shortcuts and screen reader support

## Features & Functionality

- [ ] **Multiple Hash Algorithms**: Add support for MD5, SHA1, SHA512, etc.
- [ ] **Batch Processing**: Allow multiple file selection and batch hashing
- [ ] **Hash Comparison**: Compare two files or hashes
- [ ] **Hash Database**: Save and manage hash history
- [ ] **File Integrity Check**: Verify files against known hashes
- [ ] **Export Results**: Save hash results to file (CSV, JSON, etc.)

## Units and Formatting

- [ ] **File Size Display**:
  - Choose between GB/MB/bytes (decimal: 1e9/1e6)
  - Add thousands separators
  - Up to 2 decimal places for GB/MB
- [ ] **Time Display**:
  - Choose between ms/s/m/h
  - Add thousands separators
  - Up to 2 decimal places
- [ ] **Throughput Display**:
  - Choose between GB/s, MB/s, KB/s (decimal)
  - Up to 2 decimal places

## Performance & Technical

- [ ] **Async Improvements**: Optimize file reading and hashing performance
- [ ] **Memory Management**: Handle very large files more efficiently
- [ ] **Progress Reporting**: More granular progress updates for large files
- [ ] **Error Handling**: Better error messages and recovery mechanisms
- [ ] **Logging**: Add application logging for debugging

## Configuration & Settings

- [ ] **User Preferences**: Save user settings between sessions
- [ ] **Default Values**: Configurable default hash algorithm and output format
- [ ] **Hotkeys**: Customizable keyboard shortcuts
- [ ] **Language Support**: Internationalization (i18n)

## GitHub & Deployment

- [ ] **GitHub Release Config**: Create automated release configuration
- [ ] **Version Management**: Wire tag name into version properly
- [ ] **CI/CD Pipeline**: Set up automated builds and testing
- [ ] **Code Signing**: Sign the application for Windows
- [ ] **Installer**: Create MSI or installer package

## Testing & Quality

- [ ] **Unit Tests**: Add comprehensive test coverage
- [ ] **Integration Tests**: Test file handling and hash calculation
- [ ] **Performance Tests**: Benchmark hash calculation speed
- [ ] **UI Tests**: Automated UI testing

## Documentation

- [ ] **User Manual**: Create comprehensive user documentation
- [ ] **API Documentation**: Document internal APIs and services
- [ ] **Contributing Guide**: Guidelines for contributors
- [ ] **Changelog**: Maintain detailed change history

## Future Enhancements

- [ ] **Cloud Integration**: Hash files from cloud storage (OneDrive, Google Drive)
- [ ] **Network Support**: Hash files from URLs
- [ ] **Plugin System**: Extensible architecture for custom hash algorithms
- [ ] **Cross-Platform**: Consider .NET MAUI for cross-platform support
