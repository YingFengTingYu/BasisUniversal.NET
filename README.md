# BasisUniversal.NET

`BasisUniversal.NET` is a thin .NET wrapper around Binomial's Basis Universal
GPU texture codec. It exposes the upstream encoder/transcoder C API without
taking a dependency on any higher-level texture library.

The package is intended to be used directly by tools, asset pipelines, and
adapter packages such as `TextureCompressor.BasisUniversal`.

## Current Scope

- Encode RGBA32 pixels to `.ktx2` or `.basis` payloads.
- Inspect Basis-compressed KTX2 metadata.
- Transcode KTX2 image levels to Basis Universal's supported target formats,
  including RGBA32, ETC, BC1-5, BC7, PVRTC, ATC, FXT1, and ASTC targets where
  the source Basis texture format supports them.
- Native library distribution is opt-in by runtime ID. The first local build
  produces `osx-arm64`; CI is set up to produce common Windows, Linux, and macOS
  packages.

## Example

```csharp
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
TranscodedImage bc7 = texture.TranscodeImageLevel(TranscoderTextureFormat.Bc7Rgba);
```

## Native Build

The native shim uses CMake and pins `BinomialLLC/basis_universal` to
`v2_1_0r` (`e4f439fc9545b6a9e1fd26fc7ffd0c682c4b96d4`).

```bash
cmake -S native -B native/build -DCMAKE_BUILD_TYPE=Release
cmake --build native/build --config Release --target basisu_net
```

Copy the produced native library into the matching NuGet runtime asset folder,
for example:

```bash
cp native/build/out/libbasisu_net.dylib runtimes/osx-arm64/native/
```

Then run:

```bash
dotnet test BasisUniversal.NET.slnx
dotnet pack src/BasisUniversal.NET/BasisUniversal.NET.csproj -c Release
```

## Licensing

This wrapper package uses the Apache-2.0 license. The native implementation is
built from [BinomialLLC/basis_universal](https://github.com/BinomialLLC/basis_universal),
which is also Apache-2.0. See `THIRD-PARTY-NOTICES.md` for upstream notice
details.
