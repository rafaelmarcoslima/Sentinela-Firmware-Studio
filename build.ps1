param(
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceFile = Join-Path $projectRoot 'Program.cs'
$outputFolder = Join-Path $projectRoot 'dist'
$outputFile = Join-Path $outputFolder 'SentinelaFirmwareStudio.exe'
$iconFile = Join-Path $projectRoot 'assets\app.ico'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

$versionPattern = 'const string AppVersion = "(?<version>\d+\.\d+\.\d+)";'
$source = [System.IO.File]::ReadAllText($sourceFile)
$currentMatch = [System.Text.RegularExpressions.Regex]::Match($source, $versionPattern)

if (-not $currentMatch.Success) {
    throw 'Não foi possível localizar AppVersion no Program.cs.'
}

$currentVersion = $currentMatch.Groups['version'].Value
if ($Version) {
    $source = [System.Text.RegularExpressions.Regex]::Replace(
        $source,
        $versionPattern,
        ('const string AppVersion = "{0}";' -f $Version),
        1
    )
    $utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($sourceFile, $source, $utf8WithoutBom)
    $currentVersion = $Version
    Write-Host "Versão interna atualizada para: $currentVersion"
}
else {
    Write-Host "Compilando a versão atual: $currentVersion"
}

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "Compilador C# do Windows não encontrado: $compiler"
}

New-Item -ItemType Directory -Force -Path $outputFolder | Out-Null

if (-not (Test-Path -LiteralPath $iconFile)) {
    throw "Ícone do aplicativo não encontrado: $iconFile"
}

& $compiler /nologo /target:winexe /optimize+ "/out:$outputFile" "/win32icon:$iconFile" `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.IO.Compression.dll `
    /reference:System.IO.Compression.FileSystem.dll `
    /reference:System.Windows.Forms.dll `
    /reference:System.Xml.dll `
    $sourceFile

if ($LASTEXITCODE -ne 0) {
    throw "Falha ao compilar o Sentinela Firmware Studio."
}

Write-Host "Aplicativo criado em: $outputFile"
$sha256 = (Get-FileHash -LiteralPath $outputFile -Algorithm SHA256).Hash
Write-Host ''
Write-Host 'Dados para publicar no GitHub:'
Write-Host "  Tag da Release: v$currentVersion"
Write-Host "  Título: Sentinela Firmware Studio v$currentVersion"
Write-Host "  Arquivo: $outputFile"
Write-Host "  SHA-256: $sha256"
