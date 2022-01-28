using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
   public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<AddressType> builder)
        {
            builder.HasKey(t => t.AddressTypeID);

            // Ignores
            builder.Ignore(t => t.mappedName);

            // Table & Column Mappings
            builder.ToTable("AddressType", "Person");
            builder.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
