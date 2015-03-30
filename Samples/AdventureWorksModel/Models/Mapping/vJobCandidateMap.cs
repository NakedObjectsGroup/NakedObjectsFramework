using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class vJobCandidateMap : EntityTypeConfiguration<vJobCandidate>
    {
        public vJobCandidateMap()
        {
            // Primary Key
            this.HasKey(t => new { t.JobCandidateID, t.ModifiedDate });

            // Properties
            this.Property(t => t.JobCandidateID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name_Prefix)
                .HasMaxLength(30);

            this.Property(t => t.Name_First)
                .HasMaxLength(30);

            this.Property(t => t.Name_Middle)
                .HasMaxLength(30);

            this.Property(t => t.Name_Last)
                .HasMaxLength(30);

            this.Property(t => t.Name_Suffix)
                .HasMaxLength(30);

            this.Property(t => t.Addr_Type)
                .HasMaxLength(30);

            this.Property(t => t.Addr_Loc_CountryRegion)
                .HasMaxLength(100);

            this.Property(t => t.Addr_Loc_State)
                .HasMaxLength(100);

            this.Property(t => t.Addr_Loc_City)
                .HasMaxLength(100);

            this.Property(t => t.Addr_PostalCode)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("vJobCandidate", "HumanResources");
            this.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.Name_Prefix).HasColumnName("Name.Prefix");
            this.Property(t => t.Name_First).HasColumnName("Name.First");
            this.Property(t => t.Name_Middle).HasColumnName("Name.Middle");
            this.Property(t => t.Name_Last).HasColumnName("Name.Last");
            this.Property(t => t.Name_Suffix).HasColumnName("Name.Suffix");
            this.Property(t => t.Skills).HasColumnName("Skills");
            this.Property(t => t.Addr_Type).HasColumnName("Addr.Type");
            this.Property(t => t.Addr_Loc_CountryRegion).HasColumnName("Addr.Loc.CountryRegion");
            this.Property(t => t.Addr_Loc_State).HasColumnName("Addr.Loc.State");
            this.Property(t => t.Addr_Loc_City).HasColumnName("Addr.Loc.City");
            this.Property(t => t.Addr_PostalCode).HasColumnName("Addr.PostalCode");
            this.Property(t => t.EMail).HasColumnName("EMail");
            this.Property(t => t.WebSite).HasColumnName("WebSite");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
