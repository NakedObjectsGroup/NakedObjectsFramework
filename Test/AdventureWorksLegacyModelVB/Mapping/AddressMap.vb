Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class AddressMap
		Inherits EntityTypeConfiguration(Of Address)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.AddressID)

			' Properties
			[Property](Function(t) t.AddressLine1).IsRequired().HasMaxLength(60)

			[Property](Function(t) t.AddressLine2).HasMaxLength(60)

			[Property](Function(t) t.City).IsRequired().HasMaxLength(30)

			[Property](Function(t) t.PostalCode).IsRequired().HasMaxLength(15)

			' Table & Column Mappings
			ToTable("Address", "Person")
			[Property](Function(t) t.AddressID).HasColumnName("AddressID")
			[Property](Function(t) t.AddressLine1).HasColumnName("AddressLine1")
			[Property](Function(t) t.AddressLine2).HasColumnName("AddressLine2")
			[Property](Function(t) t.City).HasColumnName("City")
			[Property](Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			[Property](Function(t) t.PostalCode).HasColumnName("PostalCode")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.StateProvince).WithMany().HasForeignKey(Function(t) t.StateProvinceID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Address))
			builder.HasKey(Function(t) t.AddressID)

			builder.Property(Function(t) t.AddressLine1).IsRequired().HasMaxLength(60)

			builder.Property(Function(t) t.AddressLine2).HasMaxLength(60)

			builder.Property(Function(t) t.City).IsRequired().HasMaxLength(30)

			builder.Property(Function(t) t.PostalCode).IsRequired().HasMaxLength(15)

			builder.ToTable("Address", "Person")
			builder.Property(Function(t) t.AddressID).HasColumnName("AddressID")
			builder.Property(Function(t) t.AddressLine1).HasColumnName("AddressLine1")
			builder.Property(Function(t) t.AddressLine2).HasColumnName("AddressLine2")
			builder.Property(Function(t) t.City).HasColumnName("City")
			builder.Property(Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			builder.Property(Function(t) t.PostalCode).HasColumnName("PostalCode")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.StateProvince).WithMany().HasForeignKey(Function(t) t.StateProvinceID)
			builder.HasOne(Function(t) t.StateProvince).WithMany().HasForeignKey(Function(t) t.StateProvinceID)
		End Sub
	End Module
End Namespace