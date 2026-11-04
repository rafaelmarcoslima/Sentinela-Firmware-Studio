using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SentinelaFirmwareStudio
{
    public static class UsbUpload
    {
        public static bool IsSerialPort(string value) { return Regex.IsMatch(value ?? "", "^COM[1-9][0-9]{0,4}$", RegexOptions.IgnoreCase); }
        public static string Arguments(string environment, string port)
        {
            if (!Regex.IsMatch(environment ?? "", "^[A-Za-z0-9_-]{1,80}$")) throw new ArgumentException("Ambiente PlatformIO inválido.");
            if (!IsSerialPort(port)) throw new ArgumentException("Selecione uma porta USB válida.");
            // PlatformIO builds and uploads the complete board image, including bootloader and partitions.
            return "run -e \"" + environment + "\" -t upload --upload-port \"" + port.ToUpperInvariant() + "\"";
        }
    }

    public partial class MainForm
    {
        ComboBox usbPortBox;
        Button usbRefreshButton, usbUploadButton;
        Label usbPortLabel, usbHintLabel;
        bool usbUploadInProgress;
        string usbUploadPort;

        Control CreateUsbUploadPanel()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 4, 0, 8) };
            usbPortLabel = new Label { AutoSize = true, Margin = new Padding(3, 11, 8, 3) };
            usbPortBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 115, Margin = new Padding(3, 7, 8, 3), AccessibleName = "Porta USB" };
            usbRefreshButton = MakeButton("Atualizar portas", delegate { RefreshUsbPorts(); });
            usbUploadButton = MakeButton("Compilar e gravar USB", delegate { StartUsbUpload(); });
            usbPortBox.SelectedIndexChanged += delegate { UpdateUsbControls(activeProcess != null); };
            usbHintLabel = new Label { AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            panel.Controls.AddRange(new Control[] { usbPortLabel, usbPortBox, usbRefreshButton, usbUploadButton, usbHintLabel });
            panel.SetFlowBreak(usbUploadButton, true);
            RefreshUsbLanguage();
            RefreshUsbPorts();
            FormClosing += delegate(object sender, FormClosingEventArgs e) { if (activeProcess != null) e.Cancel = true; };
            return panel;
        }

        void RefreshUsbLanguage()
        {
            if (usbPortBox == null) return;
            usbPortLabel.Text = L("Porta USB", "USB port", "Puerto USB");
            usbPortBox.AccessibleName = usbPortLabel.Text;
            usbRefreshButton.Text = L("Atualizar portas", "Refresh ports", "Actualizar puertos");
            usbUploadButton.Text = L("Compilar e gravar USB", "Build and flash USB", "Compilar y grabar USB");
            usbHintLabel.Text = usbPortBox.Items.Count == 0 ? L("Nenhuma porta encontrada. Conecte o ESP com um cabo de dados e clique em Atualizar portas.", "No ports found. Connect the ESP with a data cable and click Refresh ports.", "No se encontraron puertos. Conecte el ESP con un cable de datos y pulse Actualizar puertos.") : L("Conecte o ESP pelo cabo de dados, atualize as portas e selecione a porta do dispositivo.", "Connect the ESP with a data cable, refresh ports and select the device port.", "Conecte el ESP con un cable de datos, actualice los puertos y seleccione el puerto del dispositivo.");
            buttonTooltips.SetToolTip(usbUploadButton, L("Compila com a configuração selecionada e grava pelo cabo USB. Não apaga toda a memória do ESP.", "Builds with the selected configuration and flashes over USB. Does not erase the entire ESP memory.", "Compila con la configuración seleccionada y graba por USB. No borra toda la memoria del ESP."));
        }

        void RefreshUsbPorts()
        {
            if (activeProcess != null || usbPortBox == null) return;
            string previous = usbPortBox.SelectedItem as string;
            try {
                string[] ports = SerialPort.GetPortNames().Where(UsbUpload.IsSerialPort).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => int.Parse(p.Substring(3))).ToArray();
                usbPortBox.Items.Clear();
                usbPortBox.Items.AddRange(ports);
                if (previous != null && ports.Contains(previous, StringComparer.OrdinalIgnoreCase)) usbPortBox.SelectedItem = ports.First(p => p.Equals(previous, StringComparison.OrdinalIgnoreCase));
                RefreshUsbLanguage();
                UpdateUsbControls(false);
            } catch (Exception error) {
                MessageBox.Show(this, L("Não foi possível consultar as portas USB: ", "Could not list USB ports: ", "No se pudieron consultar los puertos USB: ") + error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void UpdateUsbControls(bool busy)
        {
            if (usbPortBox == null) return;
            usbPortBox.Enabled = !busy && !editingConfiguration;
            usbRefreshButton.Enabled = !busy && !editingConfiguration;
            usbUploadButton.Enabled = !busy && !editingConfiguration && SelectedProject != null && UsbUpload.IsSerialPort(usbPortBox.SelectedItem as string);
        }

        void StartUsbUpload()
        {
            string port = usbPortBox.SelectedItem as string;
            if (activeProcess != null) return;
            if (!UsbUpload.IsSerialPort(port) || !SerialPort.GetPortNames().Contains(port, StringComparer.OrdinalIgnoreCase)) {
                MessageBox.Show(this, L("A porta selecionada não está disponível. Conecte o ESP e atualize as portas.", "The selected port is unavailable. Connect the ESP and refresh ports.", "El puerto seleccionado no está disponible. Conecte el ESP y actualice los puertos."), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshUsbPorts(); return;
            }
            StartBuild(false, port);
        }

        bool ConfirmUsbUpload(string port)
        {
            try {
                string json = SelectedProfileJson();
                if (json != null && inheritProfileBox.Checked) throw new InvalidOperationException(L("Para gravar com os dados selecionados, desmarque Usar configuração já salva no ESP e selecione empresa, servidor e Wi-Fi.", "To flash with the selected settings, turn off Use configuration already saved on the ESP and select company, server and Wi-Fi.", "Para grabar con los datos seleccionados, desmarque Usar configuración guardada en el ESP y seleccione empresa, servidor y Wi-Fi."));
                string destination = json == null ? nameBox.Text : ((CompanyProfile)companyProfileBox.SelectedItem).ToString() + "\n" + ((ServerProfile)serverProfileBox.SelectedItem).BaseUrl + "\nWi-Fi: " + (wifiModeBox.SelectedIndex == 1 ? ((WifiProfile)wifiProfileBox.SelectedItem).Ssid : L("Gerenciado pelo sistema", "Managed by system", "Administrado por el sistema"));
                string message = L("Compilar e gravar neste ESP?", "Build and flash this ESP?", "¿Compilar y grabar en este ESP?") + "\n\n" + port + "\n" + destination + "\n\n" + L("O firmware atual será substituído. Mantenha o cabo conectado até terminar. Feche outros programas que estejam usando esta porta.", "The current firmware will be replaced. Keep the cable connected until finished. Close other programs using this port.", "Se sustituirá el firmware actual. Mantenga el cable conectado hasta finalizar. Cierre otros programas que utilicen este puerto.");
                return MessageBox.Show(this, message, L("Gravar no ESP por USB", "Flash ESP over USB", "Grabar ESP por USB"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            } catch (Exception error) { MessageBox.Show(this, error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
        }
    }
}
