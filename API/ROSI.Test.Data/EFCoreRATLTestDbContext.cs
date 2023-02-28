// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.EntityFrameworkCore;

namespace ROSI.Test.Data;


public  class EFCoreRATLTestDbContext : DbContext {
    private readonly string cs;

    private EFCoreRATLTestDbContext(string cs) => this.cs = cs;

    public EFCoreRATLTestDbContext() : this(Constants.CsRATL) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();

    public DbSet<Class> Classes { get; set; }

    public DbSet<Object1> Object1s { get; set; }

    public DbSet<Object2> Object2s { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }

    private static void MapClass(ModelBuilder modelBuilder) {
       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        MapClass(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Object1>().HasData(new Object1 { Id = 1, Prop1 = 1, Prop2 = "foo"});
        modelBuilder.Entity<Object2>().HasData(new Object2 { Id = 1});
      
    }
}