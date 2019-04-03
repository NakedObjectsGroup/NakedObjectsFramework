using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
