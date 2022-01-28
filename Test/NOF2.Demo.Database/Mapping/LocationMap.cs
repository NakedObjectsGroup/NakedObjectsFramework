using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(t => t.LocationID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Location", "Production");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedCostRate).HasColumnName("CostRate");
            builder.Property(t => t.mappedAvailability).HasColumnName("Availability");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
