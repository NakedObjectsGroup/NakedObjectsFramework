using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Store", "Sales");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            Property(t => t.Demographics).HasColumnName("Demographics");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasOptional(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Store> builder)
        {
            //builder.HasKey(t => t.BusinessEntityID);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Store", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            builder.Property(t => t.Demographics).HasColumnName("Demographics");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
        }
    }
}
