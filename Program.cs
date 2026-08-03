using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

    [Serializable]
    public class AppSettings
    {
        public List<FirmwareProject> Projects { get; set; }
        public AppSettings() { Projects = new List<FirmwareProject>(); }
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
        Process activeProcess;

        public MainForm()
        {
            Text = "Sentinela Firmware Studio";
            Width = 1050;
            Height = 700;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            BuildUi();
            LoadSettings();
        }

        void BuildUi()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 265));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(0, 0, 10, 0) };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.Controls.Add(new Label { Text = "PROJETOS", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 5, 3, 8) }, 0, 0);
            projects.Dock = DockStyle.Fill;
            projects.SelectedIndexChanged += delegate { DisplaySelectedProject(); };
            left.Controls.Add(projects, 0, 1);
            var projectButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            projectButtons.Controls.Add(MakeButton("Adicionar", delegate { AddProject(); }));
            projectButtons.Controls.Add(MakeButton("Remover", delegate { RemoveProject(); }));
            left.Controls.Add(projectButtons, 0, 2);
            root.Controls.Add(left, 0, 0);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 8, ColumnCount = 1 };
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(right, 1, 0);

            right.Controls.Add(FieldRow("Nome", nameBox, null), 0, 0);
            right.Controls.Add(FieldRow("Pasta do projeto", folderBox, MakeButton("Procurar...", delegate { BrowseProjectFolder(); })), 0, 1);
            right.Controls.Add(FieldRow("PlatformIO", platformIoBox, MakeButton("Procurar...", delegate { BrowsePlatformIo(); })), 0, 2);
            environmentBox.DropDownStyle = ComboBoxStyle.DropDown;
            right.Controls.Add(FieldRow("Ambiente", environmentBox, MakeButton("Detectar", delegate { DetectEnvironments(); })), 0, 3);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 43, Padding = new Padding(0, 5, 0, 3) };
            buildButton = MakeButton("Compilar", delegate { StartBuild(false); });
            cleanBuildButton = MakeButton("Limpar e compilar", delegate { StartBuild(true); });
            actions.Controls.Add(buildButton);
            actions.Controls.Add(cleanBuildButton);
            actions.Controls.Add(MakeButton("Salvar alterações", delegate { SaveCurrentProject(); }));
            actions.Controls.Add(MakeButton("Abrir pasta", delegate { OpenProjectFolder(); }));
            right.Controls.Add(actions, 0, 4);

            var artifacts = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 68, AutoScroll = true, Padding = new Padding(0, 3, 0, 3) };
            artifacts.Controls.Add(new Label { Text = "Artefato:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) });
            artifactBox.Width = 290;
            artifactBox.DropDownStyle = ComboBoxStyle.DropDownList;
            artifactBox.SelectedIndexChanged += delegate { UpdateHash(); };
            artifacts.Controls.Add(artifactBox);
            artifacts.Controls.Add(MakeButton("Atualizar", delegate { RefreshArtifacts(); }));
            artifacts.Controls.Add(MakeButton("Copiar arquivo", delegate { CopyArtifactToClipboard(); }));
            artifacts.Controls.Add(MakeButton("Copiar SHA-256", delegate { CopyHash(); }));
            artifacts.Controls.Add(new Label { Text = "Exportar:", AutoSize = true, Margin = new Padding(12, 9, 3, 3) });
            exportFormatBox.Width = 175;
            exportFormatBox.DropDownStyle = ComboBoxStyle.DropDownList;
            exportFormatBox.Items.AddRange(new object[] {
                "Firmware (.bin)",
                "Diagnóstico (.elf)",
                "Mapa de memória (.map)",
                "Intel HEX (.hex)",
                "Script CMD (.bat)",
                "Script PowerShell (.ps1)"
            });
            exportFormatBox.SelectedIndex = 0;
            artifacts.Controls.Add(exportFormatBox);
            artifacts.Controls.Add(MakeButton("Exportar...", delegate { ExportSelectedFormat(); }));
            right.Controls.Add(artifacts, 0, 5);

            logBox.Dock = DockStyle.Fill;
            logBox.Multiline = true;
            logBox.ScrollBars = ScrollBars.Both;
            logBox.ReadOnly = true;
            logBox.WordWrap = false;
            logBox.Font = new Font("Consolas", 9F);
            logBox.BackColor = Color.FromArgb(28, 31, 36);
            logBox.ForeColor = Color.Gainsboro;
            right.Controls.Add(logBox, 0, 6);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            statusLabel.Text = "Pronto";
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font(Font, FontStyle.Bold);
            hashLabel.Text = "SHA-256: —";
            hashLabel.AutoSize = true;
            footer.Controls.Add(statusLabel, 0, 0);
            footer.Controls.Add(hashLabel, 0, 1);
            right.Controls.Add(footer, 0, 7);
        }

        Button MakeButton(string text, EventHandler click)
        {
            var button = new Button { Text = text, AutoSize = true, Height = 29, Margin = new Padding(3) };
            button.Click += click;
            return button;
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
            if (project == null) return;
            nameBox.Text = project.Name;
            folderBox.Text = project.Folder;
            platformIoBox.Text = project.PlatformIoPath;
            environmentBox.Text = string.IsNullOrWhiteSpace(project.Environment) ? "esp32dev" : project.Environment;
            DetectEnvironments(false);
            RefreshArtifacts();
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
            if (project == null) return;
            project.Name = nameBox.Text.Trim();
            project.Folder = folderBox.Text.Trim();
            project.PlatformIoPath = platformIoBox.Text.Trim();
            project.Environment = environmentBox.Text.Trim();
            SaveSettings();
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
            if (cleanFirst) RunPlatformIo("run -e \"" + environmentBox.Text + "\" -t clean", delegate(bool ok) {
                if (!ok) FinishBuild(false);
                else { statusLabel.Text = "Compilando..."; RunPlatformIo("run -e \"" + environmentBox.Text + "\"", FinishBuild); }
            });
            else RunPlatformIo("run -e \"" + environmentBox.Text + "\"", FinishBuild);
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
            SetBusy(false);
            statusLabel.ForeColor = success ? Color.ForestGreen : Color.Firebrick;
            statusLabel.Text = success ? "Compilação concluída com sucesso" : "Falha na compilação";
            RefreshArtifacts();
            if (success) System.Media.SystemSounds.Asterisk.Play();
            else System.Media.SystemSounds.Hand.Play();
        }

        void SetBusy(bool busy)
        {
            buildButton.Enabled = !busy;
            cleanBuildButton.Enabled = !busy;
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
            string previous = artifactBox.SelectedItem as string;
            artifactBox.Items.Clear();
            string build = Path.Combine(folderBox.Text, ".pio", "build");
            if (Directory.Exists(build))
            {
                string[] extensions = { ".bin", ".elf", ".map", ".hex" };
                var files = Directory.GetFiles(build, "*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .OrderByDescending(File.GetLastWriteTime)
                    .ToArray();
                foreach (string file in files) artifactBox.Items.Add(file);
            }
            if (previous != null && artifactBox.Items.Contains(previous)) artifactBox.SelectedItem = previous;
            else
            {
                string preferred = artifactBox.Items.Cast<string>().FirstOrDefault(f =>
                    Path.GetFileName(f).Equals("firmware.bin", StringComparison.OrdinalIgnoreCase) &&
                    f.IndexOf(Path.DirectorySeparatorChar + environmentBox.Text + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0);
                if (preferred != null) artifactBox.SelectedItem = preferred;
                else if (artifactBox.Items.Count > 0) artifactBox.SelectedIndex = 0;
            }
            UpdateHash();
        }

        string SelectedArtifact { get { return artifactBox.SelectedItem as string; } }

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

            return artifactBox.Items.Cast<string>()
                .Where(f => Path.GetExtension(f).Equals(extension, StringComparison.OrdinalIgnoreCase))
                .Where(f => f.IndexOf(Path.DirectorySeparatorChar + environmentBox.Text + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();
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
