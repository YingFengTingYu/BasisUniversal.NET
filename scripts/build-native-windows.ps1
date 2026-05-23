param(
    [ValidateSet("x64", "x86", "arm64", "all")]
    [string[]]$Target = @("all"),

    [string]$Configuration = "Release",

    [string]$Generator = "Visual Studio 18 2026",

    [int]$Jobs = [Environment]::ProcessorCount
)

$ErrorActionPreference = "Stop"

$RootDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$NativeDir = Join-Path $RootDir "native"
$RuntimesDir = Join-Path $RootDir "runtimes"

$ArchitectureMap = @{
    x64 = @{
        Rid = "win-x64"
        CMakeArchitecture = "x64"
    }
    x86 = @{
        Rid = "win-x86"
        CMakeArchitecture = "Win32"
    }
    arm64 = @{
        Rid = "win-arm64"
        CMakeArchitecture = "ARM64"
    }
}

function Test-Command {
    param([string]$Name)
    return $null -ne (Get-Command $Name -ErrorAction SilentlyContinue)
}

function Invoke-CheckedCommand {
    param(
        [string]$FilePath,
        [string[]]$Arguments
    )

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FilePath failed with exit code $LASTEXITCODE."
    }
}

function Get-NativeOutput {
    param(
        [string]$BuildDir,
        [string]$Configuration
    )

    $Candidates = @(
        (Join-Path $BuildDir "out\$Configuration\basisu.dll"),
        (Join-Path $BuildDir "out\basisu.dll")
    )

    foreach ($Candidate in $Candidates) {
        if (Test-Path $Candidate) {
            return $Candidate
        }
    }

    throw "basisu.dll was not found under '$BuildDir\out'."
}

if (-not (Test-Command "cmake")) {
    throw "missing required command: cmake"
}

$ExpandedTargets = New-Object System.Collections.Generic.List[string]
foreach ($Item in $Target) {
    if ($Item -eq "all") {
        $ExpandedTargets.Add("x64")
        $ExpandedTargets.Add("x86")
        $ExpandedTargets.Add("arm64")
    }
    else {
        $ExpandedTargets.Add($Item)
    }
}

$CommonCMakeArgs = @()
if ($env:BASISU_SOURCE_DIR) {
    $CommonCMakeArgs += "-DFETCHCONTENT_SOURCE_DIR_BASIS_UNIVERSAL=$env:BASISU_SOURCE_DIR"
}
else {
    $ExistingSource = Join-Path $NativeDir "build\_deps\basis_universal-src"
    if (Test-Path $ExistingSource) {
        $CommonCMakeArgs += "-DFETCHCONTENT_SOURCE_DIR_BASIS_UNIVERSAL=$ExistingSource"
    }
}

foreach ($Architecture in ($ExpandedTargets | Select-Object -Unique)) {
    $Settings = $ArchitectureMap[$Architecture]
    $Rid = $Settings.Rid
    $CMakeArchitecture = $Settings.CMakeArchitecture
    $BuildDir = Join-Path $RootDir "native\build-$Rid"
    $OutputDir = Join-Path $RuntimesDir "$Rid\native"

    $ConfigureArgs = @(
        "--fresh",
        "-S", $NativeDir,
        "-B", $BuildDir,
        "-G", $Generator,
        "-A", $CMakeArchitecture
    ) + $CommonCMakeArgs

    Invoke-CheckedCommand "cmake" $ConfigureArgs

    $BuildArgs = @(
        "--build", $BuildDir,
        "--config", $Configuration,
        "--target", "basisu_native",
        "--parallel", $Jobs
    )

    Invoke-CheckedCommand "cmake" $BuildArgs

    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

    $Dll = Get-NativeOutput -BuildDir $BuildDir -Configuration $Configuration
    Copy-Item $Dll (Join-Path $OutputDir "basisu.dll") -Force

    $Pdb = [System.IO.Path]::ChangeExtension($Dll, ".pdb")
    if (Test-Path $Pdb) {
        Copy-Item $Pdb (Join-Path $OutputDir "basisu.pdb") -Force
    }

    Get-Item (Join-Path $OutputDir "basisu.dll")
}
