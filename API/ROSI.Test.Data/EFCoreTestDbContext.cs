// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.EntityFrameworkCore;

namespace ROSI.Test.Data;

public static class Constants {
    public static string AppveyorServer => @"(local)\SQL2017";
    public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
    public static string Server => LocalServer;
#endif

    public static readonly string CsROSI = @$"Data Source={Server};Initial Catalog={"ROSITests"};Integrated Security=True;";
}

public abstract class EFCoreTestDbContext : DbContext {
    private readonly string cs;

    protected EFCoreTestDbContext(string cs) => this.cs = cs;

    public DbSet<Class> Classes { get; set; }

    public DbSet<ClassWithActions> ClassesWithActions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }

    private static void MapClass(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Class>().Property("Property1").HasColumnName("Property1");
        modelBuilder.Entity<Class>().Property("Property2").HasColumnName("Property2");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        MapClass(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Class>().HasData(new Class { Id = 1, Property1 = "One", Property2 = 2 });
        modelBuilder.Entity<ClassWithActions>().HasData(new ClassWithActions { Id = 1});
    }
}

public class EFCoreObjectDbContext : EFCoreTestDbContext {
    public EFCoreObjectDbContext() : base(Constants.CsROSI) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}