using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductModelMap : EntityTypeConfiguration<ProductModel>
    {
        public ProductModelMap()
        {
            // Primary Key
            HasKey(t => t.ProductModelID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductModel", "Production");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CatalogDescription).HasColumnName("CatalogDescription");
            Property(t => t.Instructions).HasColumnName("Instructions");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
