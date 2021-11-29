Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class StoreMap
		Inherits EntityTypeConfiguration(Of Store)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Store", "Sales")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.SalesPersonID).HasColumnName("SalesPersonID")
			[Property](Function(t) t.Demographics).HasColumnName("Demographics")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")

			' Relationships
			HasOptional(Function(t) t.SalesPerson).WithMany().HasForeignKey(Function(t) t.SalesPersonID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Store))
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Store", "Sales")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.SalesPersonID).HasColumnName("SalesPersonID")
			builder.Property(Function(t) t.Demographics).HasColumnName("Demographics")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate")

			' Relationships
			builder.HasOne(Function(t) t.SalesPerson).WithMany().HasForeignKey(Function(t) t.SalesPersonID)
		End Sub
	End Module
End Namespace