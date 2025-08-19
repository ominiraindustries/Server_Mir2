param(
  [ValidateSet('Debug','Release')]
  [string]$Configuration = 'Release',
  [switch]$Clean,
  [string]$Version
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
if ($Version) { Write-Host "== Crystal Mir2 build (config: $Configuration, version: $Version) ==" -ForegroundColor Cyan }
else { Write-Host "== Crystal Mir2 build (config: $Configuration) ==" -ForegroundColor Cyan }

function Build-Project($projPath) {
  Write-Host "-- Restoring: $projPath" -ForegroundColor Yellow
  if ($Version) { dotnet restore $projPath -p:Version=$Version | Out-Host }
  else { dotnet restore $projPath | Out-Host }
  if ($Clean) {
    Write-Host "-- Cleaning:  $projPath" -ForegroundColor Yellow
    dotnet clean $projPath -c $Configuration | Out-Host
  }
  Write-Host "-- Building:  $projPath" -ForegroundColor Yellow
  if ($Version) { dotnet build $projPath -c $Configuration --nologo -p:Version=$Version | Out-Host }
  else { dotnet build $projPath -c $Configuration --nologo | Out-Host }
}

# Paths
$shared = Join-Path $root 'Shared/Shared.csproj'
$serverLib = Join-Path $root 'Server/Server.Library.csproj'
$mirForms = Join-Path $root 'Server.MirForms/Server.csproj'

# Build order
Build-Project $shared
Build-Project $serverLib
Build-Project $mirForms

# Artifact summary
Write-Host "`n== Artifacts ==" -ForegroundColor Green
$artifacts = @()
$artifacts += @{ Name = 'Shared.dll'; Path = Join-Path $root "Shared/bin/$Configuration/Shared.dll" }
$artifacts += @{ Name = 'Server.Library.dll'; Path = Join-Path $root "Server/bin/$Configuration/Server.Library.dll" }
$artifacts += @{ Name = 'Server (GUI)'; Path = Join-Path $root "Build/Server/$Configuration/Server.dll" }
$artifacts += @{ Name = 'Server.exe (if generated)'; Path = Join-Path $root "Build/Server/$Configuration/Server.exe" }

foreach ($a in $artifacts) {
  if (Test-Path $a.Path) {
    Write-Host ("✔ {0}: {1}" -f $a.Name, $a.Path) -ForegroundColor Green
  } else {
    Write-Host ("✖ {0}: NOT FOUND -> {1}" -f $a.Name, $a.Path) -ForegroundColor DarkGray
  }
}

Write-Host "`nDone." -ForegroundColor Cyan
