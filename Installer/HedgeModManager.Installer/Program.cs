using System.Net;

//var path = "C:/Users/Sajid/Desktop/HedgeModManager.exe";
//var info = new ApplicationInfo()
//{
//    IsUserLocal = true,
//    DisplayName = "HedgeModManager",
//    DisplayVersion = "7.6-2",
//    UninstallAction = path,
//    Publisher = "NeverFinishAnything",
//    DisplayIcon = path,
//    NoModify = true,
//    NoRepair = true,
//    ID = "HedgeModManager"
//};

//info.Save();

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
var app = new Application(args);
await app.Run();