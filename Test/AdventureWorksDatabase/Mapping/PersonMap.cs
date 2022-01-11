using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Person> builder)
        {
            //No key defined because defined for superclass
            builder.ToTable("Person", "Person");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedPersonType).HasColumnName("PersonType");
            builder.Property(t => t.mappedNameStyle).HasColumnName("NameStyle");
            builder.Property(t => t.mappedTitle).HasColumnName("Title");
            builder.Property(t => t.mappedFirstName).HasColumnName("FirstName");
            builder.Property(t => t.mappedMiddleName).HasColumnName("MiddleName");
            builder.Property(t => t.mappedLastName).HasColumnName("LastName");
            builder.Property(t => t.mappedSuffix).HasColumnName("Suffix");
            builder.Property(t => t.EmailPromotion).HasColumnName("EmailPromotion");
            builder.Property(t => t.mappedAdditionalContactInfo).HasColumnName("AdditionalContactInfo");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            builder.HasOne(t => t.Employee).WithOne(t => t.PersonDetails).HasForeignKey<Employee>(p => p.BusinessEntityID);
            builder.HasOne(t => t.Password).WithOne(pw => pw.Person).HasForeignKey<Password>(pw => pw.BusinessEntityID);
        }
    }
}
