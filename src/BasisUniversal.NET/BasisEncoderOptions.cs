using System;

namespace BasisUniversal;

public sealed class BasisEncoderOptions
{
    public BasisTextureFormat Format { get; set; } = BasisTextureFormat.UastcLdr4x4;

    public int QualityLevel { get; set; } = 50;

    public int EffortLevel { get; set; } = 2;

    public BasisCompressionFlags Flags { get; set; } = BasisCompressionFlags.None;

    public float RdoOrDctQuality { get; set; } = 1.0f;

    internal void Validate()
    {
        BasisUniversalCodec.ValidateBasisTextureFormat(Format, nameof(Format));

        if (QualityLevel < 0 || QualityLevel > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(QualityLevel), QualityLevel, "Quality level must be in the range 0..100.");
        }

        if (EffortLevel < 0 || EffortLevel > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(EffortLevel), EffortLevel, "Effort level must be in the range 0..10.");
        }

        if (float.IsNaN(RdoOrDctQuality) || float.IsInfinity(RdoOrDctQuality) || RdoOrDctQuality < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(RdoOrDctQuality), RdoOrDctQuality, "RDO/DCT quality must be a finite non-negative value.");
        }
    }
}
