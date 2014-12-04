//using Microsoft.SqlServer.Management.Smo;

namespace NakedObjects.Rest.App.Demo.Models {
    public class TestHelperService {
        //protected const string Server = @".\SqlExpress";
        //protected const string Database = "AdventureWorks";
        //protected const string Backup = "AdventureWorks";

        //public static void RestoreDatabase(string databaseName, string backupName, string serverName = @".\SQLEXPRESS") {
        //    Server server = string.IsNullOrEmpty(serverName) ? new Server() : new Server(serverName);
        //    var restore = new Restore {
        //        Database = databaseName,
        //        Action = RestoreActionType.Database,
        //        ReplaceDatabase = true,
        //        NoRecovery = false
        //    };

        //    restore.Devices.AddDevice(backupName + ".bak", DeviceType.File);
        //    server.KillAllProcesses(databaseName);
        //    restore.SqlRestore(server);
        //}

        //[QueryOnly]
        //public void ResetDatabase() {
        //    RestoreDatabase(Database, Backup, Server);
        //}
    }
}