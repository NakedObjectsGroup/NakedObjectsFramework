using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class PersonPhoneMap : EntityTypeConfiguration<PersonPhone> {
        public PersonPhoneMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PhoneNumber, t.PhoneNumberTypeID });

            // Table & Column Mappings
            ToTable("PersonPhone", "Person");
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<PersonPhone> builder) {
            builder.HasKey(t => new { t.BusinessEntityID, t.PhoneNumber, t.PhoneNumberTypeID });

            // Table & Column Mappings
            builder.ToTable("PersonPhone", "Person");
        }
    }
}