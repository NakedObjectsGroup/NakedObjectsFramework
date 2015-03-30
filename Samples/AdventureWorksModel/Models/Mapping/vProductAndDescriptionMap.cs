using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class vProductAndDescriptionMap : EntityTypeConfiguration<vProductAndDescription>
    {
        public vProductAndDescriptionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductID, t.Name, t.ProductModel, t.CultureID, t.Description });

            // Properties
            this.Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ProductModel)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CultureID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("vProductAndDescription", "Production");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ProductModel).HasColumnName("ProductModel");
            this.Property(t => t.CultureID).HasColumnName("CultureID");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
