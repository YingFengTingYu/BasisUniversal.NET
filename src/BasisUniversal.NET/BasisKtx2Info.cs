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
        : this(
            width,
            height,
            levels,
            layers,
            faces,
            basisTextureFormat,
            hasAlpha,
            isSrgb,
            basisTextureFormat == BasisTextureFormat.Etc1S,
            basisTextureFormat == BasisTextureFormat.UastcLdr4x4,
            IsHdrFormat(basisTextureFormat),
            basisTextureFormat == BasisTextureFormat.UastcHdr4x4,
            basisTextureFormat == BasisTextureFormat.AstcHdr6x6 || basisTextureFormat == BasisTextureFormat.UastcHdr6x6,
            !IsHdrFormat(basisTextureFormat),
            IsAstcLdrFormat(basisTextureFormat),
            IsXuastcLdrFormat(basisTextureFormat),
            GetDefaultBlockWidth(basisTextureFormat),
            GetDefaultBlockHeight(basisTextureFormat),
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            false,
            0f)
    {
    }

    internal BasisKtx2Info(
        int width,
        int height,
        int levels,
        int layers,
        int faces,
        BasisTextureFormat basisTextureFormat,
        bool hasAlpha,
        bool isSrgb,
        bool isEtc1S,
        bool isUastcLdr4x4,
        bool isHdr,
        bool isHdr4x4,
        bool isHdr6x6,
        bool isLdr,
        bool isAstcLdr,
        bool isXuastcLdr,
        int blockWidth,
        int blockHeight,
        int dfdColorModel,
        int dfdColorPrimaries,
        int dfdTransferFunction,
        int dfdFlags,
        int dfdTotalSamples,
        int dfdChannelId0,
        int dfdChannelId1,
        bool isVideo,
        float ldrHdrUpconversionNitMultiplier)
    {
        Width = width;
        Height = height;
        Levels = levels;
        Layers = layers;
        Faces = faces;
        BasisTextureFormat = basisTextureFormat;
        HasAlpha = hasAlpha;
        IsSrgb = isSrgb;
        IsEtc1S = isEtc1S;
        IsUastcLdr4x4 = isUastcLdr4x4;
        IsHdr = isHdr;
        IsHdr4x4 = isHdr4x4;
        IsHdr6x6 = isHdr6x6;
        IsLdr = isLdr;
        IsAstcLdr = isAstcLdr;
        IsXuastcLdr = isXuastcLdr;
        BlockWidth = blockWidth;
        BlockHeight = blockHeight;
        DfdColorModel = dfdColorModel;
        DfdColorPrimaries = dfdColorPrimaries;
        DfdTransferFunction = dfdTransferFunction;
        DfdFlags = dfdFlags;
        DfdTotalSamples = dfdTotalSamples;
        DfdChannelId0 = dfdChannelId0;
        DfdChannelId1 = dfdChannelId1;
        IsVideo = isVideo;
        LdrHdrUpconversionNitMultiplier = ldrHdrUpconversionNitMultiplier;
    }

    public int Width { get; }

    public int Height { get; }

    public int Levels { get; }

    public int Layers { get; }

    public int Faces { get; }

    public BasisTextureFormat BasisTextureFormat { get; }

    public bool HasAlpha { get; }

    public bool IsSrgb { get; }

    public bool IsEtc1S { get; }

    public bool IsUastcLdr4x4 { get; }

    public bool IsHdr { get; }

    public bool IsHdr4x4 { get; }

    public bool IsHdr6x6 { get; }

    public bool IsLdr { get; }

    public bool IsAstcLdr { get; }

    public bool IsXuastcLdr { get; }

    public int BlockWidth { get; }

    public int BlockHeight { get; }

    public int DfdColorModel { get; }

    public int DfdColorPrimaries { get; }

    public int DfdTransferFunction { get; }

    public int DfdFlags { get; }

    public int DfdTotalSamples { get; }

    public int DfdChannelId0 { get; }

    public int DfdChannelId1 { get; }

    public bool IsVideo { get; }

    public float LdrHdrUpconversionNitMultiplier { get; }

    private static bool IsHdrFormat(BasisTextureFormat format) =>
        format == BasisTextureFormat.UastcHdr4x4 ||
        format == BasisTextureFormat.AstcHdr6x6 ||
        format == BasisTextureFormat.UastcHdr6x6;

    private static bool IsAstcLdrFormat(BasisTextureFormat format) =>
        format >= BasisTextureFormat.AstcLdr4x4 &&
        format <= BasisTextureFormat.AstcLdr12x12;

    private static bool IsXuastcLdrFormat(BasisTextureFormat format) =>
        format >= BasisTextureFormat.XuastcLdr4x4 &&
        format <= BasisTextureFormat.XuastcLdr12x12;

    private static int GetDefaultBlockWidth(BasisTextureFormat format) =>
        format switch
        {
            BasisTextureFormat.XuastcLdr5x4 or BasisTextureFormat.AstcLdr5x4 => 5,
            BasisTextureFormat.XuastcLdr5x5 or BasisTextureFormat.AstcLdr5x5 => 5,
            BasisTextureFormat.XuastcLdr6x5 or BasisTextureFormat.AstcLdr6x5 => 6,
            BasisTextureFormat.XuastcLdr6x6 or BasisTextureFormat.AstcLdr6x6 => 6,
            BasisTextureFormat.XuastcLdr8x5 or BasisTextureFormat.AstcLdr8x5 => 8,
            BasisTextureFormat.XuastcLdr8x6 or BasisTextureFormat.AstcLdr8x6 => 8,
            BasisTextureFormat.XuastcLdr10x5 or BasisTextureFormat.AstcLdr10x5 => 10,
            BasisTextureFormat.XuastcLdr10x6 or BasisTextureFormat.AstcLdr10x6 => 10,
            BasisTextureFormat.XuastcLdr8x8 or BasisTextureFormat.AstcLdr8x8 => 8,
            BasisTextureFormat.XuastcLdr10x8 or BasisTextureFormat.AstcLdr10x8 => 10,
            BasisTextureFormat.XuastcLdr10x10 or BasisTextureFormat.AstcLdr10x10 => 10,
            BasisTextureFormat.XuastcLdr12x10 or BasisTextureFormat.AstcLdr12x10 => 12,
            BasisTextureFormat.XuastcLdr12x12 or BasisTextureFormat.AstcLdr12x12 => 12,
            _ => 4
        };

    private static int GetDefaultBlockHeight(BasisTextureFormat format) =>
        format switch
        {
            BasisTextureFormat.XuastcLdr5x4 or BasisTextureFormat.AstcLdr5x4 => 4,
            BasisTextureFormat.XuastcLdr5x5 or BasisTextureFormat.AstcLdr5x5 => 5,
            BasisTextureFormat.XuastcLdr6x5 or BasisTextureFormat.AstcLdr6x5 => 5,
            BasisTextureFormat.XuastcLdr6x6 or BasisTextureFormat.AstcLdr6x6 => 6,
            BasisTextureFormat.XuastcLdr8x5 or BasisTextureFormat.AstcLdr8x5 => 5,
            BasisTextureFormat.XuastcLdr8x6 or BasisTextureFormat.AstcLdr8x6 => 6,
            BasisTextureFormat.XuastcLdr10x5 or BasisTextureFormat.AstcLdr10x5 => 5,
            BasisTextureFormat.XuastcLdr10x6 or BasisTextureFormat.AstcLdr10x6 => 6,
            BasisTextureFormat.XuastcLdr8x8 or BasisTextureFormat.AstcLdr8x8 => 8,
            BasisTextureFormat.XuastcLdr10x8 or BasisTextureFormat.AstcLdr10x8 => 8,
            BasisTextureFormat.XuastcLdr10x10 or BasisTextureFormat.AstcLdr10x10 => 10,
            BasisTextureFormat.XuastcLdr12x10 or BasisTextureFormat.AstcLdr12x10 => 10,
            BasisTextureFormat.XuastcLdr12x12 or BasisTextureFormat.AstcLdr12x12 => 12,
            _ => 4
        };
}
