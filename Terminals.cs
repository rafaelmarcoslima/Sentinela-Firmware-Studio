using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SentinelaFirmwareStudio
{
    public sealed class ReadableSerialCombo : ComboBox
    {
        public ReadableSerialCombo() { DrawMode=DrawMode.OwnerDrawFixed;BackColor=Color.FromArgb(13,48,68);ForeColor=Color.White; }
        protected override void OnDrawItem(DrawItemEventArgs e) {
            if(e.Bounds.Width<=0||e.Bounds.Height<=0)return;
            Color background=!Enabled?Color.FromArgb(27,56,75):(e.State&DrawItemState.Selected)!=0?Color.FromArgb(28,91,125):BackColor;
            using(var brush=new SolidBrush(background))e.Graphics.FillRectangle(brush,e.Bounds);
            string value=e.Index>=0&&e.Index<Items.Count?GetItemText(Items[e.Index]):Text;
            TextRenderer.DrawText(e.Graphics,value,Font,e.Bounds,Enabled?ForeColor:Color.FromArgb(200,219,232),TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
            if(Enabled)e.DrawFocusRectangle();
        }
    }

    public sealed class TerminalHistory
    {
        readonly object gate = new object();
        readonly StringBuilder text = new StringBuilder();
        readonly int limit;
        int version;
        public TerminalHistory(int maximumCharacters) { limit = Math.Max(1024, maximumCharacters); }
        public void Append(string value) { lock (gate) { text.Append(value); if (text.Length > limit) text.Remove(0, text.Length - limit); version++; } }
        public void Clear() { lock (gate) { text.Clear(); version++; } }
        public bool TryRead(ref int previousVersion,out string value) { lock(gate) { if(previousVersion==version) { value=null;return false; }previousVersion=version;value=text.ToString();return true; } }
        public string Read(out int currentVersion) { lock (gate) { currentVersion = version; return text.ToString(); } }
    }

    public interface IMonitorConnection : IDisposable
    {
        event EventHandler DataAvailable;
        bool IsOpen { get; }
        void Open();
        string ReadExisting();
    }
    public sealed class NativeMonitorConnection : IMonitorConnection
    {
        readonly SerialPort port;
        public event EventHandler DataAvailable;
        public NativeMonitorConnection(string name, int baud)
        {
            port = new SerialPort(name, baud) { DtrEnable = false, RtsEnable = false, Handshake = Handshake.None, Encoding = Encoding.UTF8, ReadTimeout = 500, WriteTimeout = 500 };
            port.DataReceived += delegate { var handler = DataAvailable; if (handler != null) handler(this, EventArgs.Empty); };
        }
        public bool IsOpen { get { return port.IsOpen; } }
        public void Open() { port.Open(); }
        public string ReadExisting() { return port.ReadExisting(); }
        public void Dispose() { port.Dispose(); }
    }

    public partial class MainForm
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, IntPtr lParam);
        readonly TerminalHistory buildHistoryBuffer = new TerminalHistory(262144);
        readonly TerminalHistory monitorHistoryBuffer = new TerminalHistory(262144);
        readonly Dictionary<Control, bool> operationControls = new Dictionary<Control, bool>();
        readonly List<string> terminalSecrets = new List<string>();
        readonly object secretGate = new object();
        Func<string[]> monitorPortProvider = SerialPort.GetPortNames;
        Task monitorClosingTask = Task.FromResult(0);
        bool monitorDisconnecting, monitorResumePending;
        int reconnectGeneration;
        Func<string, int, IMonitorConnection> monitorFactory = (name, baud) => new NativeMonitorConnection(name, baud);
        IMonitorConnection monitorConnection;
        Task monitorTransition = Task.FromResult(0);
        System.Windows.Forms.Timer terminalTimer;
        Panel terminalHost, buildTerminalPane, monitorTerminalPane;
        SplitContainer terminalSplit, expandedSplit;
        Form expandedTerminals;
        TextBox monitorBox;
        ComboBox monitorPortBox, monitorBaudBox;
        Button monitorConnectButton, monitorDisconnectButton, monitorPauseButton, monitorOpenButton;
        Label monitorStatusLabel, terminalOperationLabel, expandedPlaceholder;
        ProgressBar terminalProgress;
        bool operationBusy, monitorConnecting, monitorPaused, monitorAutoScroll = true, buildAutoScroll = true, resumeMonitorAfterUpload, closingTerminals;
        int terminalViewMode, monitorGeneration, buildRenderedVersion = -1, monitorRenderedVersion = -1, monitorPollTicks;
        string connectedMonitorPort, reconnectMonitorPort;
        int connectedMonitorBaud, reconnectMonitorBaud;

        Control CreateTerminalArea()
        {
            terminalHost = new Panel { Dock = DockStyle.Fill, MinimumSize = new Size(0, 240) };
            var modes = CreateTerminalModes();
            terminalOperationLabel = new Label { AutoSize = true, ForeColor = Color.FromArgb(225,237,244), Margin = new Padding(8) };
            terminalProgress = new ProgressBar { Width = 90, Height = 16, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(4,8,4,4) };
            modes.Controls.Add(terminalProgress); modes.Controls.Add(terminalOperationLabel);
            terminalSplit = NewTerminalSplit();
            buildTerminalPane = CreateTerminalPane(false);
            monitorTerminalPane = CreateTerminalPane(true);
            terminalSplit.Panel1.Controls.Add(buildTerminalPane); terminalSplit.Panel2.Controls.Add(monitorTerminalPane);
            terminalHost.Controls.Add(terminalSplit); terminalHost.Controls.Add(modes);
            expandedPlaceholder = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Visible = false, ForeColor = Color.White };
            PText(expandedPlaceholder,"Terminais abertos na janela ampliada.","Terminals are open in the expanded window.","Terminales abiertos en la ventana ampliada.");
            terminalHost.Controls.Add(expandedPlaceholder);
            SetTerminalMode(0);
            terminalTimer = new System.Windows.Forms.Timer { Interval = 150 };
            terminalTimer.Tick += delegate {
                FlushTerminalViews();
                if (++monitorPollTicks % 14 == 0 && monitorConnection != null && !monitorConnecting) {
                    try { if (!monitorConnection.IsOpen || !monitorPortProvider().Contains(connectedMonitorPort, StringComparer.OrdinalIgnoreCase)) MonitorLost(monitorConnection, L("Dispositivo desconectado.","Device disconnected.","Dispositivo desconectado.")); } catch { }
                }
            };
            terminalTimer.Start();
            FormClosing += async delegate(object sender, FormClosingEventArgs e) {
                if(operationBusy) { e.Cancel=true;return; }
                if(!closingTerminals&&(monitorConnection!=null||monitorConnecting)) { e.Cancel=true;closingTerminals=true;resumeMonitorAfterUpload=false;await DisconnectMonitor();Close(); }
            };
            FormClosed += delegate { closingTerminals = true; terminalTimer.Stop(); terminalTimer.Dispose(); monitorGeneration++; var old = monitorConnection; monitorConnection = null; if (old != null) Task.Run(delegate { old.Dispose(); }); if (expandedTerminals != null) expandedTerminals.Dispose(); };
            RefreshMonitorPorts();
            return terminalHost;
        }

        SplitContainer NewTerminalSplit()
        {
            return new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, BackColor = Color.FromArgb(45,83,106), SplitterWidth = 7, Panel1MinSize = 0, Panel2MinSize = 0 };
        }
        FlowLayoutPanel CreateTerminalModes()
        {
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Padding = new Padding(0,2,0,2) };
            bar.Controls.Add(TerminalButton("Compilação","Build","Compilación",delegate { SetTerminalMode(0); }));
            bar.Controls.Add(TerminalButton("Monitor ESP","ESP monitor","Monitor ESP",delegate { SetTerminalMode(1); }));
            bar.Controls.Add(TerminalButton("Lado a lado","Side by side","Lado a lado",delegate { SetTerminalMode(2); }));
            return bar;
        }
        Button TerminalButton(string pt, string en, string es, Action action)
        {
            var button = MakeButton(pt, delegate { action(); }); PText(button,pt,en,es);
            button.ForeColor = Color.White; button.BackColor = Color.FromArgb(15,77,113); button.FlatAppearance.BorderColor = Color.FromArgb(78,145,183);
            FitTerminalButton(button);
            return button;
        }
        void FitTerminalButton(Button button) { Action resize=delegate { button.MinimumSize=new Size(TextRenderer.MeasureText(button.Text,button.Font).Width+(button.Image==null?0:button.Image.Width)+24,32); };button.TextChanged+=delegate { resize(); };resize(); }
        Panel CreateTerminalPane(bool serial)
        {
            var pane = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(9,38,57), Padding = new Padding(3) };
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
            var title = new Label { AutoSize = true, ForeColor = Color.White, Margin = new Padding(5,10,6,3) };
            PText(title,serial?"Monitor do ESP":"Compilação",serial?"ESP monitor":"Build",serial?"Monitor del ESP":"Compilación"); tools.Controls.Add(title);
            tools.Controls.Add(TerminalButton("Limpar","Clear","Limpiar",delegate { if(serial) { monitorHistoryBuffer.Clear(); monitorBox.Clear(); } else ClearBuildTerminal(); FlushTerminalViews(); }));
            tools.Controls.Add(TerminalButton("Copiar","Copy","Copiar",delegate { string value=TerminalText(serial); if(value.Length>0) { try { Clipboard.SetText(value); } catch(Exception error) { MessageBox.Show(this,error.Message,Text); } } }));
            tools.Controls.Add(TerminalButton("Salvar log","Save log","Guardar registro",delegate { SaveTerminalLog(serial); }));
            tools.Controls.Add(TerminalButton("Ampliar","Expand","Ampliar",delegate { OpenExpandedTerminals(serial?1:0); }));
            var follow = new ProfileCheckBox { Checked = true, AutoSize = true, ForeColor = Color.White, Margin = new Padding(4,9,3,3) };
            PText(follow,"Rolagem automática","Auto-scroll","Desplazamiento automático");follow.CheckedChanged += delegate { if(serial)monitorAutoScroll=follow.Checked;else buildAutoScroll=follow.Checked; };tools.Controls.Add(follow);
            if (serial) {
                monitorBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, WordWrap = false, ScrollBars = ScrollBars.Both, Font = new Font("Consolas",9F), BackColor = Color.FromArgb(2,16,27), ForeColor = Color.FromArgb(220,235,243) };
                pane.Controls.Add(monitorBox);
                var connectionTools = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
                monitorPortBox = new ReadableSerialCombo { Width = 86, DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = "Porta COM", Margin = new Padding(3,7,3,3) };
                monitorBaudBox = new ReadableSerialCombo { Width = 91, DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = "Baud rate", Margin = new Padding(3,7,3,3) };
                monitorBaudBox.Items.AddRange(new object[] { "9600","57600","115200","230400","460800","921600" });monitorBaudBox.SelectedItem="115200";
                monitorPortBox.SelectedIndexChanged += delegate { RefreshMonitorControls(); };
                connectionTools.Controls.Add(monitorPortBox); connectionTools.Controls.Add(monitorBaudBox);
                connectionTools.Controls.Add(TerminalButton("Atualizar portas","Refresh ports","Actualizar puertos",RefreshMonitorPorts));
                monitorConnectButton = TerminalButton("Conectar","Connect","Conectar",async delegate { CancelMonitorResume();await StartMonitorConnection(); });
                monitorDisconnectButton = TerminalButton("Desconectar","Disconnect","Desconectar",async delegate { CancelMonitorResume();await DisconnectMonitor(); });
                monitorPauseButton = TerminalButton("Pausar","Pause","Pausar",delegate { monitorPaused=!monitorPaused;RefreshMonitorControls();FlushTerminalViews(); });
                connectionTools.Controls.Add(monitorConnectButton);connectionTools.Controls.Add(monitorDisconnectButton);connectionTools.Controls.Add(monitorPauseButton);
                monitorStatusLabel = new Label { AutoSize = true, ForeColor = Color.FromArgb(174,205,222), Margin = new Padding(5,10,3,3) };connectionTools.Controls.Add(monitorStatusLabel);
                pane.Controls.Add(connectionTools);
            } else pane.Controls.Add(logBox);
            pane.Controls.Add(tools);
            return pane;
        }

        void SetTerminalMode(int mode)
        {
            terminalViewMode=mode;
            SplitContainer split = expandedTerminals == null ? terminalSplit : expandedSplit;
            if (split == null) return;
            split.Panel1Collapsed=false;split.Panel2Collapsed=false;
            if(mode==0)split.Panel2Collapsed=true;else if(mode==1)split.Panel1Collapsed=true;
            else if(split.Width>100)split.SplitterDistance=split.Width/2;
        }
        void OpenExpandedTerminals(int mode)
        {
            if (expandedTerminals != null) { SetTerminalMode(mode);expandedTerminals.Show();expandedTerminals.Activate();return; }
            expandedTerminals = new Form { Text = L("Terminais do ESP e compilação","ESP and build terminals","Terminales del ESP y compilación"), Size = new Size(1200,750), MinimumSize = new Size(800,450), StartPosition = FormStartPosition.CenterParent, BackColor = Color.FromArgb(9,38,57), ForeColor = Color.White };
            expandedSplit=NewTerminalSplit();expandedSplit.Panel1.Controls.Add(buildTerminalPane);expandedSplit.Panel2.Controls.Add(monitorTerminalPane);
            expandedTerminals.Controls.Add(expandedSplit);expandedTerminals.Controls.Add(CreateTerminalModes());
            terminalSplit.Visible=false;expandedPlaceholder.Visible=true;expandedPlaceholder.BringToFront();
            expandedTerminals.FormClosing += delegate {
                if(closingTerminals)return;
                terminalSplit.Panel1.Controls.Add(buildTerminalPane);terminalSplit.Panel2.Controls.Add(monitorTerminalPane);
                terminalSplit.Visible=true;expandedPlaceholder.Visible=false;expandedTerminals=null;expandedSplit=null;SetTerminalMode(terminalViewMode);
            };
            Form window=expandedTerminals;
            window.FormClosed += delegate { RemoveTerminalLocalization(window); };
            expandedTerminals.Show(this);SetTerminalMode(mode);
        }
        void RemoveTerminalLocalization(Control parent)
        {
            foreach(Control child in parent.Controls) { RemoveTerminalLocalization(child);profileTexts.Remove(child);localizationKeys.Remove(child);tooltipKeys.Remove(child);var button=child as Button;if(button!=null)buttonKeys.Remove(button); }
        }
        string TerminalText(bool serial) { int version;return MaskTerminalSecrets((serial?monitorHistoryBuffer:buildHistoryBuffer).Read(out version)); }
        void SaveTerminalLog(bool serial)
        {
            using(var dialog=new SaveFileDialog { FileName=(serial?"esp-monitor-":"build-")+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".log",Filter="Log (*.log)|*.log|Text (*.txt)|*.txt" }) {
                if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                try { File.WriteAllText(dialog.FileName,TerminalText(serial),new UTF8Encoding(false)); } catch(Exception error) { MessageBox.Show(this,error.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error); }
            }
        }
        void ClearBuildTerminal() { buildHistoryBuffer.Clear();logBox.Clear();buildRenderedVersion=-1; }
        void FlushTerminalViews()
        {
            string value;
            if(buildHistoryBuffer.TryRead(ref buildRenderedVersion,out value))PaintTerminal(logBox,MaskTerminalSecrets(value),buildAutoScroll);
            if(monitorBox!=null&&!monitorPaused&&monitorHistoryBuffer.TryRead(ref monitorRenderedVersion,out value))PaintTerminal(monitorBox,MaskTerminalSecrets(value),monitorAutoScroll);
        }
        static void PaintTerminal(TextBox box,string value,bool follow)
        {
            value=System.Text.RegularExpressions.Regex.Replace(value,"\x1B\\[[0-?]*[ -/]*[@-~]", "").Replace("\r\n","\n").Replace("\r","\n").Replace("\n","\r\n");
            int selection=box.SelectionStart,length=box.SelectionLength;
            if(!follow) SendMessage(box.Handle,0x000B,IntPtr.Zero,IntPtr.Zero);
            int firstLine=(int)SendMessage(box.Handle,0x00CE,IntPtr.Zero,IntPtr.Zero);
            box.Text=value;
            if(follow) { box.SelectionStart=box.TextLength;box.SelectionLength=0;box.ScrollToCaret(); }
            else { box.Select(Math.Min(selection,box.TextLength),Math.Min(length,Math.Max(0,box.TextLength-selection)));int now=(int)SendMessage(box.Handle,0x00CE,IntPtr.Zero,IntPtr.Zero);SendMessage(box.Handle,0x00B6,IntPtr.Zero,new IntPtr(firstLine-now));SendMessage(box.Handle,0x000B,new IntPtr(1),IntPtr.Zero);box.Invalidate(); }
        }
        void CaptureTerminalSecrets()
        {
            var values=new List<string>();
            foreach(var server in settings.Servers??new List<ServerProfile>())try { string value=ProfileSecurity.Reveal(server.ProtectedKey);if(!string.IsNullOrEmpty(value))values.Add(value); }catch { }
            foreach(var wifi in settings.WifiNetworks??new List<WifiProfile>())try { string value=ProfileSecurity.Reveal(wifi.ProtectedPassword);if(!string.IsNullOrEmpty(value))values.Add(value); }catch { }
            lock(secretGate) { terminalSecrets.Clear();terminalSecrets.AddRange(values.Distinct().OrderByDescending(v=>v.Length)); }
            buildRenderedVersion=monitorRenderedVersion=-1;
        }
        string MaskTerminalSecrets(string value) { lock(secretGate) { foreach(string secret in terminalSecrets)value=value.Replace(secret,"[credencial omitida]"); }return value; }
        void SetTerminalOperation(string message,bool busy) { if(terminalOperationLabel==null)return;terminalOperationLabel.Text=message;terminalProgress.Visible=busy; }

        void RefreshMonitorPorts()
        {
            if(monitorPortBox==null||monitorConnection!=null||monitorConnecting)return;
            string selected=monitorPortBox.SelectedItem as string??(usbPortBox==null?null:usbPortBox.SelectedItem as string);
            try { var ports=monitorPortProvider().Where(UsbUpload.IsSerialPort).OrderBy(p=>int.Parse(p.Substring(3))).ToArray();monitorPortBox.Items.Clear();monitorPortBox.Items.AddRange(ports);if(selected!=null&&ports.Contains(selected))monitorPortBox.SelectedItem=selected; }catch { }
            RefreshMonitorControls();
        }
        void RefreshMonitorControls()
        {
            if(monitorPortBox==null||monitorPortBox.IsDisposed)return;
            bool connected=monitorConnection!=null;
            monitorPortBox.Enabled=monitorBaudBox.Enabled=!connected&&!monitorConnecting&&!monitorDisconnecting&&!monitorResumePending;
            string selected=monitorPortBox.SelectedItem as string;
            bool reserved=operationBusy&&usbUploadInProgress&&string.Equals(selected,usbUploadPort,StringComparison.OrdinalIgnoreCase);
            monitorConnectButton.Enabled=!connected&&!monitorConnecting&&!monitorDisconnecting&&!reserved&&UsbUpload.IsSerialPort(selected);
            monitorDisconnectButton.Enabled=!monitorDisconnecting&&(connected||monitorConnecting||resumeMonitorAfterUpload||monitorResumePending);
            monitorPauseButton.Text=monitorPaused?L("Retomar","Resume","Reanudar"):L("Pausar","Pause","Pausar");
            monitorStatusLabel.Text=monitorResumePending?L("Reconectando...","Reconnecting...","Reconectando..."):monitorDisconnecting?L("Desconectando...","Disconnecting...","Desconectando..."):monitorConnecting?L("Conectando...","Connecting...","Conectando..."):connected?connectedMonitorPort+" · "+connectedMonitorBaud+(monitorPaused?L(" · exibição pausada"," · display paused"," · visualización pausada"):""):resumeMonitorAfterUpload?L("Aguardando gravação USB","Waiting for USB flashing","Esperando grabación USB"):L("Desconectado","Disconnected","Desconectado");
            monitorStatusLabel.ForeColor=connected?Color.FromArgb(103,226,172):Color.FromArgb(174,205,222);
        }
        async Task ConnectMonitor()
        {
            if(monitorConnection!=null||monitorConnecting||monitorDisconnecting||closingTerminals)return;
            string port=monitorPortBox.SelectedItem as string;int baud;
            if(!UsbUpload.IsSerialPort(port)||!int.TryParse(Convert.ToString(monitorBaudBox.SelectedItem),out baud))return;
            if(operationBusy&&usbUploadInProgress&&string.Equals(port,usbUploadPort,StringComparison.OrdinalIgnoreCase))return;
            int generation=++monitorGeneration;monitorConnecting=true;CaptureTerminalSecrets();RefreshMonitorControls();
            IMonitorConnection connection=null;
            try {
            connection=monitorFactory(port,baud);
            connection.DataAvailable+=delegate {
                if(generation!=monitorGeneration)return;
                try { string data=connection.ReadExisting();monitorHistoryBuffer.Append(data); }
                catch(Exception error) { if(!IsDisposed&&IsHandleCreated)BeginInvoke(new Action(delegate { MonitorLost(connection,error.Message); })); }
            };
                await Task.Run(delegate { connection.Open(); });
                if(generation!=monitorGeneration||closingTerminals) { await Task.Run(delegate { connection.Dispose(); });return; }
                monitorConnection=connection;connectedMonitorPort=port;connectedMonitorBaud=baud;
                monitorHistoryBuffer.Append("\r\n[MONITOR] "+port+" · "+baud+"\r\n");
            } catch(Exception error) {
                try { if(connection!=null)connection.Dispose(); } catch { }
                if(generation==monitorGeneration)monitorHistoryBuffer.Append("\r\n[MONITOR] "+L("Não foi possível abrir a porta. Confira o cabo, o driver e se outro programa está usando a porta: ","Could not open the port. Check the cable, driver and whether another program is using it: ","No se pudo abrir el puerto. Revise el cable, el controlador y si otro programa está usando el puerto: ")+error.Message+"\r\n");
            } finally { if(generation==monitorGeneration)monitorConnecting=false;RefreshMonitorControls();FlushTerminalViews(); }
        }
        Task StartMonitorConnection() { monitorTransition=ConnectMonitor();return monitorTransition; }
        async Task DisconnectMonitor()
        {
            if(monitorDisconnecting) { await monitorClosingTask;return; }
            bool opening=monitorConnecting;monitorGeneration++;var connection=monitorConnection;monitorConnection=null;monitorConnecting=false;monitorDisconnecting=true;RefreshMonitorControls();
            monitorClosingTask=Task.WhenAll(opening?monitorTransition:Task.FromResult(0),connection==null?Task.FromResult(0):Task.Run(delegate { connection.Dispose(); }));
            try { await monitorClosingTask; } catch(Exception error) { monitorHistoryBuffer.Append("\r\n[MONITOR] "+error.Message+"\r\n"); }
            monitorDisconnecting=false;RefreshMonitorControls();
        }
        async void MonitorLost(IMonitorConnection connection,string reason)
        {
            if(connection!=monitorConnection)return;
            await DisconnectMonitor();monitorHistoryBuffer.Append("\r\n[MONITOR] "+reason+"\r\n");FlushTerminalViews();
        }
        async Task SuspendMonitorForUpload(string port)
        {
            await monitorClosingTask;
            if(monitorConnecting)await monitorTransition;
            if(monitorConnection==null||!string.Equals(connectedMonitorPort,port,StringComparison.OrdinalIgnoreCase))return;
            reconnectMonitorPort=connectedMonitorPort;reconnectMonitorBaud=connectedMonitorBaud;resumeMonitorAfterUpload=true;
            await DisconnectMonitor();monitorHistoryBuffer.Append("\r\n[MONITOR] "+L("Porta liberada para gravação USB.","Port released for USB flashing.","Puerto liberado para grabación USB.")+"\r\n");RefreshMonitorControls();
        }
        void CancelMonitorResume() { reconnectGeneration++;monitorResumePending=false;resumeMonitorAfterUpload=false;RefreshMonitorControls(); }
        async void ResumeMonitorAfterUpload()
        {
            if(!resumeMonitorAfterUpload||closingTerminals)return;
            resumeMonitorAfterUpload=false;monitorResumePending=true;int generation=++reconnectGeneration;RefreshMonitorControls();
            await Task.Delay(800);
            if(generation!=reconnectGeneration||closingTerminals)return;
            monitorResumePending=false;RefreshMonitorControls();
            if(operationBusy||monitorConnection!=null||monitorConnecting)return;
            if(!monitorPortBox.Items.Contains(reconnectMonitorPort))monitorPortBox.Items.Add(reconnectMonitorPort);
            monitorPortBox.SelectedItem=reconnectMonitorPort;monitorBaudBox.SelectedItem=reconnectMonitorBaud.ToString();
            await StartMonitorConnection();
        }
        void ScrollToTerminals() { if(expandedTerminals==null&&terminalHost!=null)compilationTab.ScrollControlIntoView(terminalHost); }
        void ShowMonitorView() { SetTerminalMode(1);RefreshMonitorPorts();ScrollToTerminals(); }
        bool IsTerminalControl(Control control)
        {
            for(Control current=control;current!=null;current=current.Parent)if(current==terminalHost||current==buildTerminalPane||current==monitorTerminalPane)return true;
            return control==monitorOpenButton;
        }
        void LockOperationControls(bool busy)
        {
            operationBusy=busy;
            if(busy) {
                CaptureTerminalSecrets();operationControls.Clear();
                Action<Control> visit=null;visit=parent=>{foreach(Control control in parent.Controls) { if(IsTerminalControl(control))continue;if(control is Button||control is ComboBox||control is CheckBox||control is ListBox||control is DataGridView||control is TextBox&&!((TextBox)control).ReadOnly) { operationControls[control]=control.Enabled;control.Enabled=false; }else visit(control); }};visit(this);
            } else { foreach(var pair in operationControls)if(!pair.Key.IsDisposed)pair.Key.Enabled=pair.Value;operationControls.Clear(); }
            RefreshMonitorControls();
        }
    }
}
