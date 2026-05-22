using BasisUniversal;
using BasisUniversal.LowLevel;

namespace BasisUniversal.NET.Tests;

public sealed class BasisUniversalCodecTests
{
    [Fact]
    public void EncodeKtx2_ThenTranscodeRgba32_RoundTripsShape()
    {
        var source = CreateRgba32(8, 8);

        var ktx2 = BasisUniversalCodec.EncodeKtx2(source, 8, 8);

        Assert.Equal(new byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x32, 0x30, 0xBB }, ktx2.AsSpan(0, 8).ToArray());
        using var texture = BasisKtx2Texture.Open(ktx2);
        var info = texture.Info;
        Assert.Equal(8, info.Width);
        Assert.Equal(8, info.Height);
        Assert.Equal(BasisTextureFormat.UastcLdr4x4, info.BasisTextureFormat);
        Assert.True(info.IsUastcLdr4x4);
        Assert.True(info.IsLdr);
        Assert.False(info.IsHdr);
        Assert.Equal(4, info.BlockWidth);
        Assert.Equal(4, info.BlockHeight);

        var levelInfo = texture.GetLevelInfo();
        Assert.Equal(8, levelInfo.OriginalWidth);
        Assert.Equal(8, levelInfo.OriginalHeight);
        Assert.Equal(8, levelInfo.ActualWidth);
        Assert.Equal(8, levelInfo.ActualHeight);
        Assert.Equal(2, levelInfo.BlockCountX);
        Assert.Equal(2, levelInfo.BlockCountY);
        Assert.Equal(4, levelInfo.TotalBlocks);

        var decoded = texture.TranscodeImageLevel(TranscoderTextureFormat.Rgba32);

        Assert.Equal(8, decoded.Width);
        Assert.Equal(8, decoded.Height);
        Assert.Equal(TranscoderTextureFormat.Rgba32, decoded.Format);
        Assert.Equal(8 * 8 * 4, decoded.Data.Length);
        Assert.Equal(decoded.Data.Length, decoded.SizeInBytes);
        Assert.Contains(decoded.Data, value => value != 0);
    }

    [Fact]
    public void EncodeKtx2_ThenTranscodeBc7_ProducesBlockPayload()
    {
        var source = CreateRgba32(8, 8);
        var ktx2 = BasisUniversalCodec.EncodeKtx2(source, 8, 8);

        using var texture = BasisKtx2Texture.Open(ktx2);
        var bc7 = texture.TranscodeImageLevel(TranscoderTextureFormat.Bc7Rgba);

        Assert.Equal(8, bc7.Width);
        Assert.Equal(8, bc7.Height);
        Assert.Equal(4 * 16, bc7.Data.Length);
    }

    [Fact]
    public void TranscodeImageLevel_WritesIntoCallerProvidedBuffer()
    {
        var source = CreateRgba32(8, 8);
        var ktx2 = BasisUniversalCodec.EncodeKtx2(source, 8, 8);

        using var texture = BasisKtx2Texture.Open(ktx2);
        var requiredBytes = BasisUniversalCodec.GetTranscodedImageSizeInBytes(TranscoderTextureFormat.Rgba32, 8, 8);
        var destination = new byte[requiredBytes + 16];

        var written = texture.TranscodeImageLevel(destination, TranscoderTextureFormat.Rgba32);

        Assert.Equal(requiredBytes, written);
        Assert.Contains(destination.AsSpan(0, written).ToArray(), value => value != 0);
    }

    [Fact]
    public void TranscodeImageLevel_WritesIntoCallerProvidedStream()
    {
        var source = CreateRgba32(8, 8);
        var ktx2 = BasisUniversalCodec.EncodeKtx2(source, 8, 8);

        using var texture = BasisKtx2Texture.Open(ktx2);
        using var destination = new MemoryStream();
        var requiredBytes = BasisUniversalCodec.GetTranscodedImageSizeInBytes(TranscoderTextureFormat.Rgba32, 8, 8);

        var written = texture.TranscodeImageLevel(destination, TranscoderTextureFormat.Rgba32);

        Assert.Equal(requiredBytes, written);
        Assert.Equal(requiredBytes, destination.Length);
        Assert.Contains(destination.ToArray(), value => value != 0);
    }

    [Fact]
    public void EncodeBasis_ProducesNonEmptyBasisPayload()
    {
        var source = CreateRgba32(8, 8);

        var basis = BasisUniversalCodec.EncodeBasis(source, 8, 8);

        Assert.NotEmpty(basis);
        Assert.NotEqual(0, BasisUniversalCodec.EncoderVersion);
        Assert.NotEqual(0, BasisUniversalCodec.TranscoderVersion);
    }

    [Fact]
    public void EncodeKtx2_WritesIntoCallerProvidedSpanAndStream()
    {
        var source = CreateRgba32(8, 8);
        var expected = BasisUniversalCodec.EncodeKtx2(source, 8, 8);
        var requiredBytes = BasisUniversalCodec.GetEncodedKtx2SizeInBytes(source, 8, 8);
        var estimatedBytes = BasisUniversalCodec.EstimateMaxEncodedKtx2SizeInBytes(8, 8);
        var spanDestination = new byte[estimatedBytes];
        using var streamDestination = new MemoryStream();

        var spanWritten = BasisUniversalCodec.EncodeKtx2(source, 8, 8, spanDestination);
        var streamWritten = BasisUniversalCodec.EncodeKtx2(source, 8, 8, streamDestination);

        Assert.Equal(expected.Length, requiredBytes);
        Assert.True(estimatedBytes >= requiredBytes);
        Assert.Equal(expected.Length, spanWritten);
        Assert.Equal(expected, spanDestination.AsSpan(0, spanWritten).ToArray());
        Assert.Equal(expected.Length, streamWritten);
        Assert.Equal(expected, streamDestination.ToArray());
    }

    [Fact]
    public void TryEncodeKtx2_ReturnsRequiredBytesWhenDestinationIsTooSmall()
    {
        var source = CreateRgba32(8, 8);
        var expected = BasisUniversalCodec.EncodeKtx2(source, 8, 8);

        var smallResult = BasisUniversalCodec.TryEncodeKtx2(
            source,
            8,
            8,
            Span<byte>.Empty,
            out var smallBytesWritten,
            out var requiredBytes);
        var destination = new byte[requiredBytes];
        var result = BasisUniversalCodec.TryEncodeKtx2(
            source,
            8,
            8,
            destination,
            out var bytesWritten,
            out var secondRequiredBytes);

        Assert.False(smallResult);
        Assert.Equal(0, smallBytesWritten);
        Assert.Equal(expected.Length, requiredBytes);
        Assert.True(result);
        Assert.Equal(expected.Length, bytesWritten);
        Assert.Equal(requiredBytes, secondRequiredBytes);
        Assert.Equal(expected, destination);
    }

    [Fact]
    public void EncodeBasis_WritesIntoCallerProvidedSpanAndStream()
    {
        var source = CreateRgba32(8, 8);
        var expected = BasisUniversalCodec.EncodeBasis(source, 8, 8);
        var requiredBytes = BasisUniversalCodec.GetEncodedBasisSizeInBytes(source, 8, 8);
        var estimatedBytes = BasisUniversalCodec.EstimateMaxEncodedBasisSizeInBytes(8, 8);
        var spanDestination = new byte[estimatedBytes];
        using var streamDestination = new MemoryStream();

        var spanWritten = BasisUniversalCodec.EncodeBasis(source, 8, 8, spanDestination);
        var streamWritten = BasisUniversalCodec.EncodeBasis(source, 8, 8, streamDestination);

        Assert.Equal(expected.Length, requiredBytes);
        Assert.True(estimatedBytes >= requiredBytes);
        Assert.Equal(expected.Length, spanWritten);
        Assert.Equal(expected, spanDestination.AsSpan(0, spanWritten).ToArray());
        Assert.Equal(expected.Length, streamWritten);
        Assert.Equal(expected, streamDestination.ToArray());
    }

    [Fact]
    public void FormatSupport_ReflectsBasisUniversalMatrix()
    {
        Assert.True(BasisUniversalCodec.IsTranscoderFormatSupported(
            TranscoderTextureFormat.Bc7Rgba,
            BasisTextureFormat.UastcLdr4x4));
        Assert.False(BasisUniversalCodec.IsTranscoderFormatSupported(
            TranscoderTextureFormat.Bc6H,
            BasisTextureFormat.UastcLdr4x4));
    }

    [Fact]
    public void FormatHelpers_ExposeBasisUniversalMetadata()
    {
        Assert.True(BasisUniversalCodec.IsBasisTextureFormatLdr(BasisTextureFormat.UastcLdr4x4));
        Assert.False(BasisUniversalCodec.IsBasisTextureFormatHdr(BasisTextureFormat.UastcLdr4x4));
        Assert.Equal(4, BasisUniversalCodec.GetBasisTextureFormatBlockWidth(BasisTextureFormat.UastcLdr4x4));
        Assert.Equal(4, BasisUniversalCodec.GetBasisTextureFormatBlockHeight(BasisTextureFormat.UastcLdr4x4));

        var defaultFormat = BasisUniversalCodec.GetDefaultTranscoderTextureFormat(BasisTextureFormat.UastcLdr4x4);
        Assert.True(BasisUniversalCodec.IsTranscoderFormatSupported(defaultFormat, BasisTextureFormat.UastcLdr4x4));

        Assert.Equal(16, BasisUniversalCodec.GetBytesPerBlockOrPixel(TranscoderTextureFormat.Bc7Rgba));
        Assert.True(BasisUniversalCodec.TranscoderFormatHasAlpha(TranscoderTextureFormat.Bc7Rgba));
        Assert.False(BasisUniversalCodec.IsTranscoderFormatUncompressed(TranscoderTextureFormat.Bc7Rgba));
        Assert.True(BasisUniversalCodec.IsTranscoderFormatUncompressed(TranscoderTextureFormat.Rgba32));
        Assert.Equal(4, BasisUniversalCodec.GetUncompressedBytesPerPixel(TranscoderTextureFormat.Rgba32));
        Assert.Equal(4, BasisUniversalCodec.GetTranscoderTextureFormatBlockWidth(TranscoderTextureFormat.Bc7Rgba));
        Assert.Equal(4, BasisUniversalCodec.GetTranscoderTextureFormatBlockHeight(TranscoderTextureFormat.Bc7Rgba));
    }

    [Fact]
    public void LowLevelPackage_ExposesRawBasisEntryPoints()
    {
        Basisu.EnsureInitialized();

        var parameters = Basisu.BuNewCompParams();
        try
        {
            Assert.NotEqual(0u, Basisu.BuGetVersion());
            Assert.NotEqual(0u, Basisu.BtGetVersion());
            Assert.NotEqual(0UL, parameters);
            Assert.NotEqual(0u, Basisu.BtBasisIsFormatSupported(
                (uint)TranscoderTextureFormat.Bc7Rgba,
                (uint)BasisTextureFormat.UastcLdr4x4));
        }
        finally
        {
            if (parameters != 0)
            {
                Basisu.BuDeleteCompParams(parameters);
            }
        }
    }

    private static byte[] CreateRgba32(int width, int height)
    {
        var pixels = new byte[checked(width * height * 4)];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var offset = ((y * width) + x) * 4;
                pixels[offset] = (byte)(x * 31);
                pixels[offset + 1] = (byte)(y * 31);
                pixels[offset + 2] = (byte)((x + y) * 17);
                pixels[offset + 3] = 255;
            }
        }

        return pixels;
    }
}
