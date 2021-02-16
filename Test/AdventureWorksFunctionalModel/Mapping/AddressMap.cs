using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            HasKey(t => t.AddressID);

            // Properties
            Property(t => t.AddressLine1)
                .IsRequired()
                .HasMaxLength(60);

            Property(t => t.AddressLine2)
                .HasMaxLength(60);

            Property(t => t.City)
                .IsRequired()
                .HasMaxLength(30);

            Property(t => t.PostalCode)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            ToTable("Address", "Person");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.StateProvince).WithMany().HasForeignKey(t =>t.StateProvinceID);

        }
    }
}
