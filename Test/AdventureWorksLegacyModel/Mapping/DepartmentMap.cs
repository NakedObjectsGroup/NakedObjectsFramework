using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    //public class DepartmentMap : EntityTypeConfiguration<Department>
    //{
    //    public DepartmentMap()
    //    {
    //        // Primary Key
    //        HasKey(t => t.DepartmentID);

    //        // Properties
    //        Property(t => t.name)
    //            .IsRequired()
    //            .HasMaxLength(50);

    //        Property(t => t.groupName)
    //            .IsRequired()
    //            .HasMaxLength(50);

    //        // Table & Column Mappings
    //        ToTable("Department", "HumanResources");
    //        Property(t => t.DepartmentID).HasColumnName("DepartmentID");
    //        Property(t => t.name).HasColumnName("Name");
    //        Property(t => t.groupName).HasColumnName("GroupName");
    //        Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
    //    }
    //}

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Department> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentID);

            //Ignores
            builder.Ignore(t => t.Name);
            builder.Ignore(t => t.GroupName);

            // Table & Column Mappings
            builder.ToTable("Department", "HumanResources");
            builder.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            builder.Property(t => t.name).HasColumnName("Name").IsRequired().HasMaxLength(50); 
            builder.Property(t => t.groupName).HasColumnName("GroupName").IsRequired().HasMaxLength(50);
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 
        }
    }

}
