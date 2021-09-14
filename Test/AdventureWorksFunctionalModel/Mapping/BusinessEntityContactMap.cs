using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class BusinessEntityContactMap : EntityTypeConfiguration<BusinessEntityContact> {
        public BusinessEntityContactMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PersonID, t.ContactTypeID });

            // Table & Column Mappings
            ToTable("BusinessEntityContact", "Person");
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<BusinessEntityContact> builder) {
            builder.HasKey(t => new { t.BusinessEntityID, t.PersonID, t.ContactTypeID });

            // Table & Column Mappings
            builder.ToTable("BusinessEntityContact", "Person");
        }
    }
}