$ErrorActionPreference = 'Stop'
$studioRoot = Split-Path -Parent $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$testOutput = Join-Path $studioRoot '.tests-output'
New-Item -ItemType Directory -Path $testOutput -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $studioRoot 'dist/DeviceFirmwareStudio.exe') -Destination (Join-Path $testOutput 'DeviceFirmwareStudio.exe') -Force
$output = Join-Path $testOutput 'ProfilesTests.exe' 
& $compiler /nologo /target:exe "/out:$output" "/reference:$(Join-Path $studioRoot 'dist/DeviceFirmwareStudio.exe')" /reference:System.dll /reference:System.Core.dll /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Security.dll (Join-Path $PSScriptRoot 'ProfilesTests.cs') (Join-Path $PSScriptRoot 'TerminalTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar os testes.' }
$fakePlatformIo = Join-Path $testOutput 'FakePlatformIo.exe'
& $compiler /nologo /target:exe "/out:$fakePlatformIo" /reference:System.dll /reference:System.Web.Extensions.dll (Join-Path $PSScriptRoot 'FakePlatformIo.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar o simulador USB.' }
& $output (Join-Path (Split-Path -Parent $studioRoot) 'Coleta-de-Sinal-Sentinela') $fakePlatformIo
if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes do Studio.' }
