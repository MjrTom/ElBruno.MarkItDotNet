namespace ElBruno.MarkItDotNet.Converters;

/// <summary>
/// Converts image files to a Markdown image reference.
/// Supports: .jpg, .jpeg, .png, .gif, .bmp, .webp, .svg.
/// OCR support is planned for v2 (behind <see cref="MarkItDotNetOptions.EnableOcr"/>).
/// </summary>
public class ImageConverter : IMarkdownConverter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".webp",
        ".svg"
    };

    /// <inheritdoc />
    public bool CanHandle(string fileExtension) =>
        SupportedExtensions.Contains(fileExtension);

    /// <inheritdoc />
    public Task<string> ConvertAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        var extension = fileExtension.TrimStart('.').ToUpperInvariant();
        var filename = $"image.{fileExtension.TrimStart('.').ToLowerInvariant()}";

        // Attempt to read basic dimensions from known formats
        var dimensions = TryGetDimensions(fileStream, fileExtension);
        fileStream.Position = 0;

        var markdown = $"![Image]({filename})";

        if (dimensions is not null)
        {
            markdown += $"\n\n*Image: {extension} format, {dimensions.Value.Width}×{dimensions.Value.Height} pixels*";
        }
        else
        {
            markdown += $"\n\n*Image: {extension} format*";
        }

        // TODO: v2 — OCR support behind MarkItDotNetOptions.EnableOcr

        return Task.FromResult(markdown);
    }

    private static (int Width, int Height)? TryGetDimensions(Stream stream, string extension)
    {
        try
        {
            if (!stream.CanSeek)
                return null;

            var ext = extension.ToLowerInvariant();

            return ext switch
            {
                ".png" => TryReadPngDimensions(stream),
                ".jpg" or ".jpeg" => TryReadJpegDimensions(stream),
                ".gif" => TryReadGifDimensions(stream),
                ".bmp" => TryReadBmpDimensions(stream),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static (int Width, int Height)? TryReadPngDimensions(Stream stream)
    {
        // PNG: bytes 16-19 = width, 20-23 = height (big-endian)
        var header = new byte[24];
        stream.Position = 0;
        if (stream.Read(header, 0, 24) < 24) return null;
        if (header[0] != 0x89 || header[1] != 0x50) return null; // PNG magic

        var width = (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
        var height = (header[20] << 24) | (header[21] << 16) | (header[22] << 8) | header[23];
        return (width, height);
    }

    private static (int Width, int Height)? TryReadGifDimensions(Stream stream)
    {
        // GIF: bytes 6-7 = width, 8-9 = height (little-endian)
        var header = new byte[10];
        stream.Position = 0;
        if (stream.Read(header, 0, 10) < 10) return null;
        if (header[0] != 'G' || header[1] != 'I' || header[2] != 'F') return null;

        var width = header[6] | (header[7] << 8);
        var height = header[8] | (header[9] << 8);
        return (width, height);
    }

    private static (int Width, int Height)? TryReadBmpDimensions(Stream stream)
    {
        // BMP: bytes 18-21 = width, 22-25 = height (little-endian)
        var header = new byte[26];
        stream.Position = 0;
        if (stream.Read(header, 0, 26) < 26) return null;
        if (header[0] != 'B' || header[1] != 'M') return null;

        var width = header[18] | (header[19] << 8) | (header[20] << 16) | (header[21] << 24);
        var height = header[22] | (header[23] << 8) | (header[24] << 16) | (header[25] << 24);
        return (width, Math.Abs(height));
    }

    private static (int Width, int Height)? TryReadJpegDimensions(Stream stream)
    {
        // JPEG: scan for SOF0 marker (0xFF 0xC0) — height at offset+5, width at offset+7
        stream.Position = 0;
        var buffer = new byte[2];

        if (stream.Read(buffer, 0, 2) < 2 || buffer[0] != 0xFF || buffer[1] != 0xD8)
            return null;

        while (stream.Position < stream.Length - 1)
        {
            if (stream.ReadByte() != 0xFF) continue;

            var marker = stream.ReadByte();
            if (marker == -1) break;

            // SOF markers: 0xC0..0xC3 (most common frames)
            if (marker is >= 0xC0 and <= 0xC3)
            {
                var sof = new byte[7];
                if (stream.Read(sof, 0, 7) < 7) return null;
                var height = (sof[3] << 8) | sof[4];
                var width = (sof[5] << 8) | sof[6];
                return (width, height);
            }

            // Skip segment
            if (stream.Read(buffer, 0, 2) < 2) break;
            var length = (buffer[0] << 8) | buffer[1];
            if (length < 2) break;
            stream.Position += length - 2;
        }

        return null;
    }
}
