using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class JobCandidateMap : EntityTypeConfiguration<JobCandidate>
    {
        public JobCandidateMap()
        {
            // Primary Key
            HasKey(t => t.JobCandidateID);

            // Properties
            // Table & Column Mappings
            ToTable("JobCandidate", "HumanResources");
            Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.Resume).HasColumnName("Resume");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasOptional(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeID);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<JobCandidate> builder)
        {
            builder.HasKey(t => t.JobCandidateID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("JobCandidate", "HumanResources");
            builder.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            builder.Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.Resume).HasColumnName("Resume");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeID);
        }
    }
}
