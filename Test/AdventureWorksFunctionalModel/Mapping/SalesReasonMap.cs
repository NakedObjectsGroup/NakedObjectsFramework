using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<SalesReason> builder)
        {
            builder.HasKey(t => t.SalesReasonID);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.ReasonType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("SalesReason", "Sales");
            builder.Property(t => t.SalesReasonID).HasColumnName("SalesReasonID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ReasonType).HasColumnName("ReasonType");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
