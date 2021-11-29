Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class SalesTaxRateMap
		Inherits EntityTypeConfiguration(Of SalesTaxRate)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.SalesTaxRateID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("SalesTaxRate", "Sales")
			[Property](Function(t) t.SalesTaxRateID).HasColumnName("SalesTaxRateID")
			[Property](Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			[Property](Function(t) t.TaxType).HasColumnName("TaxType")
			[Property](Function(t) t.TaxRate).HasColumnName("TaxRate")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.StateProvince).WithMany().HasForeignKey(Function(t) t.StateProvinceID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of SalesTaxRate))
			builder.HasKey(Function(t) t.SalesTaxRateID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("SalesTaxRate", "Sales")
			builder.Property(Function(t) t.SalesTaxRateID).HasColumnName("SalesTaxRateID")
			builder.Property(Function(t) t.StateProvinceID).HasColumnName("StateProvinceID")
			builder.Property(Function(t) t.TaxType).HasColumnName("TaxType")
			builder.Property(Function(t) t.TaxRate).HasColumnName("TaxRate")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.StateProvince).WithMany().HasForeignKey(Function(t) t.StateProvinceID)
		End Sub
	End Module
End Namespace