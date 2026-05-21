using System;
using System.Runtime.InteropServices;

namespace BasisUniversal;

public static unsafe class BasisUniversalCodec
{
    public static int EncoderVersion
    {
        get
        {
            BasisNative.EnsureInitialized();
            return checked((int)BasisNative.GetEncoderVersion());
        }
    }

    public static int TranscoderVersion
    {
        get
        {
            BasisNative.EnsureInitialized();
            return checked((int)BasisNative.GetTranscoderVersion());
        }
    }

    public static byte[] EncodeKtx2(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, options, flags);
    }

    public static byte[] EncodeBasis(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, options, flags);
    }

    public static bool IsTranscoderFormatSupported(
        TranscoderTextureFormat transcoderFormat,
        BasisTextureFormat basisTextureFormat)
    {
        BasisNative.EnsureInitialized();
        return BasisNative.TranscoderFormatIsSupported(transcoderFormat, basisTextureFormat);
    }

    public static int GetTranscodedImageSizeInBytes(TranscoderTextureFormat format, int width, int height)
    {
        ValidateDimensions(width, height);
        BasisNative.EnsureInitialized();
        return checked((int)BasisNative.ComputeTranscodedImageSizeInBytes(format, checked((uint)width), checked((uint)height)));
    }

    private static byte[] Encode(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        options.Validate();
        ValidateDimensions(width, height);
        var requiredBytes = checked(width * height * 4);
        if (rgba32.Length < requiredBytes)
        {
            throw new ArgumentException($"RGBA32 source requires at least {requiredBytes} bytes for {width}x{height}.", nameof(rgba32));
        }

        BasisNative.EnsureInitialized();
        var parameters = BasisNative.NewCompressionParameters();
        if (parameters == 0)
        {
            throw new BasisUniversalException("Basis Universal failed to allocate compression parameters.");
        }

        try
        {
            fixed (byte* sourcePtr = rgba32)
            {
                if (!BasisNative.SetImageRgba32(
                        parameters,
                        imageIndex: 0,
                        BasisNative.AddressOf(sourcePtr),
                        checked((uint)width),
                        checked((uint)height),
                        checked((uint)(width * 4))))
                {
                    throw new BasisUniversalException("Basis Universal failed to set the RGBA32 source image.");
                }
            }

            if (!BasisNative.CompressTexture(
                    parameters,
                    options.Format,
                    options.QualityLevel,
                    options.EffortLevel,
                    flags,
                    options.RdoOrDctQuality))
            {
                throw new BasisUniversalException("Basis Universal failed to encode the texture.");
            }

            var outputSize = BasisNative.GetCompressedDataSize(parameters);
            var outputPointer = BasisNative.GetCompressedDataPointer(parameters);
            if (outputSize == 0 || outputPointer == 0)
            {
                throw new BasisUniversalException("Basis Universal returned an empty encoded payload.");
            }

            var result = new byte[checked((int)outputSize)];
            Marshal.Copy(BasisNative.ToIntPtr(outputPointer), result, 0, result.Length);
            return result;
        }
        finally
        {
            BasisNative.DeleteCompressionParameters(parameters);
        }
    }

    private static void ValidateDimensions(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");
        }
    }
}
