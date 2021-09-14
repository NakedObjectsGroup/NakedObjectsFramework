using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class BusinessEntityMap : EntityTypeConfiguration<BusinessEntity> {
        public BusinessEntityMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("BusinessEntity", "Person");
            Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            HasMany(t => t.Addresses).WithRequired(t => t.BusinessEntity);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<BusinessEntity> builder) {
            builder.HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            builder.ToTable("BusinessEntity", "Person");
            builder.Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            builder.Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            builder.HasMany(t => t.Addresses).WithOne(t => t.BusinessEntity);
        }
    }
}