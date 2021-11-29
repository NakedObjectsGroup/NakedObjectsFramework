Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
#Disable Warning BC8602

Namespace AW.Mapping
	Public Class PersonMap
		Inherits EntityTypeConfiguration(Of Person)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			[Property](Function(t) t.Title).HasMaxLength(8)

			[Property](Function(t) t.FirstName).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.MiddleName).HasMaxLength(50)

			[Property](Function(t) t.LastName).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.Suffix).HasMaxLength(10)

			' Table & Column Mappings
			ToTable("Person", "Person")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.PersonType).HasColumnName("PersonType")
			[Property](Function(t) t.NameStyle).HasColumnName("NameStyle")
			[Property](Function(t) t.Title).HasColumnName("Title")
			[Property](Function(t) t.FirstName).HasColumnName("FirstName")
			[Property](Function(t) t.MiddleName).HasColumnName("MiddleName")
			[Property](Function(t) t.LastName).HasColumnName("LastName")
			[Property](Function(t) t.Suffix).HasColumnName("Suffix")
			[Property](Function(t) t.EmailPromotion).HasColumnName("EmailPromotion")
			[Property](Function(t) t.AdditionalContactInfo).HasColumnName("AdditionalContactInfo")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")

			HasOptional(Function(t) t.Employee).WithRequired(Function(t) t.PersonDetails)
			HasOptional(Function(t) t.Password).WithRequired(Function(pw) pw.Person)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Person))
			builder.Property(Function(t) t.Title).HasMaxLength(8)

			builder.Property(Function(t) t.FirstName).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.MiddleName).HasMaxLength(50)

			builder.Property(Function(t) t.LastName).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.Suffix).HasMaxLength(10)

			' Table & Column Mappings
			builder.ToTable("Person", "Person")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.PersonType).HasColumnName("PersonType")
			builder.Property(Function(t) t.NameStyle).HasColumnName("NameStyle")
			builder.Property(Function(t) t.Title).HasColumnName("Title")
			builder.Property(Function(t) t.FirstName).HasColumnName("FirstName")
			builder.Property(Function(t) t.MiddleName).HasColumnName("MiddleName")
			builder.Property(Function(t) t.LastName).HasColumnName("LastName")
			builder.Property(Function(t) t.Suffix).HasColumnName("Suffix")
			builder.Property(Function(t) t.EmailPromotion).HasColumnName("EmailPromotion")
			builder.Property(Function(t) t.AdditionalContactInfo).HasColumnName("AdditionalContactInfo")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")

			builder.Ignore(Function(t) t.PhoneNumbers)

			builder.HasOne(Function(t) t.Employee).WithOne(Function(t) t.PersonDetails).HasForeignKey(Of Employee)(Function(p) p.BusinessEntityID)
			builder.HasOne(Function(t) t.Password).WithOne(Function(pw) pw.Person).HasForeignKey(Of Password)(Function(pw) pw.BusinessEntityID)
		End Sub
	End Module
End Namespace