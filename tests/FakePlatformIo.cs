using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
class FakePlatformIo {
 static int Main(string[] args) {
  if(Environment.GetEnvironmentVariable("FIRMWARE_FACTORY_RESET")!="0")return 6;
  string file=Environment.GetEnvironmentVariable("FIRMWARE_PROFILE_FILE");
  string build=Environment.GetEnvironmentVariable("PLATFORMIO_BUILD_DIR");
  if(!File.Exists(file)||string.IsNullOrEmpty(build)||Array.IndexOf(args,"upload")<0||Array.IndexOf(args,"COM42")<0)return 4;
  var profile=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(File.ReadAllText(file));
  if(Convert.ToString(profile["empresaId"])!="company-a"||Convert.ToString(profile["ssid"])!="Test A"||Convert.ToString(profile["baseUrl"])!="https://a.invalid")return 5;
  if(!profile.ContainsKey("wifiApplyId")||Convert.ToString(profile["wifiApplyId"]).Length!=32)return 7;
  Console.WriteLine("Writing at 0x00010000... (100 %)");
  Console.WriteLine(Convert.ToString(profile["apiKey"]));
  Console.WriteLine(Convert.ToString(profile["password"]));
  Console.WriteLine("USB simulation complete");
  return 0;
 }
}
