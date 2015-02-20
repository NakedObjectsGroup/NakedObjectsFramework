// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace NakedObjects.DatabaseHelpers {
// ReSharper restore CheckNamespace
    // These are for use by tests

    /// <summary>
    /// DatabaseUtils.BackupDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.SnapshotDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.RestoreDatabase("AdventureWorks", "startstate");
    /// DatabaseUtils.RevertDatabaseFromSnapshot("AdventureWorks", "startstate");
    /// </summary>
    public static class DatabaseUtils {
#if AV 
        private const string Server = @"(local)\SQL2012SP1";
#else
        private const string Server = @".\SQLEXPRESS";
#endif

        public static void BackupDatabase(string databaseName, string backupName, string serverName = Server) {
            Server server = string.IsNullOrEmpty(serverName) ? new Server() : new Server(serverName);
            var backup = new Backup {
                Database = databaseName,
                Action = BackupActionType.Database,
                Initialize = true
            };

            backup.Devices.AddDevice(backupName + ".bak", DeviceType.File);
            backup.SqlBackup(server);
        }

        public static void RestoreDatabase(string databaseName, string backupName, string serverName = Server) {
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

        private const string ConString = "server={0}; database=master; Integrated Security=SSPI;";

        public static void SnapshotDatabase(string databaseName, string snapshotName, string server = Server) {
            string connectionString = string.Format(ConString, server);
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

        public static void RevertDatabaseFromSnapshot(string databaseName, string snapshotName, string server = Server) {
            string connectionString = string.Format(ConString, server);
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