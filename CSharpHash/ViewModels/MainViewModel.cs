using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CSharpHash.Commands;
using CSharpHash.Services;

namespace CSharpHash.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private readonly HashingService _hashingService = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private long _token;

    // Snapshot of previous displayed results (restored on Cancel)
    private string? _prevHexOutput;
    private string? _prevBase64Output;
    private TimeSpan? _prevLastElapsed;
    private long? _prevLastBytes;
    private string? _prevLastPath;

    private string _pathInput = string.Empty;
    public string PathInput
    {
        get => _pathInput;
        set
        {
            if (_pathInput != value) // Only capture if the path is actually changing
            {
                // Capture the previous path before changing it
                if (!_isRestoringPath && !string.IsNullOrEmpty(_pathInput))
                {
                    _lastCompletedPath = _pathInput;
                }

                if (SetProperty(ref _pathInput, value))
                {
                    OnPropertyChanged(nameof(HasPathInput));
                    if (AutoHash && !_isRestoringPath && File.Exists(_pathInput))
                    {
                        _ = StartHashAsync();
                    }
                }
            }
        }
    }

    public bool HasPathInput => !string.IsNullOrEmpty(PathInput);

    private string _hexOutput = string.Empty;
    public string HexOutput
    {
        get => _hexOutput;
        set => SetProperty(ref _hexOutput, value);
    }

    private string _base64Output = string.Empty;
    public string Base64Output
    {
        get => _base64Output;
        set => SetProperty(ref _base64Output, value);
    }

    private bool _isHashing;
    public bool IsHashing
    {
        get => _isHashing;
        private set
        {
            if (SetProperty(ref _isHashing, value))
            {
                RaiseCommandStates();
                OnPropertyChanged(nameof(ShowResultInfo));
            }
        }
    }

    private bool _uppercase;
    public bool Uppercase
    {
        get => _uppercase;
        set
        {
            if (SetProperty(ref _uppercase, value))
            {
                HexOutput = _uppercase ? HexOutput.ToUpperInvariant() : HexOutput.ToLowerInvariant();
            }
        }
    }

    private bool _autoHash = true;
    public bool AutoHash
    {
        get => _autoHash;
        set => SetProperty(ref _autoHash, value);
    }

    private long _progressTotalBytes;
    public long ProgressTotalBytes
    {
        get => _progressTotalBytes;
        private set => SetProperty(ref _progressTotalBytes, value);
    }

    private long _progressProcessedBytes;
    public long ProgressProcessedBytes
    {
        get => _progressProcessedBytes;
        private set => SetProperty(ref _progressProcessedBytes, value);
    }

    public int ProgressPercent => ProgressTotalBytes <= 0 ? 0 : (int)(100 * Math.Min(1.0, (double)ProgressProcessedBytes / ProgressTotalBytes));

    private DateTimeOffset? _startedAt;
    public DateTimeOffset? StartedAt
    {
        get => _startedAt;
        private set => SetProperty(ref _startedAt, value);
    }

    private TimeSpan? _lastElapsed;
    public TimeSpan? LastElapsed
    {
        get => _lastElapsed;
        private set
        {
            if (SetProperty(ref _lastElapsed, value))
            {
                OnPropertyChanged(nameof(HasLastInfo));
                OnPropertyChanged(nameof(ShowResultInfo));
            }
        }
    }

    private long? _lastBytes;
    public long? LastBytes
    {
        get => _lastBytes;
        private set
        {
            if (SetProperty(ref _lastBytes, value))
            {
                OnPropertyChanged(nameof(HasLastInfo));
                OnPropertyChanged(nameof(ShowResultInfo));
            }
        }
    }

    private string? _lastPath;
    public string? LastPath
    {
        get => _lastPath;
        private set
        {
            if (SetProperty(ref _lastPath, value))
            {
                OnPropertyChanged(nameof(HasLastInfo));
                OnPropertyChanged(nameof(ShowResultInfo));
            }
        }
    }

    private string? _previousPathBeforeHash;
    public string? PreviousPathBeforeHash
    {
        get => _previousPathBeforeHash;
        private set => SetProperty(ref _previousPathBeforeHash, value);
    }

    private bool _isRestoringPath;
    private string? _lastCompletedPath;

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasLastInfo => !string.IsNullOrEmpty(LastPath) && LastBytes.HasValue && LastElapsed.HasValue;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool ShowResultInfo => HasLastInfo && !IsHashing;

    public ICommand BrowseCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand CopyHexCommand { get; }
    public ICommand CopyBase64Command { get; }
    public ICommand StartHashCommand { get; }

    public MainViewModel()
    {
        BrowseCommand = new RelayCommand(OnBrowse, () => !IsHashing);
        ClearCommand = new RelayCommand(OnClear, () => !IsHashing && !string.IsNullOrEmpty(PathInput));
        CancelCommand = new RelayCommand(OnCancel, () => IsHashing);
        CopyHexCommand = new RelayCommand(() => TryCopy(HexOutput), () => !IsHashing && !string.IsNullOrEmpty(HexOutput));
        CopyBase64Command = new RelayCommand(() => TryCopy(Base64Output), () => !IsHashing && !string.IsNullOrEmpty(Base64Output));
        StartHashCommand = new RelayCommand(() => _ = StartHashAsync(), () => !IsHashing && File.Exists(PathInput));
    }

    private void RaiseCommandStates()
    {
        (BrowseCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CopyHexCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CopyBase64Command as RelayCommand)?.RaiseCanExecuteChanged();
        (StartHashCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void OnBrowse()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select file to hash",
            CheckFileExists = true,
            CheckPathExists = true
        };
        if (dlg.ShowDialog() == true)
        {
            PathInput = dlg.FileName;
            if (AutoHash)
            {
                _ = StartHashAsync();
            }
        }
    }

    private void OnClear()
    {
        PathInput = string.Empty;
        HexOutput = string.Empty;
        Base64Output = string.Empty;
        ErrorMessage = null;
        LastElapsed = null;
        LastBytes = null;
        LastPath = null;
        ProgressProcessedBytes = 0;
        ProgressTotalBytes = 0;
        RaiseCommandStates();
        OnPropertyChanged(nameof(ShowResultInfo));
    }

    private void OnCancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    private static void TryCopy(string text)
    {
        try { Clipboard.SetText(text ?? string.Empty); }
        catch { /* ignore clipboard exceptions */ }
    }

    public async Task StartHashAsync()
    {
        if (IsHashing || !File.Exists(PathInput))
        {
            return;
        }

        ErrorMessage = null;
        PreviousPathBeforeHash = _lastCompletedPath;

        // Snapshot current outputs to allow restoring on cancel
        _prevHexOutput = HexOutput;
        _prevBase64Output = Base64Output;
        _prevLastElapsed = LastElapsed;
        _prevLastBytes = LastBytes;
        _prevLastPath = LastPath;

        HexOutput = string.Empty;
        Base64Output = string.Empty;
        ProgressProcessedBytes = 0;
        try
        {
            ProgressTotalBytes = new FileInfo(PathInput).Length;
        }
        catch
        {
            ProgressTotalBytes = 0;
        }
        OnPropertyChanged(nameof(ProgressPercent));
        StartedAt = DateTimeOffset.Now;
        LastElapsed = null;
        LastBytes = null;
        LastPath = null;

        _token++;
        long currentToken = _token;

        using var cts = new CancellationTokenSource();
        _cancellationTokenSource = cts;
        IsHashing = true;
        RaiseCommandStates();

        var progress = new Progress<long>(bytes =>
        {
            ProgressProcessedBytes = bytes;
            OnPropertyChanged(nameof(ProgressProcessedBytes));
            OnPropertyChanged(nameof(ProgressPercent));
        });

        try
        {
            var result = await Task.Run(() => _hashingService.ComputeSha256Async(PathInput, progress, cts.Token));

            if (_token != currentToken)
            {
                return; // stale result
            }

            LastElapsed = result.Elapsed;
            LastBytes = result.Bytes;
            LastPath = result.Path;
            ProgressTotalBytes = result.Bytes;
            ProgressProcessedBytes = result.Bytes;
            HexOutput = Uppercase ? result.Hex.ToUpperInvariant() : result.Hex;
            Base64Output = result.Base64;
            _lastCompletedPath = result.Path;
        }
        catch (OperationCanceledException)
        {
            // Restore previous path and previously displayed results on cancel
            _isRestoringPath = true;
            PathInput = PreviousPathBeforeHash ?? PathInput;
            _isRestoringPath = false;

            if (_prevLastPath != null || _prevLastBytes.HasValue || _prevLastElapsed.HasValue)
            {
                LastPath = _prevLastPath;
                LastBytes = _prevLastBytes;
                LastElapsed = _prevLastElapsed;
            }
            HexOutput = _prevHexOutput ?? string.Empty;
            Base64Output = _prevBase64Output ?? string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            if (_token == currentToken)
            {
                IsHashing = false;
                _cancellationTokenSource = null;
                RaiseCommandStates();
            }
        }
    }
}


