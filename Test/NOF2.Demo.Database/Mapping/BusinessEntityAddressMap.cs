using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model {

  public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BusinessEntityAddress> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.AddressID, t.AddressTypeID });

            // Table & Column Mappings
            builder.ToTable("BusinessEntityAddress", "Person");
            builder.Property(t => t.AddressID).HasColumnName("AddressID");
            builder.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            //Relationships
            builder.HasOne(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            builder.HasOne(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            builder.HasOne(t => t.BusinessEntity).WithMany(t => t.mappedAddresses).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}