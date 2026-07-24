[CmdletBinding()]
param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$expectedVersion = "1.2026.1.2"
$repositoryRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDirectory = $repositoryRoot
$dllPath = Join-Path $projectDirectory "bin\$Configuration\Dynamics365TemplateCompareTransfer.dll"
$nuspecPath = Join-Path $repositoryRoot "Dynamics365TemplateCompareTransfer.nuspec"
$artifactDirectory = Join-Path $repositoryRoot "artifacts"

if (-not (Test-Path -LiteralPath $dllPath -PathType Leaf)) {
    throw "Release DLL not found: $dllPath. Build the $Configuration configuration first."
}

$assemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($dllPath).Version.ToString()
if ($assemblyVersion -ne $expectedVersion) {
    throw "DLL version $assemblyVersion does not match expected package version $expectedVersion."
}

$localNuGet = Join-Path $repositoryRoot "nuget.exe"
if (Test-Path -LiteralPath $localNuGet -PathType Leaf) {
    $nuGetCommand = $localNuGet
}
else {
    $nuGetOnPath = Get-Command "nuget.exe" -ErrorAction SilentlyContinue
    if ($null -eq $nuGetOnPath) {
        throw "nuget.exe was not found. Place it in the repository root or add it to PATH."
    }

    $nuGetCommand = $nuGetOnPath.Source
}

if (-not (Test-Path -LiteralPath $artifactDirectory -PathType Container)) {
    New-Item -ItemType Directory -Path $artifactDirectory | Out-Null
}

& $nuGetCommand pack $nuspecPath `
    -BasePath $projectDirectory `
    -OutputDirectory $artifactDirectory `
    -NonInteractive

if ($LASTEXITCODE -ne 0) {
    throw "NuGet pack failed with exit code $LASTEXITCODE."
}

$packagePath = Join-Path $artifactDirectory "Dynamics365TemplateCompareTransfer.$expectedVersion.nupkg"
if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
    throw "Expected NuGet package was not created: $packagePath"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $requiredEntry = "lib/net48/Plugins/Dynamics365TemplateCompareTransfer.dll"
    $entry = $archive.Entries | Where-Object {
        $_.FullName.Replace("\", "/") -eq $requiredEntry
    }

    if ($null -eq $entry) {
        throw "Package validation failed. Missing $requiredEntry."
    }
}
finally {
    $archive.Dispose()
}

$hash = Get-FileHash -LiteralPath $packagePath -Algorithm SHA256

Write-Host ""
Write-Host "Release package validated successfully." -ForegroundColor Green
Write-Host "Assembly version: $assemblyVersion"
Write-Host "Package: $packagePath"
Write-Host "SHA-256: $($hash.Hash)"

