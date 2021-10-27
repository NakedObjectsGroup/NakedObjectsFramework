using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Human_Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class DepartmentMap : EntityTypeConfiguration<Department>
    {
        public DepartmentMap()
        {
            // Primary Key
            HasKey(t => t.DepartmentID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Department", "HumanResources");
            Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.GroupName).HasColumnName("GroupName");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Department> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.GroupName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Department", "HumanResources");
            builder.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.GroupName).HasColumnName("GroupName");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 
        }
    }

}
