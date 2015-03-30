using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class SalesReasonMap : EntityTypeConfiguration<SalesReason>
    {
        public SalesReasonMap()
        {
            // Primary Key
            this.HasKey(t => t.SalesReasonID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ReasonType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SalesReason", "Sales");
            this.Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ReasonType).HasColumnName("ReasonType");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
