// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.EntityFrameworkCore;

namespace Legacy.Rest.Test.Data {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif

        public static readonly string CsLegacy = @$"Data Source={Server};Initial Catalog={"LegacyTests"};Integrated Security=True;";
    }

    public abstract class EFCoreTestDbContext : DbContext {
        private readonly string cs;

        protected EFCoreTestDbContext(string cs) => this.cs = cs;

        public DbSet<SimpleClass> SimpleClasses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var fred = new SimpleClass { Id = 1, Name = "Fred" };

            //modelBuilder.Entity<SimpleClass>().Property();


            modelBuilder.Entity<SimpleClass>().HasData(fred);
        }
    }

    public class EFCoreObjectDbContext : EFCoreTestDbContext {
        public EFCoreObjectDbContext() : base(Constants.CsLegacy) { }
        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
    }
}