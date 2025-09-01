using System;
using System.Diagnostics;
using System.IO;
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
    private const int DefaultBufferSize = 1 * 1024 * 1024; // 1 MB

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

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            DefaultBufferSize,
            useAsync: true);

        long totalLength = fileStream.Length;
        long processed = 0;

        using var sha256 = SHA256.Create();
        var buffer = new byte[DefaultBufferSize];

        int read;
        while ((read = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sha256.TransformBlock(buffer, 0, read, null, 0);
            processed += read;
            bytesProgress?.Report(processed);
        }

        sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        var hashBytes = sha256.Hash ?? Array.Empty<byte>();

        var hex = ConvertToHex(hashBytes);
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

    private static string ConvertToHex(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            _ = sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }
}


