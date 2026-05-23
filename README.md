# BasisUniversal.NET

`BasisUniversal.NET` is a .NET wrapper around Binomial's Basis Universal GPU
texture codec.

The project is split into managed packages and one native asset package:

- `BasisUniversal.NET` provides the high-level .NET API for common encode,
  inspect, and transcode workflows.
- `BasisUniversal.NET.LowLevel` provides lower-level bindings whose public
  method names follow the upstream `bu_*` and `bt_*` C API shape in C#
  PascalCase, such as `BuGetVersion` and `BtKtx2Open`. It binds directly to
  the upstream C API symbols exported by the native `basisu` library.
- `BasisUniversal.Native` is the aggregate native runtime package consumed by
  the low-level bindings. It contains all Android, iOS, Linux, and macOS native
  assets.

These packages are currently an early preview. Native runtime binaries are not
yet fully distributed for every target platform, so users may need to build the
native shim themselves and place the output in the matching NuGet runtime asset
folder.

The package is intended to be used directly by tools, asset pipelines, and
adapter packages such as `TextureCompressor.BasisUniversal`.

## Current Scope

- Encode RGBA32 pixels to `.ktx2` or `.basis` payloads.
- Inspect Basis-compressed KTX2 metadata.
- Transcode KTX2 image levels to Basis Universal's supported target formats,
  including RGBA32, ETC, BC1-5, BC7, PVRTC, ATC, FXT1, and ASTC targets where
  the source Basis texture format supports them.
- Native assets are currently packaged for Android, iOS, Linux, and macOS.
  Other target platforms can be added with the same package layout.

## High-Level Example

```csharp
using System.IO;
using BasisUniversal;

byte[] rgba32 = LoadRgba32Pixels();

byte[] ktx2 = BasisUniversalCodec.EncodeKtx2(
    rgba32,
    width: 1024,
    height: 1024,
    new BasisEncoderOptions
    {
        Format = BasisTextureFormat.UastcLdr4x4,
        QualityLevel = 50,
        EffortLevel = 2
    });

using var texture = BasisKtx2Texture.Open(ktx2);
BasisKtx2Info info = texture.Info;
BasisKtx2LevelInfo level = texture.GetLevelInfo();

TranscodedImage bc7 = texture.TranscodeImageLevel(TranscoderTextureFormat.Bc7Rgba);

byte[] rgba = new byte[BasisUniversalCodec.GetTranscodedImageSizeInBytes(
    TranscoderTextureFormat.Rgba32,
    level.OriginalWidth,
    level.OriginalHeight)];
int bytesWritten = texture.TranscodeImageLevel(rgba, TranscoderTextureFormat.Rgba32);
```

The high-level package also exposes format metadata helpers:

```csharp
bool supported = BasisUniversalCodec.IsTranscoderFormatSupported(
    TranscoderTextureFormat.Bc7Rgba,
    info.BasisTextureFormat);

int blockBytes = BasisUniversalCodec.GetBytesPerBlockOrPixel(TranscoderTextureFormat.Bc7Rgba);
bool hdr = BasisUniversalCodec.IsBasisTextureFormatHdr(info.BasisTextureFormat);
```

When the caller owns the output buffer or stream, the high-level API can write
there directly:

```csharp
int estimatedKtx2Bytes = BasisUniversalCodec.EstimateMaxEncodedKtx2SizeInBytes(
    width,
    height);
var encodedBuffer = new byte[estimatedKtx2Bytes];
int encodedBytes = BasisUniversalCodec.EncodeKtx2(
    rgba32,
    width,
    height,
    encodedBuffer);

bool ok = BasisUniversalCodec.TryEncodeKtx2(
    rgba32,
    width,
    height,
    encodedBuffer,
    out encodedBytes,
    out var requiredKtx2Bytes);

using var encodedStream = new MemoryStream();
long streamBytes = BasisUniversalCodec.EncodeKtx2(
    rgba32,
    width,
    height,
    encodedStream);

using var decodedStream = new MemoryStream();
int decodedBytes = texture.TranscodeImageLevel(
    decodedStream,
    TranscoderTextureFormat.Rgba32);
```

## Low-Level Example

```csharp
using BasisUniversal.LowLevel;

Basisu.EnsureInitialized();

uint encoderVersion = Basisu.BuGetVersion();
uint transcoderVersion = Basisu.BtGetVersion();
uint supported = Basisu.BtBasisIsFormatSupported(
    (uint)TranscoderTextureFormat.Bc7Rgba,
    (uint)BasisTextureFormat.UastcLdr4x4);
```

## Native Build

The native build uses CMake and pins `BinomialLLC/basis_universal` to
`v2_1_0r` (`e4f439fc9545b6a9e1fd26fc7ffd0c682c4b96d4`).

```bash
cmake -S native -B native/build -DCMAKE_BUILD_TYPE=Release
cmake --build native/build --config Release --target basisu_native
```

For repeatable builds, use the native build script:

```bash
scripts/build-native.sh macos ios android linux
```

On Windows, use the PowerShell/MSVC build script:

```powershell
scripts/build-native-windows.ps1 -Target all
```

Desktop and Android native binaries use the standard NuGet RID layout under
`runtimes/{rid}/native/`, such as `runtimes/osx/native/`,
`runtimes/win-x64/native/`, `runtimes/linux-arm64/native/`, or
`runtimes/android-arm64/native/`. iOS follows the native framework asset
layout and packages signed frameworks under `runtimes/ios/native/` and
`runtimes/iossimulator/native/`. Browser WebAssembly will need its own package
shape because it is linked by the WebAssembly build toolchain rather than loaded
as a normal native library.

The Linux target uses Docker manylinux2014 images to produce `linux-x64` and
`linux-arm64` binaries. The Android target uses the local Android NDK discovered
from `ANDROID_NDK_HOME`, `ANDROID_NDK_ROOT`, or `ANDROID_HOME`. The Windows
target uses CMake with the Visual Studio 2022 generator to produce `win-x64`,
`win-x86`, and `win-arm64` DLLs.

Then run:

```bash
dotnet test BasisUniversal.NET.slnx
dotnet pack src/BasisUniversal.Native/BasisUniversal.Native.csproj -c Release
dotnet pack src/BasisUniversal.NET.LowLevel/BasisUniversal.NET.LowLevel.csproj -c Release
dotnet pack src/BasisUniversal.NET/BasisUniversal.NET.csproj -c Release
```

## Licensing

This wrapper package uses the Apache-2.0 license. The native implementation is
built from [BinomialLLC/basis_universal](https://github.com/BinomialLLC/basis_universal),
which is also Apache-2.0. See `THIRD-PARTY-NOTICES.md` for upstream notice
details.
