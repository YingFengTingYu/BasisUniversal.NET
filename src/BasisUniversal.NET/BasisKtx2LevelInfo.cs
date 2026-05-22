namespace BasisUniversal;

public readonly struct BasisKtx2LevelInfo
{
    public BasisKtx2LevelInfo(
        int levelIndex,
        int layerIndex,
        int faceIndex,
        int originalWidth,
        int originalHeight,
        int actualWidth,
        int actualHeight,
        int blockCountX,
        int blockCountY,
        int totalBlocks,
        bool hasAlpha,
        bool isIframe)
    {
        LevelIndex = levelIndex;
        LayerIndex = layerIndex;
        FaceIndex = faceIndex;
        OriginalWidth = originalWidth;
        OriginalHeight = originalHeight;
        ActualWidth = actualWidth;
        ActualHeight = actualHeight;
        BlockCountX = blockCountX;
        BlockCountY = blockCountY;
        TotalBlocks = totalBlocks;
        HasAlpha = hasAlpha;
        IsIframe = isIframe;
    }

    public int LevelIndex { get; }

    public int LayerIndex { get; }

    public int FaceIndex { get; }

    public int OriginalWidth { get; }

    public int OriginalHeight { get; }

    public int ActualWidth { get; }

    public int ActualHeight { get; }

    public int BlockCountX { get; }

    public int BlockCountY { get; }

    public int TotalBlocks { get; }

    public bool HasAlpha { get; }

    public bool IsIframe { get; }
}
