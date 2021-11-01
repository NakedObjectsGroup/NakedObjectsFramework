using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
   public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<AddressType> builder)
        {
            builder.HasKey(t => t.AddressTypeID);

            // Ignores
            builder.Ignore(t => t.Name);

            // Table & Column Mappings
            builder.ToTable("AddressType", "Person");
            builder.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
