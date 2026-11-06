# Device Firmware Studio

Aplicativo Windows para cadastrar projetos PlatformIO, compilar firmware ESP32, localizar artefatos e calcular SHA-256.

## Recursos

- Salva vários projetos em `%LOCALAPPDATA%\DeviceFirmwareStudio\projects.xml`.
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

O executável será criado em `dist\DeviceFirmwareStudio.exe`.

O aplicativo utiliza o compilador C# do próprio Windows e não requer o SDK do .NET instalado.

## Perfis de empresa e conexão (1.2.0)

- Abas Empresas, Servidores / URLs e Redes Wi-Fi. Cada empresa tem ID único, nome, endereço e situação local ativa/inativa. Isso não altera seu cadastro no sistema.
- Cada empresa pode ter vários servidores e redes. Trocar empresa limpa as seleções incompatíveis. Chaves pertencem ao servidor; não são compartilhadas automaticamente entre URLs.
- Credenciais no cadastro são protegidas com DPAPI para o usuário atual do Windows. Copiar o XML para outro usuário não transfere o acesso às senhas.
- Testar conexão consulta o diagnóstico somente leitura: confirma API, chave e banco preparado, com data do resultado. Não registra dispositivos e não comprova conectividade a partir da rede do ESP.
- Rede inicial do Studio é reaplicada uma vez no primeiro boot de cada nova compilação, mesmo que SSID e senha sejam iguais aos anteriores; os próximos reinícios preservam as alterações recebidas do sistema. Bootstrap exige uma rede inicial.
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

O perfil privado continua disponível durante a gravação e é removido ao finalizar. O Studio não adiciona `erase_flash` e desativa `FIRMWARE_FACTORY_RESET` nessa operação, preservando NVS e filas no bootstrap. O firmware continua validando mudanças de empresa/servidor; gravação USB não libera automaticamente uma instalação vinculada a outro destino. Para configurar empresa, servidor e rede inicial, use o projeto bootstrap. Os projetos de disponibilidade e refugo compilam como atualização global, preservando o perfil existente no ESP.

Feche monitores seriais que estejam usando a porta. Se a conexão ficar em `Connecting`, mantenha **BOOT** pressionado até a gravação iniciar. Se nenhuma porta aparecer, confira o cabo de dados e o driver USB da placa. O sucesso só é mostrado quando o processo termina sem erro; a janela de progresso e o log acompanham a operação.

Os testes usam um gravador simulado e perfis fictícios; não gravam em dispositivos físicos.


## Monitor serial e terminais ampliados

- **Monitor ESP** abre o painel serial. Selecione a porta, a velocidade (115200 por padrão) e clique em **Conectar**. DTR e RTS ficam desativados; o Studio não envia reset ao conectar. Algumas placas e drivers podem reiniciar o ESP ao abrir a porta.
- O monitor oferece conectar/desconectar, pausa da exibição, rolagem automática, limpar, copiar e salvar log. A pausa mantém a coleta dentro do limite do histórico; **Limpar** remove apenas o histórico exibido no computador.
- Cada terminal tem **Ampliar**, abrindo uma janela redimensionável e maximizável. Selecione **Compilação**, **Monitor ESP** ou **Lado a lado**; a divisória pode ser arrastada. Fechar a janela ampliada devolve os painéis ao Studio e não interrompe a compilação ou a conexão serial.
- A mesma conexão e os mesmos históricos são reutilizados em ambas as visualizações. Cada histórico mantém até 262144 caracteres em memória e a interface é atualizada em lotes, apenas quando há mensagens novas. Copiar e salvar exportam o histórico ainda disponível, com as credenciais conhecidas ocultadas.
- Uma compilação comum mantém o monitor conectado. Ao gravar na mesma porta USB, o Studio aguarda o fechamento do monitor, grava e tenta reconectar após o término. **Desconectar** cancela a reconexão automática. Uma gravação em outra porta não interrompe esse monitor.
- Durante a compilação, configurações e seleção de projeto ficam protegidas, enquanto terminais e ferramentas de visualização continuam disponíveis. Textos de orientação e status permanecem legíveis; controles seriais usam cores próprias para manter contraste mesmo desabilitados.

Os testes cobrem conexão simulada, porta ocupada, pausa/retomada, histórico limitado, proteção de credenciais, janela ampliada e coordenação com a gravação USB. A comunicação com o ESP e o comportamento do driver precisam de validação física.


## Organização da compilação

A aba Compilação tem quatro seções internas, sem uma rolagem geral misturando as ferramentas:

- **Preparar e compilar:** configuração de empresa, servidor e Wi-Fi, teste de conexão, compilação, limpeza e histórico.
- **Gravação USB:** resumo do destino selecionado, seleção da porta, gravação e acesso ao monitor. **Alterar configuração** retorna à primeira seção.
- **Arquivos:** artefatos, cópia, exportação, SHA-256, scripts e pacote ZIP.
- **Terminais:** log de compilação e monitor serial, separados ou lado a lado, com opção de ampliar.

Iniciar uma compilação ou abrir o monitor leva à seção de terminais. Trocar de seção preserva o perfil, a conexão e os históricos. Os logs usam barras de rolagem desenhadas pelo Studio e os seletores e caixas de seleção têm contornos próprios no tema escuro. A rolagem automática continua opcional.


### Reaplicar o Wi-Fi inicial

Cada compilação com **Rede inicial do Studio** recebe um identificador novo de aplicação de Wi-Fi. Ao compilar e gravar novamente o bootstrap pelo Studio, o primeiro boot aplica a rede selecionada mesmo que a mesma rede já tenha sido usada numa instalação anterior. Isso preserva a NVS e as filas; não é necessário apagar a memória inteira.

Após conectar, o firmware continua aceitando o Wi-Fi recebido do sistema. Essa troca pode acontecer já na primeira sincronização. Reiniciar o ESP não reaplica o Wi-Fi inicial dessa mesma compilação. Regravar exatamente o mesmo arquivo `.bin` também não cria uma identificação nova: use **Compilar e gravar USB** para gerar a nova aplicação.

O monitor mostra `[PERFIL] Rede inicial do Studio aplicada nesta compilacao: <SSID>` quando essa etapa ocorre. Os scripts protegidos BAT/PS1 também geram uma identificação nova a cada execução.

## Biblioteca global de firmwares

- Bootstrap: instalação inicial por empresa, com servidor e Wi-Fi de descoberta escolhidos no Studio.
- Disponibilidade e Refugo: atualização global automática. Empresa, servidor, chave e Wi-Fi inicial não entram no .bin. O ESP herda seu perfil NVS e continua recebendo o Wi-Fi de uso do sistema.
- Cada artefato novo possui um marcador de compatibilidade e o manifesto registra tipo, modo, placa e SHA-256 sem segredos.
- No sistema, use a Biblioteca de firmwares do superadministrador para cadastrar, validar, testar e liberar as versões. As ações exigem a senha atual.
- O SHA-256 confere os bytes; ele não é uma assinatura de autoria nem um antivírus. Compile código revisado de origem confiável.


## Nome do aplicativo

O aplicativo é distribuído como **Device Firmware Studio**, no executável `dist/DeviceFirmwareStudio.exe`.
Os cadastros ficam em `%LOCALAPPDATA%/DeviceFirmwareStudio/projects.xml`. Na primeira abertura, quando ainda não há um cadastro no novo destino, os dados da instalação anterior são importados automaticamente, preservando projetos, histórico e credenciais criptografadas para o mesmo usuário do Windows. O arquivo anterior não é apagado nem substituído.

O repositório GitHub de atualizações mantém seu endereço existente. Novas releases devem publicar o arquivo `DeviceFirmwareStudio.exe`. As instalações antigas que procuram o nome anterior precisam receber esta versão manualmente, ou a release pode também disponibilizar um arquivo com o nome exigido por elas durante a transição.

## Instância de destino dos firmwares globais

Disponibilidade e refugo exibem somente o seletor de instância de destino, com URLs equivalentes agrupadas, independentemente da empresa. A seleção identifica a confirmação, o histórico, o manifesto e os nomes dos arquivos exportados. Trocar a instância exige compilar novamente para exportar um pacote identificado com o novo destino.

O perfil enviado à compilação continua global (`mode: inherit`): empresa, servidor, chave e Wi-Fi são preservados no ESP. A seleção no Studio não publica o arquivo nem restringe o binário a uma instância. O cadastro e a aprovação continuam sendo realizados manualmente na biblioteca de cada sistema.

O firmware inicial mantém empresa, servidor e Wi-Fi inicial por empresa. Nenhuma API ou código dos três firmwares precisa ser alterado para este seletor.
