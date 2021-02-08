using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping {
    public class PasswordMap : EntityTypeConfiguration<Password> {
        public PasswordMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("Password", "Person");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

           // HasRequired(t => t.Person);
        }
    }
}