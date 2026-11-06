using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using DeviceFirmwareStudio;
class GlobalInstanceTests {
 static int checks;
 static void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
 static T Field<T>(object target,string name){return (T)target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target);}
 static object Call(object target,string name,params object[] args){return target.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,args);}
 [STAThread] static void Main(string[] args){try{
  string root=Path.Combine(Path.GetTempPath(),"studio-instance-tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
  string key="isolated-key-00000000000000000000000";
  using(var form=new MainForm(root)){
   var settings=Field<AppSettings>(form,"settings");settings.Companies=new List<CompanyProfile>{new CompanyProfile{Id="a",Name="Company A"},new CompanyProfile{Id="b",Name="Company B"}};
   settings.Servers=new List<ServerProfile>{new ServerProfile{Id="a1",CompanyId="a",Name="A",BaseUrl="https://one.invalid/",ProtectedKey=ProfileSecurity.Protect(key)},new ServerProfile{Id="b1",CompanyId="b",Name="B",BaseUrl="https://one.invalid",ProtectedKey=ProfileSecurity.Protect(key)},new ServerProfile{Id="b2",CompanyId="b",Name="Second",BaseUrl="http://two.invalid",ProtectedKey=ProfileSecurity.Protect(key)}};
   settings.WifiNetworks=new List<WifiProfile>{new WifiProfile{Id="wifi",CompanyId="a",Name="Initial",Ssid="Test SSID",ProtectedPassword=ProfileSecurity.Protect("isolated-password")}};
   var projects=Field<ListBox>(form,"projects");projects.DataSource=null;projects.Items.Clear();
   foreach(string kind in new[]{"production","refuge","bootstrap"}){string folder=Path.Combine(root,kind);Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"firmware-profile.json"),"{\"schema\":1,\"kind\":\""+kind+"\"}");projects.Items.Add(new FirmwareProject{Name=kind,Folder=folder,Environment="esp32dev",CompanyId="a",ServerId="b1",WifiId="wifi"});}
   var servers=Field<ComboBox>(form,"serverProfileBox");var companies=Field<ComboBox>(form,"companyProfileBox");var wifi=Field<ComboBox>(form,"wifiProfileBox");
   form.Show();Application.DoEvents();
   for(int index=0;index<2;index++){
    projects.SelectedIndex=index;Call(form,"RefreshProfileData");Application.DoEvents();
    Check(servers.Items.Count==2,"Duplicate instance URLs were not grouped");Check(servers.Enabled&&servers.Visible,"Global instance selector is unavailable");
    Check(!companies.Visible&&!wifi.Visible&&!Field<ComboBox>(form,"wifiModeBox").Visible,"Global build shows company or Wi-Fi");
    Check(!Field<CheckBox>(form,"adoptProfileBox").Visible&&!Field<Button>(form,"profileTestButton").Visible&&!Field<Label>(form,"profileTestLabel").Visible,"Global build exposes migration or company test");
    Check((string)Call(form,"SelectedDestinationUrl")=="https://one.invalid","Equivalent saved URL selection was lost");
    string json=(string)Call(form,"SelectedProfileJson");Check(json.Contains("inherit")&&!json.Contains("company")&&!json.Contains("baseUrl")&&!json.Contains(key),"Global payload embeds destination or credentials");
    string fingerprint=(string)Call(form,"SelectedProfileFingerprint",json);servers.SelectedItem=servers.Items.Cast<ServerProfile>().First(server=>server.Id=="b2");
    Check((string)Call(form,"SelectedProfileFingerprint",json)!=fingerprint,"Artifact identity ignores target instance");
    Check((bool)Call(form,"PrepareProfileBuild"),"Global profile preparation failed");string buildRoot=Field<string>(form,"activeProfileBuildRoot");
    Check(Field<string>(form,"activeProfileServer")=="http://two.invalid"&&Field<string>(form,"activeProfileCompany")==null,"Build metadata has wrong target or company");
    string buildProfile=File.ReadAllText(Path.Combine(buildRoot,"profile.json"));Check(!buildProfile.Contains("two.invalid")&&!buildProfile.Contains(key),"Private build input changed global firmware identity");
    string output=Path.Combine(buildRoot,"build","esp32dev");Directory.CreateDirectory(output);File.WriteAllBytes(Path.Combine(output,"firmware.bin"),new byte[]{1,2,3,4});Call(form,"FinishProfileBuild",true);
    var project=(FirmwareProject)projects.SelectedItem;string manifest=File.ReadAllText(Path.Combine(project.LastArtifactDirectory,"profile-manifest.json"));Check(manifest.Contains("http://two.invalid")&&!manifest.Contains(key),"Export manifest lost target or leaked credentials");
    Check(Call(form,"GetMainFirmwarePath")!=null,"Compiled artifact unavailable");servers.SelectedItem=servers.Items.Cast<ServerProfile>().First(server=>server.Id=="a1");Check(Call(form,"GetMainFirmwarePath")==null,"Wrong-instance artifact allowed");
    string selectedServer=project.ServerId;Call(form,"RefreshProfileData");Check(((ServerProfile)servers.SelectedItem).Id==selectedServer,"Instance selection was not preserved");
    if(index==0){form.Size=new Size(1200,900);Application.DoEvents();using(var shot=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(shot,new Rectangle(0,0,shot.Width,shot.Height));shot.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"global-instance-preview.png"));}}
   }
   projects.SelectedIndex=2;Call(form,"RefreshProfileData");Application.DoEvents();Check(companies.Visible&&wifi.Visible,"Initial firmware lost company or Wi-Fi controls");
   Check(servers.Items.Count==1&&servers.Items.Cast<ServerProfile>().All(server=>server.CompanyId=="a"),"Initial firmware server filtering crossed companies");servers.SelectedIndex=0;wifi.SelectedIndex=0;
   string initialJson=(string)Call(form,"SelectedProfileJson");Check(initialJson.Contains("installation")&&initialJson.Contains("empresaId")&&initialJson.Contains("Test SSID"),"Initial installation configuration changed");
   Check(!Field<CheckBox>(form,"inheritProfileBox").Checked,"Initial firmware inherits instead of installing");
   checks+=TerminalTests.Run(form,key,"isolated-password",root);
   Console.WriteLine(checks+" instance and terminal checks passed.");
  }
 }catch(Exception error){Console.Error.WriteLine(error);Environment.ExitCode=1;}}
}