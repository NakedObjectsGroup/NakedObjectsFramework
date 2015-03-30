using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class vProductModelCatalogDescriptionMap : EntityTypeConfiguration<vProductModelCatalogDescription>
    {
        public vProductModelCatalogDescriptionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductModelID, t.Name, t.rowguid, t.ModifiedDate });

            // Properties
            this.Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Copyright)
                .HasMaxLength(30);

            this.Property(t => t.ProductURL)
                .HasMaxLength(256);

            this.Property(t => t.WarrantyPeriod)
                .HasMaxLength(256);

            this.Property(t => t.WarrantyDescription)
                .HasMaxLength(256);

            this.Property(t => t.NoOfYears)
                .HasMaxLength(256);

            this.Property(t => t.MaintenanceDescription)
                .HasMaxLength(256);

            this.Property(t => t.Wheel)
                .HasMaxLength(256);

            this.Property(t => t.Saddle)
                .HasMaxLength(256);

            this.Property(t => t.Pedal)
                .HasMaxLength(256);

            this.Property(t => t.Crankset)
                .HasMaxLength(256);

            this.Property(t => t.PictureAngle)
                .HasMaxLength(256);

            this.Property(t => t.PictureSize)
                .HasMaxLength(256);

            this.Property(t => t.ProductPhotoID)
                .HasMaxLength(256);

            this.Property(t => t.Material)
                .HasMaxLength(256);

            this.Property(t => t.Color)
                .HasMaxLength(256);

            this.Property(t => t.ProductLine)
                .HasMaxLength(256);

            this.Property(t => t.Style)
                .HasMaxLength(256);

            this.Property(t => t.RiderExperience)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("vProductModelCatalogDescription", "Production");
            this.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Summary).HasColumnName("Summary");
            this.Property(t => t.Manufacturer).HasColumnName("Manufacturer");
            this.Property(t => t.Copyright).HasColumnName("Copyright");
            this.Property(t => t.ProductURL).HasColumnName("ProductURL");
            this.Property(t => t.WarrantyPeriod).HasColumnName("WarrantyPeriod");
            this.Property(t => t.WarrantyDescription).HasColumnName("WarrantyDescription");
            this.Property(t => t.NoOfYears).HasColumnName("NoOfYears");
            this.Property(t => t.MaintenanceDescription).HasColumnName("MaintenanceDescription");
            this.Property(t => t.Wheel).HasColumnName("Wheel");
            this.Property(t => t.Saddle).HasColumnName("Saddle");
            this.Property(t => t.Pedal).HasColumnName("Pedal");
            this.Property(t => t.BikeFrame).HasColumnName("BikeFrame");
            this.Property(t => t.Crankset).HasColumnName("Crankset");
            this.Property(t => t.PictureAngle).HasColumnName("PictureAngle");
            this.Property(t => t.PictureSize).HasColumnName("PictureSize");
            this.Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            this.Property(t => t.Material).HasColumnName("Material");
            this.Property(t => t.Color).HasColumnName("Color");
            this.Property(t => t.ProductLine).HasColumnName("ProductLine");
            this.Property(t => t.Style).HasColumnName("Style");
            this.Property(t => t.RiderExperience).HasColumnName("RiderExperience");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
