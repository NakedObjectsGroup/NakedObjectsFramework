using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class AWBuildVersionMap : EntityTypeConfiguration<AWBuildVersion>
    {
        public AWBuildVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.SystemInformationID);

            // Properties
            this.Property(t => t.Database_Version)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("AWBuildVersion");
            this.Property(t => t.SystemInformationID).HasColumnName("SystemInformationID");
            this.Property(t => t.Database_Version).HasColumnName("Database Version");
            this.Property(t => t.VersionDate).HasColumnName("VersionDate");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
