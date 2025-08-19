param(
  [ValidateSet('Debug','Release')]
  [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $root "Build/Server/$Configuration"
$logsDir = Join-Path $root 'logs'

if (!(Test-Path $buildDir)) {
  throw "Build directory not found: $buildDir. Run .\\build-all.ps1 first."
}

$exe = Join-Path $buildDir 'Server.exe'
$dll = Join-Path $buildDir 'Server.dll'

if (!(Test-Path $exe) -and !(Test-Path $dll)) {
  throw "Neither Server.exe nor Server.dll found in $buildDir. Build may have failed."
}

if (!(Test-Path $logsDir)) { New-Item -ItemType Directory -Path $logsDir | Out-Null }
$ts = Get-Date -Format 'yyyyMMdd_HHmmss'
$logFile = Join-Path $logsDir ("run_" + $ts + ".log")

"[${ts}] Launching server from $buildDir" | Out-File -FilePath $logFile -Encoding UTF8

if (Test-Path $exe) {
  Write-Host "Starting Server.exe ..." -ForegroundColor Cyan
  Start-Process -FilePath $exe
  "[${ts}] Started Server.exe" | Out-File -FilePath $logFile -Append -Encoding UTF8
}
elseif (Test-Path $dll) {
  Write-Host "Starting dotnet Server.dll ..." -ForegroundColor Cyan
  Start-Process -FilePath "dotnet" -ArgumentList @($dll)
  "[${ts}] Started dotnet Server.dll" | Out-File -FilePath $logFile -Append -Encoding UTF8
}

Write-Host "Log: $logFile" -ForegroundColor DarkGray
