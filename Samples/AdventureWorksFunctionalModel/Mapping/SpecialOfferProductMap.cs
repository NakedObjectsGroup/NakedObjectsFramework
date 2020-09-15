using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class SpecialOfferProductMap : EntityTypeConfiguration<SpecialOfferProduct>
    {
        public SpecialOfferProductMap()
        {
            // Primary Key
            HasKey(t => new { t.SpecialOfferID, t.ProductID });

            // Properties
            Property(t => t.SpecialOfferID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SpecialOfferProduct", "Sales");
            Property(t => t.SpecialOfferID).HasColumnName("SpecialOfferID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.SpecialOfferProduct)
                .HasForeignKey(d => d.ProductID);
            HasRequired(t => t.SpecialOffer).WithMany().HasForeignKey(t => t.SpecialOfferID);

        }
    }
}
