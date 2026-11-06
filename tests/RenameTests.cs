using System;
using System.IO;
using DeviceFirmwareStudio;
class RenameTests {
 [STAThread] static void Main(){
  string root=Path.Combine(Path.GetTempPath(),"studio-rename-"+Guid.NewGuid().ToString("N"));
  string oldStore=Path.Combine(root,"SentinelaFirmwareStudio","projects.xml"),newStore=Path.Combine(root,"DeviceFirmwareStudio","projects.xml");
  Directory.CreateDirectory(Path.GetDirectoryName(oldStore));
  string secret=ProfileSecurity.Protect("isolated-test-password");
  string xml="<?xml version=\"1.0\"?><AppSettings><Language>pt</Language></AppSettings>";
  File.WriteAllText(oldStore,xml);MainForm.MigrateLegacySettings(root);
  if(!File.Exists(oldStore)||File.ReadAllText(newStore)!=xml)throw new Exception("Migration changed the old settings");
  using(var form=new MainForm(Path.GetDirectoryName(newStore))){if(form.Text!="Device Firmware Studio")throw new Exception("Incorrect window title");}
  if(ProfileSecurity.Reveal(secret)!="isolated-test-password")throw new Exception("Credential protection changed");
  File.WriteAllText(newStore,"preserve destination");MainForm.MigrateLegacySettings(root);
  if(File.ReadAllText(newStore)!="preserve destination")throw new Exception("Destination overwritten");
  Console.WriteLine("Rename checks passed: title, migration, previous store, destination preservation, encrypted credentials.");
 }
}