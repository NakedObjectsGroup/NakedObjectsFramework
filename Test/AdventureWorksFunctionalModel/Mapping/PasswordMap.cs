using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping {
    public class PasswordMap : EntityTypeConfiguration<Password> {
        public PasswordMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            this.ToTable("Password", "Person");
        }
    }
}