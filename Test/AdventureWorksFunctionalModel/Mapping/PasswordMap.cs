using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#pragma warning disable 8603

namespace AW.Mapping {
    public class PasswordMap : EntityTypeConfiguration<Password> {
        public PasswordMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("Password", "Person");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.PasswordHash).HasColumnName("PasswordHash").IsRequired();
            Property(t => t.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
            HasRequired(pw => pw.Person);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<Password> builder) {
            builder.HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            builder.ToTable("Password", "Person");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.PasswordHash).HasColumnName("PasswordHash").IsRequired();
            builder.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
            builder.HasOne(pw => pw.Person).WithOne(p => p.Password);
        }
    }
}