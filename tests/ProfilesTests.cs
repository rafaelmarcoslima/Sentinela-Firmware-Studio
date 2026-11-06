using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using DeviceFirmwareStudio;
class ProfilesTests {
 static int checks;
 static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 static T Field<T>(object obj,string name){return (T)obj.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(obj);}
 static object Call(object obj,string name,params object[] args){return obj.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(obj,args);}
 [STAThread] static void Main(string[] args){
 try {
  string folder=Path.Combine(Path.GetTempPath(),"firmware-studio-tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);
  string key="test-key-not-production-000000000000",password="  test password with spaces  ";
  Check(ProfileSecurity.Reveal(ProfileSecurity.Protect(password))==password,"DPAPI roundtrip changed password");
  Check(ProfileSecurity.NormalizeUrl("https://test.invalid/api/dispositivos-remotos/")=="https://test.invalid","URL normalization");
  foreach(string url in new[]{"file:///tmp","https://user:pass@test.invalid","https://test.invalid?key=secret"}){bool failed=false;try{ProfileSecurity.NormalizeUrl(url);}catch(ArgumentException){failed=true;}Check(failed,"Unsafe URL accepted");}
  Check(UsbUpload.Arguments("esp32dev","com42")=="run -e \"esp32dev\" -t upload --upload-port \"COM42\"", "USB target or selected port lost");
  foreach(string badPort in new[]{"", "COM0", "http://device", "COM4 --erase-all", "COM4\""}){bool failed=false;try{UsbUpload.Arguments("esp32dev",badPort);}catch(ArgumentException){failed=true;}Check(failed,"Unsafe upload port accepted");}
  using(var form=new MainForm(folder)){
   var settings=Field<AppSettings>(form,"settings");
   settings.Companies=new List<CompanyProfile>{new CompanyProfile{Id="company-a",Name="Company A"},new CompanyProfile{Id="company-b",Name="Company B"}};
   settings.Servers=new List<ServerProfile>{new ServerProfile{Id="server-a",CompanyId="company-a",Name="Server A",BaseUrl="https://a.invalid",ProtectedKey=ProfileSecurity.Protect(key)},new ServerProfile{Id="server-b",CompanyId="company-b",Name="Server B",BaseUrl="https://b.invalid",ProtectedKey=ProfileSecurity.Protect(key)}};
   settings.WifiNetworks=new List<WifiProfile>{new WifiProfile{Id="wifi-a",CompanyId="company-a",Name="Network A",Ssid="Test A",ProtectedPassword=ProfileSecurity.Protect(password)},new WifiProfile{Id="wifi-b",CompanyId="company-b",Name="Network B",Ssid="Test B",ProtectedPassword=ProfileSecurity.Protect(password)}};
   string initialFolder=Path.Combine(folder,"initial-project");Directory.CreateDirectory(initialFolder);File.WriteAllText(Path.Combine(initialFolder,"firmware-profile.json"),"{\"kind\":\"bootstrap\"}");
   var project=new FirmwareProject{Name="Test initial installation",Folder=initialFolder,Environment="esp32dev"};settings.Projects.Add(project);
   var projects=Field<ListBox>(form,"projects");projects.DataSource=null;projects.Items.Clear();projects.Items.Add(project);projects.SelectedIndex=0;Call(form,"RefreshProfileData");
   var companies=Field<ComboBox>(form,"companyProfileBox");var servers=Field<ComboBox>(form,"serverProfileBox");var networks=Field<ComboBox>(form,"wifiProfileBox");var mode=Field<ComboBox>(form,"wifiModeBox");var inherit=Field<CheckBox>(form,"inheritProfileBox");
   companies.SelectedItem=settings.Companies[0];Check(servers.Items.Count==1&&networks.Items.Count==1,"Cross-company choices leaked");servers.SelectedItem=settings.Servers[0];mode.SelectedIndex=1;networks.SelectedItem=settings.WifiNetworks[0];
   string json=(string)Call(form,"SelectedProfileJson");
   string firstBuild=(string)Call(form,"ProfileForNewBuild",json),secondBuild=(string)Call(form,"ProfileForNewBuild",json);
   Check(firstBuild.Contains("wifiApplyId")&&firstBuild!=secondBuild,"Rebuilding the same Wi-Fi does not create a fresh application ID");
   Check(firstBuild.Contains(key)&&firstBuild.Contains(password),"Fresh build changed connection credentials");
   string script=(string)Call(form,"BuildProtectedProfileScript",json,".ps1");Check(!script.Contains(key)&&!script.Contains(password),"Exported script leaked credentials");File.WriteAllText(Path.Combine(folder,"generated.ps1"),script);Check(json.Contains("company-a")&&!json.Contains("company-b"),"Incorrect company profile");Check(json.Contains(password),"Password whitespace lost");
   companies.SelectedItem=settings.Companies[1];Check(servers.SelectedItem==null&&networks.SelectedItem==null,"Dependent selections not cleared");Check(servers.Items.Count==1&&((ServerProfile)servers.Items[0]).CompanyId=="company-b","Incorrect filtered server");
   companies.SelectedItem=settings.Companies[0];servers.SelectedItem=settings.Servers[0];networks.SelectedItem=settings.WifiNetworks[0];
   Check((bool)Call(form,"PrepareProfileBuild"),"Build setup failed");string build=Field<string>(form,"activeProfileBuildRoot");string output=Path.Combine(build,"build","esp32dev");Directory.CreateDirectory(output);File.WriteAllBytes(Path.Combine(output,"firmware.bin"),new byte[]{1,2,3,4});
   var usbPorts=Field<ComboBox>(form,"usbPortBox");usbPorts.Items.Add("COM42");usbPorts.SelectedItem="COM42";
   Call(form,"UpdateUsbControls",false);Check(Field<Button>(form,"usbUploadButton").Enabled,"USB button disabled for valid selection");
   Call(form,"UpdateUsbControls",true);Check(!Field<Button>(form,"usbUploadButton").Enabled&&!usbPorts.Enabled,"USB controls remain enabled during work");Call(form,"UpdateUsbControls",false);
   var cli=Field<TextBox>(form,"platformIoBox");string savedCli=cli.Text;cli.Text=args[1];
   var handle=form.Handle;bool finished=false,uploadOk=false;
   typeof(MainForm).GetField("usbUploadInProgress",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(form,true);
   Call(form,"RunPlatformIo",UsbUpload.Arguments("esp32dev","COM42"),new Action<bool>(ok=>{uploadOk=ok;finished=true;}));
   var timeout=System.Diagnostics.Stopwatch.StartNew();while(!finished&&timeout.ElapsedMilliseconds<15000){Application.DoEvents();System.Threading.Thread.Sleep(10);}
   Check(finished&&uploadOk,"USB simulator did not receive company, server, Wi-Fi, profile and upload arguments");
   Check(File.Exists(Path.Combine(build,"profile.json")),"Profile removed before upload completion");
   string uploadLog=Field<TextBox>(form,"logBox").Text;Check(uploadLog.Contains("USB simulation complete")&&!uploadLog.Contains(key)&&!uploadLog.Contains(password),"USB log not flushed or leaked credentials");
   finished=false;uploadOk=true;
   Call(form,"RunPlatformIo",UsbUpload.Arguments("esp32dev","COM43"),new Action<bool>(ok=>{uploadOk=ok;finished=true;}));
   timeout.Restart();while(!finished&&timeout.ElapsedMilliseconds<15000){Application.DoEvents();System.Threading.Thread.Sleep(10);}
   Check(finished&&!uploadOk,"USB failure was reported as success");
   typeof(MainForm).GetField("usbUploadInProgress",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(form,false);
   cli.Text=savedCli;Call(form,"FinishProfileBuild",true);
   string artifact=(string)Call(form,"GetMainFirmwarePath");Check(artifact!=null&&File.Exists(artifact),"Successful artifact not available");string manifest=File.ReadAllText(Path.Combine(Path.GetDirectoryName(artifact),"profile-manifest.json"));Check(!manifest.Contains(key)&&!manifest.Contains(password),"Manifest leaked credentials");Check(!File.Exists(Path.Combine(build,"profile.json")),"Private input left behind");
   companies.SelectedItem=settings.Companies[1];Check(Call(form,"GetMainFirmwarePath")==null,"Other company artifact exported");companies.SelectedItem=settings.Companies[0];servers.SelectedItem=settings.Servers[0];networks.SelectedItem=settings.WifiNetworks[0];Check(Call(form,"GetMainFirmwarePath")!=null,"Current artifact did not recover");
   File.AppendAllText(artifact,"corrupted");Check(Call(form,"GetMainFirmwarePath")==null,"Corrupted artifact allowed");
   Field<TextBox>(form,"folderBox").Text=args[0];Call(form,"SetProfileEnabled");servers.SelectedItem=settings.Servers[0];json=(string)Call(form,"SelectedProfileJson");Check(json.Contains("inherit")&&!json.Contains("apiKey")&&!json.Contains("company-a"),"Universal OTA embeds identity");
   var adoption=Field<CheckBox>(form,"adoptProfileBox");
   Check(!adoption.Enabled&&servers.Enabled,"Global configuration must allow instance selection and disable migration");
   Field<TextBox>(form,"folderBox").Text=initialFolder;Call(form,"SetProfileEnabled");Check(adoption.Enabled&&servers.Enabled,"Initial destination controls did not recover");
   Check(adoption.Parent is ProfileGroupBox&&adoption.Parent!=inherit.Parent,"Migration must be separate from regular update options");
   string bootstrap=Path.Combine(folder,"bootstrap");Directory.CreateDirectory(bootstrap);File.WriteAllText(Path.Combine(bootstrap,"firmware-profile.json"),"{\"kind\":\"bootstrap\"}");
   var folderInput=Field<TextBox>(form,"folderBox");folderInput.Text=bootstrap;inherit.Checked=true;Call(form,"SetProfileEnabled");
   Check(!inherit.Checked&&!project.InheritProfile,"Bootstrap retained hidden inherited configuration");
   Check(mode.SelectedIndex==1&&!mode.Enabled&&networks.Enabled,"Bootstrap must use a selectable initial network");
   form.Show();Application.DoEvents();Check(!inherit.Visible,"Bootstrap exposes saved configuration option");
   folderInput.Text=args[0];Call(form,"SetProfileEnabled");Application.DoEvents();Check(!inherit.Visible&&servers.Enabled,"Operational firmware must show only instance selection");
   inherit.Checked=true;Application.DoEvents();
   checks+=TerminalTests.Run(form,key,password,folder);
   if(args.Contains("--preview")){form.Text="Studio - revisão visual isolada";Application.Run(form);}
   form.Size=new System.Drawing.Size(1150,900);Application.DoEvents();
   using(var screenshot=new System.Drawing.Bitmap(form.Width,form.Height)){form.DrawToBitmap(screenshot,new System.Drawing.Rectangle(0,0,form.Width,form.Height));screenshot.Save(Path.Combine(folder,"usb-preview.png"));}
   var workspace=Field<TabControl>(form,"compilationSections");
   for(int section=0;section<workspace.TabCount;section++){
    workspace.SelectedIndex=section;Application.DoEvents();
    using(var screenshot=new System.Drawing.Bitmap(form.Width,form.Height)){form.DrawToBitmap(screenshot,new System.Drawing.Rectangle(0,0,form.Width,form.Height));screenshot.Save(Path.Combine(folder,"workspace-"+section+".png"));}
   }
   Check(Field<Button>(form,"usbUploadButton").Width>120,"USB button is clipped");
   Check(Field<TextBox>(form,"logBox").Height>=60,"USB controls hid the process log");
   form.Hide();
   string xml=File.ReadAllText(Path.Combine(folder,"projects.xml"));Check(!xml.Contains(key)&&!xml.Contains(password),"XML leaked credentials");
   Console.WriteLine(checks+" Studio checks passed; store isolated at "+folder);
  }
 } catch(Exception error) { Console.Error.WriteLine(error.ToString()); Environment.ExitCode=1; }
 }
}
