// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;

namespace Template.RestTest.DomainModel
{
    public static class Constants {
        public static readonly string CsObject = @$"Data Source={Server};Initial Catalog={"Spike"};Integrated Security=True;";

        private static string LocalServer => @"(localdb)\MSSQLLocalDB;";

        private static string Server => LocalServer;
    }

    public class DatabaseInitializer<T> : DropCreateDatabaseAlways<T> where T : TestDbContext {
        protected override void Seed(T context) {
            // keep names 4 characters
            var fred = new Foo {Name = "Fred"};

            context.SimpleRecords.Add(fred);
            context.SimpleRecords.Add(new Foo {Name = "Bill"});
            context.SimpleRecords.Add(new Foo {Name = "Jack"});
            context.SimpleRecords.Add(new Foo {Name = "hide it"});

            context.SaveChanges();
        }
    }

    public abstract class TestDbContext : DbContext {
        protected TestDbContext(string cs) : base(cs) { }

        public DbSet<Foo> SimpleRecords { get; set; }

        protected void OnModelCreating<T>(DbModelBuilder modelBuilder) where T : TestDbContext {
            Database.SetInitializer(new DatabaseInitializer<T>());
        }
    }

    public class ObjectDbContext : TestDbContext {
        public ObjectDbContext(string cs) : base(cs) { }
        public ObjectDbContext() : base(Constants.CsObject) { }
        public static void Delete() => Database.Delete(Constants.CsObject);
        protected override void OnModelCreating(DbModelBuilder modelBuilder) => OnModelCreating<ObjectDbContext>(modelBuilder);
    }
}