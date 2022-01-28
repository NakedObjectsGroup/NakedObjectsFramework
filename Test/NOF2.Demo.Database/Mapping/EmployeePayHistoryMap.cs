using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmployeePayHistory> builder)
        {
            builder.HasKey(t => new { t.EmployeeID, t.mappedRateChangeDate });

            // Properties
            builder.Property(t => t.EmployeeID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("EmployeePayHistory", "HumanResources");
            builder.Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedRateChangeDate).HasColumnName("RateChangeDate");
            builder.Property(t => t.mappedRate).HasColumnName("Rate");
            builder.Property(t => t.mappedPayFrequency).HasColumnName("PayFrequency");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Employee)
                   .WithMany(t => t.mappedPayHistory)
                   .HasForeignKey(d => d.EmployeeID);
        }
    }
}
