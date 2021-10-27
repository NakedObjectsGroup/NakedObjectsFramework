using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping {
    public class PasswordMap : EntityTypeConfiguration<Password> {
        public PasswordMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            this.ToTable("Password", "Person");
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Password> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            builder.ToTable("Password", "Person");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            builder.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
            //builder.HasOne(pw => pw.Person).WithOne(p => p.Password);
        }
    }
}