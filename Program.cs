using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SentinelaFirmwareStudio
{
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
    public class AppSettings
    {
        public List<FirmwareProject> Projects { get; set; }
        public string ThemeName { get; set; }
        public AppSettings() { Projects = new List<FirmwareProject>(); ThemeName = "Azul"; }
    }

    public class BuildProgressForm : Form
    {
        Label messageLabel = new Label();

        public BuildProgressForm()
        {
            Text = "Compilando firmware";
            Width = 460;
            Height = 155;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ControlBox = false;
            ShowInTaskbar = false;

            messageLabel.Text = "Preparando compilação...";
            messageLabel.AutoSize = false;
            messageLabel.Dock = DockStyle.Top;
            messageLabel.Height = 52;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            var progress = new ProgressBar { Dock = DockStyle.Top, Height = 22, Style = ProgressBarStyle.Marquee, MarqueeAnimationSpeed = 25 };
            var note = new Label { Text = "Aguarde. Os controles serão liberados ao terminar.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            Controls.Add(note);
            Controls.Add(progress);
            Controls.Add(messageLabel);
        }

        public void SetMessage(string message) { messageLabel.Text = message; }
    }

    public class ThemePickerForm : Form
    {
        public string SelectedTheme { get; private set; }

        public ThemePickerForm(string currentTheme, Dictionary<string, Color> themes)
        {
            SelectedTheme = currentTheme;
            Text = "Configurações de aparência";
            Width = 430;
            Height = 210;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var title = new Label { Text = "Escolha a cor do tema", Dock = DockStyle.Top, Height = 45, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            var palette = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), FlowDirection = FlowDirection.LeftToRight };
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
                tooltip.SetToolTip(colorButton, name);
                colorButton.Click += delegate { SelectedTheme = name; DialogResult = DialogResult.OK; Close(); };
                palette.Controls.Add(colorButton);
            }

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
        ComboBox exportFormatBox = new ComboBox();
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
        bool creatingManualProject;
        FirmwareProject projectSelectedBeforeManualCreation;
        ToolTip buttonTooltips = new ToolTip { ShowAlways = true, InitialDelay = 350, ReshowDelay = 150, AutoPopDelay = 7000 };
        TableLayoutPanel themeToolbar;
        Color currentAccent = Color.FromArgb(0, 120, 215);
        Process activeProcess;
        BuildProgressForm progressForm;
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
            ApplyTheme();
        }

        void BuildUi()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(10) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 265));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            themeToolbar = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true, Padding = new Padding(8, 4, 4, 4), Margin = new Padding(0, 0, 0, 8) };
            themeToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            themeToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            themeToolbar.Controls.Add(new Label { Text = "SENTINELA FIRMWARE STUDIO", AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 3, 3), ForeColor = Color.White }, 0, 0);
            themeToolbar.Controls.Add(MakeButton("Configurações", delegate { OpenThemeSettings(); }), 1, 0);
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

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 9, ColumnCount = 1 };
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(right, 1, 1);

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
            right.Controls.Add(configurationActions, 0, 0);

            right.Controls.Add(FieldRow("Nome", nameBox, null), 0, 1);
            browseProjectButton = MakeButton("Procurar...", delegate { BrowseProjectFolder(); });
            browsePlatformIoButton = MakeButton("Procurar...", delegate { BrowsePlatformIo(); });
            right.Controls.Add(FieldRow("Pasta do projeto", folderBox, browseProjectButton), 0, 2);
            right.Controls.Add(FieldRow("PlatformIO", platformIoBox, browsePlatformIoButton), 0, 3);
            environmentBox.DropDownStyle = ComboBoxStyle.DropDown;
            detectEnvironmentButton = MakeButton("Detectar", delegate { DetectEnvironments(); });
            right.Controls.Add(FieldRow("Ambiente", environmentBox, detectEnvironmentButton), 0, 4);

            var artifactsGroup = new GroupBox { Text = "ARTEFATOS DA COMPILAÇÃO", Dock = DockStyle.Fill, Height = 132, Padding = new Padding(10, 7, 10, 7), Font = new Font(Font, FontStyle.Bold) };
            var artifactsLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 2 };
            artifactsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            artifactsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            artifactsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            artifactsLayout.Controls.Add(new Label { Text = "Artefato:", AutoSize = true, Anchor = AnchorStyles.Left, Font = Font, Margin = new Padding(3, 8, 3, 3) }, 0, 0);
            artifactBox.DropDownStyle = ComboBoxStyle.DropDownList;
            artifactBox.Dock = DockStyle.Fill;
            artifactBox.Font = Font;
            artifactBox.SelectedIndexChanged += delegate { UpdateHash(); };
            artifactsLayout.Controls.Add(artifactBox, 1, 0);

            var artifactButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 2, 0, 0), FlowDirection = FlowDirection.LeftToRight };
            artifactButtons.Controls.Add(MakeButton("Atualizar", delegate { RefreshArtifacts(); }));
            artifactButtons.Controls.Add(MakeButton("Copiar arquivo", delegate { CopyArtifactToClipboard(); }));
            artifactButtons.Controls.Add(MakeButton("Copiar SHA-256", delegate { CopyHash(); }));
            artifactsLayout.Controls.Add(artifactButtons, 1, 1);

            artifactsLayout.Controls.Add(new Label { Text = "Exportar:", AutoSize = true, Anchor = AnchorStyles.Left, Font = Font, Margin = new Padding(3, 8, 3, 3) }, 0, 2);
            var exportControls = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            exportFormatBox.Width = 175;
            exportFormatBox.DropDownStyle = ComboBoxStyle.DropDownList;
            exportFormatBox.Font = Font;
            exportFormatBox.Items.AddRange(new object[] {
                "Firmware (.bin)",
                "Diagnóstico (.elf)",
                "Mapa de memória (.map)",
                "Intel HEX (.hex)",
                "Script CMD (.bat)",
                "Script PowerShell (.ps1)"
            });
            exportFormatBox.SelectedIndex = 0;
            exportControls.Controls.Add(exportFormatBox);
            exportControls.Controls.Add(MakeButton("Exportar...", delegate { ExportSelectedFormat(); }));
            artifactsLayout.Controls.Add(exportControls, 1, 2);
            artifactsGroup.Controls.Add(artifactsLayout);
            right.Controls.Add(artifactsGroup, 0, 5);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 43, Padding = new Padding(0, 5, 0, 3) };
            buildButton = MakeButton("Compilar", delegate { StartBuild(false); });
            cleanBuildButton = MakeButton("Limpar e compilar", delegate { StartBuild(true); });
            actions.Controls.Add(buildButton);
            actions.Controls.Add(cleanBuildButton);
            actions.Controls.Add(MakeButton("Abrir pasta", delegate { OpenProjectFolder(); }));
            right.Controls.Add(actions, 0, 6);

            logBox.Dock = DockStyle.Fill;
            logBox.Multiline = true;
            logBox.ScrollBars = ScrollBars.Both;
            logBox.ReadOnly = true;
            logBox.WordWrap = false;
            logBox.Font = new Font("Consolas", 9F);
            logBox.BackColor = Color.FromArgb(28, 31, 36);
            logBox.ForeColor = Color.Gainsboro;
            right.Controls.Add(logBox, 0, 7);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            statusLabel.Text = "Pronto";
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font(Font, FontStyle.Bold);
            hashLabel.Text = "SHA-256: —";
            hashLabel.AutoSize = true;
            footer.Controls.Add(statusLabel, 0, 0);
            footer.Controls.Add(hashLabel, 0, 1);
            right.Controls.Add(footer, 0, 8);
            SetConfigurationEditMode(false);
        }

        Button MakeButton(string text, EventHandler click)
        {
            var button = new Button { Text = text, AutoSize = true, Height = 31, Margin = new Padding(3), Font = this.Font, FlatStyle = FlatStyle.Flat, ImageAlign = ContentAlignment.MiddleLeft, TextImageRelation = TextImageRelation.ImageBeforeText };
            button.Image = CreateButtonIcon(GetButtonGlyph(text));
            buttonTooltips.SetToolTip(button, GetButtonTooltip(text));
            button.Click += click;
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
            if (text.StartsWith("Exportar")) return "Salva no local escolhido o formato de firmware ou script selecionado.";
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
            using (var dialog = new ThemePickerForm(settings.ThemeName, themes))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                settings.ThemeName = dialog.SelectedTheme;
                SaveSettings();
                ApplyTheme();
            }
        }

        void ApplyTheme()
        {
            Color accent;
            if (!themes.TryGetValue(settings.ThemeName ?? "Azul", out accent)) accent = themes["Azul"];
            currentAccent = accent;
            BackColor = BlendWithWhite(accent, 0.94F);
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
                    button.Image = CreateButtonIcon(GetButtonGlyph(button.Text), Color.White);
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
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".platformio", "penv", "Scripts", "platformio.exe");
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
            statusLabel.Text = "Projeto selecionado: " + project.Name;
        }

        void BeginConfigurationEdit()
        {
            if (SelectedProject == null) return;
            creatingManualProject = false;
            SetConfigurationEditMode(true);
            nameBox.Focus();
            nameBox.SelectAll();
            statusLabel.Text = "Editando configuração — salve ou cancele para continuar";
        }

        void BeginManualProjectCreation()
        {
            projectSelectedBeforeManualCreation = SelectedProject;
            projects.SelectedIndex = -1;
            creatingManualProject = true;
            ResetProjectView();
            nameBox.Clear();
            folderBox.Clear();
            platformIoBox.Clear();
            environmentBox.Items.Clear();
            environmentBox.Text = "";
            SetConfigurationEditMode(true);
            nameBox.Focus();
            statusLabel.Text = "Novo projeto manual — preencha os campos e salve ou cancele";
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
            statusLabel.Text = "Edição cancelada; valores salvos restaurados";
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
            environmentBox.Enabled = hasEditableTarget;
            detectEnvironmentButton.Enabled = hasEditableTarget;
        }

        void ResetProjectView()
        {
            logBox.Clear();
            artifactBox.Items.Clear();
            hashLabel.Text = "SHA-256: —";
            statusLabel.ForeColor = themes.ContainsKey(settings.ThemeName ?? "") ? themes[settings.ThemeName] : themes["Azul"];
            statusLabel.Text = "Selecione ou compile um projeto";
        }

        void AddProject()
        {
            using (var dialog = new FolderBrowserDialog { Description = "Selecione a pasta que contém platformio.ini" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                if (!File.Exists(Path.Combine(dialog.SelectedPath, "platformio.ini")))
                {
                    MessageBox.Show("A pasta selecionada não contém platformio.ini.", "Projeto inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (MessageBox.Show("Remover '" + project.Name + "' da lista?\n\nA pasta e seus arquivos não serão apagados.", "Remover projeto", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            settings.Projects.Remove(project);
            SaveSettings();
            RefreshProjectList();
        }

        void SaveCurrentProject()
        {
            var project = SelectedProject;
            if (project == null && !creatingManualProject) return;
            if (string.IsNullOrWhiteSpace(nameBox.Text)) { MessageBox.Show("Informe um nome para o projeto."); nameBox.Focus(); return; }
            if (!Directory.Exists(folderBox.Text.Trim())) { MessageBox.Show("A pasta do projeto não existe."); folderBox.Focus(); return; }
            if (!File.Exists(Path.Combine(folderBox.Text.Trim(), "platformio.ini"))) { MessageBox.Show("A pasta selecionada não contém platformio.ini."); folderBox.Focus(); return; }
            if (!File.Exists(platformIoBox.Text.Trim())) { MessageBox.Show("O executável do PlatformIO não foi encontrado."); platformIoBox.Focus(); return; }
            if (string.IsNullOrWhiteSpace(environmentBox.Text)) { MessageBox.Show("Informe ou detecte um ambiente PlatformIO."); environmentBox.Focus(); return; }
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
            statusLabel.Text = "Configuração salva";
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
            if (!File.Exists(ini)) { if (notify) MessageBox.Show("platformio.ini não encontrado."); return; }
            var found = Regex.Matches(File.ReadAllText(ini), @"(?im)^\s*\[env:([^\]]+)\]");
            environmentBox.Items.Clear();
            foreach (Match match in found) environmentBox.Items.Add(match.Groups[1].Value.Trim());
            if (!string.IsNullOrWhiteSpace(current)) environmentBox.Text = current;
            else if (environmentBox.Items.Count > 0) environmentBox.SelectedIndex = 0;
            if (notify) statusLabel.Text = environmentBox.Items.Count + " ambiente(s) detectado(s)";
        }

        bool ValidateBuildInputs()
        {
            if (!Directory.Exists(folderBox.Text)) { MessageBox.Show("A pasta do projeto não existe."); return false; }
            if (!File.Exists(Path.Combine(folderBox.Text, "platformio.ini"))) { MessageBox.Show("platformio.ini não encontrado."); return false; }
            if (!File.Exists(platformIoBox.Text)) { MessageBox.Show("platformio.exe não encontrado."); return false; }
            if (string.IsNullOrWhiteSpace(environmentBox.Text)) { MessageBox.Show("Informe o ambiente PlatformIO."); return false; }
            return true;
        }

        void StartBuild(bool cleanFirst)
        {
            if (activeProcess != null) return;
            if (!ValidateBuildInputs()) return;
            SaveCurrentProject();
            logBox.Clear();
            SetBusy(true);
            statusLabel.Text = cleanFirst ? "Limpando projeto..." : "Compilando...";
            ShowBuildProgress(cleanFirst ? "Limpando arquivos anteriores..." : "Compilando o firmware...");
            if (cleanFirst) RunPlatformIo("run -e \"" + environmentBox.Text + "\" -t clean", delegate(bool ok) {
                if (!ok) FinishBuild(false);
                else {
                    statusLabel.Text = "Compilando...";
                    if (progressForm != null) progressForm.SetMessage("Limpeza concluída. Compilando o firmware...");
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
            statusLabel.Text = success ? "Compilação concluída com sucesso" : "Falha na compilação";
            RefreshArtifacts();
            if (success) System.Media.SystemSounds.Asterisk.Play();
            else System.Media.SystemSounds.Hand.Play();
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
                    .OrderByDescending(File.GetLastWriteTime)
                    .ToArray();
                foreach (string file in files)
                {
                    string relative = file.StartsWith(build, StringComparison.OrdinalIgnoreCase)
                        ? file.Substring(build.TrimEnd(Path.DirectorySeparatorChar).Length).TrimStart(Path.DirectorySeparatorChar)
                        : Path.GetFileName(file);
                    artifactBox.Items.Add(new ArtifactItem { DisplayName = relative, FullPath = file });
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
            if (file == null || !File.Exists(file)) { MessageBox.Show("Selecione um artefato válido."); return; }
            Clipboard.SetText(CalculateSha256(file));
            statusLabel.Text = "SHA-256 copiado";
        }

        void CopyArtifactToClipboard()
        {
            string file = SelectedArtifact;
            if (file == null || !File.Exists(file)) { MessageBox.Show("Selecione um artefato válido."); return; }
            var list = new StringCollection();
            list.Add(file);
            Clipboard.SetFileDropList(list);
            statusLabel.Text = "Arquivo copiado; cole-o no Explorador de Arquivos";
        }

        string FindArtifactByExtension(string extension)
        {
            string selected = SelectedArtifact;
            if (selected != null && Path.GetExtension(selected).Equals(extension, StringComparison.OrdinalIgnoreCase)) return selected;

            ArtifactItem found = artifactBox.Items.Cast<ArtifactItem>()
                .Where(item => Path.GetExtension(item.FullPath).Equals(extension, StringComparison.OrdinalIgnoreCase))
                .Where(item => item.FullPath.IndexOf(Path.DirectorySeparatorChar + environmentBox.Text + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(item => File.GetLastWriteTime(item.FullPath))
                .FirstOrDefault();
            return found == null ? null : found.FullPath;
        }

        void ExportSelectedFormat()
        {
            if (exportFormatBox.SelectedIndex < 0) { MessageBox.Show("Selecione um formato de exportação."); return; }

            if (exportFormatBox.SelectedIndex == 4) { ExportScript(".bat"); return; }
            if (exportFormatBox.SelectedIndex == 5) { ExportScript(".ps1"); return; }

            string[] extensions = { ".bin", ".elf", ".map", ".hex" };
            string extension = extensions[exportFormatBox.SelectedIndex];
            string file = FindArtifactByExtension(extension);
            if (file == null || !File.Exists(file))
            {
                MessageBox.Show("Nenhum arquivo " + extension + " foi encontrado para o ambiente '" + environmentBox.Text + "'.\n\nCompile o projeto e tente novamente.", "Artefato não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new SaveFileDialog { FileName = Path.GetFileName(file), Filter = "Arquivo " + extension + "|*" + extension + "|Todos os arquivos|*.*" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                File.Copy(file, dialog.FileName, true);
                statusLabel.Text = "Artefato salvo em " + dialog.FileName;
            }
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
                statusLabel.Text = "Script " + extension.ToUpperInvariant() + " exportado";
            }
        }

        void OpenProjectFolder()
        {
            if (Directory.Exists(folderBox.Text)) Process.Start("explorer.exe", "\"" + folderBox.Text + "\"");
        }
    }
}
