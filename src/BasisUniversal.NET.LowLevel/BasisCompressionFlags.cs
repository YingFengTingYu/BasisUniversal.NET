using System;

namespace BasisUniversal;

[Flags]
public enum BasisCompressionFlags : ulong
{
    None = 0,
    UseOpenCl = 1UL << 8,
    Threaded = 1UL << 9,
    DebugOutput = 1UL << 10,
    Ktx2Output = 1UL << 11,
    Ktx2UastcZstd = 1UL << 12,
    Srgb = 1UL << 13,
    GenerateMipsClamp = 1UL << 14,
    GenerateMipsWrap = 1UL << 15,
    YFlip = 1UL << 16,
    PrintStats = 1UL << 18,
    PrintStatus = 1UL << 19,
    DebugImages = 1UL << 20,
    Rec2020 = 1UL << 21,
    ValidateOutput = 1UL << 22,
    XuastcLdrHybrid = 1UL << 23,
    XuastcLdrFullZstd = 2UL << 23,
    TextureType2D = 0UL << 25,
    TextureType2DArray = 1UL << 25,
    TextureTypeCubemapArray = 2UL << 25,
    TextureTypeVideoFrames = 3UL << 25
}
