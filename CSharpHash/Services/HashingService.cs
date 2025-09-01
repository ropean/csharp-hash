using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpHash.Services;

public sealed class HashResult
{
    public required string Hex { get; init; }
    public required string Base64 { get; init; }
    public required TimeSpan Elapsed { get; init; }
    public required long Bytes { get; init; }
    public required string Path { get; init; }
}

public sealed class HashingService
{
    public int BufferSize { get; set; } = 2 * 1024 * 1024;
    
    // Use incremental hash for potentially better performance
    public async Task<HashResult> ComputeSha256Async(
        string filePath,
        IProgress<long>? bytesProgress,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must be provided", nameof(filePath));
        }

        var stopwatch = Stopwatch.StartNew();

        // Use synchronous I/O in a background thread for potentially better performance
        var result = await Task.Run(() =>
        {
            using var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,  // Small OS buffer
                FileOptions.SequentialScan); // Hint for sequential access

            long totalLength = fileStream.Length;
            long processed = 0;

            // Use IncrementalHash which might be more optimized
            using var hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
            
            try
            {
                int read;
                while ((read = fileStream.Read(buffer, 0, BufferSize)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    // Process the hash incrementally
                    hasher.AppendData(buffer.AsSpan(0, read));
                    processed += read;
                    
                    // Report progress less frequently to reduce overhead
                    if (processed % (BufferSize * 4) == 0 || read < BufferSize)
                    {
                        bytesProgress?.Report(processed);
                    }
                }

                var hashBytes = hasher.GetHashAndReset();
                return (hashBytes, totalLength);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }, cancellationToken);

        var hex = ConvertToHexFast(result.hashBytes);
        var b64 = Convert.ToBase64String(result.hashBytes);

        stopwatch.Stop();

        return new HashResult
        {
            Hex = hex,
            Base64 = b64,
            Elapsed = stopwatch.Elapsed,
            Bytes = result.totalLength,
            Path = filePath
        };
    }

    // Fast hex conversion using lookup table (safe version)
    private static string ConvertToHexFast(ReadOnlySpan<byte> bytes)
    {
        // Use lookup table for fast conversion
        const string hexChars = "0123456789abcdef";
        
        var result = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            result[i * 2] = hexChars[b >> 4];
            result[i * 2 + 1] = hexChars[b & 0xF];
        }
        return new string(result);
    }

    // Alternative: Use hardware-accelerated SHA256 if available
    public async Task<HashResult> ComputeSha256HardwareAsync(
        string filePath,
        IProgress<long>? bytesProgress,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must be provided", nameof(filePath));
        }

        var stopwatch = Stopwatch.StartNew();

        await using var fileStream = File.OpenRead(filePath);
        long totalLength = fileStream.Length;

        // Try to use the fastest available SHA256 implementation
        using var sha256 = SHA256.Create(); // This will use hardware acceleration if available
        
        // Use larger buffer for better I/O efficiency
        var buffer = new byte[BufferSize];
        long processed = 0;

        int read;
        while ((read = await fileStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sha256.TransformBlock(buffer, 0, read, null, 0);
            processed += read;
            
            // Reduce progress reporting frequency
            if (processed % (BufferSize * 2) == 0 || read < BufferSize)
            {
                bytesProgress?.Report(processed);
            }
        }

        sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        var hashBytes = sha256.Hash ?? Array.Empty<byte>();

        var hex = Convert.ToHexString(hashBytes).ToLowerInvariant();
        var b64 = Convert.ToBase64String(hashBytes);

        stopwatch.Stop();

        return new HashResult
        {
            Hex = hex,
            Base64 = b64,
            Elapsed = stopwatch.Elapsed,
            Bytes = totalLength,
            Path = filePath
        };
    }
}