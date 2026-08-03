# Sentinela Firmware Studio

Aplicativo Windows para cadastrar projetos PlatformIO, compilar firmware ESP32, localizar artefatos e calcular SHA-256.

## Recursos

- Salva vários projetos em `%LOCALAPPDATA%\SentinelaFirmwareStudio\projects.xml`.
- Remove apenas o caminho salvo, sem apagar a pasta do projeto.
- Detecta ambientes declarados no `platformio.ini`.
- Executa compilação normal ou `clean` seguido de compilação.
- Mostra o log do PlatformIO em tempo real.
- Localiza arquivos `.bin`, `.elf`, `.map` e `.hex` em `.pio\build`.
- Copia o artefato para a área de transferência ou exporta `.bin`, `.elf`, `.map` e `.hex` por um seletor de formato.
- Calcula e copia o SHA-256 do artefato selecionado.
- Exporta scripts `.bat` e `.ps1` para repetir a compilação fora do aplicativo.

## Compilar o aplicativo

No PowerShell, execute:

```powershell
.\build.ps1
```

O executável será criado em `dist\SentinelaFirmwareStudio.exe`.

O aplicativo utiliza o compilador C# do próprio Windows e não requer o SDK do .NET instalado.
