using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
{
    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            Ignore(t => t.Password); //TODO: Why?

            Property(t => t.Title)
                .HasMaxLength(8);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.MiddleName)
                .HasMaxLength(50);

            Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Suffix)
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("Person", "Person");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.NameStyle).HasColumnName("NameStyle");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.MiddleName).HasColumnName("MiddleName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Suffix).HasColumnName("Suffix");
            Property(t => t.EmailPromotion).HasColumnName("EmailPromotion");
            Property(t => t.AdditionalContactInfo).HasColumnName("AdditionalContactInfo");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            HasOptional(t => t.Employee).WithRequired(t => t.PersonDetails);
        }
    }
}
