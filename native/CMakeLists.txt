cmake_minimum_required(VERSION 3.24)

project(BasisUniversalNative LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)

include(FetchContent)

set(BASISU_EXAMPLES OFF CACHE BOOL "" FORCE)
set(BASISU_BUILD_PYTHON OFF CACHE BOOL "" FORCE)
set(BASISU_OPENCL OFF CACHE BOOL "" FORCE)
set(BASISU_ZSTD ON CACHE BOOL "" FORCE)
set(BASISU_SSE OFF CACHE BOOL "" FORCE)

FetchContent_Declare(
    basis_universal
    GIT_REPOSITORY https://github.com/BinomialLLC/basis_universal.git
    GIT_TAG v2_1_0r
    GIT_SHALLOW TRUE
)

FetchContent_MakeAvailable(basis_universal)

add_library(basisu_native SHARED
    ${basis_universal_SOURCE_DIR}/encoder/basisu_wasm_api.cpp
    ${basis_universal_SOURCE_DIR}/encoder/basisu_wasm_transcoder_api.cpp
)

target_include_directories(basisu_native PRIVATE
    ${basis_universal_SOURCE_DIR}
    ${basis_universal_SOURCE_DIR}/encoder
    ${basis_universal_SOURCE_DIR}/transcoder
)

target_compile_definitions(basisu_native PRIVATE BASISD_SUPPORT_KTX2_ZSTD=1)

if (MSVC)
    target_compile_options(basisu_native PRIVATE /source-charset:.1252 /execution-charset:utf-8)
endif()

if (TARGET basisu_encoder)
    target_compile_definitions(basisu_encoder PRIVATE BASISD_SUPPORT_KTX2_ZSTD=1)
    if (MSVC)
        target_compile_options(basisu_encoder PRIVATE /source-charset:.1252 /execution-charset:utf-8)
    endif()
    target_link_libraries(basisu_native PRIVATE basisu_encoder)
endif()

if (NOT MSVC)
    find_package(Threads REQUIRED)
    target_link_libraries(basisu_native PRIVATE m Threads::Threads)
endif()

set_target_properties(basisu_native PROPERTIES
    OUTPUT_NAME basisu
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/out"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/out"
    ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/out"
)
