# Sentinela Firmware Studio

Aplicativo Windows para cadastrar projetos PlatformIO, compilar firmware ESP32, localizar artefatos e calcular SHA-256.

## Recursos

- Salva vários projetos em `%LOCALAPPDATA%\SentinelaFirmwareStudio\projects.xml`.
- Remove apenas o caminho salvo, sem apagar a pasta do projeto.
- Detecta ambientes declarados no `platformio.ini`.
- Executa compilação normal ou `clean` seguido de compilação.
- Mostra o log do PlatformIO em tempo real.
- Limpa log, status e artefato selecionado ao trocar de projeto.
- Bloqueia a interface durante a compilação e mostra uma janela de progresso.
- Oferece temas Azul, Turquesa, Verde, Roxo, Laranja, Vermelho e Grafite.
- Usa ícones nos principais botões para facilitar a identificação das ações.
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

## Perfis de empresa e conexão (1.2.0)

- Abas Empresas, Servidores / URLs e Redes Wi-Fi. Cada empresa tem ID único, nome, endereço e situação local ativa/inativa. Isso não altera seu cadastro no sistema.
- Cada empresa pode ter vários servidores e redes. Trocar empresa limpa as seleções incompatíveis. Chaves pertencem ao servidor; não são compartilhadas automaticamente entre URLs.
- Credenciais no cadastro são protegidas com DPAPI para o usuário atual do Windows. Copiar o XML para outro usuário não transfere o acesso às senhas.
- Testar conexão consulta o diagnóstico somente leitura: confirma API, chave e banco preparado, com data do resultado. Não registra dispositivos e não comprova conectividade a partir da rede do ESP.
- Rede inicial do Studio aplica a rede uma vez quando seus dados mudam; os próximos reinícios preservam as alterações recebidas do sistema. Bootstrap exige uma rede inicial.
- OTA universal usa a configuração completa previamente persistida no ESP, sem identidade ou senha incorporada. Dispositivo antigo exige primeiro uma atualização de instalação com adoção explícita.
- Uma atualização com empresa ou servidor diferentes é recusada; filas são preservadas. Não há URL/ID padrão dos projetos anteriores.
- Compilações por perfil usam diretórios exclusivos. Exportação verifica perfil, projeto, ambiente e SHA-256, bloqueando arquivos antigos ou modificados. ZIP inclui manifesto sem credenciais.
- Scripts BAT/PS1 exportados contêm um perfil protegido pelo Windows, não senhas abertas. Só funcionam no mesmo usuário do Windows.

Cadastre os valores reais antes da primeira compilação; nenhum cadastro foi preenchido automaticamente com as credenciais dos projetos antigos. A chave solicitada corresponde a DISPOSITIVOS_API_KEY do servidor.

O suporte ao perfil requer `firmware-profile.json`, `scripts/load_firmware_profile.py` e `include/firmware_profile.h` no projeto. Projetos genéricos sem esse contrato continuam no fluxo normal do PlatformIO.

### Verificação

Execute `./build.ps1` e `./tests/run-tests.ps1`. Os testes usam uma pasta temporária isolada e não alteram os cadastros existentes do usuário. Firmware físico, entrega OTA e alcance da rede precisam ser validados em um ESP antes de atualizar as máquinas.


## Gravar no ESP pelo cabo USB

1. Selecione o projeto e confira empresa, servidor e rede Wi-Fi. Para a primeira instalação, use o projeto bootstrap e uma rede inicial cadastrada no Studio.
2. Conecte o ESP usando um cabo USB de dados. Clique em **Atualizar portas** e selecione a porta COM do dispositivo. A seleção é explícita; o Studio não escolhe um dispositivo automaticamente.
3. Clique em **Compilar e gravar USB** e confira a porta e o destino na confirmação.
4. Aguarde a conclusão. O PlatformIO compila com o perfil selecionado e executa o alvo `upload` do ambiente, incluindo bootloader, partições e aplicação necessários à primeira instalação. Os três projetos ESP32 usam o protocolo serial padrão do PlatformIO.

O perfil privado continua disponível durante a gravação e é removido ao finalizar. O Studio não adiciona `erase_flash` e desativa `FIRMWARE_FACTORY_RESET` nessa operação, preservando NVS e filas no bootstrap. O firmware continua validando mudanças de empresa/servidor; gravação USB não libera automaticamente uma instalação vinculada a outro destino. Para incorporar os dados selecionados, desmarque **Usar configuração já salva no ESP**.

Feche monitores seriais que estejam usando a porta. Se a conexão ficar em `Connecting`, mantenha **BOOT** pressionado até a gravação iniciar. Se nenhuma porta aparecer, confira o cabo de dados e o driver USB da placa. O sucesso só é mostrado quando o processo termina sem erro; a janela de progresso e o log acompanham a operação.

Os testes usam um gravador simulado e perfis fictícios; não gravam em dispositivos físicos.
