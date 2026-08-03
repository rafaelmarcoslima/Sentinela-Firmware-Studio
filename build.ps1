$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceFile = Join-Path $projectRoot 'Program.cs'
$outputFolder = Join-Path $projectRoot 'dist'
$outputFile = Join-Path $outputFolder 'SentinelaFirmwareStudio.exe'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "Compilador C# do Windows não encontrado: $compiler"
}

New-Item -ItemType Directory -Force -Path $outputFolder | Out-Null

& $compiler /nologo /target:winexe /optimize+ "/out:$outputFile" `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.Windows.Forms.dll `
    /reference:System.Xml.dll `
    $sourceFile

if ($LASTEXITCODE -ne 0) {
    throw "Falha ao compilar o Sentinela Firmware Studio."
}

Write-Host "Aplicativo criado em: $outputFile"
