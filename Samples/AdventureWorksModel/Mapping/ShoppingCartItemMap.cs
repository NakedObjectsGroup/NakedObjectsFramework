using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            // Primary Key
            HasKey(t => t.ShoppingCartItemID);

            // Properties
            Property(t => t.ShoppingCartID)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ShoppingCartItem", "Sales");
            Property(t => t.ShoppingCartItemID).HasColumnName("ShoppingCartItemID");
            Property(t => t.ShoppingCartID).HasColumnName("ShoppingCartID");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.DateCreated).HasColumnName("DateCreated");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }
}
