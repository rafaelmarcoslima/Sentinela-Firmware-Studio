using System;
using System.IO;
using System.Linq;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SentinelaFirmwareStudio;

static class TerminalTests
{
    sealed class FakeSerial : IMonitorConnection
    {
        public event EventHandler DataAvailable;
        public bool IsOpen { get; private set; }
        public bool Disposed;
        public bool FailOpen;
        string pending="";
        public void Open() { if(FailOpen)throw new IOException("Port busy");IsOpen=true; }
        public string ReadExisting() { string value=pending;pending="";return value; }
        public void Emit(string value) { pending+=value;var handler=DataAvailable;if(handler!=null)handler(this,EventArgs.Empty); }
        public void Dispose() { IsOpen=false;Disposed=true; }
    }
    static int checks;
    static void Check(bool value,string message) { if(!value)throw new Exception(message);checks++; }
    static T Field<T>(object obj,string name) { return (T)obj.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(obj); }
    static void Set(object obj,string name,object value) { obj.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(obj,value); }
    static object Call(object obj,string name,params object[] args) { return obj.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(obj,args); }
    static void Wait(Task task) { var watch=System.Diagnostics.Stopwatch.StartNew();while(!task.IsCompleted&&watch.ElapsedMilliseconds<10000) { Application.DoEvents();Thread.Sleep(10); }Check(task.IsCompleted,"Monitor operation timed out");task.GetAwaiter().GetResult(); }
    public static int Run(MainForm form,string key,string password,string outputFolder)
    {
        var history=new TerminalHistory(4096);history.Append(new string('x',8000));int version;Check(history.Read(out version).Length==4096,"Terminal history is unbounded");history.Clear();Check(history.Read(out version)=="","Clear did not reset history");int rendered=-1;string snapshot;Check(history.TryRead(ref rendered,out snapshot)&&!history.TryRead(ref rendered,out snapshot),"Idle terminal repeatedly copies its buffer");
        var concurrent=new TerminalHistory(100000);Parallel.For(0,1000,i=>concurrent.Append("test\n"));Check(concurrent.Read(out version).Length==5000,"Concurrent terminal messages were lost");
        using(var native=new NativeMonitorConnection("COM42",115200)) { var port=(SerialPort)typeof(NativeMonitorConnection).GetField("port",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(native);Check(!port.DtrEnable&&!port.RtsEnable&&!port.IsOpen,"Monitor can reset/open a device before explicit connect"); }
        FakeSerial current=null;int connections=0;
        Func<string,int,IMonitorConnection> factory=(name,baud)=>{Check(name=="COM42"&&baud==115200,"Wrong monitor target or baud");connections++;current=new FakeSerial();return current;};
        Set(form,"monitorFactory",factory);Set(form,"monitorPortProvider",new Func<string[]>(()=>new[]{"COM42"}));
        Call(form,"RefreshMonitorPorts");var ports=Field<ComboBox>(form,"monitorPortBox");ports.SelectedItem="COM42";Call(form,"RefreshMonitorControls");
        Check(Field<Button>(form,"monitorConnectButton").Enabled,"Selecting a port did not enable monitor connection");
        Wait((Task)Call(form,"StartMonitorConnection"));Check(current.IsOpen,"Monitor did not connect");
        Wait((Task)Call(form,"StartMonitorConnection"));Check(connections==1,"Monitor opened duplicate serial connections");
        Call(form,"ShowMonitorView");Application.DoEvents();
        current.Emit("ESP connected\n\x1b[32mgreen-line\x1b[0m\n"+key+"\n"+password+"\n");Call(form,"FlushTerminalViews");var monitor=Field<TextBox>(form,"monitorBox");Check(monitor.Text.Contains("ESP connected")&&!monitor.Text.Contains(key)&&!monitor.Text.Contains(password),"Serial output missing or leaking credentials");Check(monitor.Text.Contains("ESP connected\r\n")&&!monitor.Text.Contains("\x1b"),"Serial newlines or color control sequences are unreadable");
        Field<Button>(form,"monitorPauseButton").PerformClick();current.Emit("paused-message\n");Call(form,"FlushTerminalViews");Check(!monitor.Text.Contains("paused-message"),"Pause still updates the monitor view");
        Field<Button>(form,"monitorPauseButton").PerformClick();Check(monitor.Text.Contains("paused-message"),"Resume lost buffered messages");
        var sections=Field<TabControl>(form,"compilationSections");Check(sections.TabCount==4,"Compilation tools are not grouped into four sections");
        var liveConnection=Field<IMonitorConnection>(form,"monitorConnection");for(int section=0;section<sections.TabCount;section++){sections.SelectedIndex=section;Application.DoEvents();Check(ReferenceEquals(liveConnection,Field<IMonitorConnection>(form,"monitorConnection"))&&current.IsOpen,"Changing sections interrupted the serial monitor");}
        Call(form,"ShowMonitorView");Application.DoEvents();
        Check(Field<TextBox>(form,"logBox").ScrollBars==ScrollBars.None&&monitor.ScrollBars==ScrollBars.None,"Terminals still use white native scrollbars");
        Check(Field<TextBox>(form,"logBox").Parent is ModernTextHost&&monitor.Parent is ModernTextHost,"Dark terminal scrolling host missing");
        var dock=Field<Panel>(form,"terminalHost");Call(form,"ShowMonitorView");Application.DoEvents();Check(dock.Bottom<=dock.Parent.ClientSize.Height,"Terminal is clipped outside the scrollable layout");
        Call(form,"OpenExpandedTerminals",2);Application.DoEvents();var popup=Field<Form>(form,"expandedTerminals");var split=Field<SplitContainer>(form,"expandedSplit");Check(!split.Panel1Collapsed&&!split.Panel2Collapsed,"Side-by-side mode hides a terminal");
        var sameConnection=Field<IMonitorConnection>(form,"monitorConnection");Call(form,"SetBusy",true);Call(form,"ShowBuildProgress","Compilando...");Check(form.Enabled&&Field<TextBox>(form,"logBox").Enabled&&monitor.Enabled,"Build disables terminal access or parent-window contrast");Check(Field<Button>(form,"monitorPauseButton").Enabled&&!Field<Button>(form,"buildButton").Enabled,"Build operation did not separate terminal tools from configuration controls");
        Call(form,"AppendLog","build-data "+key+"\n");Call(form,"FlushTerminalViews");Check(Field<TextBox>(form,"logBox").Text.Contains("build-data")&&!Field<TextBox>(form,"logBox").Text.Contains(key),"Expanded build log not synchronized or leaked credentials");
        Call(form,"SetTerminalMode",0);Check(split.Panel2Collapsed&&!split.Panel1Collapsed,"Build-only mode invalid");Call(form,"SetTerminalMode",1);Check(split.Panel1Collapsed&&!split.Panel2Collapsed,"Monitor-only mode invalid");Call(form,"SetTerminalMode",2);
        popup.Size=new System.Drawing.Size(1200,750);Application.DoEvents();using(var image=new System.Drawing.Bitmap(popup.Width,popup.Height)){popup.DrawToBitmap(image,new System.Drawing.Rectangle(0,0,popup.Width,popup.Height));image.Save(Path.Combine(outputFolder,"terminals-preview.png"));}
        popup.Close();Application.DoEvents();Check(Field<Form>(form,"expandedTerminals")==null&&ReferenceEquals(sameConnection,Field<IMonitorConnection>(form,"monitorConnection"))&&current.IsOpen,"Closing expanded view disconnected the device");
        Call(form,"SetBusy",false);Call(form,"SetTerminalOperation","",false);
        Wait((Task)Call(form,"SuspendMonitorForUpload","COM99"));Check(current.IsOpen,"Upload to another port disconnected this monitor");
        Set(form,"usbUploadInProgress",true);Set(form,"usbUploadPort","COM42");Call(form,"SetBusy",true);var old=current;
        Wait((Task)Call(form,"SuspendMonitorForUpload","COM42"));Check(old.Disposed&&Field<IMonitorConnection>(form,"monitorConnection")==null,"Monitor did not release port before upload");
        Wait((Task)Call(form,"StartMonitorConnection"));Check(connections==1,"Monitor reopened the upload port before flashing finished");
        Set(form,"usbUploadInProgress",false);Set(form,"usbUploadPort",null);Call(form,"SetBusy",false);Call(form,"ResumeMonitorAfterUpload");
        var deadline=System.Diagnostics.Stopwatch.StartNew();while(connections<2&&deadline.ElapsedMilliseconds<4000){Application.DoEvents();Thread.Sleep(10);}Check(connections==2,"Monitor did not reconnect after upload");
        Wait((Task)Call(form,"StartMonitorConnection"));Check(current.IsOpen,"Automatic reconnection did not open the port");Wait((Task)Call(form,"DisconnectMonitor"));Check(current.Disposed,"Disconnect did not dispose the serial connection");
        Set(form,"monitorFactory",new Func<string,int,IMonitorConnection>((name,baud)=>new FakeSerial{FailOpen=true}));Wait((Task)Call(form,"StartMonitorConnection"));Check(Field<IMonitorConnection>(form,"monitorConnection")==null&&monitor.Text.Contains("Port busy"),"Busy port failure was not reported safely");
        Call(form,"ShowMonitorView");Call(form,"SetTerminalMode",0);Application.DoEvents();
        Call(form,"AppendLog",string.Join("\n",Enumerable.Repeat(new string('x',400),180).ToArray()));Call(form,"FlushTerminalViews");
        var build=Field<TextBox>(form,"logBox");var host=(ModernTextHost)build.Parent;
        var vertical=host.Controls.OfType<ModernScrollBar>().First(bar=>!bar.Horizontal);
        Check(vertical.Maximum>0&&vertical.Value>0,"Dark vertical scrollbar does not track the log");
        Set(form,"buildAutoScroll",false);vertical.Value=0;Call(form,"AppendLog","\nnew line");Call(form,"FlushTerminalViews");Check(vertical.Value==0,"Disabled auto-scroll still jumps to the end");Set(form,"buildAutoScroll",true);
        return checks;
    }
}
