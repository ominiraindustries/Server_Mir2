param(
    [Parameter(Mandatory=$true)][string]$Version,
    [string]$Remote = 'origin',
    [string]$Branch,
    [string]$RepoUrl,
    [string]$Message,
    [switch]$PushAllTags
)

function Exec($cmd) {
    Write-Host "> $cmd" -ForegroundColor Cyan
    & powershell -NoProfile -Command $cmd
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed: $cmd"
    }
}

# 1) Preconditions
try { git --version | Out-Null } catch { throw "Git no está instalado o no está en PATH." }

if (-not (Test-Path .git)) {
    Write-Host "Inicializando repositorio git..." -ForegroundColor Yellow
    Exec "git init"
    if ($RepoUrl) {
        Exec "git remote add $Remote $RepoUrl"
    } else {
        Write-Host "Aviso: no hay .git y no se proporcionó -RepoUrl. Puedes configurarlo luego con 'git remote add'." -ForegroundColor Yellow
    }
}

# 2) Determinar rama
$CurrentBranch = (git rev-parse --abbrev-ref HEAD 2>$null)
if ($Branch) {
    Write-Host "Usando rama: $Branch" -ForegroundColor Green
    Exec "git checkout -B $Branch"
} elseif ($CurrentBranch) {
    $Branch = $CurrentBranch
    Write-Host "Rama actual: $Branch" -ForegroundColor Green
} else {
    $Branch = 'main'
    Write-Host "Creando rama por defecto: $Branch" -ForegroundColor Yellow
    Exec "git checkout -b $Branch"
}

# 3) Verificar remoto
$RemoteUrl = ""
try { $RemoteUrl = (git remote get-url $Remote 2>$null) } catch { $RemoteUrl = "" }
if (-not $RemoteUrl) {
    if ($RepoUrl) {
        Exec "git remote add $Remote $RepoUrl"
        $RemoteUrl = $RepoUrl
    } else {
        Write-Host "Aviso: remoto '$Remote' no existe y no se proporcionó -RepoUrl. Los pushes fallarán." -ForegroundColor Yellow
    }
}

# 4) Stage & commit
Exec "git add -A"
if (-not $Message) { $Message = "release: v$Version" }
# Permitir commit vacío si no hay cambios
& git commit -m $Message
if ($LASTEXITCODE -ne 0) {
    Write-Host "Sin cambios que commitear o commit fallido. Continuando..." -ForegroundColor Yellow
}

# 5) Tag
$TagName = "v$Version"
$tagExists = (& git tag --list $TagName)
if (-not $tagExists) {
    Exec "git tag -a $TagName -m \"$TagName\""
} else {
    Write-Host "Tag $TagName ya existe localmente." -ForegroundColor Yellow
}

# 6) Push branch
if ($RemoteUrl) {
    # Establecer upstream si no existe
    & git rev-parse --abbrev-ref --symbolic-full-name "@{u}" 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Exec "git push -u $Remote $Branch"
    } else {
        Exec "git push $Remote $Branch"
    }

    # 7) Push tag(s)
    if ($PushAllTags) {
        Exec "git push --tags $Remote"
    } else {
        Exec "git push $Remote $TagName"
    }
} else {
    Write-Host "Saltando push: remoto no configurado." -ForegroundColor Yellow
}

Write-Host "Release listo: $TagName en rama $Branch" -ForegroundColor Green
