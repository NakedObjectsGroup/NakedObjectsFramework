Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductVendorMap
		Inherits EntityTypeConfiguration(Of ProductVendor)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.VendorID
			})

			' Properties
			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.VendorID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			ToTable("ProductVendor", "Purchasing")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.VendorID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.AverageLeadTime).HasColumnName("AverageLeadTime")
			[Property](Function(t) t.StandardPrice).HasColumnName("StandardPrice")
			[Property](Function(t) t.LastReceiptCost).HasColumnName("LastReceiptCost")
			[Property](Function(t) t.LastReceiptDate).HasColumnName("LastReceiptDate")
			[Property](Function(t) t.MinOrderQty).HasColumnName("MinOrderQty")
			[Property](Function(t) t.MaxOrderQty).HasColumnName("MaxOrderQty")
			[Property](Function(t) t.OnOrderQty).HasColumnName("OnOrderQty")
			[Property](Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			HasRequired(Function(t) t.UnitMeasure).WithMany().HasForeignKey(Function(t) t.UnitMeasureCode)
			HasRequired(Function(t) t.Vendor).WithMany(Function(t) t.Products).HasForeignKey(Function(d) d.VendorID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductVendor))
			' Primary Key
			builder.HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.VendorID
			})

			' Properties
			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			builder.Property(Function(t) t.VendorID).ValueGeneratedNever()

			builder.Property(Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			builder.ToTable("ProductVendor", "Purchasing")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.VendorID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.AverageLeadTime).HasColumnName("AverageLeadTime")
			builder.Property(Function(t) t.StandardPrice).HasColumnName("StandardPrice")
			builder.Property(Function(t) t.LastReceiptCost).HasColumnName("LastReceiptCost")
			builder.Property(Function(t) t.LastReceiptDate).HasColumnName("LastReceiptDate")
			builder.Property(Function(t) t.MinOrderQty).HasColumnName("MinOrderQty")
			builder.Property(Function(t) t.MaxOrderQty).HasColumnName("MaxOrderQty")
			builder.Property(Function(t) t.OnOrderQty).HasColumnName("OnOrderQty")
			builder.Property(Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany().HasForeignKey(Function(t) t.ProductID)
			builder.HasOne(Function(t) t.UnitMeasure).WithMany().HasForeignKey(Function(t) t.UnitMeasureCode)
			builder.HasOne(Function(t) t.Vendor).WithMany(Function(t) t.Products).HasForeignKey(Function(d) d.VendorID)
		End Sub
	End Module
End Namespace