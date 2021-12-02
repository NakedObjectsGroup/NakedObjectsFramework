// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

using Microsoft.EntityFrameworkCore;

namespace RestfulObjects.Test.Data;

public class EFCoreCodeFirstContext : DbContext {
    private readonly string cs;

    public EFCoreCodeFirstContext(string cs) => this.cs = cs;

    public DbSet<Immutable> Immutables { get; set; }
    public DbSet<MostSimple> MostSimples { get; set; }
    public DbSet<RedirectedObject> RedirectedObjects { get; set; }
    public DbSet<VerySimple> VerySimples { get; set; }
    public DbSet<VerySimpleEager> VerySimpleEagers { get; set; }
    public DbSet<WithActionObject> WithActionObjects { get; set; }
    public DbSet<WithAttachments> WithAttachments { get; set; }
    public DbSet<WithCollection> WithCollections { get; set; }
    public DbSet<WithDateTimeKey> WithDateTimeKeys { get; set; }
    public DbSet<WithError> WithErrors { get; set; }
    public DbSet<WithGetError> WithGetErrors { get; set; }
    public DbSet<WithReference> WithReferences { get; set; }
    public DbSet<WithScalars> WithScalarses { get; set; }
    public DbSet<WithValue> WithValues { get; set; }

    public DbSet<MostSimplePersist> MostSimplePersists { get; set; }
    public DbSet<VerySimplePersist> VerySimplePersists { get; set; }
    public DbSet<WithValuePersist> WithValuePersists { get; set; }
    public DbSet<WithReferencePersist> WithReferencePersists { get; set; }
    public DbSet<WithCollectionPersist> WithCollectionPersists { get; set; }

    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }
}