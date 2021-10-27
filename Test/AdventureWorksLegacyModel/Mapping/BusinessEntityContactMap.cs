using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping {
    public class BusinessEntityContactMap : EntityTypeConfiguration<BusinessEntityContact>
    {
        public BusinessEntityContactMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PersonID, t.ContactTypeID });

            // Table & Column Mappings
            ToTable("BusinessEntityContact", "Person");
        }
    }

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
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);


            builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
            builder.HasOne(t => t.ContactType).WithMany().HasForeignKey(t => t.ContactTypeID);
            builder.HasOne(t => t.BusinessEntity).WithMany(t => t.Contacts).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}