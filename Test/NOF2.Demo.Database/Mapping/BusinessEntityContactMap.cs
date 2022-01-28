using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BusinessEntityContact> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.PersonID, t.ContactTypeID });

            // Table & Column Mappings
            builder.ToTable("BusinessEntityContact", "Person");

            builder.ToTable("BusinessEntityContact", "Person");
            builder.Property(t => t.PersonID).HasColumnName("PersonID");
            builder.Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);


            builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
            builder.HasOne(t => t.ContactType).WithMany().HasForeignKey(t => t.ContactTypeID);
            builder.HasOne(t => t.BusinessEntity).WithMany(t => t.mappedContacts).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}