using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using DeviceFirmwareStudio;
class ServerDialogTests {
 static int checks;
 static void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
 static IEnumerable<Control> All(Control root){foreach(Control child in root.Controls){yield return child;foreach(Control nested in All(child))yield return nested;}}
 [STAThread] static void Main(){try{
  string key=new string('x',32);
  Check(!MainForm.ValidServerFields(new[]{"Test","https://server.invalid",""},null),"New server accepts blank key");
  Check(MainForm.ValidServerFields(new[]{"Test","https://server.invalid",key},null),"Valid new server refused");
  Check(MainForm.ValidServerFields(new[]{"Test","https://server.invalid",""},key),"Editing fails to preserve existing key");
  Check(!MainForm.ValidServerFields(new[]{"Test","invalid",key},null),"Invalid URL accepted");
  Check(!MainForm.ValidServerFields(new[]{" ","https://server.invalid",key},null),"Empty name accepted");
  Check(!MainForm.ValidServerFields(new[]{"Test","https://server.invalid","short"},key),"Invalid replacement key accepted");
  string folder=Path.Combine(Path.GetTempPath(),"studio-server-tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);
  using(var owner=new MainForm(folder)){
   Exception failure=null;var timer=new Timer{Interval=150};
   timer.Tick+=delegate{timer.Stop();Form dialog=Application.OpenForms.Cast<Form>().First(form=>form!=owner);
    try{
     var controls=All(dialog).ToList();var inputs=controls.OfType<TextBox>().ToList();var save=(Button)dialog.AcceptButton;
     Check(controls.OfType<Label>().Any(label=>label.Text=="Empresa: Test company — 123"),"Missing company identity");
     Check(!save.Enabled,"Save initially enabled");inputs[0].Text="Test";inputs[1].Text="https://server.invalid";Check(!save.Enabled,"Blank key enabled Save");inputs[2].Text=key;Check(save.Enabled,"Valid fields did not enable Save");inputs[1].Text="invalid";Check(!save.Enabled,"Invalid URL did not disable Save");inputs[1].Text="https://server.invalid";
     Check(save.TextImageRelation==TextImageRelation.ImageBeforeText&&save.Height>=36,"Button icon overlaps text");
     using(var image=new Bitmap(dialog.Width,dialog.Height)){dialog.DrawToBitmap(image,new Rectangle(0,0,image.Width,image.Height));image.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"server-dialog-preview.png"));}
    }catch(Exception error){failure=error;}finally{dialog.DialogResult=DialogResult.Cancel;dialog.Close();}
   };
   timer.Start();typeof(MainForm).GetMethod("ProfileDialog",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(owner,new object[]{"Cadastrar servidor / URL",new[]{"Nome / ambiente","URL","Chave API (obrigatória)"},new string[3],2,false,null,null,"Empresa: Test company — 123",new Func<string[],bool>(values=>MainForm.ValidServerFields(values,null))});timer.Dispose();if(failure!=null)throw failure;
  }
  Console.WriteLine(checks+" server dialog checks passed.");
 }catch(Exception error){Console.Error.WriteLine(error);Environment.ExitCode=1;}}
}