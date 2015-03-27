using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class vIndividualDemographicMap : EntityTypeConfiguration<vIndividualDemographic>
    {
        public vIndividualDemographicMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerID);

            // Properties
            this.Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MaritalStatus)
                .HasMaxLength(1);

            this.Property(t => t.YearlyIncome)
                .HasMaxLength(30);

            this.Property(t => t.Gender)
                .HasMaxLength(1);

            this.Property(t => t.Education)
                .HasMaxLength(30);

            this.Property(t => t.Occupation)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("vIndividualDemographics", "Sales");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.TotalPurchaseYTD).HasColumnName("TotalPurchaseYTD");
            this.Property(t => t.DateFirstPurchase).HasColumnName("DateFirstPurchase");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            this.Property(t => t.YearlyIncome).HasColumnName("YearlyIncome");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.TotalChildren).HasColumnName("TotalChildren");
            this.Property(t => t.NumberChildrenAtHome).HasColumnName("NumberChildrenAtHome");
            this.Property(t => t.Education).HasColumnName("Education");
            this.Property(t => t.Occupation).HasColumnName("Occupation");
            this.Property(t => t.HomeOwnerFlag).HasColumnName("HomeOwnerFlag");
            this.Property(t => t.NumberCarsOwned).HasColumnName("NumberCarsOwned");
        }
    }
}
