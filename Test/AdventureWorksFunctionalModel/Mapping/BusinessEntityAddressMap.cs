using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class BusinessEntityAddressMap : EntityTypeConfiguration<BusinessEntityAddress>
    {
        public BusinessEntityAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.AddressTypeID, t.AddressID });

            // Table & Column Mappings
            ToTable("BusinessEntityAddress", "Person");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            //Relationships
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            HasRequired(t => t.BusinessEntity).WithMany().HasForeignKey(t => t.BusinessEntityID);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BusinessEntityAddress> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.AddressTypeID, t.AddressID });

            // Table & Column Mappings
            builder.ToTable("BusinessEntityAddress", "Person");
            builder.Property(t => t.AddressID).HasColumnName("AddressID");
            builder.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            //Relationships
            //builder.HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            //builder.HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            //builder.HasRequired(t => t.BusinessEntity).WithMany().HasForeignKey(t => t.BusinessEntityID);
        }
    }
}