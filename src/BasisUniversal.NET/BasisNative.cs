using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BasisUniversal;

internal static unsafe class BasisNative
{
    private const string LibraryName = "basisu_net";
    private static readonly object s_initLock = new object();
    private static int s_initialized;

    internal static void EnsureInitialized()
    {
        if (Volatile.Read(ref s_initialized) != 0)
        {
            return;
        }

        lock (s_initLock)
        {
            if (s_initialized != 0)
            {
                return;
            }

            basisu_net_encoder_init();
            basisu_net_transcoder_init();
            Volatile.Write(ref s_initialized, 1);
        }
    }

    internal static ulong AddressOf(void* pointer) => ((UIntPtr)pointer).ToUInt64();

    internal static ulong AddressOf(IntPtr pointer) => unchecked((ulong)pointer.ToInt64());

    internal static IntPtr ToIntPtr(ulong address) => new IntPtr(unchecked((long)address));

    internal static uint GetEncoderVersion() => basisu_net_get_encoder_version();

    internal static uint GetTranscoderVersion() => basisu_net_get_transcoder_version();

    internal static ulong NewCompressionParameters() => basisu_net_new_comp_params();

    internal static void DeleteCompressionParameters(ulong parameters) => basisu_net_delete_comp_params(parameters);

    internal static bool SetImageRgba32(ulong parameters, uint imageIndex, ulong imageData, uint width, uint height, uint pitchInBytes) =>
        basisu_net_comp_params_set_image_rgba32(parameters, imageIndex, imageData, width, height, pitchInBytes) != 0;

    internal static bool CompressTexture(
        ulong parameters,
        BasisTextureFormat format,
        int qualityLevel,
        int effortLevel,
        BasisCompressionFlags flags,
        float rdoOrDctQuality) =>
        basisu_net_compress_texture(parameters, (uint)format, qualityLevel, effortLevel, (ulong)flags, rdoOrDctQuality) != 0;

    internal static ulong GetCompressedDataSize(ulong parameters) => basisu_net_comp_params_get_comp_data_size(parameters);

    internal static ulong GetCompressedDataPointer(ulong parameters) => basisu_net_comp_params_get_comp_data_pointer(parameters);

    internal static bool TranscoderFormatIsUncompressed(TranscoderTextureFormat format) =>
        basisu_net_transcoder_format_is_uncompressed((uint)format) != 0;

    internal static bool TranscoderFormatIsSupported(TranscoderTextureFormat transcoderFormat, BasisTextureFormat basisTextureFormat) =>
        basisu_net_transcoder_format_is_supported((uint)transcoderFormat, (uint)basisTextureFormat) != 0;

    internal static uint ComputeTranscodedImageSizeInBytes(TranscoderTextureFormat format, uint width, uint height) =>
        basisu_net_compute_transcoded_image_size_in_bytes((uint)format, width, height);

    internal static ulong Ktx2Open(ulong data, uint dataLength) => basisu_net_ktx2_open(data, dataLength);

    internal static void Ktx2Close(ulong handle) => basisu_net_ktx2_close(handle);

    internal static uint Ktx2GetWidth(ulong handle) => basisu_net_ktx2_get_width(handle);

    internal static uint Ktx2GetHeight(ulong handle) => basisu_net_ktx2_get_height(handle);

    internal static uint Ktx2GetLevels(ulong handle) => basisu_net_ktx2_get_levels(handle);

    internal static uint Ktx2GetFaces(ulong handle) => basisu_net_ktx2_get_faces(handle);

    internal static uint Ktx2GetLayers(ulong handle) => basisu_net_ktx2_get_layers(handle);

    internal static uint Ktx2GetBasisTextureFormat(ulong handle) => basisu_net_ktx2_get_basis_texture_format(handle);

    internal static bool Ktx2HasAlpha(ulong handle) => basisu_net_ktx2_has_alpha(handle) != 0;

    internal static bool Ktx2IsSrgb(ulong handle) => basisu_net_ktx2_is_srgb(handle) != 0;

    internal static uint Ktx2GetLevelOrigWidth(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        basisu_net_ktx2_get_level_orig_width(handle, levelIndex, layerIndex, faceIndex);

    internal static uint Ktx2GetLevelOrigHeight(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        basisu_net_ktx2_get_level_orig_height(handle, levelIndex, layerIndex, faceIndex);

    internal static uint Ktx2GetLevelTotalBlocks(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        basisu_net_ktx2_get_level_total_blocks(handle, levelIndex, layerIndex, faceIndex);

    internal static bool Ktx2StartTranscoding(ulong handle) => basisu_net_ktx2_start_transcoding(handle) != 0;

    internal static bool Ktx2TranscodeImageLevel(
        ulong handle,
        uint levelIndex,
        uint layerIndex,
        uint faceIndex,
        ulong output,
        uint outputBlocksOrPixels,
        TranscoderTextureFormat transcoderFormat,
        BasisDecodeFlags decodeFlags) =>
        basisu_net_ktx2_transcode_image_level(
            handle,
            levelIndex,
            layerIndex,
            faceIndex,
            output,
            outputBlocksOrPixels,
            (uint)transcoderFormat,
            (uint)decodeFlags,
            0,
            0,
            -1,
            -1) != 0;

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_get_encoder_version();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_get_transcoder_version();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void basisu_net_encoder_init();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong basisu_net_new_comp_params();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_delete_comp_params(ulong parameters);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_comp_params_set_image_rgba32(
        ulong parameters,
        uint imageIndex,
        ulong imageData,
        uint width,
        uint height,
        uint pitchInBytes);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_compress_texture(
        ulong parameters,
        uint format,
        int qualityLevel,
        int effortLevel,
        ulong flags,
        float rdoOrDctQuality);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong basisu_net_comp_params_get_comp_data_size(ulong parameters);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong basisu_net_comp_params_get_comp_data_pointer(ulong parameters);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void basisu_net_transcoder_init();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_transcoder_format_is_uncompressed(uint format);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_transcoder_format_is_supported(uint transcoderFormat, uint basisTextureFormat);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_compute_transcoded_image_size_in_bytes(uint transcoderFormat, uint width, uint height);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong basisu_net_ktx2_open(ulong data, uint dataLength);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void basisu_net_ktx2_close(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_width(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_height(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_levels(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_faces(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_layers(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_basis_texture_format(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_has_alpha(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_is_srgb(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_level_orig_width(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_level_orig_height(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_get_level_total_blocks(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_start_transcoding(ulong handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint basisu_net_ktx2_transcode_image_level(
        ulong handle,
        uint levelIndex,
        uint layerIndex,
        uint faceIndex,
        ulong output,
        uint outputBlocksOrPixels,
        uint transcoderFormat,
        uint decodeFlags,
        uint outputRowPitchBlocksOrPixels,
        uint outputRowsInPixels,
        int channel0,
        int channel1);
}
