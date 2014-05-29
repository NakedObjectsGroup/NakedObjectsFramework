// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace NakedObjects.Xat.Database {
// ReSharper restore CheckNamespace
    // These are for use by tests

    /// <summary>
    /// DatabaseUtils.BackupDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.SnapshotDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.RestoreDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.RevertDatabaseFromSnapshot("AdventureWorks", "startstate");
    /// </summary>
    public static class DatabaseUtils {
        public static void BackupDatabase(string databaseName, string backupName, string serverName = @".\SQLEXPRESS") {
            Server server = string.IsNullOrEmpty(serverName) ? new Server() : new Server(serverName);
            var backup = new Backup {
                Database = databaseName,
                Action = BackupActionType.Database,
                Initialize = true
            };

            backup.Devices.AddDevice(backupName + ".bak", DeviceType.File);
            backup.SqlBackup(server);
        }

        public static void RestoreDatabase(string databaseName, string backupName, string serverName = @".\SQLEXPRESS") {
            Server server = string.IsNullOrEmpty(serverName) ? new Server() : new Server(serverName);
            var restore = new Restore {
                Database = databaseName,
                Action = RestoreActionType.Database,
                ReplaceDatabase = true,
                NoRecovery = false
            };

            restore.Devices.AddDevice(backupName + ".bak", DeviceType.File);
            server.KillAllProcesses(databaseName);
            restore.SqlRestore(server);
        }

        private static string GetDefaultPathForSnapshot(string snapshotName) {
            string snapshotFile = snapshotName;

            if (!Path.HasExtension(snapshotFile)) {
                snapshotFile += ".snap";
            }
            if (Path.IsPathRooted(snapshotFile)) {
                return snapshotFile;
            }
            string snapshotLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"NakedObjects\test snapshots");
            if (!Directory.Exists(snapshotLocation)) {
                Directory.CreateDirectory(snapshotLocation);
            }
            return Path.Combine(snapshotLocation, snapshotFile);
        }

        private const string conString = "server={0}; database=master; Integrated Security=SSPI;";

        public static void SnapshotDatabase(string databaseName, string snapshotName, string server = "(local)") {
            string connectionString = string.Format(conString, server);
            using (var connection = new SqlConnection(connectionString)) {
                using (var command = new SqlCommand()) {
                    command.Connection = connection;
                    command.CommandTimeout = 1000;
                    string snapshotFile = GetDefaultPathForSnapshot(snapshotName);
                    string logicalName = new Server().Databases[databaseName].FileGroups[0].Files[0].Name;

                    command.CommandText = string.Format("CREATE DATABASE {0} ON (NAME = {1}, FILENAME = '{2}') AS SNAPSHOT OF {3};",
                                                        Path.GetFileNameWithoutExtension(snapshotName),
                                                        logicalName,
                                                        snapshotFile,
                                                        databaseName);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void RevertDatabaseFromSnapshot(string databaseName, string snapshotName, string server = "(local)") {
            string connectionString = string.Format(conString, server);
            new Server().KillAllProcesses(databaseName);
            using (var connection = new SqlConnection(connectionString)) {
                using (var command = new SqlCommand()) {
                    command.Connection = connection;
                    command.CommandTimeout = 1000;
                    command.CommandText = string.Format("RESTORE DATABASE {0} FROM DATABASE_SNAPSHOT = '{1}'; DROP DATABASE {1};",
                                                        databaseName,
                                                        snapshotName);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}