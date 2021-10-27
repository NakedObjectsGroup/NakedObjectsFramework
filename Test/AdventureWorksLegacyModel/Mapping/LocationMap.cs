using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class LocationMap : EntityTypeConfiguration<Location>
    {
        public LocationMap()
        {
            // Primary Key
            HasKey(t => t.LocationID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Location", "Production");
            Property(t => t.LocationID).HasColumnName("LocationID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CostRate).HasColumnName("CostRate");
            Property(t => t.Availability).HasColumnName("Availability");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(t => t.LocationID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Location", "Production");
            builder.Property(t => t.LocationID).HasColumnName("LocationID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.CostRate).HasColumnName("CostRate");
            builder.Property(t => t.Availability).HasColumnName("Availability");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
