using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class AddressTypeMap : EntityTypeConfiguration<AddressType>
    {
        public AddressTypeMap()
        {
            // Primary Key
            HasKey(t => t.AddressTypeID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("AddressType", "Person");
            Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<AddressType> builder)
        {
            builder.HasKey(t => t.AddressTypeID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("AddressType", "Person");
            builder.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
