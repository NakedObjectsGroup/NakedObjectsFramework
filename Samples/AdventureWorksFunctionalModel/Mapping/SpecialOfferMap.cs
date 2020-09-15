using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SpecialOfferMap : EntityTypeConfiguration<SpecialOffer>
    {
        public SpecialOfferMap()
        {
            // Primary Key
            HasKey(t => t.SpecialOfferID);

            // Properties
            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(255);

            Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("SpecialOffer", "Sales");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.DiscountPct).HasColumnName("DiscountPct");
            Property(t => t.Type).HasColumnName("Type");
            Property(t => t.Category).HasColumnName("Category");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.MinQty).HasColumnName("MinQty");
            Property(t => t.MaxQty).HasColumnName("MaxQty");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
