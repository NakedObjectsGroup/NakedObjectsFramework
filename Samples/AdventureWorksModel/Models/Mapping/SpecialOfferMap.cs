using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SpecialOfferMap : EntityTypeConfiguration<SpecialOffer>
    {
        public SpecialOfferMap()
        {
            // Primary Key
            this.HasKey(t => t.SpecialOfferID);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SpecialOffer", "Sales");
            this.Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DiscountPct).HasColumnName("DiscountPct");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.MinQty).HasColumnName("MinQty");
            this.Property(t => t.MaxQty).HasColumnName("MaxQty");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
