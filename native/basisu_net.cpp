#include <stdint.h>

#include "basisu_wasm_api.h"
#include "basisu_wasm_transcoder_api.h"

#if defined(_WIN32)
#define BASISU_NET_EXPORT extern "C" __declspec(dllexport)
#else
#define BASISU_NET_EXPORT extern "C" __attribute__((visibility("default")))
#endif

BASISU_NET_EXPORT uint32_t basisu_net_get_encoder_version()
{
    return bu_get_version();
}

BASISU_NET_EXPORT uint32_t basisu_net_get_transcoder_version()
{
    return bt_get_version();
}

BASISU_NET_EXPORT void basisu_net_encoder_init()
{
    bu_init();
}

BASISU_NET_EXPORT uint64_t basisu_net_new_comp_params()
{
    return bu_new_comp_params();
}

BASISU_NET_EXPORT uint32_t basisu_net_delete_comp_params(uint64_t params)
{
    return bu_delete_comp_params(params);
}

BASISU_NET_EXPORT uint32_t basisu_net_comp_params_set_image_rgba32(
    uint64_t params,
    uint32_t image_index,
    uint64_t image_data,
    uint32_t width,
    uint32_t height,
    uint32_t pitch_in_bytes)
{
    return bu_comp_params_set_image_rgba32(params, image_index, image_data, width, height, pitch_in_bytes);
}

BASISU_NET_EXPORT uint32_t basisu_net_compress_texture(
    uint64_t params,
    uint32_t basis_texture_format,
    int quality_level,
    int effort_level,
    uint64_t flags,
    float rdo_or_dct_quality)
{
    return bu_compress_texture(params, basis_texture_format, quality_level, effort_level, flags, rdo_or_dct_quality);
}

BASISU_NET_EXPORT uint64_t basisu_net_comp_params_get_comp_data_size(uint64_t params)
{
    return bu_comp_params_get_comp_data_size(params);
}

BASISU_NET_EXPORT uint64_t basisu_net_comp_params_get_comp_data_pointer(uint64_t params)
{
    return bu_comp_params_get_comp_data_ofs(params);
}

BASISU_NET_EXPORT void basisu_net_transcoder_init()
{
    bt_init();
}

BASISU_NET_EXPORT uint32_t basisu_net_transcoder_format_is_uncompressed(uint32_t format)
{
    return bt_basis_transcoder_format_is_uncompressed(format);
}

BASISU_NET_EXPORT uint32_t basisu_net_transcoder_format_is_supported(uint32_t transcoder_format, uint32_t basis_texture_format)
{
    return bt_basis_is_format_supported(transcoder_format, basis_texture_format);
}

BASISU_NET_EXPORT uint32_t basisu_net_compute_transcoded_image_size_in_bytes(
    uint32_t transcoder_format,
    uint32_t width,
    uint32_t height)
{
    return bt_basis_compute_transcoded_image_size_in_bytes(transcoder_format, width, height);
}

BASISU_NET_EXPORT uint64_t basisu_net_ktx2_open(uint64_t data, uint32_t data_length)
{
    return bt_ktx2_open(data, data_length);
}

BASISU_NET_EXPORT void basisu_net_ktx2_close(uint64_t handle)
{
    bt_ktx2_close(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_width(uint64_t handle)
{
    return bt_ktx2_get_width(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_height(uint64_t handle)
{
    return bt_ktx2_get_height(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_levels(uint64_t handle)
{
    return bt_ktx2_get_levels(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_faces(uint64_t handle)
{
    return bt_ktx2_get_faces(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_layers(uint64_t handle)
{
    return bt_ktx2_get_layers(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_basis_texture_format(uint64_t handle)
{
    return bt_ktx2_get_basis_tex_format(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_has_alpha(uint64_t handle)
{
    return bt_ktx2_has_alpha(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_is_srgb(uint64_t handle)
{
    return bt_ktx2_is_srgb(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_level_orig_width(
    uint64_t handle,
    uint32_t level_index,
    uint32_t layer_index,
    uint32_t face_index)
{
    return bt_ktx2_get_level_orig_width(handle, level_index, layer_index, face_index);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_level_orig_height(
    uint64_t handle,
    uint32_t level_index,
    uint32_t layer_index,
    uint32_t face_index)
{
    return bt_ktx2_get_level_orig_height(handle, level_index, layer_index, face_index);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_get_level_total_blocks(
    uint64_t handle,
    uint32_t level_index,
    uint32_t layer_index,
    uint32_t face_index)
{
    return bt_ktx2_get_level_total_blocks(handle, level_index, layer_index, face_index);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_start_transcoding(uint64_t handle)
{
    return bt_ktx2_start_transcoding(handle);
}

BASISU_NET_EXPORT uint32_t basisu_net_ktx2_transcode_image_level(
    uint64_t handle,
    uint32_t level_index,
    uint32_t layer_index,
    uint32_t face_index,
    uint64_t output,
    uint32_t output_blocks_or_pixels,
    uint32_t transcoder_format,
    uint32_t decode_flags,
    uint32_t output_row_pitch_blocks_or_pixels,
    uint32_t output_rows_in_pixels,
    int channel0,
    int channel1)
{
    return bt_ktx2_transcode_image_level(
        handle,
        level_index,
        layer_index,
        face_index,
        output,
        output_blocks_or_pixels,
        transcoder_format,
        decode_flags,
        output_row_pitch_blocks_or_pixels,
        output_rows_in_pixels,
        channel0,
        channel1,
        0);
}
