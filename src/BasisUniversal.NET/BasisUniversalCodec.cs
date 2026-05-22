using System;
using System.IO;
using System.Runtime.InteropServices;
using BasisUniversal.LowLevel;

namespace BasisUniversal;

public static unsafe class BasisUniversalCodec
{
    private const int EstimatedBasisContainerOverheadBytes = 64 * 1024;
    private const int EstimatedKtx2ContainerOverheadBytes = 128 * 1024;
    private const int EstimatedPerLevelOverheadBytes = 4096;
    private const int EstimatedEncodedBytesPerBlock = 64;

    public static int EncoderVersion
    {
        get
        {
            Basisu.EnsureInitialized();
            return checked((int)Basisu.BuGetVersion());
        }
    }

    public static int TranscoderVersion
    {
        get
        {
            Basisu.EnsureInitialized();
            return checked((int)Basisu.BtGetVersion());
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

    public static int EncodeKtx2(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, destination, options, flags);
    }

    public static bool TryEncodeKtx2(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        out int bytesWritten,
        out int requiredBytes,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return TryEncode(rgba32, width, height, destination, out bytesWritten, out requiredBytes, options, flags);
    }

    public static long EncodeKtx2(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Stream destination,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, destination, options, flags);
    }

    public static int GetEncodedKtx2SizeInBytes(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return GetEncodedSizeInBytes(rgba32, width, height, options, flags);
    }

    public static int EstimateMaxEncodedKtx2SizeInBytes(
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags | BasisCompressionFlags.Ktx2Output;
        return EstimateMaxEncodedSizeInBytes(width, height, options, flags, EstimatedKtx2ContainerOverheadBytes);
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

    public static int EncodeBasis(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, destination, options, flags);
    }

    public static bool TryEncodeBasis(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        out int bytesWritten,
        out int requiredBytes,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return TryEncode(rgba32, width, height, destination, out bytesWritten, out requiredBytes, options, flags);
    }

    public static long EncodeBasis(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Stream destination,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return Encode(rgba32, width, height, destination, options, flags);
    }

    public static int GetEncodedBasisSizeInBytes(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return GetEncodedSizeInBytes(rgba32, width, height, options, flags);
    }

    public static int EstimateMaxEncodedBasisSizeInBytes(
        int width,
        int height,
        BasisEncoderOptions? options = null)
    {
        options ??= new BasisEncoderOptions();
        var flags = options.Flags & ~BasisCompressionFlags.Ktx2Output;
        return EstimateMaxEncodedSizeInBytes(width, height, options, flags, EstimatedBasisContainerOverheadBytes);
    }

    public static bool IsTranscoderFormatSupported(
        TranscoderTextureFormat transcoderFormat,
        BasisTextureFormat basisTextureFormat)
    {
        ValidateTranscoderTextureFormat(transcoderFormat, nameof(transcoderFormat));
        ValidateBasisTextureFormat(basisTextureFormat, nameof(basisTextureFormat));
        Basisu.EnsureInitialized();
        return Basisu.BtBasisIsFormatSupported((uint)transcoderFormat, (uint)basisTextureFormat) != 0;
    }

    public static int GetTranscodedImageSizeInBytes(TranscoderTextureFormat format, int width, int height)
    {
        ValidateTranscoderTextureFormat(format);
        ValidateDimensions(width, height);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisComputeTranscodedImageSizeInBytes((uint)format, checked((uint)width), checked((uint)height)));
    }

    public static bool IsBasisTextureFormatXuastcLdr(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTexFormatIsXuastcLdr((uint)format) != 0;
    }

    public static bool IsBasisTextureFormatAstcLdr(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTexFormatIsAstcLdr((uint)format) != 0;
    }

    public static bool IsBasisTextureFormatHdr(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTexFormatIsHdr((uint)format) != 0;
    }

    public static bool IsBasisTextureFormatLdr(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTexFormatIsLdr((uint)format) != 0;
    }

    public static int GetBasisTextureFormatBlockWidth(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisTexFormatGetBlockWidth((uint)format));
    }

    public static int GetBasisTextureFormatBlockHeight(BasisTextureFormat format)
    {
        ValidateBasisTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisTexFormatGetBlockHeight((uint)format));
    }

    public static TranscoderTextureFormat GetDefaultTranscoderTextureFormat(BasisTextureFormat basisTextureFormat)
    {
        ValidateBasisTextureFormat(basisTextureFormat, nameof(basisTextureFormat));
        Basisu.EnsureInitialized();
        return (TranscoderTextureFormat)Basisu.BtBasisGetTranscoderTextureFormatFromBasisTexFormat((uint)basisTextureFormat);
    }

    public static int GetBytesPerBlockOrPixel(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisGetBytesPerBlockOrPixel((uint)format));
    }

    public static bool TranscoderFormatHasAlpha(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTranscoderFormatHasAlpha((uint)format) != 0;
    }

    public static bool IsTranscoderFormatHdr(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTranscoderFormatIsHdr((uint)format) != 0;
    }

    public static bool IsTranscoderFormatLdr(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTranscoderFormatIsLdr((uint)format) != 0;
    }

    public static bool IsTranscoderFormatAstc(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTranscoderTextureFormatIsAstc((uint)format) != 0;
    }

    public static bool IsTranscoderFormatUncompressed(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return Basisu.BtBasisTranscoderFormatIsUncompressed((uint)format) != 0;
    }

    public static int GetUncompressedBytesPerPixel(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisGetUncompressedBytesPerPixel((uint)format));
    }

    public static int GetTranscoderTextureFormatBlockWidth(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisGetBlockWidth((uint)format));
    }

    public static int GetTranscoderTextureFormatBlockHeight(TranscoderTextureFormat format)
    {
        ValidateTranscoderTextureFormat(format);
        Basisu.EnsureInitialized();
        return checked((int)Basisu.BtBasisGetBlockHeight((uint)format));
    }

    private static byte[] Encode(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        using var encoded = EncodeToNative(rgba32, width, height, options, flags);
        var result = new byte[checked((int)encoded.DataSize)];
        Marshal.Copy(Basisu.ToIntPtr(encoded.DataPointer), result, 0, result.Length);
        return result;
    }

    private static int Encode(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        using var encoded = EncodeToNative(rgba32, width, height, options, flags);
        return CopyNativeBytes(encoded.DataPointer, encoded.DataSize, destination);
    }

    private static bool TryEncode(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Span<byte> destination,
        out int bytesWritten,
        out int requiredBytes,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        using var encoded = EncodeToNative(rgba32, width, height, options, flags);
        requiredBytes = checked((int)encoded.DataSize);
        if (destination.Length < requiredBytes)
        {
            bytesWritten = 0;
            return false;
        }

        CopyNativeBytes(encoded.DataPointer, encoded.DataSize, destination);
        bytesWritten = requiredBytes;
        return true;
    }

    private static long Encode(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        Stream destination,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        ValidateDestinationStream(destination);
        using var encoded = EncodeToNative(rgba32, width, height, options, flags);
        var bytesWritten = checked((long)encoded.DataSize);
        WriteNativeBytes(encoded.DataPointer, encoded.DataSize, destination);
        return bytesWritten;
    }

    private static int GetEncodedSizeInBytes(
        ReadOnlySpan<byte> rgba32,
        int width,
        int height,
        BasisEncoderOptions options,
        BasisCompressionFlags flags)
    {
        using var encoded = EncodeToNative(rgba32, width, height, options, flags);
        return checked((int)encoded.DataSize);
    }

    private static int EstimateMaxEncodedSizeInBytes(
        int width,
        int height,
        BasisEncoderOptions options,
        BasisCompressionFlags flags,
        int containerOverheadBytes)
    {
        options.Validate();
        ValidateDimensions(width, height);

        var (blockWidth, blockHeight) = GetBasisTextureFormatBlockSize(options.Format);
        var includeGeneratedMips = (flags & (BasisCompressionFlags.GenerateMipsClamp | BasisCompressionFlags.GenerateMipsWrap)) != 0;
        var levelWidth = width;
        var levelHeight = height;
        long rawBytes = 0;
        long encodedBlockBudgetBytes = 0;
        long levelCount = 0;

        while (true)
        {
            levelCount++;
            rawBytes = checked(rawBytes + ((long)levelWidth * levelHeight * 4));

            var blocksX = DivideRoundUp(levelWidth, blockWidth);
            var blocksY = DivideRoundUp(levelHeight, blockHeight);
            encodedBlockBudgetBytes = checked(encodedBlockBudgetBytes + ((long)blocksX * blocksY * EstimatedEncodedBytesPerBlock));

            if (!includeGeneratedMips || (levelWidth == 1 && levelHeight == 1))
            {
                break;
            }

            levelWidth = Math.Max(1, levelWidth / 2);
            levelHeight = Math.Max(1, levelHeight / 2);
        }

        var estimate = checked(
            (long)containerOverheadBytes +
            (levelCount * EstimatedPerLevelOverheadBytes) +
            rawBytes +
            encodedBlockBudgetBytes);
        return checked((int)estimate);
    }

    private static NativeEncodedTexture EncodeToNative(
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

        Basisu.EnsureInitialized();
        var parameters = Basisu.BuNewCompParams();
        if (parameters == 0)
        {
            throw new BasisUniversalException("Basis Universal failed to allocate compression parameters.");
        }

        try
        {
            fixed (byte* sourcePtr = rgba32)
            {
                if (Basisu.BuCompParamsSetImageRgba32(
                        parameters,
                        imageIndex: 0,
                        Basisu.ToUInt64(Basisu.AddressOf(sourcePtr)),
                        checked((uint)width),
                        checked((uint)height),
                        checked((uint)(width * 4))) == 0)
                {
                    throw new BasisUniversalException("Basis Universal failed to set the RGBA32 source image.");
                }
            }

            if (Basisu.BuCompressTexture(
                    parameters,
                    (uint)options.Format,
                    options.QualityLevel,
                    options.EffortLevel,
                    (ulong)flags,
                    options.RdoOrDctQuality) == 0)
            {
                throw new BasisUniversalException("Basis Universal failed to encode the texture.");
            }

            var outputSize = Basisu.BuCompParamsGetCompDataSize(parameters);
            var outputPointer = Basisu.BuCompParamsGetCompDataOfs(parameters);
            if (outputSize == 0 || outputPointer == 0)
            {
                throw new BasisUniversalException("Basis Universal returned an empty encoded payload.");
            }

            var encoded = new NativeEncodedTexture(parameters, outputPointer, outputSize);
            parameters = 0;
            return encoded;
        }
        finally
        {
            if (parameters != 0)
            {
                Basisu.BuDeleteCompParams(parameters);
            }
        }
    }

    private static int CopyNativeBytes(ulong sourcePointer, ulong sourceSize, Span<byte> destination)
    {
        var size = checked((int)sourceSize);
        if (destination.Length < size)
        {
            throw new ArgumentException($"Destination requires at least {size} bytes.", nameof(destination));
        }

        new ReadOnlySpan<byte>(Basisu.ToIntPtr(sourcePointer).ToPointer(), size).CopyTo(destination);
        return size;
    }

    private static void WriteNativeBytes(ulong sourcePointer, ulong sourceSize, Stream destination)
    {
        const int MaxStreamWriteSize = 1024 * 1024;
        var source = (byte*)Basisu.ToIntPtr(sourcePointer).ToPointer();
        var remaining = sourceSize;
        while (remaining != 0)
        {
            var bytesToWrite = remaining > MaxStreamWriteSize ? MaxStreamWriteSize : (int)remaining;
            destination.Write(new ReadOnlySpan<byte>(source, bytesToWrite));
            source += bytesToWrite;
            remaining -= (ulong)bytesToWrite;
        }
    }

    internal static void ValidateDestinationStream(Stream destination)
    {
        if (destination is null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination stream must be writable.", nameof(destination));
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

    private static int DivideRoundUp(int value, int divisor) => checked(((value - 1) / divisor) + 1);

    private static (int Width, int Height) GetBasisTextureFormatBlockSize(BasisTextureFormat format) =>
        format switch
        {
            BasisTextureFormat.AstcHdr6x6 or
            BasisTextureFormat.UastcHdr6x6 or
            BasisTextureFormat.XuastcLdr6x6 or
            BasisTextureFormat.AstcLdr6x6 => (6, 6),

            BasisTextureFormat.XuastcLdr5x4 or
            BasisTextureFormat.AstcLdr5x4 => (5, 4),

            BasisTextureFormat.XuastcLdr5x5 or
            BasisTextureFormat.AstcLdr5x5 => (5, 5),

            BasisTextureFormat.XuastcLdr6x5 or
            BasisTextureFormat.AstcLdr6x5 => (6, 5),

            BasisTextureFormat.XuastcLdr8x5 or
            BasisTextureFormat.AstcLdr8x5 => (8, 5),

            BasisTextureFormat.XuastcLdr8x6 or
            BasisTextureFormat.AstcLdr8x6 => (8, 6),

            BasisTextureFormat.XuastcLdr10x5 or
            BasisTextureFormat.AstcLdr10x5 => (10, 5),

            BasisTextureFormat.XuastcLdr10x6 or
            BasisTextureFormat.AstcLdr10x6 => (10, 6),

            BasisTextureFormat.XuastcLdr8x8 or
            BasisTextureFormat.AstcLdr8x8 => (8, 8),

            BasisTextureFormat.XuastcLdr10x8 or
            BasisTextureFormat.AstcLdr10x8 => (10, 8),

            BasisTextureFormat.XuastcLdr10x10 or
            BasisTextureFormat.AstcLdr10x10 => (10, 10),

            BasisTextureFormat.XuastcLdr12x10 or
            BasisTextureFormat.AstcLdr12x10 => (12, 10),

            BasisTextureFormat.XuastcLdr12x12 or
            BasisTextureFormat.AstcLdr12x12 => (12, 12),

            _ => (4, 4)
        };

    internal static void ValidateBasisTextureFormat(BasisTextureFormat format, string parameterName = "format")
    {
        if ((uint)format >= (uint)BasisTextureFormat.TotalFormats)
        {
            throw new ArgumentOutOfRangeException(parameterName, format, "Unsupported Basis texture format.");
        }
    }

    internal static void ValidateTranscoderTextureFormat(TranscoderTextureFormat format, string parameterName = "format")
    {
        if ((uint)format >= (uint)TranscoderTextureFormat.TotalTextureFormats || (uint)format == 7)
        {
            throw new ArgumentOutOfRangeException(parameterName, format, "Unsupported transcoder texture format.");
        }
    }

    private readonly struct NativeEncodedTexture : IDisposable
    {
        public NativeEncodedTexture(ulong parameters, ulong dataPointer, ulong dataSize)
        {
            Parameters = parameters;
            DataPointer = dataPointer;
            DataSize = dataSize;
        }

        public ulong Parameters { get; }

        public ulong DataPointer { get; }

        public ulong DataSize { get; }

        public void Dispose()
        {
            if (Parameters != 0)
            {
                Basisu.BuDeleteCompParams(Parameters);
            }
        }
    }
}
