using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class EmployeePayHistoryMap : EntityTypeConfiguration<EmployeePayHistory>
    {
        public EmployeePayHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.EmployeeID, t.RateChangeDate });

            // Properties
            Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("EmployeePayHistory", "HumanResources");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.RateChangeDate).HasColumnName("RateChangeDate");
            Property(t => t.Rate).HasColumnName("Rate");
            Property(t => t.PayFrequency).HasColumnName("PayFrequency");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Employee)
                .WithMany(t => t.PayHistory)
                .HasForeignKey(d => d.EmployeeID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmployeePayHistory> builder)
        {
            builder.HasKey(t => new { t.EmployeeID, t.RateChangeDate });

            // Properties
            builder.Property(t => t.EmployeeID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("EmployeePayHistory", "HumanResources");
            builder.Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.RateChangeDate).HasColumnName("RateChangeDate");
            builder.Property(t => t.Rate).HasColumnName("Rate");
            builder.Property(t => t.PayFrequency).HasColumnName("PayFrequency");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Employee)
                   .WithMany(t => t.PayHistory)
                   .HasForeignKey(d => d.EmployeeID);
        }
    }
}
