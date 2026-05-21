namespace BasisUniversal;

public sealed class TranscodedImage
{
    public TranscodedImage(int width, int height, TranscoderTextureFormat format, byte[] data)
    {
        Width = width;
        Height = height;
        Format = format;
        Data = data;
    }

    public int Width { get; }

    public int Height { get; }

    public TranscoderTextureFormat Format { get; }

    public byte[] Data { get; }
}
