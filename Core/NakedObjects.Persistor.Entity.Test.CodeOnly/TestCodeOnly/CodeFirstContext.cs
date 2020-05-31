// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace TestCodeOnly {
    public class CodeFirstContext : DbContext {
        public CodeFirstContext(string cs) : base(cs) { }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CountryCode> CountryCodes { get; set; }
        public DbSet<DomesticAddress> DomesticAddresses { get; set; }
        public DbSet<InternationalAddress> InternationalAddresses { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}