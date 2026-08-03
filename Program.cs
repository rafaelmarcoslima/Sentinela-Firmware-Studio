using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SentinelaFirmwareStudio
{
    public static class I18n
    {
        public static string Language = "pt";
        static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            { "Sentinela Firmware Studio", "Sentinela Firmware Studio" }, { "PROJETOS SALVOS", "SAVED PROJECTS" },
            { "Configuração do projeto", "Project settings" }, { "Compilação e arquivos", "Build and files" },
            { "CONFIGURAÇÃO DO PROJETO", "PROJECT SETTINGS" }, { "Novo manual", "New manual" }, { "Editar configuração", "Edit settings" },
            { "Salvar configuração", "Save settings" }, { "Cancelar edição", "Cancel editing" }, { "Nome", "Name" },
            { "Pasta do projeto", "Project folder" }, { "PlatformIO", "PlatformIO" }, { "Ambiente", "Environment" },
            { "Procurar...", "Browse..." }, { "Detectar", "Detect" }, { "Abrir pasta", "Open folder" },
            { "ARQUIVOS GERADOS E EXPORTAÇÃO", "GENERATED FILES AND EXPORT" }, { "Arquivo gerado:", "Generated file:" },
            { "Atualizar", "Refresh" }, { "Copiar arquivo", "Copy file" }, { "Salvar como...", "Save as..." },
            { "Copiar SHA-256", "Copy SHA-256" }, { "Scripts:", "Scripts:" }, { "Exportar .BAT", "Export .BAT" },
            { "Exportar .PS1", "Export .PS1" }, { "Compilar", "Build" }, { "Limpar e compilar", "Clean and build" },
            { "Salvar firmware", "Save firmware" }, { "Criar pacote ZIP", "Create ZIP package" }, { "Histórico", "History" },
            { "Adicionar por pasta", "Add from folder" }, { "Remover", "Remove" }, { "Atualizações", "Updates" },
            { "Configurações", "Settings" }, { "Escolha a cor do tema", "Choose theme color" },
            { "Configurações de aparência", "Appearance settings" }, { "Idioma da interface", "Interface language" },
            { "Português", "Portuguese" }, { "Inglês", "English" }, { "Espanhol", "Spanish" },
            { "Pronto", "Ready" }, { "Projeto", "Project" }, { "Data e hora", "Date and time" },
            { "Resultado", "Result" }, { "Duração", "Duration" }, { "Tamanho", "Size" },
            { "Caminho do firmware", "Firmware path" }, { "Histórico de compilações", "Build history" },
            { "Sucesso", "Success" }, { "Falha", "Failure" }, { "Compilação concluída", "Build completed" },
            { "Falha na compilação", "Build failed" }, { "Atualização disponível", "Update available" },
            { "Fluxo automático: selecione uma pasta com platformio.ini e o projeto será cadastrado com os dados detectados.", "Automatic flow: select a folder containing platformio.ini and the detected project data will be registered." },
            { "Fluxo manual: limpa os campos para cadastrar nome, pasta, PlatformIO e ambiente separadamente.", "Manual flow: clears the fields so name, folder, PlatformIO and environment can be entered separately." },
            { "Remove apenas o caminho salvo da lista. A pasta e os arquivos do projeto não são excluídos.", "Removes only the saved path from the list. The project folder and files are not deleted." },
            { "Compila o projeto selecionado usando o ambiente PlatformIO informado.", "Builds the selected project using the chosen PlatformIO environment." },
            { "Apaga os arquivos temporários da compilação anterior e compila o projeto novamente.", "Deletes previous temporary build files and builds the project again." },
            { "Desbloqueia o nome e os caminhos para permitir alterações controladas.", "Unlocks the name and paths for controlled editing." },
            { "Salva as alterações e protege novamente os campos de configuração.", "Saves changes and locks the settings fields again." },
            { "Descarta as alterações feitas e restaura os últimos valores salvos.", "Discards changes and restores the last saved values." },
            { "Abre a pasta do projeto selecionado no Explorador de Arquivos.", "Opens the selected project folder in File Explorer." },
            { "Abre uma janela para localizar a pasta ou o executável solicitado.", "Opens a window to locate the requested folder or executable." },
            { "Lê o platformio.ini e lista automaticamente os ambientes disponíveis.", "Reads platformio.ini and automatically lists available environments." },
            { "Procura novamente os artefatos gerados na pasta .pio\\build do projeto.", "Scans the project's .pio\\build folder again for generated files." },
            { "Copia o artefato selecionado para a área de transferência do Windows.", "Copies the selected file to the Windows clipboard." },
            { "Calcula e copia o código SHA-256 do artefato selecionado.", "Calculates and copies the selected file's SHA-256." },
            { "Salva uma cópia do arquivo gerado selecionado no local escolhido.", "Saves a copy of the selected generated file to the chosen location." },
            { "Salva diretamente o firmware.bin principal no local escolhido.", "Saves the main firmware.bin directly to the chosen location." },
            { "Cria um ZIP com firmware.bin, SHA-256 e informações da compilação.", "Creates a ZIP containing firmware.bin, SHA-256 and build information." },
            { "Mostra as compilações anteriores, duração, resultado e SHA-256.", "Shows previous builds, duration, result and SHA-256." },
            { "Consulta o GitHub para verificar se existe uma versão mais recente do aplicativo.", "Checks GitHub for a newer application version." },
            { "Cria um script CMD para compilar este projeto fora do aplicativo.", "Creates a CMD script to build this project outside the application." },
            { "Cria um script PowerShell para compilar este projeto fora do aplicativo.", "Creates a PowerShell script to build this project outside the application." },
            { "Abre as configurações visuais e permite escolher a cor do tema.", "Opens appearance settings and lets you choose the theme color." },
            { "Executa esta ação.", "Runs this action." }
            ,{ "Firmware principal", "Main firmware" }, { "Compilando firmware", "Building firmware" }, { "Preparando compilação...", "Preparing build..." },
            { "Aguarde. Os controles serão liberados ao terminar.", "Please wait. Controls will be enabled when finished." },
            { "Arquivo gerado", "Generated file" }, { "Scripts de compilação", "Build scripts" }, { "Clique para entender: ", "Click to learn: " },
            { "Escolhe um dos arquivos produzidos pelo PlatformIO. Esta escolha é usada pelos botões Copiar arquivo e Copiar SHA-256; ela não muda o processo de compilação.", "Selects one of the files produced by PlatformIO. This selection is used by Copy file and Copy SHA-256; it does not change the build process." },
            { "Cria um arquivo .BAT ou .PS1 com o comando para recompilar o projeto fora deste aplicativo. Esses scripts não são firmware.", "Creates a .BAT or .PS1 command file to rebuild the project outside this application. These scripts are not firmware." }
            ,{ "Arquivo já produzido pela última compilação do PlatformIO.", "File already produced by the latest PlatformIO build." }
            ,{ "Selecione o arquivo gerado que deseja copiar ou consultar o SHA-256.", "Select the generated file you want to copy or inspect its SHA-256." }
            ,{ "Arquivos de comando para repetir a compilação fora do aplicativo.", "Command files used to repeat the build outside the application." }
            ,{ "Azul", "Blue" }, { "Turquesa", "Turquoise" }, { "Verde", "Green" }, { "Roxo", "Purple" }, { "Laranja", "Orange" }, { "Vermelho", "Red" }, { "Grafite", "Graphite" }
            ,{ "A pasta selecionada não contém platformio.ini.", "The selected folder does not contain platformio.ini." }, { "Projeto inválido", "Invalid project" },
            { "Informe um nome para o projeto.", "Enter a project name." }, { "A pasta do projeto não existe.", "The project folder does not exist." },
            { "O executável do PlatformIO não foi encontrado.", "The PlatformIO executable was not found." }, { "Informe ou detecte um ambiente PlatformIO.", "Enter or detect a PlatformIO environment." },
            { "platformio.ini não encontrado.", "platformio.ini was not found." }, { "platformio.exe não encontrado.", "platformio.exe was not found." },
            { "Informe o ambiente PlatformIO.", "Select a PlatformIO environment." }, { "Selecione um artefato válido.", "Select a valid generated file." },
            { "Selecione um arquivo gerado válido.", "Select a valid generated file." }, { "firmware.bin não encontrado. Compile o projeto primeiro.", "firmware.bin was not found. Build the project first." },
            { "Firmware indisponível", "Firmware unavailable" }, { "Remover projeto", "Remove project" }
        };
        static readonly Dictionary<string, string> Spanish = new Dictionary<string, string>
        {
            { "Sentinela Firmware Studio", "Sentinela Firmware Studio" }, { "PROJETOS SALVOS", "PROYECTOS GUARDADOS" },
            { "Configuração do projeto", "Configuración del proyecto" }, { "Compilação e arquivos", "Compilación y archivos" },
            { "CONFIGURAÇÃO DO PROJETO", "CONFIGURACIÓN DEL PROYECTO" }, { "Novo manual", "Nuevo manual" }, { "Editar configuração", "Editar configuración" },
            { "Salvar configuração", "Guardar configuración" }, { "Cancelar edição", "Cancelar edición" }, { "Nome", "Nombre" },
            { "Pasta do projeto", "Carpeta del proyecto" }, { "PlatformIO", "PlatformIO" }, { "Ambiente", "Entorno" },
            { "Procurar...", "Buscar..." }, { "Detectar", "Detectar" }, { "Abrir pasta", "Abrir carpeta" },
            { "ARQUIVOS GERADOS E EXPORTAÇÃO", "ARCHIVOS GENERADOS Y EXPORTACIÓN" }, { "Arquivo gerado:", "Archivo generado:" },
            { "Atualizar", "Actualizar" }, { "Copiar arquivo", "Copiar archivo" }, { "Salvar como...", "Guardar como..." },
            { "Copiar SHA-256", "Copiar SHA-256" }, { "Scripts:", "Scripts:" }, { "Exportar .BAT", "Exportar .BAT" },
            { "Exportar .PS1", "Exportar .PS1" }, { "Compilar", "Compilar" }, { "Limpar e compilar", "Limpiar y compilar" },
            { "Salvar firmware", "Guardar firmware" }, { "Criar pacote ZIP", "Crear paquete ZIP" }, { "Histórico", "Historial" },
            { "Adicionar por pasta", "Añadir desde carpeta" }, { "Remover", "Eliminar" }, { "Atualizações", "Actualizaciones" },
            { "Configurações", "Configuración" }, { "Escolha a cor do tema", "Elija el color del tema" },
            { "Configurações de aparência", "Configuración de apariencia" }, { "Idioma da interface", "Idioma de la interfaz" },
            { "Português", "Portugués" }, { "Inglês", "Inglés" }, { "Espanhol", "Español" },
            { "Pronto", "Listo" }, { "Projeto", "Proyecto" }, { "Data e hora", "Fecha y hora" },
            { "Resultado", "Resultado" }, { "Duração", "Duración" }, { "Tamanho", "Tamaño" },
            { "Caminho do firmware", "Ruta del firmware" }, { "Histórico de compilações", "Historial de compilaciones" },
            { "Sucesso", "Éxito" }, { "Falha", "Error" }, { "Compilação concluída", "Compilación completada" },
            { "Falha na compilação", "Error de compilación" }, { "Atualização disponível", "Actualización disponible" },
            { "Fluxo automático: selecione uma pasta com platformio.ini e o projeto será cadastrado com os dados detectados.", "Flujo automático: seleccione una carpeta con platformio.ini y se registrarán los datos detectados." },
            { "Fluxo manual: limpa os campos para cadastrar nome, pasta, PlatformIO e ambiente separadamente.", "Flujo manual: limpia los campos para introducir nombre, carpeta, PlatformIO y entorno por separado." },
            { "Remove apenas o caminho salvo da lista. A pasta e os arquivos do projeto não são excluídos.", "Elimina solo la ruta guardada de la lista. La carpeta y los archivos no se borran." },
            { "Compila o projeto selecionado usando o ambiente PlatformIO informado.", "Compila el proyecto seleccionado con el entorno PlatformIO elegido." },
            { "Apaga os arquivos temporários da compilação anterior e compila o projeto novamente.", "Elimina los archivos temporales anteriores y vuelve a compilar." },
            { "Desbloqueia o nome e os caminhos para permitir alterações controladas.", "Desbloquea el nombre y las rutas para una edición controlada." },
            { "Salva as alterações e protege novamente os campos de configuração.", "Guarda los cambios y vuelve a bloquear los campos." },
            { "Descarta as alterações feitas e restaura os últimos valores salvos.", "Descarta los cambios y restaura los últimos valores guardados." },
            { "Abre a pasta do projeto selecionado no Explorador de Arquivos.", "Abre la carpeta del proyecto en el Explorador de archivos." },
            { "Abre uma janela para localizar a pasta ou o executável solicitado.", "Abre una ventana para localizar la carpeta o el ejecutable." },
            { "Lê o platformio.ini e lista automaticamente os ambientes disponíveis.", "Lee platformio.ini y muestra automáticamente los entornos disponibles." },
            { "Procura novamente os artefatos gerados na pasta .pio\\build do projeto.", "Vuelve a buscar archivos generados en .pio\\build." },
            { "Copia o artefato selecionado para a área de transferência do Windows.", "Copia el archivo seleccionado al portapapeles de Windows." },
            { "Calcula e copia o código SHA-256 do artefato selecionado.", "Calcula y copia el SHA-256 del archivo seleccionado." },
            { "Salva uma cópia do arquivo gerado selecionado no local escolhido.", "Guarda una copia del archivo seleccionado en la ubicación elegida." },
            { "Salva diretamente o firmware.bin principal no local escolhido.", "Guarda directamente el firmware.bin principal." },
            { "Cria um ZIP com firmware.bin, SHA-256 e informações da compilação.", "Crea un ZIP con firmware.bin, SHA-256 e información de compilación." },
            { "Mostra as compilações anteriores, duração, resultado e SHA-256.", "Muestra compilaciones anteriores, duración, resultado y SHA-256." },
            { "Consulta o GitHub para verificar se existe uma versão mais recente do aplicativo.", "Consulta GitHub para buscar una versión más reciente." },
            { "Cria um script CMD para compilar este projeto fora do aplicativo.", "Crea un script CMD para compilar fuera de la aplicación." },
            { "Cria um script PowerShell para compilar este projeto fora do aplicativo.", "Crea un script PowerShell para compilar fuera de la aplicación." },
            { "Abre as configurações visuais e permite escolher a cor do tema.", "Abre la configuración visual y permite elegir el color del tema." },
            { "Executa esta ação.", "Ejecuta esta acción." }
            ,{ "Firmware principal", "Firmware principal" }, { "Compilando firmware", "Compilando firmware" }, { "Preparando compilação...", "Preparando compilación..." },
            { "Aguarde. Os controles serão liberados ao terminar.", "Espere. Los controles se habilitarán al finalizar." },
            { "Arquivo gerado", "Archivo generado" }, { "Scripts de compilação", "Scripts de compilación" }, { "Clique para entender: ", "Haga clic para entender: " },
            { "Escolhe um dos arquivos produzidos pelo PlatformIO. Esta escolha é usada pelos botões Copiar arquivo e Copiar SHA-256; ela não muda o processo de compilação.", "Selecciona uno de los archivos producidos por PlatformIO. Se usa para Copiar archivo y Copiar SHA-256; no cambia la compilación." },
            { "Cria um arquivo .BAT ou .PS1 com o comando para recompilar o projeto fora deste aplicativo. Esses scripts não são firmware.", "Crea un archivo .BAT o .PS1 para recompilar fuera de la aplicación. Estos scripts no son firmware." }
            ,{ "Arquivo já produzido pela última compilação do PlatformIO.", "Archivo producido por la última compilación de PlatformIO." }
            ,{ "Selecione o arquivo gerado que deseja copiar ou consultar o SHA-256.", "Seleccione el archivo generado que desea copiar o consultar mediante SHA-256." }
            ,{ "Arquivos de comando para repetir a compilação fora do aplicativo.", "Archivos de comandos para repetir la compilación fuera de la aplicación." }
            ,{ "Azul", "Azul" }, { "Turquesa", "Turquesa" }, { "Verde", "Verde" }, { "Roxo", "Morado" }, { "Laranja", "Naranja" }, { "Vermelho", "Rojo" }, { "Grafite", "Grafito" }
            ,{ "A pasta selecionada não contém platformio.ini.", "La carpeta seleccionada no contiene platformio.ini." }, { "Projeto inválido", "Proyecto no válido" },
            { "Informe um nome para o projeto.", "Introduzca un nombre para el proyecto." }, { "A pasta do projeto não existe.", "La carpeta del proyecto no existe." },
            { "O executável do PlatformIO não foi encontrado.", "No se encontró el ejecutable de PlatformIO." }, { "Informe ou detecte um ambiente PlatformIO.", "Introduzca o detecte un entorno PlatformIO." },
            { "platformio.ini não encontrado.", "No se encontró platformio.ini." }, { "platformio.exe não encontrado.", "No se encontró platformio.exe." },
            { "Informe o ambiente PlatformIO.", "Seleccione un entorno PlatformIO." }, { "Selecione um artefato válido.", "Seleccione un archivo generado válido." },
            { "Selecione um arquivo gerado válido.", "Seleccione un archivo generado válido." }, { "firmware.bin não encontrado. Compile o projeto primeiro.", "No se encontró firmware.bin. Compile primero el proyecto." },
            { "Firmware indisponível", "Firmware no disponible" }, { "Remover projeto", "Eliminar proyecto" }
        };
        public static string T(string portuguese)
        {
            string translated;
            if (Language == "en" && English.TryGetValue(portuguese, out translated)) return translated;
            if (Language == "es" && Spanish.TryGetValue(portuguese, out translated)) return translated;
            return portuguese;
        }
    }

    [Serializable]
    public class FirmwareProject
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Environment { get; set; }
        public string PlatformIoPath { get; set; }
        public override string ToString() { return Name; }
    }

    public class ArtifactItem
    {
        public string DisplayName { get; set; }
        public string FullPath { get; set; }
        public override string ToString() { return DisplayName; }
    }

    [Serializable]
    public class BuildHistoryEntry
    {
        public string Project { get; set; }
        public string Date { get; set; }
        public string Result { get; set; }
        public string Duration { get; set; }
        public string FirmwarePath { get; set; }
        public string FirmwareSize { get; set; }
        public string Sha256 { get; set; }
    }

    [Serializable]
    public class AppSettings
    {
        public List<FirmwareProject> Projects { get; set; }
        public List<BuildHistoryEntry> BuildHistory { get; set; }
        public string ThemeName { get; set; }
        public string Language { get; set; }
        public AppSettings() { Projects = new List<FirmwareProject>(); BuildHistory = new List<BuildHistoryEntry>(); ThemeName = "Azul"; Language = "pt"; }
    }

    public class BuildProgressForm : Form
    {
        Label messageLabel = new Label();

        public BuildProgressForm()
        {
            Text = I18n.T("Compilando firmware");
            Width = 460;
            Height = 155;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ControlBox = false;
            ShowInTaskbar = false;

            messageLabel.Text = I18n.T("Preparando compilação...");
            messageLabel.AutoSize = false;
            messageLabel.Dock = DockStyle.Top;
            messageLabel.Height = 52;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            var progress = new ProgressBar { Dock = DockStyle.Top, Height = 22, Style = ProgressBarStyle.Marquee, MarqueeAnimationSpeed = 25 };
            var note = new Label { Text = I18n.T("Aguarde. Os controles serão liberados ao terminar."), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            Controls.Add(note);
            Controls.Add(progress);
            Controls.Add(messageLabel);
        }

        public void SetMessage(string message) { messageLabel.Text = message; }
    }

    public class ThemePickerForm : Form
    {
        public string SelectedTheme { get; private set; }
        public string SelectedLanguage { get; private set; }

        public ThemePickerForm(string currentTheme, string currentLanguage, Dictionary<string, Color> themes)
        {
            SelectedTheme = currentTheme;
            SelectedLanguage = currentLanguage;
            Text = I18n.T("Configurações de aparência");
            Width = 510;
            Height = 285;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var title = new Label { Text = I18n.T("Escolha a cor do tema"), Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            var palette = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 76, Padding = new Padding(16, 4, 16, 4), FlowDirection = FlowDirection.LeftToRight };
            var tooltip = new ToolTip { ShowAlways = true };

            foreach (var theme in themes)
            {
                string name = theme.Key;
                var colorButton = new Button { Width = 46, Height = 46, BackColor = theme.Value, FlatStyle = FlatStyle.Flat, Margin = new Padding(7), TabStop = false };
                colorButton.FlatAppearance.BorderSize = name == currentTheme ? 4 : 1;
                colorButton.FlatAppearance.BorderColor = name == currentTheme ? Color.Black : Color.White;
                var path = new GraphicsPath();
                path.AddEllipse(1, 1, 42, 42);
                colorButton.Region = new Region(path);
                tooltip.SetToolTip(colorButton, I18n.T(name));
                colorButton.Click += delegate { SelectedTheme = name; };
                palette.Controls.Add(colorButton);
            }

            var languagePanel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 55, ColumnCount = 2, Padding = new Padding(20, 8, 20, 4) };
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            languagePanel.Controls.Add(new Label { Text = I18n.T("Idioma da interface"), AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            var languageBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            languageBox.Items.AddRange(new object[] { "Português", "English", "Español" });
            languageBox.SelectedIndex = currentLanguage == "en" ? 1 : currentLanguage == "es" ? 2 : 0;
            languageBox.SelectedIndexChanged += delegate { SelectedLanguage = languageBox.SelectedIndex == 1 ? "en" : languageBox.SelectedIndex == 2 ? "es" : "pt"; };
            languagePanel.Controls.Add(languageBox, 1, 0);
            var saveButton = new Button { Text = I18n.T("Salvar configuração"), AutoSize = true, Height = 32, Anchor = AnchorStyles.None };
            saveButton.Click += delegate { DialogResult = DialogResult.OK; Close(); };
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            bottom.Controls.Add(saveButton);
            Controls.Add(bottom);
            Controls.Add(languagePanel);
            Controls.Add(palette);
            Controls.Add(title);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        const string AppVersion = "1.1.0";
        const string GitHubLatestReleaseApi = "https://api.github.com/repos/rafaelmarcoslima/Sentinela-Firmware-Studio/releases/latest";
        const string GitHubReleasesPage = "https://github.com/rafaelmarcoslima/Sentinela-Firmware-Studio/releases";
        readonly string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SentinelaFirmwareStudio");
        string SettingsFile { get { return Path.Combine(settingsFolder, "projects.xml"); } }

        AppSettings settings = new AppSettings();
        ListBox projects = new ListBox();
        TextBox nameBox = new TextBox();
        TextBox folderBox = new TextBox();
        TextBox platformIoBox = new TextBox();
        ComboBox environmentBox = new ComboBox();
        ComboBox artifactBox = new ComboBox();
        TextBox logBox = new TextBox();
        Label statusLabel = new Label();
        Label hashLabel = new Label();
        Button buildButton = new Button();
        Button cleanBuildButton = new Button();
        Button addProjectButton = new Button();
        Button removeProjectButton = new Button();
        Button browseProjectButton = new Button();
        Button browsePlatformIoButton = new Button();
        Button detectEnvironmentButton = new Button();
        Button newManualButton = new Button();
        Button editConfigButton = new Button();
        Button saveConfigButton = new Button();
        Button cancelEditButton = new Button();
        bool editingConfiguration;
        bool singleEnvironmentDetected;
        bool creatingManualProject;
        FirmwareProject projectSelectedBeforeManualCreation;
        ToolTip buttonTooltips = new ToolTip { ShowAlways = true, InitialDelay = 350, ReshowDelay = 150, AutoPopDelay = 7000 };
        TableLayoutPanel themeToolbar;
        TabControl mainTabs;
        TabPage configurationTab;
        TabPage compilationTab;
        Color currentAccent = Color.FromArgb(0, 120, 215);
        Process activeProcess;
        Stopwatch buildStopwatch = new Stopwatch();
        BuildProgressForm progressForm;
        readonly Dictionary<Control, string> localizationKeys = new Dictionary<Control, string>();
        readonly Dictionary<Button, string> buttonKeys = new Dictionary<Button, string>();
        readonly Dictionary<Control, string> tooltipKeys = new Dictionary<Control, string>();
        readonly Dictionary<string, Color> themes = new Dictionary<string, Color>
        {
            { "Azul", Color.FromArgb(0, 120, 215) },
            { "Turquesa", Color.FromArgb(0, 137, 123) },
            { "Verde", Color.FromArgb(46, 125, 50) },
            { "Roxo", Color.FromArgb(123, 31, 162) },
            { "Laranja", Color.FromArgb(230, 81, 0) },
            { "Vermelho", Color.FromArgb(198, 40, 40) },
            { "Grafite", Color.FromArgb(69, 90, 100) }
        };

        public MainForm()
        {
            Text = "Sentinela Firmware Studio";
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            Width = 1050;
            Height = 700;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            BuildUi();
            LoadSettings();
            I18n.Language = string.IsNullOrWhiteSpace(settings.Language) ? "pt" : settings.Language;
            CaptureLocalizableControls(this);
            ApplyLanguage();
            ApplyTheme();
            Shown += delegate { BeginInvoke(new Action(delegate { CheckForUpdates(false); })); };
        }

        void BuildUi()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(10) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 265));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            themeToolbar = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = false, Height = 52, MinimumSize = new Size(0, 52), MaximumSize = new Size(0, 52), Padding = new Padding(8, 5, 4, 5), Margin = new Padding(0, 0, 0, 8) };
            themeToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            themeToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            themeToolbar.Controls.Add(new Label { Text = "SENTINELA FIRMWARE STUDIO", AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 3, 3), ForeColor = Color.White }, 0, 0);
            var headerActions = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Anchor = AnchorStyles.Right };
            headerActions.Controls.Add(new Label { Text = "v" + AppVersion, AutoSize = true, ForeColor = Color.White, Margin = new Padding(3, 10, 8, 3) });
            headerActions.Controls.Add(MakeButton("Atualizações", delegate { CheckForUpdates(true); }));
            headerActions.Controls.Add(MakeButton("Configurações", delegate { OpenThemeSettings(); }));
            themeToolbar.Controls.Add(headerActions, 1, 0);
            root.Controls.Add(themeToolbar, 0, 0);
            root.SetColumnSpan(themeToolbar, 2);

            var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(0, 0, 10, 0) };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.Controls.Add(new Label { Text = "PROJETOS SALVOS", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 5, 3, 8) }, 0, 0);
            projects.Dock = DockStyle.Fill;
            projects.DrawMode = DrawMode.OwnerDrawFixed;
            projects.ItemHeight = 22;
            projects.DrawItem += DrawProjectItem;
            projects.SelectedIndexChanged += delegate { DisplaySelectedProject(); };
            left.Controls.Add(projects, 0, 1);
            var projectButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            addProjectButton = MakeButton("Adicionar por pasta", delegate { AddProject(); });
            removeProjectButton = MakeButton("Remover", delegate { RemoveProject(); });
            projectButtons.Controls.Add(addProjectButton);
            projectButtons.Controls.Add(removeProjectButton);
            left.Controls.Add(projectButtons, 0, 2);
            root.Controls.Add(left, 0, 1);

            var rightHost = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            rightHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightHost.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(rightHost, 1, 1);

            mainTabs = new TabControl { Dock = DockStyle.Fill, Font = this.Font, Padding = new Point(16, 6) };
            configurationTab = new TabPage("Configuração do projeto") { Padding = new Padding(10), BackColor = BackColor };
            compilationTab = new TabPage("Compilação e arquivos") { Padding = new Padding(10), BackColor = BackColor };
            mainTabs.TabPages.Add(configurationTab);
            mainTabs.TabPages.Add(compilationTab);
            rightHost.Controls.Add(mainTabs, 0, 0);

            var configurationLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 7, ColumnCount = 1 };
            for (int i = 0; i < 6; i++) configurationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            configurationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            configurationTab.Controls.Add(configurationLayout);

            var compilationLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            compilationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            compilationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            compilationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            compilationTab.Controls.Add(compilationLayout);

            var configurationActions = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 45, Padding = new Padding(0, 3, 0, 6), FlowDirection = FlowDirection.LeftToRight };
            configurationActions.Controls.Add(new Label { Text = "CONFIGURAÇÃO DO PROJETO", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 10, 16, 3) });
            newManualButton = MakeButton("Novo manual", delegate { BeginManualProjectCreation(); });
            editConfigButton = MakeButton("Editar configuração", delegate { BeginConfigurationEdit(); });
            saveConfigButton = MakeButton("Salvar configuração", delegate { SaveCurrentProject(); });
            cancelEditButton = MakeButton("Cancelar edição", delegate { CancelConfigurationEdit(); });
            configurationActions.Controls.Add(newManualButton);
            configurationActions.Controls.Add(editConfigButton);
            configurationActions.Controls.Add(saveConfigButton);
            configurationActions.Controls.Add(cancelEditButton);
            configurationLayout.Controls.Add(configurationActions, 0, 0);

            configurationLayout.Controls.Add(FieldRow("Nome", nameBox, null), 0, 1);
            browseProjectButton = MakeButton("Procurar...", delegate { BrowseProjectFolder(); });
            browsePlatformIoButton = MakeButton("Procurar...", delegate { BrowsePlatformIo(); });
            configurationLayout.Controls.Add(FieldRow("Pasta do projeto", folderBox, browseProjectButton), 0, 2);
            configurationLayout.Controls.Add(FieldRow("PlatformIO", platformIoBox, browsePlatformIoButton), 0, 3);
            environmentBox.DropDownStyle = ComboBoxStyle.DropDown;
            detectEnvironmentButton = MakeButton("Detectar", delegate { DetectEnvironments(); });
            configurationLayout.Controls.Add(FieldRow("Ambiente", environmentBox, detectEnvironmentButton), 0, 4);

            var artifactsGroup = new GroupBox { Text = "ARQUIVOS GERADOS E EXPORTAÇÃO", Dock = DockStyle.Fill, AutoSize = false, Height = 165, MinimumSize = new Size(0, 165), MaximumSize = new Size(0, 165), Padding = new Padding(10, 8, 10, 12), Font = new Font(Font, FontStyle.Bold) };
            var artifactsLayout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = false, RowCount = 3, ColumnCount = 3, Padding = new Padding(0, 0, 0, 6) };
            artifactsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            artifactsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            artifactsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var artifactLabel = new Label { Text = "Arquivo gerado:", AutoSize = true, Anchor = AnchorStyles.Left, Font = Font, Margin = new Padding(3, 8, 3, 3) };
            artifactsLayout.Controls.Add(artifactLabel, 0, 0);
            artifactBox.DropDownStyle = ComboBoxStyle.DropDownList;
            artifactBox.Dock = DockStyle.Fill;
            artifactBox.Font = Font;
            artifactBox.SelectedIndexChanged += delegate { UpdateHash(); };
            artifactsLayout.Controls.Add(artifactBox, 1, 0);
            artifactsLayout.Controls.Add(MakeHelpButton("Arquivo gerado", "Escolhe um dos arquivos produzidos pelo PlatformIO. Esta escolha é usada pelos botões Copiar arquivo e Copiar SHA-256; ela não muda o processo de compilação."), 2, 0);
            SetLocalizedTooltip(artifactLabel, "Arquivo já produzido pela última compilação do PlatformIO.");
            SetLocalizedTooltip(artifactBox, "Selecione o arquivo gerado que deseja copiar ou consultar o SHA-256.");

            var artifactButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 2, 0, 0), FlowDirection = FlowDirection.LeftToRight };
            artifactButtons.Controls.Add(MakeButton("Atualizar", delegate { RefreshArtifacts(); }));
            artifactButtons.Controls.Add(MakeButton("Copiar arquivo", delegate { CopyArtifactToClipboard(); }));
            artifactButtons.Controls.Add(MakeButton("Salvar como...", delegate { SaveSelectedArtifactAs(); }));
            artifactButtons.Controls.Add(MakeButton("Copiar SHA-256", delegate { CopyHash(); }));
            artifactsLayout.Controls.Add(artifactButtons, 1, 1);
            artifactsLayout.SetColumnSpan(artifactButtons, 2);

            var scriptsLabel = new Label { Text = "Scripts:", AutoSize = true, Anchor = AnchorStyles.Left, Font = Font, Margin = new Padding(3, 8, 3, 3) };
            artifactsLayout.Controls.Add(scriptsLabel, 0, 2);
            var scriptControls = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 2, 0, 7) };
            scriptControls.Controls.Add(MakeButton("Exportar .BAT", delegate { ExportScript(".bat"); }));
            scriptControls.Controls.Add(MakeButton("Exportar .PS1", delegate { ExportScript(".ps1"); }));
            artifactsLayout.Controls.Add(scriptControls, 1, 2);
            artifactsLayout.Controls.Add(MakeHelpButton("Scripts de compilação", "Cria um arquivo .BAT ou .PS1 com o comando para recompilar o projeto fora deste aplicativo. Esses scripts não são firmware."), 2, 2);
            SetLocalizedTooltip(scriptsLabel, "Arquivos de comando para repetir a compilação fora do aplicativo.");
            artifactsGroup.Controls.Add(artifactsLayout);
            compilationLayout.Controls.Add(artifactsGroup, 0, 0);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 5, 0, 3) };
            buildButton = MakeButton("Compilar", delegate { StartBuild(false); });
            cleanBuildButton = MakeButton("Limpar e compilar", delegate { StartBuild(true); });
            actions.Controls.Add(buildButton);
            actions.Controls.Add(cleanBuildButton);
            actions.Controls.Add(MakeButton("Salvar firmware", delegate { SaveFirmware(); }));
            actions.Controls.Add(MakeButton("Criar pacote ZIP", delegate { CreateFirmwarePackage(); }));
            actions.Controls.Add(MakeButton("Histórico", delegate { ShowBuildHistory(); }));
            compilationLayout.Controls.Add(actions, 0, 1);

            var configurationFolderActions = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 43, Padding = new Padding(112, 5, 0, 3) };
            configurationFolderActions.Controls.Add(MakeButton("Abrir pasta", delegate { OpenProjectFolder(); }));
            configurationLayout.Controls.Add(configurationFolderActions, 0, 5);

            logBox.Dock = DockStyle.Fill;
            logBox.Multiline = true;
            logBox.ScrollBars = ScrollBars.Both;
            logBox.ReadOnly = true;
            logBox.WordWrap = false;
            logBox.Font = new Font("Consolas", 9F);
            logBox.BackColor = Color.FromArgb(28, 31, 36);
            logBox.ForeColor = Color.Gainsboro;
            compilationLayout.Controls.Add(logBox, 0, 2);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            statusLabel.Text = "Pronto";
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font(Font, FontStyle.Bold);
            hashLabel.Text = "SHA-256: —";
            hashLabel.AutoSize = true;
            footer.Controls.Add(statusLabel, 0, 0);
            footer.Controls.Add(hashLabel, 0, 1);
            rightHost.Controls.Add(footer, 0, 1);
            SetConfigurationEditMode(false);
        }

        Button MakeButton(string text, EventHandler click)
        {
            var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(0, 32), Margin = new Padding(3), Font = this.Font, FlatStyle = FlatStyle.Flat, ImageAlign = ContentAlignment.MiddleLeft, TextImageRelation = TextImageRelation.ImageBeforeText };
            buttonKeys[button] = text;
            button.Image = CreateButtonIcon(GetButtonGlyph(text));
            buttonTooltips.SetToolTip(button, I18n.T(GetButtonTooltip(text)));
            button.Click += click;
            return button;
        }

        Button MakeHelpButton(string title, string explanation)
        {
            var button = new Button { Text = "?", Width = 28, Height = 28, MinimumSize = new Size(28, 28), Margin = new Padding(3), Font = new Font(this.Font, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Tag = "help" };
            SetLocalizedTooltip(button, "Clique para entender: " + title);
            button.Click += delegate { MessageBox.Show(I18n.T(explanation), I18n.T(title), MessageBoxButtons.OK, MessageBoxIcon.Information); };
            return button;
        }

        string GetButtonTooltip(string text)
        {
            if (text.StartsWith("Adicionar por pasta")) return "Fluxo automático: selecione uma pasta com platformio.ini e o projeto será cadastrado com os dados detectados.";
            if (text.StartsWith("Novo manual")) return "Fluxo manual: limpa os campos para cadastrar nome, pasta, PlatformIO e ambiente separadamente.";
            if (text.StartsWith("Remover")) return "Remove apenas o caminho salvo da lista. A pasta e os arquivos do projeto não são excluídos.";
            if (text.StartsWith("Compilar")) return "Compila o projeto selecionado usando o ambiente PlatformIO informado.";
            if (text.StartsWith("Limpar")) return "Apaga os arquivos temporários da compilação anterior e compila o projeto novamente.";
            if (text.StartsWith("Editar configuração")) return "Desbloqueia o nome e os caminhos para permitir alterações controladas.";
            if (text.StartsWith("Salvar configuração")) return "Salva as alterações e protege novamente os campos de configuração.";
            if (text.StartsWith("Cancelar edição")) return "Descarta as alterações feitas e restaura os últimos valores salvos.";
            if (text.StartsWith("Abrir pasta")) return "Abre a pasta do projeto selecionado no Explorador de Arquivos.";
            if (text.StartsWith("Procurar")) return "Abre uma janela para localizar a pasta ou o executável solicitado.";
            if (text.StartsWith("Detectar")) return "Lê o platformio.ini e lista automaticamente os ambientes disponíveis.";
            if (text.StartsWith("Atualizar")) return "Procura novamente os artefatos gerados na pasta .pio\\build do projeto.";
            if (text.StartsWith("Copiar arquivo")) return "Copia o artefato selecionado para a área de transferência do Windows.";
            if (text.StartsWith("Copiar SHA-256")) return "Calcula e copia o código SHA-256 do artefato selecionado.";
            if (text.StartsWith("Salvar como")) return "Salva uma cópia do arquivo gerado selecionado no local escolhido.";
            if (text.StartsWith("Salvar firmware")) return "Salva diretamente o firmware.bin principal no local escolhido.";
            if (text.StartsWith("Criar pacote ZIP")) return "Cria um ZIP com firmware.bin, SHA-256 e informações da compilação.";
            if (text.StartsWith("Histórico")) return "Mostra as compilações anteriores, duração, resultado e SHA-256.";
            if (text.StartsWith("Atualizações")) return "Consulta o GitHub para verificar se existe uma versão mais recente do aplicativo.";
            if (text.StartsWith("Salvar formato")) return "Localiza e salva o arquivo correspondente ao formato escolhido.";
            if (text.StartsWith("Exportar .BAT")) return "Cria um script CMD para compilar este projeto fora do aplicativo.";
            if (text.StartsWith("Exportar .PS1")) return "Cria um script PowerShell para compilar este projeto fora do aplicativo.";
            if (text.StartsWith("Configurações")) return "Abre as configurações visuais e permite escolher a cor do tema.";
            return "Executa esta ação.";
        }

        string GetButtonGlyph(string text)
        {
            if (text.StartsWith("Adicionar")) return "+";
            if (text.StartsWith("Novo manual")) return "✦";
            if (text.StartsWith("Remover")) return "−";
            if (text.StartsWith("Compilar")) return "▶";
            if (text.StartsWith("Limpar")) return "↻";
            if (text.StartsWith("Editar configuração")) return "✎";
            if (text.StartsWith("Salvar configuração")) return "✓";
            if (text.StartsWith("Cancelar edição")) return "×";
            if (text.StartsWith("Abrir pasta") || text.StartsWith("Procurar")) return "▣";
            if (text.StartsWith("Detectar")) return "⌕";
            if (text.StartsWith("Atualizar")) return "↻";
            if (text.StartsWith("Copiar")) return "⧉";
            if (text.StartsWith("Salvar como")) return "⇩";
            if (text.StartsWith("Salvar firmware")) return "★";
            if (text.StartsWith("Criar pacote ZIP")) return "▤";
            if (text.StartsWith("Histórico")) return "◷";
            if (text.StartsWith("Atualizações")) return "↥";
            if (text.StartsWith("Salvar formato")) return "⇩";
            if (text.StartsWith("Exportar")) return "⇩";
            if (text.StartsWith("Configurações")) return "⚙";
            return "•";
        }

        Bitmap CreateButtonIcon(string glyph) { return CreateButtonIcon(glyph, Color.FromArgb(55, 55, 55)); }

        Bitmap CreateButtonIcon(string glyph, Color color)
        {
            var bitmap = new Bitmap(18, 18);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Segoe UI Symbol", 10F, FontStyle.Bold, GraphicsUnit.Point))
            using (var brush = new SolidBrush(color))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                var size = graphics.MeasureString(glyph, font);
                graphics.DrawString(glyph, font, brush, (18 - size.Width) / 2F, (18 - size.Height) / 2F);
            }
            return bitmap;
        }

        void OpenThemeSettings()
        {
            using (var dialog = new ThemePickerForm(settings.ThemeName, settings.Language ?? "pt", themes))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                settings.ThemeName = dialog.SelectedTheme;
                settings.Language = dialog.SelectedLanguage;
                I18n.Language = settings.Language;
                SaveSettings();
                ApplyLanguage();
                ApplyTheme();
            }
        }

        void CaptureLocalizableControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if ((control is Label || control is GroupBox || control is TabPage) && !string.IsNullOrWhiteSpace(control.Text)) localizationKeys[control] = control.Text;
                CaptureLocalizableControls(control);
            }
        }

        void ApplyLanguage()
        {
            Text = I18n.T("Sentinela Firmware Studio");
            foreach (var pair in localizationKeys) pair.Key.Text = I18n.T(pair.Value);
            foreach (var pair in buttonKeys)
            {
                pair.Key.Text = I18n.T(pair.Value);
                buttonTooltips.SetToolTip(pair.Key, I18n.T(GetButtonTooltip(pair.Value)));
            }
            foreach (var pair in tooltipKeys) buttonTooltips.SetToolTip(pair.Key, TranslateTooltipKey(pair.Value));
            statusLabel.Text = I18n.T("Pronto");
        }

        void SetLocalizedTooltip(Control control, string portugueseKey)
        {
            tooltipKeys[control] = portugueseKey;
            buttonTooltips.SetToolTip(control, TranslateTooltipKey(portugueseKey));
        }

        string TranslateTooltipKey(string key)
        {
            if (key.StartsWith("Clique para entender: ")) return I18n.T("Clique para entender: ") + I18n.T(key.Substring("Clique para entender: ".Length));
            return I18n.T(key);
        }

        string L(string portuguese, string english, string spanish)
        {
            return I18n.Language == "en" ? english : I18n.Language == "es" ? spanish : portuguese;
        }

        void ApplyTheme()
        {
            Color accent;
            if (!themes.TryGetValue(settings.ThemeName ?? "Azul", out accent)) accent = themes["Azul"];
            currentAccent = accent;
            BackColor = BlendWithWhite(accent, 0.94F);
            if (configurationTab != null) configurationTab.BackColor = BackColor;
            if (compilationTab != null) compilationTab.BackColor = BackColor;
            ApplyThemeToControls(this, accent);
            if (themeToolbar != null)
            {
                themeToolbar.BackColor = accent;
                foreach (Control control in themeToolbar.Controls)
                {
                    var label = control as Label;
                    if (label != null) label.ForeColor = Color.White;
                }
            }
            logBox.BackColor = Color.FromArgb(28, 31, 36);
            logBox.ForeColor = Color.Gainsboro;
            statusLabel.ForeColor = accent;
            projects.Invalidate();
            ApplyWindowsTitleBar(accent);
        }

        void ApplyThemeToControls(Control parent, Color accent)
        {
            foreach (Control control in parent.Controls)
            {
                var button = control as Button;
                if (button != null)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = accent;
                    button.BackColor = accent;
                    button.ForeColor = Color.White;
                    string glyphKey = buttonKeys.ContainsKey(button) ? buttonKeys[button] : button.Text;
                    button.Image = string.Equals(button.Tag as string, "help", StringComparison.Ordinal) ? null : CreateButtonIcon(GetButtonGlyph(glyphKey), Color.White);
                    button.FlatAppearance.MouseOverBackColor = BlendWithWhite(accent, 0.18F);
                    button.FlatAppearance.MouseDownBackColor = BlendWithWhite(accent, 0.32F);
                }
                ApplyThemeToControls(control, accent);
            }
        }

        Color BlendWithWhite(Color color, float whiteAmount)
        {
            int red = (int)(color.R * (1F - whiteAmount) + 255F * whiteAmount);
            int green = (int)(color.G * (1F - whiteAmount) + 255F * whiteAmount);
            int blue = (int)(color.B * (1F - whiteAmount) + 255F * whiteAmount);
            return Color.FromArgb(red, green, blue);
        }

        void DrawProjectItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color background = selected ? currentAccent : projects.BackColor;
            Color foreground = selected ? Color.White : projects.ForeColor;
            using (var backgroundBrush = new SolidBrush(background)) e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            var textBounds = new Rectangle(e.Bounds.Left + 4, e.Bounds.Top, Math.Max(0, e.Bounds.Width - 8), e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, projects.Items[e.Index].ToString(), projects.Font, textBounds, foreground,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.DrawFocusRectangle();
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        void ApplyWindowsTitleBar(Color accent)
        {
            try
            {
                int captionColor = accent.R | (accent.G << 8) | (accent.B << 16);
                int textColor = 255 | (255 << 8) | (255 << 16);
                DwmSetWindowAttribute(Handle, 35, ref captionColor, sizeof(int));
                DwmSetWindowAttribute(Handle, 36, ref textColor, sizeof(int));
            }
            catch { }
        }

        Control FieldRow(string label, Control input, Button action)
        {
            var row = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = action == null ? 2 : 3, Height = 35 };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            if (action != null) row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(3, 4, 3, 4);
            row.Controls.Add(input, 1, 0);
            if (action != null) row.Controls.Add(action, 2, 0);
            return row;
        }

        string DefaultPlatformIo()
        {
            string detected = FindPlatformIo();
            return detected ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".platformio", "penv", "Scripts", "platformio.exe");
        }

        string FindPlatformIo()
        {
            var candidates = new List<string>();
            candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".platformio", "penv", "Scripts", "platformio.exe"));
            string pathValue = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (string folder in pathValue.Split(Path.PathSeparator))
                if (!string.IsNullOrWhiteSpace(folder)) candidates.Add(Path.Combine(folder.Trim(), "platformio.exe"));
            return candidates.FirstOrDefault(File.Exists);
        }

        void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    using (var stream = File.OpenRead(SettingsFile))
                        settings = (AppSettings)new XmlSerializer(typeof(AppSettings)).Deserialize(stream);
                }
            }
            catch (Exception ex) { MessageBox.Show("Não foi possível carregar os projetos: " + ex.Message); }
            if (settings.Projects == null) settings.Projects = new List<FirmwareProject>();
            if (settings.BuildHistory == null) settings.BuildHistory = new List<BuildHistoryEntry>();
            string detectedPlatformIo = FindPlatformIo();
            if (detectedPlatformIo != null)
                foreach (FirmwareProject project in settings.Projects)
                    if (string.IsNullOrWhiteSpace(project.PlatformIoPath) || !File.Exists(project.PlatformIoPath)) project.PlatformIoPath = detectedPlatformIo;
            RefreshProjectList();
        }

        void SaveSettings()
        {
            Directory.CreateDirectory(settingsFolder);
            using (var stream = File.Create(SettingsFile))
                new XmlSerializer(typeof(AppSettings)).Serialize(stream, settings);
        }

        void RefreshProjectList()
        {
            projects.DataSource = null;
            projects.DataSource = settings.Projects;
            if (projects.Items.Count > 0 && projects.SelectedIndex < 0) projects.SelectedIndex = 0;
        }

        FirmwareProject SelectedProject { get { return projects.SelectedItem as FirmwareProject; } }

        void DisplaySelectedProject()
        {
            var project = SelectedProject;
            ResetProjectView();
            SetConfigurationEditMode(false);
            if (project == null)
            {
                nameBox.Clear();
                folderBox.Clear();
                platformIoBox.Clear();
                environmentBox.Text = "";
                return;
            }
            nameBox.Text = project.Name;
            folderBox.Text = project.Folder;
            platformIoBox.Text = project.PlatformIoPath;
            environmentBox.Text = string.IsNullOrWhiteSpace(project.Environment) ? "esp32dev" : project.Environment;
            DetectEnvironments(false);
            RefreshArtifacts();
            statusLabel.Text = L("Projeto selecionado: ", "Selected project: ", "Proyecto seleccionado: ") + project.Name;
        }

        void BeginConfigurationEdit()
        {
            if (SelectedProject == null) return;
            mainTabs.SelectedTab = configurationTab;
            creatingManualProject = false;
            SetConfigurationEditMode(true);
            nameBox.Focus();
            nameBox.SelectAll();
            statusLabel.Text = L("Editando configuração — salve ou cancele para continuar", "Editing settings — save or cancel to continue", "Editando configuración — guarde o cancele para continuar");
        }

        void BeginManualProjectCreation()
        {
            mainTabs.SelectedTab = configurationTab;
            projectSelectedBeforeManualCreation = SelectedProject;
            projects.SelectedIndex = -1;
            creatingManualProject = true;
            ResetProjectView();
            nameBox.Clear();
            folderBox.Clear();
            platformIoBox.Text = DefaultPlatformIo();
            environmentBox.Items.Clear();
            environmentBox.Text = "";
            singleEnvironmentDetected = false;
            SetConfigurationEditMode(true);
            nameBox.Focus();
            statusLabel.Text = L("Novo projeto manual — preencha os campos e salve ou cancele", "New manual project — complete the fields and save or cancel", "Nuevo proyecto manual — complete los campos y guarde o cancele");
        }

        void CancelConfigurationEdit()
        {
            if (creatingManualProject)
            {
                FirmwareProject previous = projectSelectedBeforeManualCreation;
                creatingManualProject = false;
                projectSelectedBeforeManualCreation = null;
                RefreshProjectList();
                if (previous != null) projects.SelectedItem = previous;
                else DisplaySelectedProject();
            }
            else DisplaySelectedProject();
            statusLabel.Text = L("Edição cancelada; valores salvos restaurados", "Editing canceled; saved values restored", "Edición cancelada; valores guardados restaurados");
        }

        void SetConfigurationEditMode(bool editing)
        {
            bool hasProject = SelectedProject != null;
            bool hasEditableTarget = hasProject || creatingManualProject;
            editingConfiguration = editing && hasEditableTarget;
            nameBox.ReadOnly = !editingConfiguration;
            folderBox.ReadOnly = !editingConfiguration;
            platformIoBox.ReadOnly = !editingConfiguration;
            browseProjectButton.Enabled = editingConfiguration;
            browsePlatformIoButton.Enabled = editingConfiguration;
            editConfigButton.Visible = hasProject && !editingConfiguration;
            saveConfigButton.Visible = editingConfiguration;
            cancelEditButton.Visible = editingConfiguration;
            newManualButton.Enabled = !editingConfiguration;
            projects.Enabled = !editingConfiguration;
            addProjectButton.Enabled = !editingConfiguration;
            removeProjectButton.Enabled = hasProject && !editingConfiguration;
            buildButton.Enabled = hasProject && !editingConfiguration;
            cleanBuildButton.Enabled = hasProject && !editingConfiguration;
            environmentBox.Enabled = hasEditableTarget && !singleEnvironmentDetected;
            detectEnvironmentButton.Enabled = hasEditableTarget;
        }

        void ResetProjectView()
        {
            logBox.Clear();
            artifactBox.Items.Clear();
            hashLabel.Text = "SHA-256: —";
            statusLabel.ForeColor = themes.ContainsKey(settings.ThemeName ?? "") ? themes[settings.ThemeName] : themes["Azul"];
            statusLabel.Text = L("Selecione ou compile um projeto", "Select or build a project", "Seleccione o compile un proyecto");
        }

        void AddProject()
        {
            using (var dialog = new FolderBrowserDialog { Description = "Selecione a pasta que contém platformio.ini" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                if (!File.Exists(Path.Combine(dialog.SelectedPath, "platformio.ini")))
                {
                    MessageBox.Show(I18n.T("A pasta selecionada não contém platformio.ini."), I18n.T("Projeto inválido"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var project = new FirmwareProject
                {
                    Name = new DirectoryInfo(dialog.SelectedPath).Name,
                    Folder = dialog.SelectedPath,
                    Environment = "esp32dev",
                    PlatformIoPath = DefaultPlatformIo()
                };
                settings.Projects.Add(project);
                SaveSettings();
                RefreshProjectList();
                projects.SelectedItem = project;
                mainTabs.SelectedTab = configurationTab;
            }
        }

        void RemoveProject()
        {
            var project = SelectedProject;
            if (project == null) return;
            string removePrompt = L("Remover '" + project.Name + "' da lista?\n\nA pasta e seus arquivos não serão apagados.", "Remove '" + project.Name + "' from the list?\n\nThe project folder and files will not be deleted.", "¿Eliminar '" + project.Name + "' de la lista?\n\nLa carpeta y los archivos no se borrarán.");
            if (MessageBox.Show(removePrompt, I18n.T("Remover projeto"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            settings.Projects.Remove(project);
            SaveSettings();
            RefreshProjectList();
        }

        void SaveCurrentProject()
        {
            var project = SelectedProject;
            if (project == null && !creatingManualProject) return;
            if (string.IsNullOrWhiteSpace(nameBox.Text)) { MessageBox.Show(I18n.T("Informe um nome para o projeto.")); nameBox.Focus(); return; }
            if (!Directory.Exists(folderBox.Text.Trim())) { MessageBox.Show(I18n.T("A pasta do projeto não existe.")); folderBox.Focus(); return; }
            if (!File.Exists(Path.Combine(folderBox.Text.Trim(), "platformio.ini"))) { MessageBox.Show(I18n.T("A pasta selecionada não contém platformio.ini.")); folderBox.Focus(); return; }
            if (!File.Exists(platformIoBox.Text.Trim())) { MessageBox.Show(I18n.T("O executável do PlatformIO não foi encontrado.")); platformIoBox.Focus(); return; }
            if (string.IsNullOrWhiteSpace(environmentBox.Text)) { MessageBox.Show(I18n.T("Informe ou detecte um ambiente PlatformIO.")); environmentBox.Focus(); return; }
            if (creatingManualProject)
            {
                project = new FirmwareProject();
                settings.Projects.Add(project);
            }
            project.Name = nameBox.Text.Trim();
            project.Folder = folderBox.Text.Trim();
            project.PlatformIoPath = platformIoBox.Text.Trim();
            project.Environment = environmentBox.Text.Trim();
            creatingManualProject = false;
            projectSelectedBeforeManualCreation = null;
            SaveSettings();
            SetConfigurationEditMode(false);
            RefreshProjectList();
            projects.SelectedItem = project;
            statusLabel.Text = L("Configuração salva", "Settings saved", "Configuración guardada");
        }

        void BrowseProjectFolder()
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = folderBox.Text, Description = "Selecione a pasta do projeto" })
                if (dialog.ShowDialog(this) == DialogResult.OK) folderBox.Text = dialog.SelectedPath;
        }

        void BrowsePlatformIo()
        {
            using (var dialog = new OpenFileDialog { Filter = "PlatformIO (platformio.exe)|platformio.exe|Executáveis (*.exe)|*.exe", FileName = platformIoBox.Text })
                if (dialog.ShowDialog(this) == DialogResult.OK) platformIoBox.Text = dialog.FileName;
        }

        void DetectEnvironments() { DetectEnvironments(true); }
        void DetectEnvironments(bool notify)
        {
            string current = environmentBox.Text;
            string ini = Path.Combine(folderBox.Text, "platformio.ini");
            if (!File.Exists(ini)) { if (notify) MessageBox.Show(I18n.T("platformio.ini não encontrado.")); return; }
            var found = Regex.Matches(File.ReadAllText(ini), @"(?im)^\s*\[env:([^\]]+)\]");
            environmentBox.Items.Clear();
            foreach (Match match in found) environmentBox.Items.Add(match.Groups[1].Value.Trim());
            singleEnvironmentDetected = environmentBox.Items.Count == 1;
            if (singleEnvironmentDetected) environmentBox.SelectedIndex = 0;
            else if (!string.IsNullOrWhiteSpace(current) && environmentBox.Items.Contains(current)) environmentBox.Text = current;
            else if (environmentBox.Items.Count > 0) environmentBox.SelectedIndex = 0;
            environmentBox.DropDownStyle = ComboBoxStyle.DropDownList;
            environmentBox.Enabled = environmentBox.Items.Count > 1;
            if (notify) statusLabel.Text = environmentBox.Items.Count + L(" ambiente(s) detectado(s)", " environment(s) detected", " entorno(s) detectado(s)");
        }

        bool ValidateBuildInputs()
        {
            if (!Directory.Exists(folderBox.Text)) { MessageBox.Show(I18n.T("A pasta do projeto não existe.")); return false; }
            if (!File.Exists(Path.Combine(folderBox.Text, "platformio.ini"))) { MessageBox.Show(I18n.T("platformio.ini não encontrado.")); return false; }
            if (!File.Exists(platformIoBox.Text)) { MessageBox.Show(I18n.T("platformio.exe não encontrado.")); return false; }
            if (string.IsNullOrWhiteSpace(environmentBox.Text)) { MessageBox.Show(I18n.T("Informe o ambiente PlatformIO.")); return false; }
            return true;
        }

        void StartBuild(bool cleanFirst)
        {
            if (activeProcess != null) return;
            if (!ValidateBuildInputs()) return;
            mainTabs.SelectedTab = compilationTab;
            SaveCurrentProject();
            logBox.Clear();
            buildStopwatch.Restart();
            SetBusy(true);
            statusLabel.Text = cleanFirst ? L("Limpando projeto...", "Cleaning project...", "Limpiando proyecto...") : L("Compilando...", "Building...", "Compilando...");
            ShowBuildProgress(cleanFirst ? L("Limpando arquivos anteriores...", "Cleaning previous files...", "Limpiando archivos anteriores...") : L("Compilando o firmware...", "Building firmware...", "Compilando firmware..."));
            if (cleanFirst) RunPlatformIo("run -e \"" + environmentBox.Text + "\" -t clean", delegate(bool ok) {
                if (!ok) FinishBuild(false);
                else {
                    statusLabel.Text = L("Compilando...", "Building...", "Compilando...");
                    if (progressForm != null) progressForm.SetMessage(L("Limpeza concluída. Compilando o firmware...", "Clean completed. Building firmware...", "Limpieza completada. Compilando firmware..."));
                    RunPlatformIo("run -e \"" + environmentBox.Text + "\"", FinishBuild);
                }
            });
            else RunPlatformIo("run -e \"" + environmentBox.Text + "\"", FinishBuild);
        }

        void ShowBuildProgress(string message)
        {
            progressForm = new BuildProgressForm();
            progressForm.SetMessage(message);
            progressForm.Show(this);
            Enabled = false;
            progressForm.Enabled = true;
            progressForm.BringToFront();
        }

        void RunPlatformIo(string arguments, Action<bool> completed)
        {
            AppendLog("> \"" + platformIoBox.Text + "\" " + arguments + Environment.NewLine + Environment.NewLine);
            var info = new ProcessStartInfo
            {
                FileName = platformIoBox.Text,
                Arguments = arguments,
                WorkingDirectory = folderBox.Text,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            activeProcess = new Process { StartInfo = info, EnableRaisingEvents = true };
            activeProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { if (e.Data != null) AppendLog(e.Data + Environment.NewLine); };
            activeProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { if (e.Data != null) AppendLog(e.Data + Environment.NewLine); };
            activeProcess.Exited += delegate
            {
                int exitCode = activeProcess.ExitCode;
                activeProcess.Dispose();
                activeProcess = null;
                BeginInvoke(new Action(delegate { completed(exitCode == 0); }));
            };
            try
            {
                activeProcess.Start();
                activeProcess.BeginOutputReadLine();
                activeProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                AppendLog("ERRO: " + ex.Message + Environment.NewLine);
                activeProcess = null;
                completed(false);
            }
        }

        void FinishBuild(bool success)
        {
            buildStopwatch.Stop();
            Enabled = true;
            if (progressForm != null)
            {
                progressForm.Close();
                progressForm.Dispose();
                progressForm = null;
            }
            Activate();
            SetBusy(false);
            statusLabel.ForeColor = success ? Color.ForestGreen : Color.Firebrick;
            statusLabel.Text = success ? L("Compilação concluída com sucesso", "Build completed successfully", "Compilación completada con éxito") : I18n.T("Falha na compilação");
            RefreshArtifacts();
            string firmware = GetMainFirmwarePath();
            string sha = success && firmware != null && File.Exists(firmware) ? CalculateSha256(firmware) : "—";
            string size = success && firmware != null && File.Exists(firmware) ? FormatFileSize(new FileInfo(firmware).Length) : "—";
            var historyEntry = new BuildHistoryEntry
            {
                Project = SelectedProject == null ? nameBox.Text : SelectedProject.Name,
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Result = success ? "Sucesso" : "Falha",
                Duration = buildStopwatch.Elapsed.ToString(@"mm\:ss"),
                FirmwarePath = firmware ?? "",
                FirmwareSize = size,
                Sha256 = sha
            };
            if (settings.BuildHistory == null) settings.BuildHistory = new List<BuildHistoryEntry>();
            settings.BuildHistory.Insert(0, historyEntry);
            if (settings.BuildHistory.Count > 100) settings.BuildHistory.RemoveRange(100, settings.BuildHistory.Count - 100);
            SaveSettings();
            string summary = success
                ? L("Compilação concluída com sucesso.\n\nFirmware: ", "Build completed successfully.\n\nFirmware: ", "Compilación completada con éxito.\n\nFirmware: ") + (firmware == null ? L("não localizado", "not found", "no encontrado") : Path.GetFileName(firmware)) + L("\nTamanho: ", "\nSize: ", "\nTamaño: ") + size + "\nSHA-256: " + sha + L("\nDuração: ", "\nDuration: ", "\nDuración: ") + historyEntry.Duration + L("\nData: ", "\nDate: ", "\nFecha: ") + historyEntry.Date
                : L("A compilação falhou.\n\nDuração: ", "The build failed.\n\nDuration: ", "La compilación falló.\n\nDuración: ") + historyEntry.Duration + L("\nConsulte o log para identificar o erro.", "\nCheck the log to identify the error.", "\nConsulte el registro para identificar el error.");
            MessageBox.Show(summary, success ? I18n.T("Compilação concluída") : I18n.T("Falha na compilação"), MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) System.Media.SystemSounds.Asterisk.Play();
            else System.Media.SystemSounds.Hand.Play();
        }

        string GetMainFirmwarePath()
        {
            string candidate = Path.Combine(folderBox.Text, ".pio", "build", environmentBox.Text, "firmware.bin");
            return File.Exists(candidate) ? candidate : null;
        }

        string FormatFileSize(long bytes)
        {
            if (bytes >= 1024 * 1024) return (bytes / 1024D / 1024D).ToString("0.00") + " MB";
            if (bytes >= 1024) return (bytes / 1024D).ToString("0.0") + " KB";
            return bytes + " bytes";
        }

        void SetBusy(bool busy)
        {
            bool canBuild = !busy && !editingConfiguration && SelectedProject != null;
            buildButton.Enabled = canBuild;
            cleanBuildButton.Enabled = canBuild;
        }

        void AppendLog(string text)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string>(AppendLog), text); return; }
            logBox.AppendText(text);
            logBox.SelectionStart = logBox.TextLength;
            logBox.ScrollToCaret();
        }

        void RefreshArtifacts()
        {
            string previous = SelectedArtifact;
            artifactBox.Items.Clear();
            string build = Path.Combine(folderBox.Text, ".pio", "build");
            if (Directory.Exists(build))
            {
                string[] extensions = { ".bin", ".elf", ".map", ".hex" };
                var files = Directory.GetFiles(build, "*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderByDescending(f => Path.GetFileName(f).Equals("firmware.bin", StringComparison.OrdinalIgnoreCase))
                    .ThenByDescending(File.GetLastWriteTime)
                    .ToArray();
                foreach (string file in files)
                {
                    string relative = file.StartsWith(build, StringComparison.OrdinalIgnoreCase)
                        ? file.Substring(build.TrimEnd(Path.DirectorySeparatorChar).Length).TrimStart(Path.DirectorySeparatorChar)
                        : Path.GetFileName(file);
                    bool mainFirmware = Path.GetFileName(file).Equals("firmware.bin", StringComparison.OrdinalIgnoreCase);
                    artifactBox.Items.Add(new ArtifactItem { DisplayName = mainFirmware ? "★ " + relative + " — " + I18n.T("Firmware principal") : relative, FullPath = file });
                }
            }
            ArtifactItem previousItem = artifactBox.Items.Cast<ArtifactItem>().FirstOrDefault(item => string.Equals(item.FullPath, previous, StringComparison.OrdinalIgnoreCase));
            if (previousItem != null) artifactBox.SelectedItem = previousItem;
            else
            {
                ArtifactItem preferred = artifactBox.Items.Cast<ArtifactItem>().FirstOrDefault(item =>
                    Path.GetFileName(item.FullPath).Equals("firmware.bin", StringComparison.OrdinalIgnoreCase) &&
                    item.FullPath.IndexOf(Path.DirectorySeparatorChar + environmentBox.Text + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0);
                if (preferred != null) artifactBox.SelectedItem = preferred;
                else if (artifactBox.Items.Count > 0) artifactBox.SelectedIndex = 0;
            }
            int widest = artifactBox.Width;
            foreach (ArtifactItem item in artifactBox.Items)
                widest = Math.Max(widest, TextRenderer.MeasureText(item.DisplayName, artifactBox.Font).Width + 35);
            artifactBox.DropDownWidth = Math.Min(900, widest);
            UpdateHash();
        }

        string SelectedArtifact
        {
            get
            {
                ArtifactItem item = artifactBox.SelectedItem as ArtifactItem;
                return item == null ? null : item.FullPath;
            }
        }

        string CalculateSha256(string file)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(file))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        }

        void UpdateHash()
        {
            string file = SelectedArtifact;
            hashLabel.Text = file != null && File.Exists(file) ? "SHA-256: " + CalculateSha256(file) : "SHA-256: —";
            buttonTooltips.SetToolTip(artifactBox, file == null ? "Nenhum artefato selecionado." : file);
        }

        void CopyHash()
        {
            string file = SelectedArtifact;
            if (file == null || !File.Exists(file)) { MessageBox.Show(I18n.T("Selecione um artefato válido.")); return; }
            Clipboard.SetText(CalculateSha256(file));
            statusLabel.Text = L("SHA-256 copiado", "SHA-256 copied", "SHA-256 copiado");
        }

        void CopyArtifactToClipboard()
        {
            string file = SelectedArtifact;
            if (file == null || !File.Exists(file)) { MessageBox.Show(I18n.T("Selecione um artefato válido.")); return; }
            var list = new StringCollection();
            list.Add(file);
            Clipboard.SetFileDropList(list);
            statusLabel.Text = L("Arquivo copiado; cole-o no Explorador de Arquivos", "File copied; paste it in File Explorer", "Archivo copiado; péguelo en el Explorador de archivos");
        }

        void SaveSelectedArtifactAs()
        {
            string file = SelectedArtifact;
            if (file == null || !File.Exists(file)) { MessageBox.Show(I18n.T("Selecione um arquivo gerado válido.")); return; }
            string extension = Path.GetExtension(file);
            using (var dialog = new SaveFileDialog { FileName = Path.GetFileName(file), Filter = "Arquivo " + extension + "|*" + extension + "|Todos os arquivos|*.*" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                File.Copy(file, dialog.FileName, true);
                statusLabel.Text = L("Arquivo salvo em ", "File saved to ", "Archivo guardado en ") + dialog.FileName;
            }
        }

        void SaveFirmware()
        {
            string firmware = GetMainFirmwarePath();
            if (firmware == null)
            {
                MessageBox.Show(I18n.T("firmware.bin não encontrado. Compile o projeto primeiro."), I18n.T("Firmware indisponível"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var dialog = new SaveFileDialog { FileName = "firmware.bin", Filter = "Firmware ESP32 (*.bin)|*.bin" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                File.Copy(firmware, dialog.FileName, true);
                statusLabel.Text = L("Firmware salvo em ", "Firmware saved to ", "Firmware guardado en ") + dialog.FileName;
            }
        }

        void CreateFirmwarePackage()
        {
            string firmware = GetMainFirmwarePath();
            if (firmware == null)
            {
                MessageBox.Show(I18n.T("firmware.bin não encontrado. Compile o projeto primeiro."), I18n.T("Firmware indisponível"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string projectName = string.IsNullOrWhiteSpace(nameBox.Text) ? "sentinela" : Regex.Replace(nameBox.Text, @"[^A-Za-z0-9_-]", "-");
            using (var dialog = new SaveFileDialog { FileName = projectName + "-firmware-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".zip", Filter = "Pacote ZIP (*.zip)|*.zip" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                using (var fileStream = new FileStream(dialog.FileName, FileMode.Create))
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    var firmwareEntry = archive.CreateEntry("firmware.bin", CompressionLevel.Optimal);
                    using (var input = File.OpenRead(firmware))
                    using (var output = firmwareEntry.Open()) input.CopyTo(output);
                    string sha = CalculateSha256(firmware);
                    using (var writer = new StreamWriter(archive.CreateEntry("firmware.sha256").Open(), new UTF8Encoding(false))) writer.WriteLine(sha + "  firmware.bin");
                    using (var writer = new StreamWriter(archive.CreateEntry("informacoes.txt").Open(), new UTF8Encoding(true)))
                    {
                        writer.WriteLine(I18n.T("Projeto") + ": " + nameBox.Text);
                        writer.WriteLine(I18n.T("Ambiente") + ": " + environmentBox.Text);
                        writer.WriteLine(I18n.T("Data e hora") + ": " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        writer.WriteLine(I18n.T("Tamanho") + ": " + FormatFileSize(new FileInfo(firmware).Length));
                        writer.WriteLine("SHA-256: " + sha);
                        writer.WriteLine("Aplicativo: Sentinela Firmware Studio v" + AppVersion);
                    }
                }
                statusLabel.Text = L("Pacote ZIP criado em ", "ZIP package created at ", "Paquete ZIP creado en ") + dialog.FileName;
            }
        }

        void ShowBuildHistory()
        {
            var historyForm = new Form { Text = I18n.T("Histórico de compilações"), Width = 950, Height = 430, StartPosition = FormStartPosition.CenterParent, Icon = Icon };
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoGenerateColumns = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false };
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Project", HeaderText = I18n.T("Projeto"), MinimumWidth = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = I18n.T("Data e hora"), MinimumWidth = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Result", HeaderText = I18n.T("Resultado"), MinimumWidth = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Duration", HeaderText = I18n.T("Duração"), MinimumWidth = 75 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirmwareSize", HeaderText = I18n.T("Tamanho"), MinimumWidth = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirmwarePath", HeaderText = I18n.T("Caminho do firmware"), MinimumWidth = 260 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Sha256", HeaderText = "SHA-256", MinimumWidth = 300 });
            grid.DataSource = (settings.BuildHistory ?? new List<BuildHistoryEntry>()).ToList();
            grid.CellFormatting += delegate(object sender, DataGridViewCellFormattingEventArgs e) { if (e.ColumnIndex == 2 && e.Value != null) e.Value = I18n.T(e.Value.ToString()); };
            grid.BackgroundColor = Color.White;
            historyForm.Controls.Add(grid);
            historyForm.ShowDialog(this);
        }

        void CheckForUpdates(bool interactive)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    string json;
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.UserAgent] = "SentinelaFirmwareStudio/" + AppVersion;
                        client.Headers[HttpRequestHeader.Accept] = "application/vnd.github+json";
                        json = client.DownloadString(GitHubLatestReleaseApi);
                    }
                    Match match = Regex.Match(json, "\\\"tag_name\\\"\\s*:\\s*\\\"([^\\\"]+)\\\"");
                    if (!match.Success) throw new Exception("Resposta do GitHub sem versão.");
                    string latestText = match.Groups[1].Value.TrimStart('v', 'V');
                    Version latest;
                    Version current;
                    if (!Version.TryParse(latestText, out latest) || !Version.TryParse(AppVersion, out current)) throw new Exception("Versão inválida.");
                    BeginInvoke(new Action(delegate
                    {
                        if (latest > current)
                        {
                            string updatePrompt = L("Uma nova versão está disponível: v" + latest + "\n\nVersão instalada: v" + current + "\n\nDeseja abrir a página segura de releases do GitHub?", "A new version is available: v" + latest + "\n\nInstalled version: v" + current + "\n\nOpen the secure GitHub releases page?", "Hay una nueva versión disponible: v" + latest + "\n\nVersión instalada: v" + current + "\n\n¿Abrir la página segura de versiones de GitHub?");
                            if (MessageBox.Show(updatePrompt, I18n.T("Atualização disponível"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                Process.Start(GitHubReleasesPage);
                        }
                        else if (interactive) MessageBox.Show(L("Você já está usando a versão mais recente (v" + AppVersion + ").", "You are already using the latest version (v" + AppVersion + ").", "Ya está usando la versión más reciente (v" + AppVersion + ")."), I18n.T("Atualizações"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                catch (Exception ex)
                {
                    if (interactive) BeginInvoke(new Action(delegate { MessageBox.Show(L("Não foi possível verificar atualizações agora.\n\n", "Could not check for updates now.\n\n", "No se pudieron buscar actualizaciones ahora.\n\n") + ex.Message, I18n.T("Atualizações"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }));
                }
            });
        }

        void ExportScript(string extension)
        {
            if (!ValidateBuildInputs()) return;
            bool powershell = extension == ".ps1";
            string filter = powershell ? "Script PowerShell (*.ps1)|*.ps1" : "Script em lote (*.bat)|*.bat";
            using (var dialog = new SaveFileDialog { FileName = "compilar-" + environmentBox.Text + extension, Filter = filter })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                string contents;
                if (powershell)
                {
                    contents = "$ErrorActionPreference = 'Stop'\r\nSet-Location -LiteralPath '" + folderBox.Text.Replace("'", "''") + "'\r\n& '" + platformIoBox.Text.Replace("'", "''") + "' run -e '" + environmentBox.Text.Replace("'", "''") + "'\r\nif ($LASTEXITCODE -ne 0) { throw 'Falha na compilação.' }\r\nWrite-Host 'Compilação concluída.'\r\n";
                    File.WriteAllText(dialog.FileName, contents, new UTF8Encoding(true));
                }
                else
                {
                    contents = "@echo off\r\ncd /d \"" + folderBox.Text + "\"\r\n\"" + platformIoBox.Text + "\" run -e \"" + environmentBox.Text + "\"\r\nif errorlevel 1 pause & exit /b 1\r\necho.\r\necho Compilacao concluida.\r\npause\r\n";
                    File.WriteAllText(dialog.FileName, contents, Encoding.Default);
                }
                statusLabel.Text = "Script " + extension.ToUpperInvariant() + L(" exportado", " exported", " exportado");
            }
        }

        void OpenProjectFolder()
        {
            if (Directory.Exists(folderBox.Text)) Process.Start("explorer.exe", "\"" + folderBox.Text + "\"");
        }
    }
}
