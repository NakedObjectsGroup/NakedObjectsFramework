using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Person> builder)
        {
            //builder.HasKey(t => t.BusinessEntityID);

            builder.Property(t => t.Title)
                .HasMaxLength(8);

            builder.Property(t => t.mappedFirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.mappedMiddleName)
                .HasMaxLength(50);

            builder.Property(t => t.mappedLastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.mappedSuffix)
                .HasMaxLength(10);

            // Table & Column Mappings
            builder.ToTable("Person", "Person");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            //builder.Property(t => t.PersonType).HasColumnName("PersonType");
            builder.Property(t => t.mappedNameStyle).HasColumnName("NameStyle");
            builder.Property(t => t.Title).HasColumnName("Title");
            builder.Property(t => t.mappedFirstName).HasColumnName("FirstName");
            builder.Property(t => t.mappedMiddleName).HasColumnName("MiddleName");
            builder.Property(t => t.mappedLastName).HasColumnName("LastName");
            builder.Property(t => t.mappedSuffix).HasColumnName("Suffix");
            builder.Property(t => t.EmailPromotion).HasColumnName("EmailPromotion");
            builder.Property(t => t.mappedAdditionalContactInfo).HasColumnName("AdditionalContactInfo");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            //builder.Ignore(t => t.InitialPassword);
            //builder.Ignore(t => t.ContactType);
            //builder.Ignore(t => t.ForEntity);
            //TODO: Temp ignored
            builder.Ignore(t => t.Password);

            builder.HasOne(t => t.Employee).WithOne(t => t.PersonDetails).HasForeignKey<Employee>(p => p.BusinessEntityID);
            //builder.HasOne(t => t.Password).WithOne(pw => pw.Person).HasForeignKey<Password>(pw => pw.BusinessEntityID);
           // builder.HasMany(t => t.mappedPhoneNumbers).WithOne(t => t.).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}
