#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
NATIVE_DIR="$ROOT_DIR/native"
RUNTIMES_DIR="$ROOT_DIR/runtimes"

ANDROID_PLATFORM="${ANDROID_PLATFORM:-android-24}"
IOS_DEPLOYMENT_TARGET="${IOS_DEPLOYMENT_TARGET:-13.0}"
MACOS_DEPLOYMENT_TARGET="${MACOS_DEPLOYMENT_TARGET:-11.0}"

if command -v sysctl >/dev/null 2>&1; then
  JOBS="${JOBS:-$(sysctl -n hw.logicalcpu)}"
elif command -v nproc >/dev/null 2>&1; then
  JOBS="${JOBS:-$(nproc)}"
else
  JOBS="${JOBS:-4}"
fi

COMMON_CMAKE_ARGS=(-DCMAKE_BUILD_TYPE=Release)
if [[ -n "${BASISU_SOURCE_DIR:-}" ]]; then
  COMMON_CMAKE_ARGS+=("-DFETCHCONTENT_SOURCE_DIR_BASIS_UNIVERSAL=$BASISU_SOURCE_DIR")
elif [[ -d "$NATIVE_DIR/build/_deps/basis_universal-src" ]]; then
  COMMON_CMAKE_ARGS+=("-DFETCHCONTENT_SOURCE_DIR_BASIS_UNIVERSAL=$NATIVE_DIR/build/_deps/basis_universal-src")
fi

usage() {
  cat <<'EOF'
Usage: scripts/build-native.sh <target> [target...]

Targets:
  macos     Build runtimes/osx/native/libbasisu.dylib as arm64+x64 universal.
  ios       Build signed iOS and iOS simulator frameworks.
  android   Build android-arm, android-arm64, android-x86, android-x64.
  linux     Build linux-x64 and linux-arm64 with manylinux2014 Docker images.
  all       Build macos, ios, android, and linux.

Environment:
  BASISU_SOURCE_DIR          Use an existing basis_universal checkout.
  ANDROID_NDK_HOME           Android NDK root. ANDROID_NDK_ROOT also works.
  ANDROID_HOME               Android SDK root used to discover ndk/*.
  ANDROID_PLATFORM           Default: android-24.
  IOS_DEPLOYMENT_TARGET      Default: 13.0.
  MACOS_DEPLOYMENT_TARGET    Default: 11.0.
  JOBS                       Parallel build jobs.
EOF
}

die() {
  echo "error: $*" >&2
  exit 1
}

require_command() {
  command -v "$1" >/dev/null 2>&1 || die "missing required command: $1"
}

require_darwin() {
  [[ "$(uname -s)" == "Darwin" ]] || die "$1 builds require macOS"
}

host_tag() {
  case "$(uname -s)" in
    Darwin) echo darwin-x86_64 ;;
    Linux) echo linux-x86_64 ;;
    *) die "unsupported Android build host: $(uname -s)" ;;
  esac
}

build_cmake() {
  local build_dir="$1"
  shift

  cmake -S "$NATIVE_DIR" -B "$ROOT_DIR/$build_dir" "${COMMON_CMAKE_ARGS[@]}" "$@"
  cmake --build "$ROOT_DIR/$build_dir" --config Release --target basisu_native --parallel "$JOBS"
}

strip_mach_o() {
  local file="$1"
  strip -x "$file"
}

find_android_ndk() {
  local candidates=()

  [[ -n "${ANDROID_NDK_HOME:-}" ]] && candidates+=("$ANDROID_NDK_HOME")
  [[ -n "${ANDROID_NDK_ROOT:-}" ]] && candidates+=("$ANDROID_NDK_ROOT")

  local sdk_root="${ANDROID_HOME:-${ANDROID_SDK_ROOT:-}}"
  if [[ -z "$sdk_root" && -d "$HOME/Library/Android/sdk" ]]; then
    sdk_root="$HOME/Library/Android/sdk"
  fi

  if [[ -n "$sdk_root" && -d "$sdk_root/ndk" ]]; then
    local latest_ndk
    latest_ndk="$(find "$sdk_root/ndk" -mindepth 1 -maxdepth 1 -type d | sort | tail -n 1 || true)"
    [[ -n "$latest_ndk" ]] && candidates+=("$latest_ndk")
  fi

  for ndk in "${candidates[@]}"; do
    if [[ -f "$ndk/build/cmake/android.toolchain.cmake" ]]; then
      echo "$ndk"
      return
    fi
  done

  die "Android NDK not found. Set ANDROID_NDK_HOME or ANDROID_HOME."
}

build_android_one() {
  local rid="$1"
  local abi="$2"
  shift 2

  local ndk="$ANDROID_NDK"
  local build_dir="native/build-${rid}"
  local out_dir="$RUNTIMES_DIR/$rid/native"
  local strip_tool="$ndk/toolchains/llvm/prebuilt/$(host_tag)/bin/llvm-strip"

  build_cmake "$build_dir" \
    "-DCMAKE_TOOLCHAIN_FILE=$ndk/build/cmake/android.toolchain.cmake" \
    "-DANDROID_ABI=$abi" \
    "-DANDROID_PLATFORM=$ANDROID_PLATFORM" \
    "$@"

  mkdir -p "$out_dir"
  cp "$ROOT_DIR/$build_dir/out/libbasisu.so" "$out_dir/libbasisu.so"
  if [[ -x "$strip_tool" ]]; then
    "$strip_tool" --strip-unneeded "$out_dir/libbasisu.so"
  fi
  file "$out_dir/libbasisu.so"
}

build_android() {
  require_command cmake
  ANDROID_NDK="$(find_android_ndk)"

  build_android_one android-arm64 arm64-v8a
  build_android_one android-arm armeabi-v7a -DANDROID_ARM_NEON=TRUE
  build_android_one android-x64 x86_64
  build_android_one android-x86 x86
}

build_macos() {
  require_darwin macOS
  require_command cmake

  build_cmake native/build-macos-universal \
    "-DCMAKE_OSX_ARCHITECTURES=arm64;x86_64" \
    "-DCMAKE_OSX_DEPLOYMENT_TARGET=$MACOS_DEPLOYMENT_TARGET"

  mkdir -p "$RUNTIMES_DIR/osx/native"
  cp "$ROOT_DIR/native/build-macos-universal/out/libbasisu.dylib" "$RUNTIMES_DIR/osx/native/libbasisu.dylib"
  strip_mach_o "$RUNTIMES_DIR/osx/native/libbasisu.dylib"
  file "$RUNTIMES_DIR/osx/native/libbasisu.dylib"
}

write_ios_info_plist() {
  local plist="$1"
  local platform="$2"

  cat > "$plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "https://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleDevelopmentRegion</key>
  <string>en</string>
  <key>CFBundleExecutable</key>
  <string>basisu</string>
  <key>CFBundleIdentifier</key>
  <string>net.basisuniversal.basisu</string>
  <key>CFBundleInfoDictionaryVersion</key>
  <string>6.0</string>
  <key>CFBundleName</key>
  <string>basisu</string>
  <key>CFBundlePackageType</key>
  <string>FMWK</string>
  <key>CFBundleShortVersionString</key>
  <string>1.0.0</string>
  <key>CFBundleSupportedPlatforms</key>
  <array>
    <string>${platform}</string>
  </array>
  <key>CFBundleVersion</key>
  <string>1</string>
  <key>MinimumOSVersion</key>
  <string>${IOS_DEPLOYMENT_TARGET}</string>
</dict>
</plist>
EOF
}

copy_ios_framework() {
  local built_library="$1"
  local framework_dir="$2"
  local platform="$3"

  rm -rf "$framework_dir"
  mkdir -p "$framework_dir"
  cp "$built_library" "$framework_dir/basisu"
  install_name_tool -id @rpath/basisu.framework/basisu "$framework_dir/basisu"
  strip_mach_o "$framework_dir/basisu"
  write_ios_info_plist "$framework_dir/Info.plist" "$platform"
  codesign --force --sign - "$framework_dir"
  codesign --verify --verbose=2 "$framework_dir"
  file "$framework_dir/basisu"
}

build_ios() {
  require_darwin iOS
  require_command cmake
  require_command install_name_tool
  require_command codesign

  build_cmake native/build-ios-arm64 \
    -DCMAKE_SYSTEM_NAME=iOS \
    -DCMAKE_OSX_SYSROOT=iphoneos \
    -DCMAKE_OSX_ARCHITECTURES=arm64 \
    "-DCMAKE_OSX_DEPLOYMENT_TARGET=$IOS_DEPLOYMENT_TARGET"

  build_cmake native/build-iossim-universal \
    -DCMAKE_SYSTEM_NAME=iOS \
    -DCMAKE_OSX_SYSROOT=iphonesimulator \
    "-DCMAKE_OSX_ARCHITECTURES=arm64;x86_64" \
    "-DCMAKE_OSX_DEPLOYMENT_TARGET=$IOS_DEPLOYMENT_TARGET"

  copy_ios_framework \
    "$ROOT_DIR/native/build-ios-arm64/out/libbasisu.dylib" \
    "$RUNTIMES_DIR/ios/native/basisu.framework" \
    iPhoneOS

  copy_ios_framework \
    "$ROOT_DIR/native/build-iossim-universal/out/libbasisu.dylib" \
    "$RUNTIMES_DIR/iossimulator/native/basisu.framework" \
    iPhoneSimulator
}

build_linux_one() {
  local rid="$1"
  local platform="$2"
  local image="$3"
  local build_dir="native/build-${rid}-manylinux"

  docker run --rm \
    --platform "$platform" \
    -e RID="$rid" \
    -e BUILD_DIR="$build_dir" \
    -v "$ROOT_DIR:/repo" \
    -w /repo \
    "$image" \
    bash -lc '
set -euo pipefail
cmake_args=(-DCMAKE_BUILD_TYPE=Release)
if [[ -d /repo/native/build/_deps/basis_universal-src ]]; then
  cmake_args+=(-DFETCHCONTENT_SOURCE_DIR_BASIS_UNIVERSAL=/repo/native/build/_deps/basis_universal-src)
fi
cmake -S native -B "$BUILD_DIR" "${cmake_args[@]}"
cmake --build "$BUILD_DIR" --config Release --target basisu_native --parallel "$(nproc)"
mkdir -p "runtimes/$RID/native"
cp "$BUILD_DIR/out/libbasisu.so" "runtimes/$RID/native/libbasisu.so"
strip --strip-unneeded "runtimes/$RID/native/libbasisu.so"
file "runtimes/$RID/native/libbasisu.so"
'
}

build_linux() {
  require_command docker
  build_linux_one linux-x64 linux/amd64 quay.io/pypa/manylinux2014_x86_64
  build_linux_one linux-arm64 linux/arm64 quay.io/pypa/manylinux2014_aarch64
}

if [[ "$#" -eq 0 ]]; then
  usage
  exit 2
fi

if [[ "$#" -eq 1 ]]; then
  case "$1" in
    -h|--help|help)
      usage
      exit 0
      ;;
  esac
fi

targets=()
for target in "$@"; do
  if [[ "$target" == "all" ]]; then
    targets+=(macos ios android linux)
  else
    targets+=("$target")
  fi
done

for target in "${targets[@]}"; do
  case "$target" in
    macos) build_macos ;;
    ios) build_ios ;;
    android) build_android ;;
    linux) build_linux ;;
    -h|--help|help) usage ;;
    *) die "unknown target: $target" ;;
  esac
done
