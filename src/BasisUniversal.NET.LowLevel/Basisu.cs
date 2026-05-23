using System;
using System.Runtime.InteropServices;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Threading;

namespace BasisUniversal.LowLevel;

public static unsafe partial class Basisu
{
    private const string LibraryName = "basisu";
    private static readonly object s_initLock = new object();
    private static int s_initialized;

    public static void EnsureInitialized()
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

            bu_init();
            bt_init();
            Volatile.Write(ref s_initialized, 1);
        }
    }

    public static nint AddressOf(void* pointer) => (nint)pointer;

    public static nint AddressOf(IntPtr pointer) => pointer;

    public static ulong ToUInt64(nint address) => unchecked((ulong)(long)address);

    public static IntPtr ToIntPtr(ulong address) => new IntPtr(unchecked((long)address));

    public static uint BuGetVersion() => bu_get_version();

    public static void BuEnableDebugPrintf(uint flag) => bu_enable_debug_printf(flag);

    public static void BuInit() => bu_init();

    public static ulong BuAlloc(ulong size) => bu_alloc(size);

    public static void BuFree(ulong ofs) => bu_free(ofs);

    public static ulong BuNewCompParams() => bu_new_comp_params();

    public static uint BuDeleteCompParams(ulong paramsOfs) => bu_delete_comp_params(paramsOfs);

    public static ulong BuCompParamsGetCompDataSize(ulong paramsOfs) => bu_comp_params_get_comp_data_size(paramsOfs);

    public static ulong BuCompParamsGetCompDataOfs(ulong paramsOfs) => bu_comp_params_get_comp_data_ofs(paramsOfs);

    public static uint BuCompParamsClear(ulong paramsOfs) => bu_comp_params_clear(paramsOfs);

    public static uint BuCompParamsSetImageRgba32(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes) =>
        bu_comp_params_set_image_rgba32(paramsOfs, imageIndex, imageDataOfs, width, height, pitchInBytes);

    public static uint BuCompParamsSetImageFloatRgba(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes) =>
        bu_comp_params_set_image_float_rgba(paramsOfs, imageIndex, imageDataOfs, width, height, pitchInBytes);

    public static uint BuCompressTexture(
        ulong paramsOfs,
        uint desiredBasisTextureFormat,
        int qualityLevel,
        int effortLevel,
        ulong flagsAndQuality,
        float lowLevelUastcRdoOrDctQuality) =>
        bu_compress_texture(
            paramsOfs,
            desiredBasisTextureFormat,
            qualityLevel,
            effortLevel,
            flagsAndQuality,
            lowLevelUastcRdoOrDctQuality);

    public static uint BtGetVersion() => bt_get_version();

    public static void BtEnableDebugPrintf(uint flag) => bt_enable_debug_printf(flag);

    public static void BtInit() => bt_init();

    public static ulong BtAlloc(ulong size) => bt_alloc(size);

    public static void BtFree(ulong ofs) => bt_free(ofs);

    public static uint BtBasisTexFormatIsXuastcLdr(uint basisTexFormat) =>
        bt_basis_tex_format_is_xuastc_ldr(basisTexFormat);

    public static uint BtBasisTexFormatIsAstcLdr(uint basisTexFormat) =>
        bt_basis_tex_format_is_astc_ldr(basisTexFormat);

    public static uint BtBasisTexFormatGetBlockWidth(uint basisTexFormat) =>
        bt_basis_tex_format_get_block_width(basisTexFormat);

    public static uint BtBasisTexFormatGetBlockHeight(uint basisTexFormat) =>
        bt_basis_tex_format_get_block_height(basisTexFormat);

    public static uint BtBasisTexFormatIsHdr(uint basisTexFormat) =>
        bt_basis_tex_format_is_hdr(basisTexFormat);

    public static uint BtBasisTexFormatIsLdr(uint basisTexFormat) =>
        bt_basis_tex_format_is_ldr(basisTexFormat);

    public static uint BtBasisGetBytesPerBlockOrPixel(uint transcoderTextureFormat) =>
        bt_basis_get_bytes_per_block_or_pixel(transcoderTextureFormat);

    public static uint BtBasisTranscoderFormatHasAlpha(uint transcoderTextureFormat) =>
        bt_basis_transcoder_format_has_alpha(transcoderTextureFormat);

    public static uint BtBasisTranscoderFormatIsHdr(uint transcoderTextureFormat) =>
        bt_basis_transcoder_format_is_hdr(transcoderTextureFormat);

    public static uint BtBasisTranscoderFormatIsLdr(uint transcoderTextureFormat) =>
        bt_basis_transcoder_format_is_ldr(transcoderTextureFormat);

    public static uint BtBasisTranscoderTextureFormatIsAstc(uint transcoderTextureFormat) =>
        bt_basis_transcoder_texture_format_is_astc(transcoderTextureFormat);

    public static uint BtBasisTranscoderFormatIsUncompressed(uint transcoderTextureFormat) =>
        bt_basis_transcoder_format_is_uncompressed(transcoderTextureFormat);

    public static uint BtBasisGetUncompressedBytesPerPixel(uint transcoderTextureFormat) =>
        bt_basis_get_uncompressed_bytes_per_pixel(transcoderTextureFormat);

    public static uint BtBasisGetBlockWidth(uint transcoderTextureFormat) =>
        bt_basis_get_block_width(transcoderTextureFormat);

    public static uint BtBasisGetBlockHeight(uint transcoderTextureFormat) =>
        bt_basis_get_block_height(transcoderTextureFormat);

    public static uint BtBasisGetTranscoderTextureFormatFromBasisTexFormat(uint basisTexFormat) =>
        bt_basis_get_transcoder_texture_format_from_basis_tex_format(basisTexFormat);

    public static uint BtBasisIsFormatSupported(uint transcoderTextureFormat, uint basisTextureFormat) =>
        bt_basis_is_format_supported(transcoderTextureFormat, basisTextureFormat);

    public static uint BtBasisComputeTranscodedImageSizeInBytes(
        uint transcoderTextureFormat,
        uint origWidth,
        uint origHeight) =>
        bt_basis_compute_transcoded_image_size_in_bytes(transcoderTextureFormat, origWidth, origHeight);

    public static ulong BtKtx2Open(ulong dataMemOfs, uint dataLen) => bt_ktx2_open(dataMemOfs, dataLen);

    public static void BtKtx2Close(ulong handle) => bt_ktx2_close(handle);

    public static uint BtKtx2GetWidth(ulong handle) => bt_ktx2_get_width(handle);

    public static uint BtKtx2GetHeight(ulong handle) => bt_ktx2_get_height(handle);

    public static uint BtKtx2GetLevels(ulong handle) => bt_ktx2_get_levels(handle);

    public static uint BtKtx2GetFaces(ulong handle) => bt_ktx2_get_faces(handle);

    public static uint BtKtx2GetLayers(ulong handle) => bt_ktx2_get_layers(handle);

    public static uint BtKtx2GetBasisTexFormat(ulong handle) => bt_ktx2_get_basis_tex_format(handle);

    public static uint BtKtx2IsEtc1s(ulong handle) => bt_ktx2_is_etc1s(handle);

    public static uint BtKtx2IsUastcLdr4x4(ulong handle) => bt_ktx2_is_uastc_ldr_4x4(handle);

    public static uint BtKtx2IsHdr(ulong handle) => bt_ktx2_is_hdr(handle);

    public static uint BtKtx2IsHdr4x4(ulong handle) => bt_ktx2_is_hdr_4x4(handle);

    public static uint BtKtx2IsHdr6x6(ulong handle) => bt_ktx2_is_hdr_6x6(handle);

    public static uint BtKtx2IsLdr(ulong handle) => bt_ktx2_is_ldr(handle);

    public static uint BtKtx2IsAstcLdr(ulong handle) => bt_ktx2_is_astc_ldr(handle);

    public static uint BtKtx2IsXuastcLdr(ulong handle) => bt_ktx2_is_xuastc_ldr(handle);

    public static uint BtKtx2GetBlockWidth(ulong handle) => bt_ktx2_get_block_width(handle);

    public static uint BtKtx2GetBlockHeight(ulong handle) => bt_ktx2_get_block_height(handle);

    public static uint BtKtx2HasAlpha(ulong handle) => bt_ktx2_has_alpha(handle);

    public static uint BtKtx2GetDfdColorModel(ulong handle) => bt_ktx2_get_dfd_color_model(handle);

    public static uint BtKtx2GetDfdColorPrimaries(ulong handle) => bt_ktx2_get_dfd_color_primaries(handle);

    public static uint BtKtx2GetDfdTransferFunc(ulong handle) => bt_ktx2_get_dfd_transfer_func(handle);

    public static uint BtKtx2IsSrgb(ulong handle) => bt_ktx2_is_srgb(handle);

    public static uint BtKtx2GetDfdFlags(ulong handle) => bt_ktx2_get_dfd_flags(handle);

    public static uint BtKtx2GetDfdTotalSamples(ulong handle) => bt_ktx2_get_dfd_total_samples(handle);

    public static uint BtKtx2GetDfdChannelId0(ulong handle) => bt_ktx2_get_dfd_channel_id0(handle);

    public static uint BtKtx2GetDfdChannelId1(ulong handle) => bt_ktx2_get_dfd_channel_id1(handle);

    public static uint BtKtx2IsVideo(ulong handle) => bt_ktx2_is_video(handle);

    public static float BtKtx2GetLdrHdrUpconversionNitMultiplier(ulong handle) =>
        bt_ktx2_get_ldr_hdr_upconversion_nit_multiplier(handle);

    public static uint BtKtx2GetLevelOrigWidth(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_orig_width(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelOrigHeight(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_orig_height(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelActualWidth(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_actual_width(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelActualHeight(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_actual_height(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelNumBlocksX(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_num_blocks_x(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelNumBlocksY(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_num_blocks_y(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelTotalBlocks(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_total_blocks(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelAlphaFlag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_alpha_flag(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2GetLevelIframeFlag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex) =>
        bt_ktx2_get_level_iframe_flag(handle, levelIndex, layerIndex, faceIndex);

    public static uint BtKtx2StartTranscoding(ulong handle) => bt_ktx2_start_transcoding(handle);

    public static ulong BtKtx2CreateTranscodeState() => bt_ktx2_create_transcode_state();

    public static void BtKtx2DestroyTranscodeState(ulong handle) => bt_ktx2_destroy_transcode_state(handle);

    public static uint BtKtx2TranscodeImageLevel(
        ulong ktx2Handle,
        uint levelIndex,
        uint layerIndex,
        uint faceIndex,
        ulong outputBlockMemOfs,
        uint outputBlocksBufSizeInBlocksOrPixels,
        uint transcoderTextureFormat,
        uint decodeFlags,
        uint outputRowPitchInBlocksOrPixels,
        uint outputRowsInPixels,
        int channel0,
        int channel1,
        ulong stateHandle) =>
        bt_ktx2_transcode_image_level(
            ktx2Handle,
            levelIndex,
            layerIndex,
            faceIndex,
            outputBlockMemOfs,
            outputBlocksBufSizeInBlocksOrPixels,
            transcoderTextureFormat,
            decodeFlags,
            outputRowPitchInBlocksOrPixels,
            outputRowsInPixels,
            channel0,
            channel1,
            stateHandle);

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_get_version();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_get_version();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bu_enable_debug_printf(uint flag);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bu_enable_debug_printf(uint flag);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bu_init();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bu_init();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bu_alloc(ulong size);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bu_alloc(ulong size);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bu_free(ulong ofs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bu_free(ulong ofs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bu_new_comp_params();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bu_new_comp_params();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_delete_comp_params(ulong paramsOfs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_delete_comp_params(ulong paramsOfs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bu_comp_params_get_comp_data_size(ulong paramsOfs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bu_comp_params_get_comp_data_size(ulong paramsOfs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bu_comp_params_get_comp_data_ofs(ulong paramsOfs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bu_comp_params_get_comp_data_ofs(ulong paramsOfs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_comp_params_clear(ulong paramsOfs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_comp_params_clear(ulong paramsOfs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_comp_params_set_image_rgba32(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_comp_params_set_image_rgba32(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_comp_params_set_image_float_rgba(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_comp_params_set_image_float_rgba(
        ulong paramsOfs,
        uint imageIndex,
        ulong imageDataOfs,
        uint width,
        uint height,
        uint pitchInBytes);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bu_compress_texture(
        ulong paramsOfs,
        uint desiredBasisTextureFormat,
        int qualityLevel,
        int effortLevel,
        ulong flagsAndQuality,
        float lowLevelUastcRdoOrDctQuality);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bu_compress_texture(
        ulong paramsOfs,
        uint desiredBasisTextureFormat,
        int qualityLevel,
        int effortLevel,
        ulong flagsAndQuality,
        float lowLevelUastcRdoOrDctQuality);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_get_version();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_get_version();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bt_enable_debug_printf(uint flag);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bt_enable_debug_printf(uint flag);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bt_init();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bt_init();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bt_alloc(ulong size);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bt_alloc(ulong size);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bt_free(ulong ofs);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bt_free(ulong ofs);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_is_xuastc_ldr(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_is_xuastc_ldr(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_is_astc_ldr(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_is_astc_ldr(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_get_block_width(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_get_block_width(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_get_block_height(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_get_block_height(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_is_hdr(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_is_hdr(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_tex_format_is_ldr(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_tex_format_is_ldr(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_get_bytes_per_block_or_pixel(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_get_bytes_per_block_or_pixel(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_transcoder_format_has_alpha(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_transcoder_format_has_alpha(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_transcoder_format_is_hdr(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_transcoder_format_is_hdr(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_transcoder_format_is_ldr(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_transcoder_format_is_ldr(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_transcoder_texture_format_is_astc(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_transcoder_texture_format_is_astc(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_transcoder_format_is_uncompressed(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_transcoder_format_is_uncompressed(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_get_uncompressed_bytes_per_pixel(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_get_uncompressed_bytes_per_pixel(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_get_block_width(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_get_block_width(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_get_block_height(uint transcoderTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_get_block_height(uint transcoderTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_get_transcoder_texture_format_from_basis_tex_format(uint basisTexFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_get_transcoder_texture_format_from_basis_tex_format(uint basisTexFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_is_format_supported(uint transcoderTextureFormat, uint basisTextureFormat);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_is_format_supported(uint transcoderTextureFormat, uint basisTextureFormat);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_basis_compute_transcoded_image_size_in_bytes(
        uint transcoderTextureFormat,
        uint origWidth,
        uint origHeight);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_basis_compute_transcoded_image_size_in_bytes(
        uint transcoderTextureFormat,
        uint origWidth,
        uint origHeight);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bt_ktx2_open(ulong dataMemOfs, uint dataLen);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bt_ktx2_open(ulong dataMemOfs, uint dataLen);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bt_ktx2_close(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bt_ktx2_close(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_width(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_width(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_height(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_height(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_levels(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_levels(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_faces(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_faces(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_layers(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_layers(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_basis_tex_format(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_basis_tex_format(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_etc1s(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_etc1s(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_uastc_ldr_4x4(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_uastc_ldr_4x4(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_hdr(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_hdr(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_hdr_4x4(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_hdr_4x4(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_hdr_6x6(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_hdr_6x6(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_ldr(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_ldr(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_astc_ldr(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_astc_ldr(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_xuastc_ldr(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_xuastc_ldr(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_block_width(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_block_width(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_block_height(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_block_height(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_has_alpha(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_has_alpha(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_color_model(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_color_model(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_color_primaries(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_color_primaries(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_transfer_func(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_transfer_func(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_srgb(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_srgb(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_flags(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_flags(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_total_samples(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_total_samples(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_channel_id0(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_channel_id0(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_dfd_channel_id1(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_dfd_channel_id1(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_is_video(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_is_video(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial float bt_ktx2_get_ldr_hdr_upconversion_nit_multiplier(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern float bt_ktx2_get_ldr_hdr_upconversion_nit_multiplier(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_orig_width(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_orig_width(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_orig_height(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_orig_height(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_actual_width(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_actual_width(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_actual_height(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_actual_height(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_num_blocks_x(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_num_blocks_x(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_num_blocks_y(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_num_blocks_y(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_total_blocks(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_total_blocks(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_alpha_flag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_alpha_flag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_get_level_iframe_flag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_get_level_iframe_flag(ulong handle, uint levelIndex, uint layerIndex, uint faceIndex);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_start_transcoding(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_start_transcoding(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial ulong bt_ktx2_create_transcode_state();
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong bt_ktx2_create_transcode_state();
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial void bt_ktx2_destroy_transcode_state(ulong handle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bt_ktx2_destroy_transcode_state(ulong handle);
#endif

#if NET8_0_OR_GREATER
    [LibraryImport(LibraryName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static partial uint bt_ktx2_transcode_image_level(
        ulong ktx2Handle,
        uint levelIndex,
        uint layerIndex,
        uint faceIndex,
        ulong outputBlockMemOfs,
        uint outputBlocksBufSizeInBlocksOrPixels,
        uint transcoderTextureFormat,
        uint decodeFlags,
        uint outputRowPitchInBlocksOrPixels,
        uint outputRowsInPixels,
        int channel0,
        int channel1,
        ulong stateHandle);
#else
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint bt_ktx2_transcode_image_level(
        ulong ktx2Handle,
        uint levelIndex,
        uint layerIndex,
        uint faceIndex,
        ulong outputBlockMemOfs,
        uint outputBlocksBufSizeInBlocksOrPixels,
        uint transcoderTextureFormat,
        uint decodeFlags,
        uint outputRowPitchInBlocksOrPixels,
        uint outputRowsInPixels,
        int channel0,
        int channel1,
        ulong stateHandle);
#endif
}
