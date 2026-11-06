using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeviceFirmwareStudio
{
    public partial class MainForm
    {
        ProfileTabs compilationSections;
        TabPage prepareSection, usbSection, filesSection, terminalsSection;
        Label usbDestinationSummary;

        void OrganizeCompilationWorkspace(TableLayoutPanel previousLayout, Control profileGroup)
        {
            Control artifacts=previousLayout.GetControlFromPosition(0,1);
            var oldActions=(FlowLayoutPanel)previousLayout.GetControlFromPosition(0,2);
            Control usbPanel=previousLayout.GetControlFromPosition(0,3);
            Control terminals=previousLayout.GetControlFromPosition(0,4);
            var compileActions=new FlowLayoutPanel { AutoSize=true, Dock=DockStyle.Top, WrapContents=true };
            var exportActions=new FlowLayoutPanel { AutoSize=true, Dock=DockStyle.Top, WrapContents=true };
            foreach(Control control in oldActions.Controls.Cast<Control>().ToArray()) {
                var button=control as Button;
                string key=button!=null&&buttonKeys.ContainsKey(button)?buttonKeys[button]:"";
                if(key=="Salvar firmware"||key=="Criar pacote ZIP")exportActions.Controls.Add(control);
                else compileActions.Controls.Add(control);
            }
            compilationSections=new ProfileTabs { Dock=DockStyle.Fill, DrawMode=TabDrawMode.OwnerDrawFixed, SizeMode=TabSizeMode.Fixed, ItemSize=new Size(153,34), Margin=Padding.Empty };
            prepareSection=CompilationPage("Preparar e compilar","Prepare and build","Preparar y compilar");
            usbSection=CompilationPage("Gravação USB","USB flashing","Grabación USB");
            filesSection=CompilationPage("Arquivos","Files","Archivos");
            terminalsSection=CompilationPage("Terminais","Terminals","Terminales");
            compilationSections.TabPages.AddRange(new[]{prepareSection,usbSection,filesSection,terminalsSection});

            profileGroup.Dock=DockStyle.Top;profileGroup.Margin=new Padding(0,0,0,14);
            var prepareLayout=PageRows();prepareLayout.Controls.Add(profileGroup,0,0);
            prepareLayout.Controls.Add(WorkspaceGroup("Compilação","Build","Compilación",compileActions),0,1);
            prepareSection.Controls.Add(prepareLayout);

            var usbLayout=PageRows();
            usbDestinationSummary=new Label { Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(4,6,4,6),ForeColor=Color.FromArgb(225,237,244) };
            var destination=new FlowLayoutPanel { AutoSize=true,Dock=DockStyle.Top,FlowDirection=FlowDirection.TopDown,WrapContents=false };
            destination.Controls.Add(usbDestinationSummary);
            destination.Controls.Add(TerminalButton("Alterar configuração","Change configuration","Cambiar configuración",delegate { compilationSections.SelectedTab=prepareSection; }));
            usbLayout.Controls.Add(WorkspaceGroup("Destino selecionado","Selected destination","Destino seleccionado",destination),0,0);
            usbLayout.Controls.Add(WorkspaceGroup("Conectar e gravar","Connect and flash","Conectar y grabar",usbPanel),0,1);
            usbSection.Controls.Add(usbLayout);

            var filesLayout=PageRows();artifacts.Dock=DockStyle.Top;
            filesLayout.Controls.Add(artifacts,0,0);
            filesLayout.Controls.Add(WorkspaceGroup("Exportar firmware","Export firmware","Exportar firmware",exportActions),0,1);
            filesSection.Controls.Add(filesLayout);
            terminals.Dock=DockStyle.Fill;terminals.MinimumSize=Size.Empty;terminals.Margin=Padding.Empty;
            terminalsSection.Controls.Add(terminals);

            compilationTab.Controls.Remove(previousLayout);
            compilationTab.AutoScroll=false;compilationTab.AutoScrollMinSize=Size.Empty;compilationTab.Padding=new Padding(8);
            compilationTab.Controls.Add(compilationSections);
            previousLayout.Dispose();
            RefreshCompilationWorkspace();
        }
        TabPage CompilationPage(string pt,string en,string es)
        {
            var page=new TabPage { Padding=new Padding(12),BackColor=Color.FromArgb(9,38,57),UseVisualStyleBackColor=false };
            PText(page,pt,en,es);return page;
        }
        TableLayoutPanel PageRows()
        {
            var table=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=1,RowCount=3,Margin=Padding.Empty,Padding=Padding.Empty };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));table.RowStyles.Add(new RowStyle(SizeType.AutoSize));table.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            return table;
        }
        Control WorkspaceGroup(string pt,string en,string es,Control content)
        {
            var group=new ProfileGroupBox { Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(12,24,12,12),Margin=new Padding(0,0,0,14),MinimumSize=new Size(0,80) };
            PText(group,pt,en,es);content.Dock=DockStyle.Top;group.Controls.Add(content);return group;
        }
        void UpdateCompilationSummary()
        {
            if(usbDestinationSummary==null)return;
            var company=companyProfileBox.SelectedItem as CompanyProfile;
            var server=serverProfileBox.SelectedItem as ServerProfile;
            var wifi=wifiProfileBox.SelectedItem as WifiProfile;
            string empty=L("Não selecionado","Not selected","No seleccionado");
            string target=inheritProfileBox.Checked?L("Configuração já salva no ESP","Configuration already saved on the ESP","Configuración guardada en el ESP"):(company==null?empty:company.ToString());
            usbDestinationSummary.Text=L("Projeto: ","Project: ","Proyecto: ")+nameBox.Text+"\r\n"+L("Empresa: ","Company: ","Empresa: ")+target+"\r\n"+L("Servidor: ","Server: ","Servidor: ")+(inheritProfileBox.Checked?target:server==null?empty:server.BaseUrl)+"\r\nWi-Fi: "+(inheritProfileBox.Checked?target:wifiModeBox.SelectedIndex==1?(wifi==null?empty:wifi.Ssid):L("Gerenciado pelo sistema","Managed by system","Administrado por el sistema"));
        }
        void RefreshCompilationWorkspace()
        {
            if(compilationSections!=null)compilationSections.SetPalette(Color.FromArgb(9,38,57),Darken(currentAccent,.35F),currentAccent);
            UpdateCompilationSummary();
        }
        void SelectTerminalSection()
        {
            if(compilationSections!=null)compilationSections.SelectedTab=terminalsSection;
        }
    }
}
