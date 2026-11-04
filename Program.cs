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
        static I18n() {
            English["Credencial indisponível. Informe-a novamente neste usuário do Windows."] = "Credential unavailable. Enter it again for this Windows user.";
            Spanish["Credencial indisponível. Informe-a novamente neste usuário do Windows."] = "Credencial no disponible. Introdúzcala de nuevo para este usuario de Windows.";
            English["URL HTTP/HTTPS inválida: não informe credenciais ou parâmetros."] = "Invalid HTTP/HTTPS URL: do not include credentials or parameters.";
            Spanish["URL HTTP/HTTPS inválida: não informe credenciais ou parâmetros."] = "URL HTTP/HTTPS inválida: no incluya credenciales ni parámetros.";
            English["URL muito longa."] = "URL is too long."; Spanish["URL muito longa."] = "La URL es demasiado larga.";
        }
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
            { "Adicionar projeto", "Add project" },
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
            { "Adicionar projeto", "Añadir proyecto" },
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
        public string IconPath { get; set; }
        public bool IsFavorite { get; set; }
        public string CompanyId { get; set; }
        public string ServerId { get; set; }
        public string WifiId { get; set; }
        public string WifiMode { get; set; }
        public bool InheritProfile { get; set; }
        public bool AdoptLegacy { get; set; }
        public string LastArtifactDirectory { get; set; }
        public string LastProfileFingerprint { get; set; }
        public string LastArtifactHash { get; set; }
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
        public string CompanyId { get; set; }
        public string Server { get; set; }
        public string ProfileFingerprint { get; set; }
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
        public List<CompanyProfile> Companies { get; set; }
        public List<ServerProfile> Servers { get; set; }
        public List<WifiProfile> WifiNetworks { get; set; }
        public List<BuildHistoryEntry> BuildHistory { get; set; }
        public string ThemeName { get; set; }
        public string Language { get; set; }
        public bool SidebarCollapsed { get; set; }
        public DateTime UpdateDeferredUntilUtc { get; set; }
        public AppSettings() { Companies = new List<CompanyProfile>(); Servers = new List<ServerProfile>(); WifiNetworks = new List<WifiProfile>(); Projects = new List<FirmwareProject>(); BuildHistory = new List<BuildHistoryEntry>(); ThemeName = "Azul"; Language = "pt"; }
    }

    public sealed class GradientPanel : Panel
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public GradientPanel() { DoubleBuffered = true; ResizeRedraw = true; }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(ClientRectangle, StartColor, EndColor, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }

    public sealed class ModernScrollBar : Control
    {
        int value, maximum, largeChange = 1, dragOffset; bool dragging;
        public bool Horizontal { get; set; }
        public Color AccentColor { get; set; }
        public int Maximum { get { return maximum; } set { maximum = Math.Max(0, value); Value = Math.Min(Value, maximum); Invalidate(); } }
        public int LargeChange { get { return largeChange; } set { largeChange = Math.Max(1, value); Invalidate(); } }
        public int Value { get { return value; } set { int next = Math.Max(0, Math.Min(Maximum, value)); if (this.value == next) return; this.value = next; Invalidate(); if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); } }
        public event EventHandler ValueChanged;
        public ModernScrollBar() { AccentColor = Color.FromArgb(0, 120, 215); Cursor = Cursors.Hand; SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true); }
        Rectangle Thumb()
        {
            int length = Horizontal ? Width : Height, cross = Horizontal ? Height : Width;
            int thumbLength = Maximum <= 0 ? length : Math.Max(24, (int)((double)length * LargeChange / (Maximum + LargeChange)));
            int available = Math.Max(0, length - thumbLength), position = Maximum <= 0 ? 0 : (int)((double)available * Value / Maximum);
            return Horizontal ? new Rectangle(position, 2, thumbLength, Math.Max(2, cross - 4)) : new Rectangle(2, position, Math.Max(2, cross - 4), thumbLength);
        }
        protected override void OnPaint(PaintEventArgs e) { e.Graphics.Clear(Color.FromArgb(4, 20, 36)); using (var b = new SolidBrush(Color.FromArgb(20, 65, 94))) e.Graphics.FillRectangle(b, ClientRectangle); using (var b = new SolidBrush(AccentColor)) e.Graphics.FillRectangle(b, Thumb()); }
        protected override void OnMouseDown(MouseEventArgs e) { Rectangle thumb = Thumb(); int point = Horizontal ? e.X : e.Y; if (thumb.Contains(e.Location)) { dragging = true; dragOffset = point - (Horizontal ? thumb.X : thumb.Y); Capture = true; } else SetPoint(point, (Horizontal ? thumb.Width : thumb.Height) / 2); base.OnMouseDown(e); }
        protected override void OnMouseMove(MouseEventArgs e) { if (dragging) SetPoint(Horizontal ? e.X : e.Y, dragOffset); base.OnMouseMove(e); }
        protected override void OnMouseUp(MouseEventArgs e) { dragging = false; Capture = false; base.OnMouseUp(e); }
        protected override void OnMouseWheel(MouseEventArgs e) { Value -= Math.Sign(e.Delta) * Math.Max(1, LargeChange / 3); base.OnMouseWheel(e); }
        void SetPoint(int point, int offset) { Rectangle thumb = Thumb(); int available = Math.Max(1, (Horizontal ? Width - thumb.Width : Height - thumb.Height)); Value = (int)Math.Round((double)Math.Max(0, Math.Min(available, point - offset)) * Maximum / available); }
    }

    public sealed class ModernTextHost : TableLayoutPanel
    {
        const int EmGetFirstVisibleLine = 0x00CE, EmLineScroll = 0x00B6;
        readonly TextBox textBox; readonly ModernScrollBar vertical = new ModernScrollBar(); readonly ModernScrollBar horizontal = new ModernScrollBar { Horizontal = true }; bool syncing; int horizontalPosition;
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        bool followTail = true;
        public bool FollowTail { get { return followTail; } set { followTail=value; } }
        public void SyncScrollBars(bool followEnd) { UpdateBars(followEnd); }
        public ModernTextHost(TextBox target, Color accent)
        {
            textBox = target; Dock = DockStyle.Fill; Margin = Padding.Empty; Padding = Padding.Empty; ColumnCount = 2; RowCount = 2; BackColor = Color.FromArgb(4, 20, 36);
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13)); RowStyles.Add(new RowStyle(SizeType.Percent, 100)); RowStyles.Add(new RowStyle(SizeType.Absolute, 13));
            textBox.BorderStyle = BorderStyle.None; Padding = new Padding(1);
            textBox.Dock = DockStyle.Fill; textBox.ScrollBars = ScrollBars.None; vertical.Dock = DockStyle.Fill; horizontal.Dock = DockStyle.Fill; vertical.AccentColor = horizontal.AccentColor = accent;
            Controls.Add(textBox, 0, 0); Controls.Add(vertical, 1, 0); Controls.Add(horizontal, 0, 1); Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(4, 20, 36) }, 1, 1);
            vertical.ValueChanged += delegate { if (syncing || !textBox.IsHandleCreated) return; int current = (int)SendMessage(textBox.Handle, EmGetFirstVisibleLine, IntPtr.Zero, IntPtr.Zero); int delta = vertical.Value - current; if (delta != 0) SendMessage(textBox.Handle, EmLineScroll, IntPtr.Zero, new IntPtr(delta)); };
            horizontal.ValueChanged += delegate { if (syncing || !textBox.IsHandleCreated) return; int delta = horizontal.Value - horizontalPosition; if (delta != 0) { SendMessage(textBox.Handle, EmLineScroll, new IntPtr(delta), IntPtr.Zero); horizontalPosition = horizontal.Value; } };
            textBox.MouseWheel += delegate(object sender, MouseEventArgs e) { vertical.Value -= Math.Sign(e.Delta) * 3; };
            textBox.TextChanged += delegate { UpdateBars(FollowTail); }; textBox.Resize += delegate { UpdateBars(false); }; Resize += delegate { UpdateBars(false); };
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (Width > 1 && Height > 1) using (var pen = new Pen(vertical.AccentColor)) e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
        void UpdateBars(bool followEnd)
        {
            if (!textBox.IsHandleCreated) return; syncing = true;
            try { int lines = Math.Max(1, textBox.Lines.Length), visible = Math.Max(1, textBox.ClientSize.Height / Math.Max(1, textBox.Font.Height)); vertical.Maximum = Math.Max(0, lines - visible); vertical.LargeChange = visible; vertical.Visible = vertical.Maximum > 0; ColumnStyles[1].Width = vertical.Visible ? 13 : 0; if (followEnd) { vertical.Value = vertical.Maximum; int current = (int)SendMessage(textBox.Handle, EmGetFirstVisibleLine, IntPtr.Zero, IntPtr.Zero); if (vertical.Maximum > current) SendMessage(textBox.Handle, EmLineScroll, IntPtr.Zero, new IntPtr(vertical.Maximum - current)); } else vertical.Value=Math.Min(vertical.Maximum,(int)SendMessage(textBox.Handle, EmGetFirstVisibleLine, IntPtr.Zero, IntPtr.Zero)); int longest = textBox.Lines.Length == 0 ? 0 : textBox.Lines.Max(x => x.Length); int visibleChars = Math.Max(1, textBox.ClientSize.Width / Math.Max(1, TextRenderer.MeasureText("M", textBox.Font).Width)); horizontal.Maximum = Math.Max(0, longest - visibleChars); horizontal.LargeChange = visibleChars; horizontal.Visible = horizontal.Maximum > 0; RowStyles[1].Height = horizontal.Visible ? 13 : 0; horizontalPosition = Math.Min(horizontalPosition, horizontal.Maximum); horizontal.Value = horizontalPosition; }
            finally { syncing = false; }
        }
    }

    public sealed class ModernGridHost : TableLayoutPanel
    {
        readonly DataGridView grid; readonly ModernScrollBar vertical = new ModernScrollBar(); readonly ModernScrollBar horizontal = new ModernScrollBar { Horizontal = true }; bool syncing;
        public ModernGridHost(DataGridView target, Color accent)
        {
            grid = target; Dock = DockStyle.Fill; Margin = Padding.Empty; Padding = Padding.Empty; ColumnCount = 2; RowCount = 2; BackColor = Color.FromArgb(9, 38, 57);
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13)); RowStyles.Add(new RowStyle(SizeType.Percent, 100)); RowStyles.Add(new RowStyle(SizeType.Absolute, 13));
            grid.Dock = DockStyle.Fill; grid.ScrollBars = ScrollBars.None; vertical.Dock = DockStyle.Fill; horizontal.Dock = DockStyle.Fill; vertical.AccentColor = horizontal.AccentColor = accent;
            Controls.Add(grid, 0, 0); Controls.Add(vertical, 1, 0); Controls.Add(horizontal, 0, 1); Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(4, 20, 36) }, 1, 1);
            vertical.ValueChanged += delegate { if (syncing || grid.RowCount == 0) return; try { grid.FirstDisplayedScrollingRowIndex = Math.Min(vertical.Value, Math.Max(0, grid.RowCount - 1)); } catch { } };
            horizontal.ValueChanged += delegate { if (syncing) return; try { grid.HorizontalScrollingOffset = horizontal.Value; } catch { } };
            MouseEventHandler wheel = delegate(object sender, MouseEventArgs e) { vertical.Value -= Math.Sign(e.Delta) * Math.Max(1, vertical.LargeChange / 3); }; grid.MouseWheel += wheel; MouseWheel += wheel;
            Action update = delegate { if (IsHandleCreated) BeginInvoke((MethodInvoker)UpdateBars); };
            grid.RowsAdded += delegate { update(); }; grid.RowsRemoved += delegate { update(); }; grid.DataBindingComplete += delegate { update(); }; grid.ColumnWidthChanged += delegate { update(); }; grid.Resize += delegate { update(); }; Resize += delegate { update(); };
        }
        void UpdateBars()
        {
            if (syncing || grid.IsDisposed) return; syncing = true;
            try { int shown = Math.Max(1, grid.DisplayedRowCount(false)); vertical.Maximum = Math.Max(0, grid.RowCount - shown); vertical.LargeChange = shown; if (grid.FirstDisplayedScrollingRowIndex >= 0) vertical.Value = Math.Min(vertical.Maximum, grid.FirstDisplayedScrollingRowIndex); int total = grid.Columns.Cast<DataGridViewColumn>().Where(x => x.Visible).Sum(x => x.Width); int viewport = Math.Max(1, grid.DisplayRectangle.Width); horizontal.Maximum = Math.Max(0, total - viewport); horizontal.LargeChange = viewport; horizontal.Value = Math.Min(horizontal.Maximum, grid.HorizontalScrollingOffset); vertical.Visible = vertical.Maximum > 0; horizontal.Visible = horizontal.Maximum > 0; ColumnStyles[1].Width = vertical.Visible ? 13 : 0; RowStyles[1].Height = horizontal.Visible ? 13 : 0; }
            finally { syncing = false; }
        }
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
            BackColor = Color.FromArgb(7, 30, 47);
            ForeColor = Color.White;

            messageLabel.Text = I18n.T("Preparando compilação...");
            messageLabel.AutoSize = false;
            messageLabel.Dock = DockStyle.Top;
            messageLabel.Height = 52;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            var progress = new ProgressBar { Dock = DockStyle.Top, Height = 22, Style = ProgressBarStyle.Marquee, MarqueeAnimationSpeed = 25 };
            var note = new Label { Text = I18n.T("Aguarde. Os controles serão liberados ao terminar."), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            note.ForeColor = Color.FromArgb(180, 210, 226);
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
            BackColor = Color.FromArgb(7, 30, 47);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            var title = new Label { Text = I18n.T("Escolha a cor do tema"), Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            var palette = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 76, Padding = new Padding(16, 4, 16, 4), FlowDirection = FlowDirection.LeftToRight };
            palette.BackColor = BackColor;
            var tooltip = new ToolTip { ShowAlways = true };

            foreach (var theme in themes)
            {
                string name = theme.Key;
                var colorButton = new ProfileButton { Width = 46, Height = 46, BackColor = theme.Value, FlatStyle = FlatStyle.Flat, Margin = new Padding(7), TabStop = false };
                colorButton.FlatAppearance.BorderSize = name == currentTheme ? 4 : 1;
                colorButton.FlatAppearance.BorderColor = name == currentTheme ? Color.White : Color.FromArgb(84, 116, 134);
                var path = new GraphicsPath();
                path.AddArc(1, 1, 10, 10, 180, 90); path.AddArc(33, 1, 10, 10, 270, 90); path.AddArc(33, 33, 10, 10, 0, 90); path.AddArc(1, 33, 10, 10, 90, 90); path.CloseFigure();
                colorButton.Region = new Region(path);
                tooltip.SetToolTip(colorButton, I18n.T(name));
                colorButton.Click += delegate { SelectedTheme = name; };
                palette.Controls.Add(colorButton);
            }

            var languagePanel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 55, ColumnCount = 2, Padding = new Padding(20, 8, 20, 4) };
            languagePanel.BackColor = BackColor;
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            languagePanel.Controls.Add(new Label { Text = I18n.T("Idioma da interface"), AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            var languageBox = new ProfileComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            languageBox.BackColor = Color.FromArgb(13, 48, 68); languageBox.ForeColor = Color.White;
            languageBox.Items.AddRange(new object[] { "Português", "English", "Español" });
            languageBox.SelectedIndex = currentLanguage == "en" ? 1 : currentLanguage == "es" ? 2 : 0;
            languageBox.SelectedIndexChanged += delegate { SelectedLanguage = languageBox.SelectedIndex == 1 ? "en" : languageBox.SelectedIndex == 2 ? "es" : "pt"; };
            languagePanel.Controls.Add(languageBox, 1, 0);
            var saveButton = new ProfileButton { Text = I18n.T("Salvar configuração"), AutoSize = true, Height = 32, Anchor = AnchorStyles.None };
            saveButton.FlatStyle = FlatStyle.Flat; saveButton.BackColor = Color.FromArgb(0, 120, 215); saveButton.ForeColor = Color.White; saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += delegate { DialogResult = DialogResult.OK; Close(); };
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            bottom.BackColor = BackColor;
            bottom.Controls.Add(saveButton);
            Controls.Add(bottom);
            Controls.Add(languagePanel);
            Controls.Add(palette);
            Controls.Add(title);
            foreach (Control control in Controls) if (control != saveButton) control.ForeColor = Color.White;
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

    public partial class MainForm : Form
    {
        const string AppVersion = "1.2.0";
        const string UpdateAssetName = "SentinelaFirmwareStudio.exe";
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
        ComboBox environmentBox = new ProfileComboBox();
        ComboBox artifactBox = new ProfileComboBox();
        TextBox logBox = new TextBox();
        Label statusLabel = new Label();
        Label hashLabel = new Label();
        Button buildButton = new ProfileButton();
        Button cleanBuildButton = new ProfileButton();
        Button addProjectButton = new ProfileButton();
        Button removeProjectButton = new ProfileButton();
        Button browseProjectButton = new ProfileButton();
        Button browsePlatformIoButton = new ProfileButton();
        Button detectEnvironmentButton = new ProfileButton();
        Button newManualButton = new ProfileButton();
        Button editConfigButton = new ProfileButton();
        Button saveConfigButton = new ProfileButton();
        Button cancelEditButton = new ProfileButton();
        bool editingConfiguration;
        bool singleEnvironmentDetected;
        bool creatingManualProject;
        FirmwareProject projectSelectedBeforeManualCreation;
        ToolTip buttonTooltips = new ToolTip { ShowAlways = true, InitialDelay = 350, ReshowDelay = 150, AutoPopDelay = 7000 };
        TableLayoutPanel themeToolbar;
        TableLayoutPanel rootLayout;
        Panel sidebarPanel;
        FlowLayoutPanel projectCards;
        Button sidebarToggleButton;
        Button sidebarAddButton;
        Label sidebarTitle;
        Label sidebarSubtitle;
        Label headerProjectTitle;
        Label headerProjectSubtitle;
        GradientPanel modernHeader;
        bool sidebarCollapsed;
        TabControl mainTabs;
        Panel configurationTab;
        Panel compilationTab;
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

        public MainForm(string profileStore = null)
        {
            Text = "Sentinela Firmware Studio";
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            Width = 1050;
            if (profileStore != null) settingsFolder = Path.GetFullPath(profileStore);
            Height = Math.Min(920, Screen.PrimaryScreen.WorkingArea.Height - 35);
            MinimumSize = new Size(1000, 760);
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
            rootLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty, Padding = Padding.Empty };
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(rootLayout);

            sidebarPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 12, 10, 10) };
            var sidebarLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty, Padding = Padding.Empty };
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            var brand = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            sidebarToggleButton = new ProfileButton { Width = 58, Height = 58, Left = 0, Top = 0, FlatStyle = FlatStyle.Flat, TabStop = false, Cursor = Cursors.Hand };
            sidebarToggleButton.FlatAppearance.BorderSize = 0;
            sidebarToggleButton.Image = CreateApplicationIcon(48);
            sidebarToggleButton.ImageAlign = ContentAlignment.MiddleCenter;
            sidebarToggleButton.Tag = "sidebar-toggle";
            sidebarToggleButton.Click += delegate { ToggleSidebar(); };
            sidebarTitle = new Label { Text = "Sentinela Firmware", AutoSize = true, Left = 68, Top = 8, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
            sidebarSubtitle = new Label { Text = "PROJETOS SALVOS", AutoSize = true, Left = 68, Top = 35, Font = new Font("Segoe UI", 7.5F) };
            brand.Controls.Add(sidebarToggleButton);
            brand.Controls.Add(sidebarTitle);
            brand.Controls.Add(sidebarSubtitle);
            sidebarLayout.Controls.Add(brand, 0, 0);

            projectCards = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Margin = Padding.Empty, Padding = new Padding(0, 4, 0, 4) };
            sidebarLayout.Controls.Add(projectCards, 0, 1);
            projects.Dock = DockStyle.Fill;
            projects.Visible = false;
            projects.DrawMode = DrawMode.OwnerDrawFixed;
            projects.ItemHeight = 22;
            projects.DrawItem += DrawProjectItem;
            projects.SelectedIndexChanged += delegate { DisplaySelectedProject(); };
            addProjectButton = MakeButton("Adicionar por pasta", delegate { ShowAddProjectMenu(); });
            removeProjectButton = MakeButton("Remover", delegate { RemoveProject(); });
            sidebarAddButton = addProjectButton;
            sidebarAddButton.Tag = "sidebar-add";
            sidebarAddButton.Dock = DockStyle.Fill;
            sidebarAddButton.AutoSize = false;
            sidebarAddButton.Margin = new Padding(0, 7, 0, 0);
            sidebarLayout.Controls.Add(sidebarAddButton, 0, 2);
            sidebarPanel.Controls.Add(sidebarLayout);
            sidebarPanel.Controls.Add(projects);
            rootLayout.Controls.Add(sidebarPanel, 0, 0);

            var mainShell = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(14, 12, 14, 12), Margin = Padding.Empty };
            mainShell.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            mainShell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rootLayout.Controls.Add(mainShell, 1, 0);

            modernHeader = new GradientPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10), Padding = new Padding(18, 8, 12, 8) };
            var headerLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, Margin = Padding.Empty, Padding = Padding.Empty };
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var titlePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            headerProjectTitle = new Label { Text = "SENTINELA FIRMWARE STUDIO", AutoSize = true, Left = 0, Top = 2, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent };
            headerProjectSubtitle = new Label { Text = "Selecione um projeto para começar", AutoSize = false, Dock = DockStyle.Bottom, Height = 23, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.WhiteSmoke, BackColor = Color.Transparent };
            titlePanel.Controls.Add(headerProjectTitle);
            titlePanel.Controls.Add(headerProjectSubtitle);
            headerLayout.Controls.Add(titlePanel, 0, 0);
            var headerActions = new FlowLayoutPanel { AutoSize = false, Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Margin = Padding.Empty, Padding = new Padding(0, 1, 0, 0) };
            headerActions.Controls.Add(new Label { Text = "v" + AppVersion, AutoSize = false, Width = 58, Height = 38, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White, BackColor = Color.Transparent, Font = new Font("Segoe UI Semibold", 9.5F), Margin = new Padding(0, 2, 6, 0) });
            var updateButton = MakeButton("Atualizações", delegate { CheckForUpdates(true); });
            var settingsButton = MakeButton("Configurações", delegate { OpenThemeSettings(); });
            updateButton.Text = I18n.T("Atualizações"); updateButton.Width = 128; updateButton.Height = 38; updateButton.MinimumSize = new Size(128, 38); updateButton.AutoSize = false; updateButton.Tag = "header-update"; updateButton.Image = CreateHeaderUpdateIcon(22); updateButton.ImageAlign = ContentAlignment.MiddleLeft; updateButton.TextAlign = ContentAlignment.MiddleCenter; updateButton.Padding = new Padding(7, 0, 4, 0); updateButton.Margin = new Padding(3, 2, 3, 0);
            settingsButton.Text = I18n.T("Configurações"); settingsButton.Width = 145; settingsButton.Height = 38; settingsButton.MinimumSize = new Size(145, 38); settingsButton.AutoSize = false; settingsButton.Tag = "header-settings"; settingsButton.Image = CreateHeaderSettingsIcon(22); settingsButton.ImageAlign = ContentAlignment.MiddleLeft; settingsButton.TextAlign = ContentAlignment.MiddleCenter; settingsButton.Padding = new Padding(7, 0, 4, 0); settingsButton.Margin = new Padding(3, 2, 3, 0);
            headerActions.Controls.Add(updateButton);
            headerActions.Controls.Add(settingsButton);
            headerLayout.Controls.Add(headerActions, 1, 0);
            modernHeader.Controls.Add(headerLayout);
            themeToolbar = headerLayout;
            mainShell.Controls.Add(modernHeader, 0, 0);

            var rightHost = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = Padding.Empty };
            rightHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightHost.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainShell.Controls.Add(rightHost, 0, 1);

            mainTabs = new TabControl { Visible = false };
            configurationTab = new Panel { Padding = new Padding(10), BackColor = BackColor };
            compilationTab = new Panel { Padding = new Padding(14), BackColor = BackColor, Dock = DockStyle.Fill, Margin = Padding.Empty, AutoScroll = true };
            rightHost.Controls.Add(compilationTab, 0, 0);

            var configurationLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 7, ColumnCount = 1 };
            for (int i = 0; i < 6; i++) configurationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            configurationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            configurationTab.Controls.Add(configurationLayout);

            var compilationLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, MinimumSize = new Size(0, 740), RowCount = 4, ColumnCount = 1 };
            compilationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            var artifactsGroup = new Panel { Dock = DockStyle.Fill, AutoSize = false, Height = 215, MinimumSize = new Size(0, 215), MaximumSize = new Size(0, 215), Padding = new Padding(12, 36, 12, 12), Margin = new Padding(0, 0, 0, 8) };
            artifactsGroup.Controls.Add(new Label { Text = "ARQUIVOS GERADOS E EXPORTAÇÃO", Dock = DockStyle.Top, Height = 30, Top = 0, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.White });
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
            artifactsLayout.BringToFront();
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
            compilationLayout.Controls.Add(CreateUsbUploadPanel(), 0, 2);

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
            compilationLayout.Controls.Add(CreateTerminalArea(), 0, 3);

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
            InitializeProfilesUi(rightHost, compilationLayout);
        }

        Button MakeButton(string text, EventHandler click)
        {
            var button = new ProfileButton { Text = text, AutoSize = true, MinimumSize = new Size(0, 32), Margin = new Padding(3), Font = this.Font, FlatStyle = FlatStyle.Flat, ImageAlign = ContentAlignment.MiddleLeft, TextImageRelation = TextImageRelation.ImageBeforeText };
            buttonKeys[button] = text;
            button.Image = CreateButtonIcon(GetButtonGlyph(text));
            buttonTooltips.SetToolTip(button, I18n.T(GetButtonTooltip(text)));
            button.Click += click;
            return button;
        }

        Button MakeHelpButton(string title, string explanation)
        {
            var button = new ProfileButton { Text = "?", Width = 28, Height = 28, MinimumSize = new Size(28, 28), Margin = new Padding(3), Font = new Font(this.Font, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Tag = "help" };
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

        Bitmap CreateGlyphBitmap(string glyph, int size, Color color)
        {
            var bitmap = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Segoe UI Symbol", size * .66F, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                graphics.DrawString(glyph, font, brush, new RectangleF(0, -1, size, size), format);
            }
            return bitmap;
        }

        Bitmap CreateHeaderUpdateIcon(int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            using (var pen = new Pen(Color.White, 2.2F))
            using (var brush = new SolidBrush(Color.White))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawArc(pen, 3, 3, size - 7, size - 7, 35, 285);
                g.FillPolygon(brush, new[] { new Point(size - 3, 4), new Point(size - 10, 4), new Point(size - 4, 10) });
            }
            return bitmap;
        }

        Bitmap CreateHeaderSettingsIcon(int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            using (var pen = new Pen(Color.White, 2F))
            using (var brush = new SolidBrush(Color.White))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int center = size / 2;
                for (int i = 0; i < 8; i++)
                {
                    double angle = Math.PI * i / 4D;
                    int x1 = center + (int)(Math.Cos(angle) * (size * .29)); int y1 = center + (int)(Math.Sin(angle) * (size * .29));
                    int x2 = center + (int)(Math.Cos(angle) * (size * .46)); int y2 = center + (int)(Math.Sin(angle) * (size * .46));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
                g.DrawEllipse(pen, 5, 5, size - 11, size - 11);
                g.FillEllipse(brush, center - 2, center - 2, 4, 4);
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
                if (string.Equals(pair.Key.Tag as string, "header-update")) pair.Key.Text = I18n.T("Atualizações");
                else if (string.Equals(pair.Key.Tag as string, "header-settings")) pair.Key.Text = I18n.T("Configurações");
                else pair.Key.Text = I18n.T(pair.Value);
                buttonTooltips.SetToolTip(pair.Key, I18n.T(GetButtonTooltip(pair.Value)));
            }
            foreach (var pair in tooltipKeys) buttonTooltips.SetToolTip(pair.Key, TranslateTooltipKey(pair.Value));
            statusLabel.Text = I18n.T("Pronto");
            ApplySidebarState();
            RefreshProfileLanguage();
            RefreshUsbLanguage();
            RefreshMonitorControls();
            RefreshCompilationWorkspace();
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
            var profileTabs = mainTabs as ProfileTabs;
            if (profileTabs != null) profileTabs.SetPalette(Color.FromArgb(9, 38, 57), Darken(accent, 0.35F), accent);
            BackColor = Color.FromArgb(5, 22, 38);
            if (configurationTab != null) configurationTab.BackColor = Color.FromArgb(9, 38, 57);
            if (compilationTab != null) compilationTab.BackColor = Color.FromArgb(9, 38, 57);
            ApplyThemeToControls(this, accent);
            if (modernHeader != null)
            {
                modernHeader.StartColor = Darken(accent, 0.42F);
                modernHeader.EndColor = accent;
                modernHeader.Invalidate();
            }
            if (sidebarPanel != null) sidebarPanel.BackColor = Color.FromArgb(16, 32, 47);
            if (projectCards != null) projectCards.BackColor = Color.FromArgb(16, 32, 47);
            if (sidebarTitle != null) sidebarTitle.ForeColor = Color.White;
            if (sidebarSubtitle != null) sidebarSubtitle.ForeColor = Color.FromArgb(151, 187, 207);
            logBox.BackColor = Color.FromArgb(2, 16, 27);
            logBox.ForeColor = Color.FromArgb(220, 235, 243);
            statusLabel.ForeColor = Color.FromArgb(102, 205, 255);
            hashLabel.ForeColor = Color.FromArgb(174, 205, 222);
            projects.Invalidate();
            if (mainTabs != null) mainTabs.Invalidate();
            RebuildProjectCards();
            RefreshCompilationWorkspace();
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
                    button.BackColor = Darken(accent, 0.18F);
                    button.ForeColor = Color.White;
                    string glyphKey = buttonKeys.ContainsKey(button) ? buttonKeys[button] : button.Text;
                    string buttonTag = button.Tag as string;
                    if (string.Equals(buttonTag, "sidebar-toggle", StringComparison.Ordinal)) button.Image = CreateApplicationIcon(48);
                    else if (string.Equals(buttonTag, "sidebar-add", StringComparison.Ordinal)) button.Image = sidebarCollapsed ? CreateGlyphBitmap("+", 28, Color.White) : CreateButtonIcon("+", Color.White);
                    else if (string.Equals(buttonTag, "header-update", StringComparison.Ordinal)) button.Image = CreateHeaderUpdateIcon(22);
                    else if (string.Equals(buttonTag, "header-settings", StringComparison.Ordinal)) button.Image = CreateHeaderSettingsIcon(22);
                    else button.Image = string.Equals(buttonTag, "help", StringComparison.Ordinal) ? null : CreateButtonIcon(GetButtonGlyph(glyphKey), Color.White);
                    button.FlatAppearance.MouseOverBackColor = BlendWithWhite(accent, 0.18F);
                    button.FlatAppearance.MouseDownBackColor = BlendWithWhite(accent, 0.32F);
                }
                else if (control is CheckBox)
                {
                    control.BackColor = Color.FromArgb(9, 38, 57);
                    control.ForeColor = Color.FromArgb(225, 237, 244);
                    var check=control as ProfileCheckBox;if(check!=null)check.OutlineColor=Darken(accent,.35F);
                    control.Invalidate();
                }
                else if (control is TextBox)
                {
                    var text = (TextBox)control;
                    text.BorderStyle = BorderStyle.None;
                    if (text != logBox && text != monitorBox) { text.BackColor = Color.FromArgb(13, 48, 68); text.ForeColor = Color.White; }
                }
                else if (control is ModernScrollBar)
                {
                    ((ModernScrollBar)control).AccentColor = accent; control.Invalidate();
                }
                else if (control is ComboBox)
                {
                    control.BackColor = Color.FromArgb(13, 48, 68); control.ForeColor = Color.White;
                    var combo=control as ReadableSerialCombo;if(combo!=null) { combo.OutlineColor=Darken(accent,.35F);combo.Invalidate(); }
                }
                else if (control is Label || control is GroupBox)
                {
                    if (control.Parent != modernHeader && !IsInsideHeader(control)) control.ForeColor = Color.FromArgb(225, 237, 244);
                    var profileGroup = control as ProfileGroupBox;
                    if (profileGroup != null) { profileGroup.BackColor = Color.FromArgb(9, 38, 57); profileGroup.FrameColor = Darken(accent, 0.35F); profileGroup.Invalidate(); }
                }
                else if (control is Panel || control is TableLayoutPanel || control is FlowLayoutPanel || control is TabPage)
                {
                    if (control != modernHeader && control != sidebarPanel && control != projectCards && !IsInsideHeader(control)) control.BackColor = Color.FromArgb(9, 38, 57);
                }
                ApplyThemeToControls(control, accent);
            }
        }

        bool IsInsideHeader(Control control)
        {
            while (control != null) { if (control == modernHeader) return true; control = control.Parent; }
            return false;
        }

        Color Darken(Color color, float amount)
        {
            return Color.FromArgb((int)(color.R * (1F - amount)), (int)(color.G * (1F - amount)), (int)(color.B * (1F - amount)));
        }

        void DrawModernTab(object sender, DrawItemEventArgs e)
        {
            bool selected = e.Index == mainTabs.SelectedIndex;
            Color background = selected ? currentAccent : Color.FromArgb(11, 42, 61);
            using (var brush = new SolidBrush(background)) e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, mainTabs.TabPages[e.Index].Text, Font, e.Bounds, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        Color BlendWithWhite(Color color, float whiteAmount)
        {
            int red = (int)(color.R * (1F - whiteAmount) + 255F * whiteAmount);
            int green = (int)(color.G * (1F - whiteAmount) + 255F * whiteAmount);
            int blue = (int)(color.B * (1F - whiteAmount) + 255F * whiteAmount);
            return Color.FromArgb(red, green, blue);
        }

        Bitmap CreateFirmwareIcon(int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var body = new SolidBrush(Color.FromArgb(12, 201, 170)))
                using (var path = RoundedRectangle(new Rectangle(2, 5, size - 5, size - 10), 6)) g.FillPath(body, path);
                using (var dark = new SolidBrush(Color.FromArgb(4, 42, 62))) g.FillRectangle(dark, size / 3, 9, size / 3, size - 18);
                using (var line = new Pen(Color.White, Math.Max(1, size / 14F)))
                {
                    g.DrawLine(line, 5, size / 2, size / 3, size / 2);
                    g.DrawLine(line, size * 2 / 3, size / 2, size - 5, size / 2);
                }
                using (var flash = new SolidBrush(Color.FromArgb(255, 210, 52)))
                {
                    Point[] points = { new Point(size / 2 + 1, 7), new Point(size / 2 - 4, size / 2), new Point(size / 2 + 1, size / 2), new Point(size / 2 - 2, size - 7), new Point(size / 2 + 7, size / 2 - 3), new Point(size / 2 + 2, size / 2 - 3) };
                    g.FillPolygon(flash, points);
                }
            }
            return bitmap;
        }

        Bitmap CreateApplicationIcon(int size)
        {
            try
            {
                using (Stream embedded = typeof(MainForm).Assembly.GetManifestResourceStream("SentinelaFirmwareStudio.AppIconPng"))
                {
                    if (embedded != null)
                    {
                        using (var original = Image.FromStream(embedded))
                        {
                            var rendered = new Bitmap(size, size);
                            using (Graphics graphics = Graphics.FromImage(rendered))
                            {
                                graphics.Clear(Color.Transparent);
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.DrawImage(original, new Rectangle(0, 0, size, size));
                            }
                            return rendered;
                        }
                    }
                }
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "app.ico");
                if (!File.Exists(iconPath)) iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                {
                    using (var source = new Icon(iconPath, size, size)) return source.ToBitmap();
                }
                if (Icon != null)
                {
                    using (var source = new Icon(Icon, size, size)) return source.ToBitmap();
                }
            }
            catch { }
            return CreateFirmwareIcon(size);
        }

        GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        void ToggleSidebar()
        {
            sidebarCollapsed = !sidebarCollapsed;
            settings.SidebarCollapsed = sidebarCollapsed;
            ApplySidebarState();
            SaveSettings();
        }

        void ApplySidebarState()
        {
            if (rootLayout == null) return;
            rootLayout.ColumnStyles[0].Width = sidebarCollapsed ? 76 : 270;
            sidebarTitle.Visible = !sidebarCollapsed;
            sidebarSubtitle.Visible = !sidebarCollapsed;
            sidebarToggleButton.Width = sidebarCollapsed ? 48 : 58;
            sidebarToggleButton.Height = sidebarCollapsed ? 48 : 58;
            sidebarToggleButton.Left = sidebarCollapsed ? 4 : 0;
            sidebarToggleButton.Top = sidebarCollapsed ? 4 : 0;
            sidebarToggleButton.Image = CreateApplicationIcon(sidebarCollapsed ? 38 : 48);
            sidebarAddButton.Text = sidebarCollapsed ? "" : I18n.T("Adicionar projeto");
            sidebarAddButton.Image = sidebarCollapsed ? CreateGlyphBitmap("+", 28, Color.White) : CreateButtonIcon("+", Color.White);
            sidebarAddButton.ImageAlign = sidebarCollapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
            sidebarAddButton.TextImageRelation = sidebarCollapsed ? TextImageRelation.Overlay : TextImageRelation.ImageBeforeText;
            sidebarAddButton.Padding = Padding.Empty;
            if (sidebarCollapsed) { sidebarAddButton.Dock = DockStyle.None; sidebarAddButton.Width = 48; sidebarAddButton.Height = 48; sidebarAddButton.MinimumSize = new Size(48, 48); sidebarAddButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom; sidebarAddButton.Font = Font; sidebarAddButton.Margin = Padding.Empty; }
            else { sidebarAddButton.Dock = DockStyle.Fill; sidebarAddButton.MinimumSize = new Size(0, 32); sidebarAddButton.Font = Font; sidebarAddButton.Margin = new Padding(0, 7, 0, 0); }
            RebuildProjectCards();
        }

        void RebuildProjectCards()
        {
            if (projectCards == null) return;
            projectCards.SuspendLayout();
            projectCards.Controls.Clear();
            FirmwareProject selectedProject = SelectedProject;
            foreach (FirmwareProject project in settings.Projects)
            {
                var card = new Panel { Width = sidebarCollapsed ? 50 : Math.Max(220, projectCards.ClientSize.Width - 22), Height = sidebarCollapsed ? 52 : 66, Margin = new Padding(0, 3, 0, 3), Cursor = Cursors.Hand, Tag = project };
                bool selected = object.ReferenceEquals(project, selectedProject);
                card.BackColor = selected ? currentAccent : Color.FromArgb(19, 42, 59);
                var icon = new PictureBox { Image = CreateProjectIcon(project, sidebarCollapsed ? 34 : 38), SizeMode = PictureBoxSizeMode.CenterImage, Width = sidebarCollapsed ? 50 : 48, Height = card.Height, Left = 0, Top = 0, BackColor = Color.Transparent, Tag = project };
                card.Controls.Add(icon);
                if (!sidebarCollapsed)
                {
                    var title = new Label { Text = (project.IsFavorite ? "★  " : "") + project.Name, AutoEllipsis = true, Left = 52, Top = 12, Width = Math.Max(90, card.Width - 86), Height = 20, ForeColor = Color.White, Font = new Font(Font, FontStyle.Bold), BackColor = Color.Transparent, Tag = project };
                    var sub = new Label { Text = string.IsNullOrWhiteSpace(project.Environment) ? "PlatformIO" : project.Environment, AutoEllipsis = true, Left = 52, Top = 35, Width = Math.Max(90, card.Width - 86), Height = 18, ForeColor = Color.FromArgb(176, 205, 222), BackColor = Color.Transparent, Tag = project };
                    var menu = new ProfileButton { Text = "•••", Width = 30, Height = 30, Left = card.Width - 34, Top = 17, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.Transparent, Tag = project, Cursor = Cursors.Hand };
                    menu.FlatAppearance.BorderSize = 0;
                    menu.Click += delegate(object sender, EventArgs e) { ShowProjectMenu((Button)sender, project); };
                    card.Controls.Add(title); card.Controls.Add(sub); card.Controls.Add(menu);
                    title.Click += ProjectCardClick; sub.Click += ProjectCardClick;
                }
                icon.Click += ProjectCardClick;
                card.Click += ProjectCardClick;
                buttonTooltips.SetToolTip(card, project.Name + " — " + project.Folder);
                projectCards.Controls.Add(card);
            }
            projectCards.ResumeLayout();
        }

        void ProjectCardClick(object sender, EventArgs e)
        {
            if (operationBusy) return;
            var control = sender as Control;
            var project = control == null ? null : control.Tag as FirmwareProject;
            if (project != null && !editingConfiguration) projects.SelectedItem = project;
        }

        Image CreateProjectIcon(FirmwareProject project, int size)
        {
            try
            {
                string path = project == null ? null : project.IconPath;
                if (!string.IsNullOrWhiteSpace(path) && path.StartsWith("glyph:", StringComparison.OrdinalIgnoreCase)) return CreateGlyphIcon(path.Substring(6), size);
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    string extension = Path.GetExtension(path).ToLowerInvariant();
                    if (extension == ".exe" || extension == ".ico" || extension == ".lnk")
                    {
                        using (Icon icon = Icon.ExtractAssociatedIcon(path)) if (icon != null) return new Bitmap(icon.ToBitmap(), new Size(size, size));
                    }
                    using (Image image = Image.FromFile(path)) return new Bitmap(image, new Size(size, size));
                }
            }
            catch { }
            return CreateFirmwareIcon(size);
        }

        Image CreateGlyphIcon(string glyph, int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias; g.Clear(Color.Transparent);
                using (var path = RoundedRectangle(new Rectangle(1, 1, size - 2, size - 2), Math.Max(5, size / 5)))
                using (var brush = new SolidBrush(currentAccent)) g.FillPath(brush, path);
                using (var font = new Font("Segoe UI Symbol", size * .46F, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(string.IsNullOrWhiteSpace(glyph) ? "⚡" : glyph, font, Brushes.White, new RectangleF(1, 0, size - 2, size - 2), format);
            }
            return bitmap;
        }

        void ShowProjectMenu(Button anchor, FirmwareProject project)
        {
            if (operationBusy) return;
            var menu = new ContextMenuStrip { ShowImageMargin = false, BackColor = Color.FromArgb(19, 42, 59), ForeColor = Color.White, Renderer = new ToolStripProfessionalRenderer(new DarkColorTable(currentAccent)) };
            menu.Items.Add(I18n.T("Editar configuração"), null, delegate { projects.SelectedItem = project; BeginConfigurationEdit(); });
            menu.Items.Add(I18n.T("Abrir pasta"), null, delegate { projects.SelectedItem = project; OpenProjectFolder(); });
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(project.IsFavorite ? L("Remover dos favoritos", "Remove from favorites", "Quitar de favoritos") : L("Favoritar", "Add to favorites", "Añadir a favoritos"), null, delegate { ToggleProjectFavorite(project); });
            ToolStripItem moveUp = menu.Items.Add(L("Mover para cima", "Move up", "Mover hacia arriba"), null, delegate { MoveProject(project, -1); });
            ToolStripItem moveDown = menu.Items.Add(L("Mover para baixo", "Move down", "Mover hacia abajo"), null, delegate { MoveProject(project, 1); });
            int projectIndex = settings.Projects.IndexOf(project);
            moveUp.Enabled = projectIndex > 0;
            moveDown.Enabled = projectIndex >= 0 && projectIndex < settings.Projects.Count - 1;
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(I18n.T("Remover"), null, delegate { projects.SelectedItem = project; RemoveProject(); });
            menu.Show(anchor, new Point(0, anchor.Height));
        }

        void ToggleProjectFavorite(FirmwareProject project)
        {
            if (project == null) return;
            project.IsFavorite = !project.IsFavorite;
            SaveSettings();
            RefreshProjectList();
            projects.SelectedItem = project;
            RebuildProjectCards();
        }

        void MoveProject(FirmwareProject project, int direction)
        {
            if (project == null || direction == 0) return;
            int currentIndex = settings.Projects.IndexOf(project);
            int targetIndex = currentIndex + direction;
            if (currentIndex < 0 || targetIndex < 0 || targetIndex >= settings.Projects.Count) return;
            settings.Projects.RemoveAt(currentIndex);
            settings.Projects.Insert(targetIndex, project);
            SaveSettings();
            RefreshProjectList();
            projects.SelectedItem = project;
            RebuildProjectCards();
        }

        void ShowAddProjectMenu()
        {
            var menu = new ContextMenuStrip { ShowImageMargin = false, BackColor = Color.FromArgb(19, 42, 59), ForeColor = Color.White, Renderer = new ToolStripProfessionalRenderer(new DarkColorTable(currentAccent)) };
            menu.Items.Add(L("Adicionar automaticamente por pasta", "Add automatically from folder", "Añadir automáticamente desde carpeta"), null, delegate { AddProject(); });
            menu.Items.Add(I18n.T("Novo manual"), null, delegate { BeginManualProjectCreation(); });
            menu.Show(sidebarAddButton, new Point(0, sidebarAddButton.Height));
        }

        sealed class DarkColorTable : ProfessionalColorTable
        {
            readonly Color accent;
            public DarkColorTable(Color accentColor) { accent = accentColor; UseSystemColors = false; }
            public override Color MenuItemSelected { get { return accent; } }
            public override Color MenuItemBorder { get { return accent; } }
            public override Color ToolStripDropDownBackground { get { return Color.FromArgb(19, 42, 59); } }
            public override Color ImageMarginGradientBegin { get { return Color.FromArgb(19, 42, 59); } }
            public override Color ImageMarginGradientMiddle { get { return Color.FromArgb(19, 42, 59); } }
            public override Color ImageMarginGradientEnd { get { return Color.FromArgb(19, 42, 59); } }
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
            RefreshProfileData();
            if (settings.Projects == null) settings.Projects = new List<FirmwareProject>();
            if (settings.BuildHistory == null) settings.BuildHistory = new List<BuildHistoryEntry>();
            sidebarCollapsed = settings.SidebarCollapsed;
            string detectedPlatformIo = FindPlatformIo();
            if (detectedPlatformIo != null)
                foreach (FirmwareProject project in settings.Projects)
                    if (string.IsNullOrWhiteSpace(project.PlatformIoPath) || !File.Exists(project.PlatformIoPath)) project.PlatformIoPath = detectedPlatformIo;
            RefreshProjectList();
            ApplySidebarState();
        }

        void SaveSettings()
        {
            Directory.CreateDirectory(settingsFolder);
            using (var stream = File.Create(SettingsFile))
                new XmlSerializer(typeof(AppSettings)).Serialize(stream, settings);
        }

        void RefreshProjectList()
        {
            FirmwareProject selected = SelectedProject;
            projects.DataSource = null;
            projects.DataSource = settings.Projects;
            if (selected != null && settings.Projects.Contains(selected)) projects.SelectedItem = selected;
            if (projects.Items.Count > 0 && projects.SelectedIndex < 0) projects.SelectedIndex = 0;
            RebuildProjectCards();
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
                if (headerProjectTitle != null) headerProjectTitle.Text = "SENTINELA FIRMWARE STUDIO";
                if (headerProjectSubtitle != null) headerProjectSubtitle.Text = L("Selecione um projeto para começar", "Select a project to begin", "Seleccione un proyecto para comenzar");
                RebuildProjectCards();
                return;
            }
            nameBox.Text = project.Name;
            folderBox.Text = project.Folder;
            platformIoBox.Text = project.PlatformIoPath;
            environmentBox.Text = string.IsNullOrWhiteSpace(project.Environment) ? "esp32dev" : project.Environment;
            DetectEnvironments(false);
            RefreshProfileData();
            RefreshArtifacts();
            statusLabel.Text = L("Projeto selecionado: ", "Selected project: ", "Proyecto seleccionado: ") + project.Name;
            if (headerProjectTitle != null) headerProjectTitle.Text = project.Name;
            if (headerProjectSubtitle != null) headerProjectSubtitle.Text = project.Environment + "  •  " + project.Folder;
            RebuildProjectCards();
        }

        void BeginConfigurationEdit()
        {
            if (SelectedProject == null) return;
            ShowProjectEditor(SelectedProject, false);
        }

        void BeginManualProjectCreation()
        {
            ShowProjectEditor(null, true);
        }

        void ShowProjectEditor(FirmwareProject source, bool createNew)
        {
            var dialog = new Form { Text = createNew ? I18n.T("Novo manual") : I18n.T("Editar configuração"), Width = 760, Height = 520, MinimumSize = new Size(680, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.FromArgb(7, 30, 47), ForeColor = Color.White, Font = Font, Icon = Icon };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(22), BackColor = dialog.BackColor };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            var title = new Label { Text = createNew ? L("NOVO PROJETO MANUAL", "NEW MANUAL PROJECT", "NUEVO PROYECTO MANUAL") : L("EDITAR PROJETO", "EDIT PROJECT", "EDITAR PROYECTO"), Dock = DockStyle.Fill, Font = new Font("Segoe UI Semibold", 15F), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft };
            root.Controls.Add(title, 0, 0);

            var fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 6, BackColor = Color.FromArgb(10, 38, 59), Padding = new Padding(18) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            for (int i = 0; i < 5; i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var n = new TextBox { Text = source == null ? "" : source.Name, Dock = DockStyle.Fill, Margin = new Padding(3, 10, 3, 10) };
            var f = new TextBox { Text = source == null ? "" : source.Folder, Dock = DockStyle.Fill, Margin = new Padding(3, 10, 3, 10) };
            var p = new TextBox { Text = source == null ? DefaultPlatformIo() : source.PlatformIoPath, Dock = DockStyle.Fill, Margin = new Padding(3, 10, 3, 10) };
            var env = new ProfileComboBox { Text = source == null ? "" : source.Environment, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, Margin = new Padding(3, 10, 3, 10) };
            string selectedIconPath = source == null ? "" : source.IconPath;
            var preview = new PictureBox { Width = 48, Height = 48, SizeMode = PictureBoxSizeMode.CenterImage, Image = CreateProjectIcon(source, 42), BackColor = Color.Transparent, Margin = new Padding(3) };
            Action<Control, int> addLabel = delegate(Control control, int row) { fields.Controls.Add(control, 0, row); };
            addLabel(new Label { Text = I18n.T("Nome"), AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Color.White }, 0); fields.Controls.Add(n, 1, 0); fields.SetColumnSpan(n, 2);
            addLabel(new Label { Text = I18n.T("Pasta do projeto"), AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Color.White }, 1); fields.Controls.Add(f, 1, 1);
            var browseFolder = MakeModernDialogButton(I18n.T("Procurar..."), 115); fields.Controls.Add(browseFolder, 2, 1);
            addLabel(new Label { Text = "PlatformIO", AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Color.White }, 2); fields.Controls.Add(p, 1, 2);
            var browsePio = MakeModernDialogButton(I18n.T("Procurar..."), 115); fields.Controls.Add(browsePio, 2, 2);
            addLabel(new Label { Text = I18n.T("Ambiente"), AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Color.White }, 3); fields.Controls.Add(env, 1, 3);
            var detect = MakeModernDialogButton(I18n.T("Detectar"), 115); fields.Controls.Add(detect, 2, 3);
            addLabel(new Label { Text = L("Ícone do projeto", "Project icon", "Icono del proyecto"), AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Color.White }, 4);
            var iconActions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, BackColor = Color.Transparent, Margin = Padding.Empty };
            iconActions.Controls.Add(preview);
            var chooseFile = MakeModernDialogButton(L("Imagem ou aplicativo", "Image or application", "Imagen o aplicación"), 170);
            var chooseSystem = MakeModernDialogButton(L("Ícones do sistema", "System icons", "Iconos del sistema"), 145);
            var resetIcon = MakeModernDialogButton(L("Usar padrão", "Use default", "Usar predeterminado"), 120);
            iconActions.Controls.Add(chooseFile); iconActions.Controls.Add(chooseSystem); iconActions.Controls.Add(resetIcon); fields.Controls.Add(iconActions, 1, 4); fields.SetColumnSpan(iconActions, 2);
            root.Controls.Add(fields, 0, 1);

            Action detectEnvironments = delegate
            {
                string ini = Path.Combine(f.Text.Trim(), "platformio.ini"); if (!File.Exists(ini)) { MessageBox.Show(dialog, I18n.T("platformio.ini não encontrado.")); return; }
                string old = env.Text; env.Items.Clear(); foreach (Match match in Regex.Matches(File.ReadAllText(ini), @"(?im)^\s*\[env:([^\]]+)\]")) env.Items.Add(match.Groups[1].Value.Trim());
                env.DropDownStyle = ComboBoxStyle.DropDownList; if (env.Items.Contains(old)) env.Text = old; else if (env.Items.Count > 0) env.SelectedIndex = 0;
            };
            browseFolder.Click += delegate { using (var picker = new FolderBrowserDialog { SelectedPath = f.Text, Description = I18n.T("Pasta do projeto") }) if (picker.ShowDialog(dialog) == DialogResult.OK) { f.Text = picker.SelectedPath; if (string.IsNullOrWhiteSpace(n.Text)) n.Text = new DirectoryInfo(f.Text).Name; detectEnvironments(); } };
            browsePio.Click += delegate { using (var picker = new OpenFileDialog { Filter = "PlatformIO (platformio.exe)|platformio.exe|Executáveis (*.exe)|*.exe", FileName = p.Text }) if (picker.ShowDialog(dialog) == DialogResult.OK) p.Text = picker.FileName; };
            detect.Click += delegate { detectEnvironments(); };
            chooseFile.Click += delegate { using (var picker = new OpenFileDialog { Filter = "Imagens e aplicativos|*.png;*.jpg;*.jpeg;*.bmp;*.ico;*.exe;*.lnk|Todos os arquivos|*.*" }) if (picker.ShowDialog(dialog) == DialogResult.OK) { selectedIconPath = picker.FileName; preview.Image = CreateProjectIcon(new FirmwareProject { IconPath = selectedIconPath }, 42); } };
            chooseSystem.Click += delegate { string glyph = ChooseSystemIcon(dialog); if (glyph != null) { selectedIconPath = "glyph:" + glyph; preview.Image = CreateGlyphIcon(glyph, 42); } };
            resetIcon.Click += delegate { selectedIconPath = ""; preview.Image = CreateFirmwareIcon(42); };

            var footer = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = dialog.BackColor, Padding = new Padding(0, 12, 0, 0) };
            var save = MakeModernDialogButton(I18n.T("Salvar configuração"), 155); var cancel = MakeModernDialogButton(L("Cancelar", "Cancel", "Cancelar"), 110); footer.Controls.Add(save); footer.Controls.Add(cancel); root.Controls.Add(footer, 0, 2);
            cancel.Click += delegate { dialog.DialogResult = DialogResult.Cancel; dialog.Close(); };
            save.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(n.Text)) { MessageBox.Show(dialog, I18n.T("Informe um nome para o projeto.")); return; }
                if (!Directory.Exists(f.Text.Trim())) { MessageBox.Show(dialog, I18n.T("A pasta do projeto não existe.")); return; }
                if (!File.Exists(Path.Combine(f.Text.Trim(), "platformio.ini"))) { MessageBox.Show(dialog, I18n.T("A pasta selecionada não contém platformio.ini.")); return; }
                if (!File.Exists(p.Text.Trim())) { MessageBox.Show(dialog, I18n.T("O executável do PlatformIO não foi encontrado.")); return; }
                if (string.IsNullOrWhiteSpace(env.Text)) { MessageBox.Show(dialog, I18n.T("Informe ou detecte um ambiente PlatformIO.")); return; }
                FirmwareProject target = source ?? new FirmwareProject(); target.Name = n.Text.Trim(); target.Folder = f.Text.Trim(); target.PlatformIoPath = p.Text.Trim(); target.Environment = env.Text.Trim(); target.IconPath = selectedIconPath;
                if (source == null) settings.Projects.Add(target); SaveSettings(); RefreshProjectList(); projects.SelectedItem = target; dialog.DialogResult = DialogResult.OK; dialog.Close();
            };
            dialog.Controls.Add(root);
            StyleModernDialog(dialog);
            dialog.ShowDialog(this);
        }

        Button MakeModernDialogButton(string text, int width)
        {
            var button = new ProfileButton { Text = text, Width = width, Height = 36, Margin = new Padding(4), BackColor = Color.FromArgb(20, 65, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI Semibold", 9F) };
            button.FlatAppearance.BorderSize = 0; button.FlatAppearance.MouseOverBackColor = currentAccent; return button;
        }

        void StyleModernDialog(Control root)
        {
            foreach (Control control in root.Controls)
            {
                var text = control as TextBox; if (text != null) { text.BackColor = Color.FromArgb(13, 48, 68); text.ForeColor = Color.White; text.BorderStyle = BorderStyle.None; }
                var combo = control as ComboBox; if (combo != null) { combo.BackColor = Color.FromArgb(13, 48, 68); combo.ForeColor = Color.White; combo.FlatStyle = FlatStyle.Flat; }
                StyleModernDialog(control);
            }
        }

        string ChooseSystemIcon(IWin32Window owner)
        {
            string selected = null; string[] glyphs = { "⚡", "⚙", "★", "◆", "●", "■", "⌂", "✦", "✓", "↻", "♟", "∞", "▶", "▣", "⧉", "⌕", "☁", "♜", "♥", "☀", "✉", "♫", "⚑", "✎" };
            using (var dialog = new Form { Text = L("Ícones do sistema", "System icons", "Iconos del sistema"), Width = 570, Height = 410, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = Color.FromArgb(7, 30, 47), ForeColor = Color.White, Font = Font, Icon = Icon })
            {
                var title = new Label { Text = L("Escolha um ícone", "Choose an icon", "Elija un icono"), Dock = DockStyle.Top, Height = 58, Padding = new Padding(18, 18, 0, 0), Font = new Font("Segoe UI Semibold", 13F), ForeColor = Color.White };
                var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 4, Padding = new Padding(18), BackColor = Color.FromArgb(10, 38, 59) };
                for (int i = 0; i < 6; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F)); for (int i = 0; i < 4; i++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                for (int i = 0; i < glyphs.Length; i++) { string glyph = glyphs[i]; var button = new ProfileButton { Dock = DockStyle.Fill, Margin = new Padding(7), Image = CreateGlyphIcon(glyph, 48), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 65, 94), Cursor = Cursors.Hand }; button.FlatAppearance.BorderSize = 0; button.Click += delegate { selected = glyph; dialog.DialogResult = DialogResult.OK; dialog.Close(); }; grid.Controls.Add(button, i % 6, i / 6); }
                dialog.Controls.Add(grid); dialog.Controls.Add(title); dialog.ShowDialog(owner);
            }
            return selected;
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
            if (projectCards != null) projectCards.Enabled = !editingConfiguration;
            addProjectButton.Enabled = !editingConfiguration;
            removeProjectButton.Enabled = hasProject && !editingConfiguration;
            buildButton.Enabled = hasProject && !editingConfiguration;
            cleanBuildButton.Enabled = hasProject && !editingConfiguration;
            UpdateUsbControls(activeProcess != null);
            environmentBox.Enabled = hasEditableTarget && !singleEnvironmentDetected;
            detectEnvironmentButton.Enabled = hasEditableTarget;
        }

        void ResetProjectView()
        {
            ClearBuildTerminal();
            artifactBox.Items.Clear();
            hashLabel.Text = "SHA-256: —";
            statusLabel.ForeColor = Color.FromArgb(174, 205, 222);
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
            if (!Regex.IsMatch(environmentBox.Text, "^[A-Za-z0-9_-]{1,80}$")) { MessageBox.Show(I18n.T("Informe ou detecte um ambiente PlatformIO.")); environmentBox.Focus(); return; }
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
            if (!Regex.IsMatch(environmentBox.Text, "^[A-Za-z0-9_-]{1,80}$")) { MessageBox.Show(I18n.T("Informe o ambiente PlatformIO.")); return false; }
            return true;
        }

        void StartBuild(bool cleanFirst) { StartBuild(cleanFirst, null); }

        async void StartBuild(bool cleanFirst, string uploadPort)
        {
            if (activeProcess != null || operationBusy) return;
            if (!ValidateBuildInputs()) return;
            if (uploadPort == null ? !ConfirmProfileBuild() : !ConfirmUsbUpload(uploadPort)) return;
            if (!PrepareProfileBuild()) return;
            usbUploadInProgress = uploadPort != null;
            usbUploadPort = uploadPort;
            ClearBuildTerminal();
            buildStopwatch.Restart();
            SetBusy(true);
            if (uploadPort != null) {
                try { await SuspendMonitorForUpload(uploadPort); }
                catch(Exception error) { AppendLog("USB: "+error.Message+Environment.NewLine);FlushTerminalViews();FinishBuild(false);return; }
            }
            statusLabel.Text = cleanFirst ? L("Limpando projeto...", "Cleaning project...", "Limpiando proyecto...") : L("Compilando...", "Building...", "Compilando...");
            ShowBuildProgress(cleanFirst ? L("Limpando arquivos anteriores...", "Cleaning previous files...", "Limpiando archivos anteriores...") : L("Compilando o firmware...", "Building firmware...", "Compilando firmware..."));
            if (usbUploadInProgress) {
                statusLabel.Text = L("Compilando e gravando USB...", "Building and flashing USB...", "Compilando y grabando USB...");
                if (progressForm != null) { progressForm.Text = L("Gravação USB", "USB flashing", "Grabación USB"); progressForm.SetMessage(L("Compilando e gravando no ESP. Não desconecte o cabo.", "Building and flashing the ESP. Do not disconnect the cable.", "Compilando y grabando el ESP. No desconecte el cable.")); }
                RunPlatformIo(UsbUpload.Arguments(environmentBox.Text, uploadPort), FinishBuild);
            }
            else if (cleanFirst) RunPlatformIo("run -e \"" + environmentBox.Text + "\" -t clean", delegate(bool ok) {
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
            SetTerminalOperation(message, true);
            ScrollToTerminals();
        }

        void RunPlatformIo(string arguments, Action<bool> completed)
        {
            CaptureTerminalSecrets();
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
            if (usbUploadInProgress) info.EnvironmentVariables["FIRMWARE_FACTORY_RESET"] = "0";
            if (activeProfileBuild) {
                info.EnvironmentVariables["FIRMWARE_PROFILE_FILE"] = activeProfileFile;
                info.EnvironmentVariables["PLATFORMIO_BUILD_DIR"] = Path.Combine(activeProfileBuildRoot, "build");
            }
            var runningProcess = new Process { StartInfo = info, EnableRaisingEvents = true };
            activeProcess = runningProcess;
            activeProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { if (e.Data != null) AppendLog(e.Data + Environment.NewLine); };
            activeProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { if (e.Data != null) AppendLog(e.Data + Environment.NewLine); };
            activeProcess.Exited += delegate
            {
                // Flush asynchronous output before releasing the profile or reporting success.
                runningProcess.WaitForExit();
                int exitCode = runningProcess.ExitCode;
                BeginInvoke(new Action(delegate {
                    activeProcess = null;
                    runningProcess.Dispose();
                    FlushTerminalViews();
                    completed(exitCode == 0);
                }));
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
                runningProcess.Dispose();
                activeProcess = null;
                completed(false);
            }
        }

        void FinishBuild(bool success)
        {
            try {
                if (activeProfileBuild && success && !File.Exists(Path.Combine(activeProfileBuildRoot, "build", environmentBox.Text, "firmware.bin"))) success = false;
                FinishProfileBuild(success);
            } catch (Exception error) { success = false; AppendLog("Falha ao finalizar artefatos: " + error.Message + Environment.NewLine); }
            buildStopwatch.Stop();
            Enabled = true;
            if (progressForm != null)
            {
                progressForm.Close();
                progressForm.Dispose();
                progressForm = null;
            }
            Activate();
            SetTerminalOperation("", false);
            SetBusy(false);
            statusLabel.ForeColor = success ? Color.FromArgb(103, 226, 172) : Color.FromArgb(255, 151, 151);
            statusLabel.Text = success ? L("Compilação concluída com sucesso", "Build completed successfully", "Compilación completada con éxito") : I18n.T("Falha na compilação");
            RefreshArtifacts();
            string firmware = GetMainFirmwarePath();
            string sha = success && firmware != null && File.Exists(firmware) ? CalculateSha256(firmware) : "—";
            string size = success && firmware != null && File.Exists(firmware) ? FormatFileSize(new FileInfo(firmware).Length) : "—";
            var historyEntry = new BuildHistoryEntry
            {
                Project = SelectedProject == null ? nameBox.Text : SelectedProject.Name,
                CompanyId = activeProfileBuild ? activeProfileCompany : "",
                Server = activeProfileBuild ? activeProfileServer : "",
                ProfileFingerprint = activeProfileBuild ? activeProfileFingerprint : "",
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
            if (usbUploadInProgress) {
                statusLabel.Text = success ? L("Gravação USB concluída", "USB flashing completed", "Grabación USB completada") : L("Falha na gravação USB", "USB flashing failed", "Error de grabación USB");
                summary = (success ? L("Firmware compilado e gravado no ESP com sucesso.", "Firmware built and flashed to the ESP successfully.", "Firmware compilado y grabado en el ESP correctamente.") : L("Não foi possível concluir a gravação USB. Confira o log, a porta e o cabo. Se ficar em Connecting, mantenha BOOT pressionado até iniciar a gravação.", "USB flashing could not be completed. Check the log, port and cable. If stuck at Connecting, hold BOOT until flashing starts.", "No se pudo completar la grabación USB. Revise el registro, el puerto y el cable. Si queda en Connecting, mantenga BOOT pulsado hasta iniciar la grabación.")) + "\n\n" + usbUploadPort + "\n\n" + summary;
            }
            string resultTitle = usbUploadInProgress ? statusLabel.Text : (success ? I18n.T("Compilação concluída") : I18n.T("Falha na compilação"));
            usbUploadInProgress = false; usbUploadPort = null;
            ResumeMonitorAfterUpload();
            MessageBox.Show(summary, resultTitle, MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) System.Media.SystemSounds.Asterisk.Play();
            else System.Media.SystemSounds.Hand.Play();
        }

        string GetMainFirmwarePath()
        {
            if (File.Exists(Path.Combine(folderBox.Text, "firmware-profile.json"))) { string directory = VerifiedProfileArtifacts(); return directory == null ? null : Path.Combine(directory, "firmware.bin"); }
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
            LockOperationControls(busy);
            bool canBuild = !busy && !editingConfiguration && SelectedProject != null;
            buildButton.Enabled = canBuild;
            cleanBuildButton.Enabled = canBuild;
            UpdateUsbControls(busy);
        }

        void AppendLog(string text)
        {
            buildHistoryBuffer.Append(text);
        }

        void RefreshArtifacts()
        {
            UpdateCompilationSummary();
            string previous = SelectedArtifact;
            artifactBox.Items.Clear();
            string build = !File.Exists(Path.Combine(folderBox.Text, "firmware-profile.json")) ? Path.Combine(folderBox.Text, ".pio", "build") : VerifiedProfileArtifacts();
            if (build == null) { UpdateHash(); return; }
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
            using (var dialog = new SaveFileDialog { FileName = File.Exists(Path.Combine(folderBox.Text, "firmware-profile.json")) ? "firmware-" + (inheritProfileBox.Checked ? "universal" : ((CompanyProfile)companyProfileBox.SelectedItem).Id) + "-" + CalculateSha256(firmware).Substring(0, 8) + ".bin" : "firmware.bin", Filter = "Firmware ESP32 (*.bin)|*.bin" })
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
                    string profileManifest = Path.Combine(Path.GetDirectoryName(firmware), "profile-manifest.json");
                    if (File.Exists(profileManifest)) {
                        using (var input = File.OpenRead(profileManifest))
                        using (var output = archive.CreateEntry("profile-manifest.json").Open()) input.CopyTo(output);
                    }
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
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CompanyId", HeaderText = L("Empresa (ID)", "Company (ID)", "Empresa (ID)"), MinimumWidth = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Server", HeaderText = L("Servidor", "Server", "Servidor"), MinimumWidth = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FirmwarePath", HeaderText = I18n.T("Caminho do firmware"), MinimumWidth = 260 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Sha256", HeaderText = "SHA-256", MinimumWidth = 300 });
            grid.DataSource = (settings.BuildHistory ?? new List<BuildHistoryEntry>()).ToList();
            grid.CellFormatting += delegate(object sender, DataGridViewCellFormattingEventArgs e) { if (e.ColumnIndex == 2 && e.Value != null) e.Value = I18n.T(e.Value.ToString()); };
            grid.BackgroundColor = Color.White;
            grid.BackgroundColor = Color.FromArgb(7, 30, 47);
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Darken(currentAccent, 0.35F);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Darken(currentAccent, 0.35F);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(9, 38, 57);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = currentAccent;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(12, 45, 65);
            grid.GridColor = Color.FromArgb(35, 75, 96);
            historyForm.BackColor = Color.FromArgb(7, 30, 47);
            historyForm.Controls.Add(new ModernGridHost(grid, currentAccent));
            historyForm.ShowDialog(this);
        }

        void CheckForUpdates(bool interactive)
        {
            if (!interactive && settings.UpdateDeferredUntilUtc > DateTime.UtcNow) return;
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
                    string assetUrl = null;
                    foreach (Match assetMatch in Regex.Matches(json, "\\\"browser_download_url\\\"\\s*:\\s*\\\"([^\\\"]+)\\\""))
                    {
                        string candidate = assetMatch.Groups[1].Value;
                        if (candidate.EndsWith("/" + UpdateAssetName, StringComparison.OrdinalIgnoreCase)) { assetUrl = candidate; break; }
                    }
                    Match digestMatch = Regex.Match(json, "\\\"digest\\\"\\s*:\\s*\\\"sha256:([0-9a-fA-F]{64})\\\"");
                    string expectedSha256 = digestMatch.Success ? digestMatch.Groups[1].Value : null;
                    BeginInvoke(new Action(delegate
                    {
                        if (latest > current)
                        {
                            string updatePrompt = L("Uma nova versão está disponível: v" + latest + "\n\nVersão instalada: v" + current + "\n\nDeseja baixar e instalar a atualização agora?\n\nSe escolher Não, perguntaremos novamente após 24 horas.", "A new version is available: v" + latest + "\n\nInstalled version: v" + current + "\n\nDownload and install the update now?\n\nIf you choose No, we will ask again after 24 hours.", "Hay una nueva versión disponible: v" + latest + "\n\nVersión instalada: v" + current + "\n\n¿Desea descargar e instalar la actualización ahora?\n\nSi elige No, volveremos a preguntar después de 24 horas.");
                            if (MessageBox.Show(updatePrompt, I18n.T("Atualização disponível"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                if (string.IsNullOrWhiteSpace(assetUrl)) ShowUpdateFailure(L("A Release mais recente não contém o arquivo " + UpdateAssetName + ".", "The latest Release does not contain " + UpdateAssetName + ".", "La versión más reciente no contiene " + UpdateAssetName + "."));
                                else DownloadAndInstallUpdate(assetUrl, latest, expectedSha256);
                            }
                            else
                            {
                                settings.UpdateDeferredUntilUtc = DateTime.UtcNow.AddHours(24);
                                SaveSettings();
                            }
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

        void DownloadAndInstallUpdate(string downloadUrl, Version latest, string expectedSha256)
        {
            string updateFolder = Path.Combine(settingsFolder, "Updates");
            Directory.CreateDirectory(updateFolder);
            string downloadedFile = Path.Combine(updateFolder, "SentinelaFirmwareStudio-v" + latest + ".exe");
            if (File.Exists(downloadedFile)) File.Delete(downloadedFile);

            var progressForm = new Form
            {
                Text = L("Atualizando o sistema", "Updating application", "Actualizando la aplicación"),
                Width = 500,
                Height = 165,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                StartPosition = FormStartPosition.CenterParent
            };
            var message = new Label { Text = L("Baixando a versão v" + latest + "...", "Downloading version v" + latest + "...", "Descargando la versión v" + latest + "..."), Dock = DockStyle.Top, Height = 55, TextAlign = ContentAlignment.MiddleCenter };
            var progress = new ProgressBar { Dock = DockStyle.Top, Height = 24, Margin = new Padding(18), Style = ProgressBarStyle.Continuous };
            var note = new Label { Text = L("O aplicativo será reiniciado automaticamente.", "The application will restart automatically.", "La aplicación se reiniciará automáticamente."), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            progressForm.Controls.Add(note);
            progressForm.Controls.Add(progress);
            progressForm.Controls.Add(message);

            var client = new WebClient();
            client.Headers[HttpRequestHeader.UserAgent] = "SentinelaFirmwareStudio/" + AppVersion;
            client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
            {
                progress.Value = Math.Max(0, Math.Min(100, e.ProgressPercentage));
                message.Text = L("Baixando a versão v" + latest + "... ", "Downloading version v" + latest + "... ", "Descargando la versión v" + latest + "... ") + e.ProgressPercentage + "%";
            };
            client.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                progressForm.Close();
                client.Dispose();
                if (e.Error != null || e.Cancelled)
                {
                    ShowUpdateFailure(e.Error == null ? L("O download foi cancelado.", "The download was cancelled.", "La descarga fue cancelada.") : e.Error.Message);
                    return;
                }
                try
                {
                    ValidateDownloadedUpdate(downloadedFile, expectedSha256);
                    StartUpdateHelper(downloadedFile);
                }
                catch (Exception ex) { ShowUpdateFailure(ex.Message); }
            };
            progressForm.Shown += delegate
            {
                try { client.DownloadFileAsync(new Uri(downloadUrl), downloadedFile); }
                catch (Exception ex) { progressForm.Close(); client.Dispose(); ShowUpdateFailure(ex.Message); }
            };
            progressForm.ShowDialog(this);
        }

        void ValidateDownloadedUpdate(string file, string expectedSha256)
        {
            if (!File.Exists(file) || new FileInfo(file).Length < 10240) throw new Exception(L("O arquivo de atualização recebido é inválido.", "The downloaded update file is invalid.", "El archivo de actualización descargado no es válido."));
            using (var stream = File.OpenRead(file))
            {
                if (stream.ReadByte() != 'M' || stream.ReadByte() != 'Z') throw new Exception(L("O arquivo baixado não é um executável Windows válido.", "The downloaded file is not a valid Windows executable.", "El archivo descargado no es un ejecutable de Windows válido."));
            }
            if (!string.IsNullOrWhiteSpace(expectedSha256) && !string.Equals(CalculateSha256(file), expectedSha256, StringComparison.OrdinalIgnoreCase))
                throw new Exception(L("A verificação SHA-256 da atualização falhou.", "The update SHA-256 verification failed.", "La verificación SHA-256 de la actualización falló."));
        }

        void StartUpdateHelper(string downloadedFile)
        {
            string script = Path.Combine(settingsFolder, "install-update-" + Guid.NewGuid().ToString("N") + ".cmd");
            string executable = Application.ExecutablePath;
            string contents = "@echo off\r\n" +
                "setlocal\r\n" +
                ":wait\r\n" +
                "tasklist /FI \"PID eq " + Process.GetCurrentProcess().Id + "\" 2>NUL | find \"" + Process.GetCurrentProcess().Id + "\" >NUL\r\n" +
                "if not errorlevel 1 (timeout /t 1 /nobreak >NUL & goto wait)\r\n" +
                "copy /Y \"" + downloadedFile + "\" \"" + executable + "\" >NUL\r\n" +
                "if errorlevel 1 (start \"\" \"" + GitHubReleasesPage + "\" & goto cleanup)\r\n" +
                "start \"\" \"" + executable + "\"\r\n" +
                ":cleanup\r\n" +
                "del /F /Q \"" + downloadedFile + "\" >NUL 2>NUL\r\n" +
                "del /F /Q \"%~f0\" >NUL 2>NUL\r\n";
            File.WriteAllText(script, contents, Encoding.Default);
            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("COMSPEC") ?? "cmd.exe",
                Arguments = "/c \"\"" + script + "\"\"",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            Application.Exit();
        }

        void ShowUpdateFailure(string details)
        {
            string text = L("Não foi possível instalar a atualização automaticamente.\n\n", "Could not install the update automatically.\n\n", "No se pudo instalar la actualización automáticamente.\n\n") + details + L("\n\nDeseja abrir a página da Release?", "\n\nOpen the Release page?", "\n\n¿Desea abrir la página de la versión?");
            if (MessageBox.Show(text, I18n.T("Atualizações"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) Process.Start(GitHubReleasesPage);
        }

        void ExportScript(string extension)
        {
            if (!ValidateBuildInputs()) return;
            if (ExportProtectedProfileScript(extension)) return;
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
