using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductDescriptionMap : EntityTypeConfiguration<ProductDescription>
    {
        public ProductDescriptionMap()
        {
            // Primary Key
            HasKey(t => t.ProductDescriptionID);

            // Properties
            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            ToTable("ProductDescription", "Production");
            Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
