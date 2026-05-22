using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using BasisUniversal.LowLevel;

namespace BasisUniversal;

public sealed unsafe class BasisKtx2Texture : IDisposable
{
    private readonly GCHandle _pin;
    private ulong _handle;
    private bool _started;

    private BasisKtx2Texture(GCHandle pin, ulong handle)
    {
        _pin = pin;
        _handle = handle;
    }

    public BasisKtx2Info Info
    {
        get
        {
            ThrowIfDisposed();
            return new BasisKtx2Info(
                checked((int)Basisu.BtKtx2GetWidth(_handle)),
                checked((int)Basisu.BtKtx2GetHeight(_handle)),
                checked((int)Basisu.BtKtx2GetLevels(_handle)),
                checked((int)Basisu.BtKtx2GetLayers(_handle)),
                checked((int)Basisu.BtKtx2GetFaces(_handle)),
                (BasisTextureFormat)Basisu.BtKtx2GetBasisTexFormat(_handle),
                Basisu.BtKtx2HasAlpha(_handle) != 0,
                Basisu.BtKtx2IsSrgb(_handle) != 0,
                Basisu.BtKtx2IsEtc1s(_handle) != 0,
                Basisu.BtKtx2IsUastcLdr4x4(_handle) != 0,
                Basisu.BtKtx2IsHdr(_handle) != 0,
                Basisu.BtKtx2IsHdr4x4(_handle) != 0,
                Basisu.BtKtx2IsHdr6x6(_handle) != 0,
                Basisu.BtKtx2IsLdr(_handle) != 0,
                Basisu.BtKtx2IsAstcLdr(_handle) != 0,
                Basisu.BtKtx2IsXuastcLdr(_handle) != 0,
                checked((int)Basisu.BtKtx2GetBlockWidth(_handle)),
                checked((int)Basisu.BtKtx2GetBlockHeight(_handle)),
                checked((int)Basisu.BtKtx2GetDfdColorModel(_handle)),
                checked((int)Basisu.BtKtx2GetDfdColorPrimaries(_handle)),
                checked((int)Basisu.BtKtx2GetDfdTransferFunc(_handle)),
                checked((int)Basisu.BtKtx2GetDfdFlags(_handle)),
                checked((int)Basisu.BtKtx2GetDfdTotalSamples(_handle)),
                checked((int)Basisu.BtKtx2GetDfdChannelId0(_handle)),
                checked((int)Basisu.BtKtx2GetDfdChannelId1(_handle)),
                Basisu.BtKtx2IsVideo(_handle) != 0,
                Basisu.BtKtx2GetLdrHdrUpconversionNitMultiplier(_handle));
        }
    }

    public static BasisKtx2Texture Open(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
        {
            throw new ArgumentException("KTX2 data cannot be empty.", nameof(data));
        }

        Basisu.EnsureInitialized();
        var copy = data.ToArray();
        var pin = GCHandle.Alloc(copy, GCHandleType.Pinned);
        try
        {
            var handle = Basisu.BtKtx2Open(Basisu.ToUInt64(Basisu.AddressOf(pin.AddrOfPinnedObject())), checked((uint)copy.Length));
            if (handle == 0)
            {
                throw new BasisUniversalException("Basis Universal failed to open the KTX2 payload.");
            }

            return new BasisKtx2Texture(pin, handle);
        }
        catch
        {
            pin.Free();
            throw;
        }
    }

    public BasisKtx2LevelInfo GetLevelInfo(int levelIndex = 0, int layerIndex = 0, int faceIndex = 0)
    {
        ThrowIfDisposed();
        ValidateCoordinates(levelIndex, layerIndex, faceIndex);

        var level = checked((uint)levelIndex);
        var layer = checked((uint)layerIndex);
        var face = checked((uint)faceIndex);
        return new BasisKtx2LevelInfo(
            levelIndex,
            layerIndex,
            faceIndex,
            checked((int)Basisu.BtKtx2GetLevelOrigWidth(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelOrigHeight(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelActualWidth(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelActualHeight(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelNumBlocksX(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelNumBlocksY(_handle, level, layer, face)),
            checked((int)Basisu.BtKtx2GetLevelTotalBlocks(_handle, level, layer, face)),
            Basisu.BtKtx2GetLevelAlphaFlag(_handle, level, layer, face) != 0,
            Basisu.BtKtx2GetLevelIframeFlag(_handle, level, layer, face) != 0);
    }

    public TranscodedImage TranscodeImageLevel(
        TranscoderTextureFormat format,
        int levelIndex = 0,
        int layerIndex = 0,
        int faceIndex = 0,
        BasisDecodeFlags decodeFlags = BasisDecodeFlags.None)
    {
        ThrowIfDisposed();
        var levelInfo = GetLevelInfo(levelIndex, layerIndex, faceIndex);
        var outputBytes = GetRequiredOutputByteCount(format, levelInfo);
        var output = new byte[checked((int)outputBytes)];
        TranscodeImageLevel(
            output,
            format,
            levelIndex,
            layerIndex,
            faceIndex,
            decodeFlags);

        return new TranscodedImage(levelInfo.OriginalWidth, levelInfo.OriginalHeight, format, output);
    }

    public int TranscodeImageLevel(
        Span<byte> destination,
        TranscoderTextureFormat format,
        int levelIndex = 0,
        int layerIndex = 0,
        int faceIndex = 0,
        BasisDecodeFlags decodeFlags = BasisDecodeFlags.None)
    {
        ThrowIfDisposed();
        BasisUniversalCodec.ValidateTranscoderTextureFormat(format);
        var levelInfo = GetLevelInfo(levelIndex, layerIndex, faceIndex);
        var outputBytes = GetRequiredOutputByteCount(format, levelInfo);
        if (destination.Length < outputBytes)
        {
            throw new ArgumentException($"Destination requires at least {outputBytes} bytes.", nameof(destination));
        }

        EnsureTranscodingStarted();
        var outputUnits = Basisu.BtBasisTranscoderFormatIsUncompressed((uint)format) != 0
            ? checked((uint)(levelInfo.OriginalWidth * levelInfo.OriginalHeight))
            : checked((uint)levelInfo.TotalBlocks);

        fixed (byte* outputPtr = destination)
        {
            if (Basisu.BtKtx2TranscodeImageLevel(
                    _handle,
                    checked((uint)levelIndex),
                    checked((uint)layerIndex),
                    checked((uint)faceIndex),
                    Basisu.ToUInt64(Basisu.AddressOf(outputPtr)),
                    outputUnits,
                    (uint)format,
                    (uint)decodeFlags,
                    outputRowPitchInBlocksOrPixels: 0,
                    outputRowsInPixels: 0,
                    channel0: -1,
                    channel1: -1,
                    stateHandle: 0) == 0)
            {
                throw new BasisUniversalException($"Basis Universal failed to transcode KTX2 level {levelIndex} to '{format}'.");
            }
        }

        return checked((int)outputBytes);
    }

    public int TranscodeImageLevel(
        Stream destination,
        TranscoderTextureFormat format,
        int levelIndex = 0,
        int layerIndex = 0,
        int faceIndex = 0,
        BasisDecodeFlags decodeFlags = BasisDecodeFlags.None)
    {
        ThrowIfDisposed();
        BasisUniversalCodec.ValidateDestinationStream(destination);
        var levelInfo = GetLevelInfo(levelIndex, layerIndex, faceIndex);
        var outputBytes = checked((int)GetRequiredOutputByteCount(format, levelInfo));
        var buffer = ArrayPool<byte>.Shared.Rent(outputBytes);
        try
        {
            var written = TranscodeImageLevel(
                buffer.AsSpan(0, outputBytes),
                format,
                levelIndex,
                layerIndex,
                faceIndex,
                decodeFlags);
            destination.Write(buffer.AsSpan(0, written));
            return written;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public void Dispose()
    {
        if (_handle != 0)
        {
            Basisu.BtKtx2Close(_handle);
            _handle = 0;
        }

        if (_pin.IsAllocated)
        {
            _pin.Free();
        }
    }

    private void EnsureTranscodingStarted()
    {
        if (_started)
        {
            return;
        }

        if (Basisu.BtKtx2StartTranscoding(_handle) == 0)
        {
            throw new BasisUniversalException("Basis Universal failed to start KTX2 transcoding.");
        }

        _started = true;
    }

    private uint GetRequiredOutputByteCount(TranscoderTextureFormat format, BasisKtx2LevelInfo levelInfo)
    {
        BasisUniversalCodec.ValidateTranscoderTextureFormat(format);
        if (levelInfo.OriginalWidth == 0 || levelInfo.OriginalHeight == 0 || levelInfo.TotalBlocks == 0)
        {
            throw new BasisUniversalException("Basis Universal returned invalid KTX2 level metadata.");
        }

        var outputBytes = Basisu.BtBasisComputeTranscodedImageSizeInBytes(
            (uint)format,
            checked((uint)levelInfo.OriginalWidth),
            checked((uint)levelInfo.OriginalHeight));
        if (outputBytes == 0)
        {
            throw new BasisUniversalException($"Basis Universal cannot compute output size for transcoder format '{format}'.");
        }

        return outputBytes;
    }

    private void ValidateCoordinates(int levelIndex, int layerIndex, int faceIndex)
    {
        ValidateIndex(levelIndex, nameof(levelIndex));
        ValidateIndex(layerIndex, nameof(layerIndex));
        ValidateIndex(faceIndex, nameof(faceIndex));

        var info = Info;
        ValidateIndexUpperBound(levelIndex, Math.Max(info.Levels, 1), nameof(levelIndex));
        ValidateIndexUpperBound(layerIndex, Math.Max(info.Layers, 1), nameof(layerIndex));
        ValidateIndexUpperBound(faceIndex, Math.Max(info.Faces, 1), nameof(faceIndex));
    }

    private static void ValidateIndex(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Index must be non-negative.");
        }
    }

    private static void ValidateIndexUpperBound(int value, int count, string parameterName)
    {
        if (value >= count)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Index must be less than {count}.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (_handle == 0)
        {
            throw new ObjectDisposedException(nameof(BasisKtx2Texture));
        }
    }
}
