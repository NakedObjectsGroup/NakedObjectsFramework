using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ScrapReasonMap : EntityTypeConfiguration<ScrapReason>
    {
        public ScrapReasonMap()
        {
            // Primary Key
            HasKey(t => t.ScrapReasonID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ScrapReason", "Production");
            Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
