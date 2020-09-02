using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SalesReasonMap : EntityTypeConfiguration<SalesReason>
    {
        public SalesReasonMap()
        {
            // Primary Key
            HasKey(t => t.SalesReasonID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ReasonType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SalesReason", "Sales");
            Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ReasonType).HasColumnName("ReasonType");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
