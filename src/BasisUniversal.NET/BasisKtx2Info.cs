namespace BasisUniversal;

public readonly struct BasisKtx2Info
{
    public BasisKtx2Info(
        int width,
        int height,
        int levels,
        int layers,
        int faces,
        BasisTextureFormat basisTextureFormat,
        bool hasAlpha,
        bool isSrgb)
    {
        Width = width;
        Height = height;
        Levels = levels;
        Layers = layers;
        Faces = faces;
        BasisTextureFormat = basisTextureFormat;
        HasAlpha = hasAlpha;
        IsSrgb = isSrgb;
    }

    public int Width { get; }

    public int Height { get; }

    public int Levels { get; }

    public int Layers { get; }

    public int Faces { get; }

    public BasisTextureFormat BasisTextureFormat { get; }

    public bool HasAlpha { get; }

    public bool IsSrgb { get; }
}
