using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Employee)
                .WithMany(t => t.PayHistory)
                .HasForeignKey(d => d.EmployeeID);

        }
    }
}
