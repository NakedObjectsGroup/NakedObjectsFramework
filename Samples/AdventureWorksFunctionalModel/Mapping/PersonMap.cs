using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            //Ignores
            Ignore(t => t.InitialPassword);
            Ignore(t => t.ContactType);
            Ignore(t => t.ForEntity);
            //TODO: Temp ignored
            Ignore(t => t.Password);

            // Properties
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            HasOptional(t => t.Employee).WithRequired(t => t.PersonDetails);
        }
    }
}
