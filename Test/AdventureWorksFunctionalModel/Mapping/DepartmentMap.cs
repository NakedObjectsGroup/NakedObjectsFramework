using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
