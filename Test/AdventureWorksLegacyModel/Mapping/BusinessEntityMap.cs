using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {
    public class BusinessEntityMap : EntityTypeConfiguration<BusinessEntity>
    {
        public BusinessEntityMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("BusinessEntity", "Person");
            Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            HasMany(t => t.Addresses).WithRequired(t => t.BusinessEntity);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BusinessEntity> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            builder.ToTable("BusinessEntity", "Person");
            builder.Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            builder.Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            builder.HasMany(t => t.Addresses).WithOne(t => t.BusinessEntity).HasForeignKey(t => t.BusinessEntityID);
            builder.HasMany(t => t.Contacts).WithOne(t => t.BusinessEntity).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}