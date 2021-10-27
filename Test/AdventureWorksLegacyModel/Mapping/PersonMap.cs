using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Human_Resources;
using AdventureWorksLegacyModel.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
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

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Person> builder)
        {
            //builder.HasKey(t => t.BusinessEntityID);

            builder.Property(t => t.Title)
                .HasMaxLength(8);

            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.MiddleName)
                .HasMaxLength(50);

            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Suffix)
                .HasMaxLength(10);

            // Table & Column Mappings
            builder.ToTable("Person", "Person");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            //builder.Property(t => t.PersonType).HasColumnName("PersonType");
            builder.Property(t => t.NameStyle).HasColumnName("NameStyle");
            builder.Property(t => t.Title).HasColumnName("Title");
            builder.Property(t => t.FirstName).HasColumnName("FirstName");
            builder.Property(t => t.MiddleName).HasColumnName("MiddleName");
            builder.Property(t => t.LastName).HasColumnName("LastName");
            builder.Property(t => t.Suffix).HasColumnName("Suffix");
            builder.Property(t => t.EmailPromotion).HasColumnName("EmailPromotion");
            builder.Property(t => t.AdditionalContactInfo).HasColumnName("AdditionalContactInfo");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            builder.Ignore(t => t.InitialPassword);
            builder.Ignore(t => t.ContactType);
            builder.Ignore(t => t.ForEntity);
            //TODO: Temp ignored
            builder.Ignore(t => t.Password);

            builder.HasOne(t => t.Employee).WithOne(t => t.PersonDetails).HasForeignKey<Employee>(p => p.BusinessEntityID);
            //builder.HasOne(t => t.Password).WithOne(pw => pw.Person).HasForeignKey<Password>(pw => pw.BusinessEntityID);
            builder.HasMany(t => t.PhoneNumbers).WithOne(t => t.Person).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}
