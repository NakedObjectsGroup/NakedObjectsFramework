using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Department> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentID);

            //Ignores
            //builder.Ignore(t => t.Name);
            //builder.Ignore(t => t.GroupName);

            // Table & Column Mappings
            builder.ToTable("Department", "HumanResources");
            builder.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50); 
            builder.Property(t => t.mappedGroupName).HasColumnName("GroupName").IsRequired().HasMaxLength(50);
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 
        }
    }

}
