using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class vJobCandidateEducationMap : EntityTypeConfiguration<vJobCandidateEducation>
    {
        public vJobCandidateEducationMap()
        {
            // Primary Key
            this.HasKey(t => t.JobCandidateID);

            // Properties
            this.Property(t => t.Edu_Degree)
                .HasMaxLength(50);

            this.Property(t => t.Edu_Major)
                .HasMaxLength(50);

            this.Property(t => t.Edu_Minor)
                .HasMaxLength(50);

            this.Property(t => t.Edu_GPA)
                .HasMaxLength(5);

            this.Property(t => t.Edu_GPAScale)
                .HasMaxLength(5);

            this.Property(t => t.Edu_School)
                .HasMaxLength(100);

            this.Property(t => t.Edu_Loc_CountryRegion)
                .HasMaxLength(100);

            this.Property(t => t.Edu_Loc_State)
                .HasMaxLength(100);

            this.Property(t => t.Edu_Loc_City)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("vJobCandidateEducation", "HumanResources");
            this.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            this.Property(t => t.Edu_Level).HasColumnName("Edu.Level");
            this.Property(t => t.Edu_StartDate).HasColumnName("Edu.StartDate");
            this.Property(t => t.Edu_EndDate).HasColumnName("Edu.EndDate");
            this.Property(t => t.Edu_Degree).HasColumnName("Edu.Degree");
            this.Property(t => t.Edu_Major).HasColumnName("Edu.Major");
            this.Property(t => t.Edu_Minor).HasColumnName("Edu.Minor");
            this.Property(t => t.Edu_GPA).HasColumnName("Edu.GPA");
            this.Property(t => t.Edu_GPAScale).HasColumnName("Edu.GPAScale");
            this.Property(t => t.Edu_School).HasColumnName("Edu.School");
            this.Property(t => t.Edu_Loc_CountryRegion).HasColumnName("Edu.Loc.CountryRegion");
            this.Property(t => t.Edu_Loc_State).HasColumnName("Edu.Loc.State");
            this.Property(t => t.Edu_Loc_City).HasColumnName("Edu.Loc.City");
        }
    }
}
