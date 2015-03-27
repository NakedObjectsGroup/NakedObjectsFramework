using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class vAdditionalContactInfoMap : EntityTypeConfiguration<vAdditionalContactInfo>
    {
        public vAdditionalContactInfoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ContactID, t.FirstName, t.LastName, t.rowguid, t.ModifiedDate });

            // Properties
            this.Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MiddleName)
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TelephoneNumber)
                .HasMaxLength(50);

            this.Property(t => t.Street)
                .HasMaxLength(50);

            this.Property(t => t.City)
                .HasMaxLength(50);

            this.Property(t => t.StateProvince)
                .HasMaxLength(50);

            this.Property(t => t.PostalCode)
                .HasMaxLength(50);

            this.Property(t => t.CountryRegion)
                .HasMaxLength(50);

            this.Property(t => t.EMailAddress)
                .HasMaxLength(128);

            this.Property(t => t.EMailTelephoneNumber)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("vAdditionalContactInfo", "Person");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleName).HasColumnName("MiddleName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.TelephoneNumber).HasColumnName("TelephoneNumber");
            this.Property(t => t.TelephoneSpecialInstructions).HasColumnName("TelephoneSpecialInstructions");
            this.Property(t => t.Street).HasColumnName("Street");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.StateProvince).HasColumnName("StateProvince");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.CountryRegion).HasColumnName("CountryRegion");
            this.Property(t => t.HomeAddressSpecialInstructions).HasColumnName("HomeAddressSpecialInstructions");
            this.Property(t => t.EMailAddress).HasColumnName("EMailAddress");
            this.Property(t => t.EMailSpecialInstructions).HasColumnName("EMailSpecialInstructions");
            this.Property(t => t.EMailTelephoneNumber).HasColumnName("EMailTelephoneNumber");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
