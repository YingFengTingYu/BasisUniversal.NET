using BasisUniversal;

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

        var decoded = texture.TranscodeImageLevel(TranscoderTextureFormat.Rgba32);

        Assert.Equal(8, decoded.Width);
        Assert.Equal(8, decoded.Height);
        Assert.Equal(TranscoderTextureFormat.Rgba32, decoded.Format);
        Assert.Equal(8 * 8 * 4, decoded.Data.Length);
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
    public void EncodeBasis_ProducesNonEmptyBasisPayload()
    {
        var source = CreateRgba32(8, 8);

        var basis = BasisUniversalCodec.EncodeBasis(source, 8, 8);

        Assert.NotEmpty(basis);
        Assert.NotEqual(0, BasisUniversalCodec.EncoderVersion);
        Assert.NotEqual(0, BasisUniversalCodec.TranscoderVersion);
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
