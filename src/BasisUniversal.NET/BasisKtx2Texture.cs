using System;
using System.Runtime.InteropServices;

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
                checked((int)BasisNative.Ktx2GetWidth(_handle)),
                checked((int)BasisNative.Ktx2GetHeight(_handle)),
                checked((int)BasisNative.Ktx2GetLevels(_handle)),
                checked((int)BasisNative.Ktx2GetLayers(_handle)),
                checked((int)BasisNative.Ktx2GetFaces(_handle)),
                (BasisTextureFormat)BasisNative.Ktx2GetBasisTextureFormat(_handle),
                BasisNative.Ktx2HasAlpha(_handle),
                BasisNative.Ktx2IsSrgb(_handle));
        }
    }

    public static BasisKtx2Texture Open(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
        {
            throw new ArgumentException("KTX2 data cannot be empty.", nameof(data));
        }

        BasisNative.EnsureInitialized();
        var copy = data.ToArray();
        var pin = GCHandle.Alloc(copy, GCHandleType.Pinned);
        try
        {
            var handle = BasisNative.Ktx2Open(BasisNative.AddressOf(pin.AddrOfPinnedObject()), checked((uint)copy.Length));
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

    public TranscodedImage TranscodeImageLevel(
        TranscoderTextureFormat format,
        int levelIndex = 0,
        int layerIndex = 0,
        int faceIndex = 0,
        BasisDecodeFlags decodeFlags = BasisDecodeFlags.None)
    {
        ThrowIfDisposed();
        ValidateIndex(levelIndex, nameof(levelIndex));
        ValidateIndex(layerIndex, nameof(layerIndex));
        ValidateIndex(faceIndex, nameof(faceIndex));

        if (!_started)
        {
            if (!BasisNative.Ktx2StartTranscoding(_handle))
            {
                throw new BasisUniversalException("Basis Universal failed to start KTX2 transcoding.");
            }

            _started = true;
        }

        var level = checked((uint)levelIndex);
        var layer = checked((uint)layerIndex);
        var face = checked((uint)faceIndex);
        var width = BasisNative.Ktx2GetLevelOrigWidth(_handle, level, layer, face);
        var height = BasisNative.Ktx2GetLevelOrigHeight(_handle, level, layer, face);
        var totalBlocks = BasisNative.Ktx2GetLevelTotalBlocks(_handle, level, layer, face);
        if (width == 0 || height == 0 || totalBlocks == 0)
        {
            throw new BasisUniversalException("Basis Universal returned invalid KTX2 level metadata.");
        }

        var outputBytes = BasisNative.ComputeTranscodedImageSizeInBytes(format, width, height);
        if (outputBytes == 0)
        {
            throw new BasisUniversalException($"Basis Universal cannot compute output size for transcoder format '{format}'.");
        }

        var output = new byte[checked((int)outputBytes)];
        var outputUnits = BasisNative.TranscoderFormatIsUncompressed(format)
            ? checked(width * height)
            : totalBlocks;

        fixed (byte* outputPtr = output)
        {
            if (!BasisNative.Ktx2TranscodeImageLevel(
                    _handle,
                    level,
                    layer,
                    face,
                    BasisNative.AddressOf(outputPtr),
                    outputUnits,
                    format,
                    decodeFlags))
            {
                throw new BasisUniversalException($"Basis Universal failed to transcode KTX2 level {levelIndex} to '{format}'.");
            }
        }

        return new TranscodedImage(checked((int)width), checked((int)height), format, output);
    }

    public void Dispose()
    {
        if (_handle != 0)
        {
            BasisNative.Ktx2Close(_handle);
            _handle = 0;
        }

        if (_pin.IsAllocated)
        {
            _pin.Free();
        }
    }

    private static void ValidateIndex(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Index must be non-negative.");
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
