using System;

namespace BasisUniversal;

[Flags]
public enum BasisDecodeFlags : uint
{
    None = 0,
    PvrtcDecodeToNextPowerOfTwo = 2,
    TranscodeAlphaDataToOpaqueFormats = 4,
    Bc1ForbidThreeColorBlocks = 8,
    OutputHasAlphaIndices = 16,
    HighQuality = 32,
    NoEtc1sChromaFiltering = 64,
    NoDeblockFiltering = 128,
    StrongerDeblockFiltering = 256,
    ForceDeblockFiltering = 512,
    XuastcLdrDisableFastBc7Transcoding = 1024
}
