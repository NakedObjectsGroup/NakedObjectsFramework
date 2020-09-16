// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;

namespace NakedFunctions.Rest.Test.Data {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<TestDbContext> {
        protected override void Seed(TestDbContext context) {
            context.SimpleRecords.Add(new SimpleRecord {Name = "Fred"});
            context.SaveChanges();
        }
    }

    public class TestDbContext : DbContext {
        public const string DatabaseName = "FunctionRestTests";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public TestDbContext() : base(Cs) { }

        public DbSet<SimpleRecord> SimpleRecords { get; set; }

        public static void Delete() => Database.Delete(Cs);

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }
}