Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders
#Disable Warning BC8602

Namespace AW.Mapping
	Public Class ProductMap
		Inherits EntityTypeConfiguration(Of Product)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.ProductNumber).IsRequired().HasMaxLength(25)

			[Property](Function(t) t.Color).HasMaxLength(15)

			[Property](Function(t) t.Size).HasMaxLength(5)

			[Property](Function(t) t.SizeUnitMeasureCode).IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.WeightUnitMeasureCode).IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.ProductLine).IsFixedLength().HasMaxLength(2)

			[Property](Function(t) t.Class).IsFixedLength().HasMaxLength(2)

			[Property](Function(t) t.Style).IsFixedLength().HasMaxLength(2)

			' Table & Column Mappings
			ToTable("Product", "Production")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ProductNumber).HasColumnName("ProductNumber")
			[Property](Function(t) t.Make).HasColumnName("MakeFlag")
			[Property](Function(t) t.FinishedGoods).HasColumnName("FinishedGoodsFlag")
			[Property](Function(t) t.Color).HasColumnName("Color")
			[Property](Function(t) t.SafetyStockLevel).HasColumnName("SafetyStockLevel")
			[Property](Function(t) t.ReorderPoint).HasColumnName("ReorderPoint")
			[Property](Function(t) t.StandardCost).HasColumnName("StandardCost")
			[Property](Function(t) t.ListPrice).HasColumnName("ListPrice")
			[Property](Function(t) t.Size).HasColumnName("Size")
			[Property](Function(t) t.SizeUnitMeasureCode).HasColumnName("SizeUnitMeasureCode")
			[Property](Function(t) t.WeightUnitMeasureCode).HasColumnName("WeightUnitMeasureCode")
			[Property](Function(t) t.Weight).HasColumnName("Weight")
			[Property](Function(t) t.DaysToManufacture).HasColumnName("DaysToManufacture")
			[Property](Function(t) t.ProductLine).HasColumnName("ProductLine")
			[Property](Function(t) t.Class).HasColumnName("Class")
			[Property](Function(t) t.Style).HasColumnName("Style")
			[Property](Function(t) t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID")
			[Property](Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			[Property](Function(t) t.SellStartDate).HasColumnName("SellStartDate")
			[Property](Function(t) t.SellEndDate).HasColumnName("SellEndDate")
			[Property](Function(t) t.DiscontinuedDate).HasColumnName("DiscontinuedDate")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasOptional(Function(t) t.ProductModel).WithMany(Function(t) t.ProductVariants).HasForeignKey(Function(d) d.ProductModelID)
			HasOptional(Function(t) t.ProductSubcategory).WithMany().HasForeignKey(Function(t) t.ProductSubcategoryID)
			HasOptional(Function(t) t.SizeUnit).WithMany().HasForeignKey(Function(t) t.SizeUnitMeasureCode)
			HasOptional(Function(t) t.WeightUnit).WithMany().HasForeignKey(Function(t) t.WeightUnitMeasureCode)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Product))
			builder.HasKey(Function(t) t.ProductID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.ProductNumber).IsRequired().HasMaxLength(25)

			builder.Property(Function(t) t.Color).HasMaxLength(15)

			builder.Property(Function(t) t.Size).HasMaxLength(5)

			builder.Property(Function(t) t.SizeUnitMeasureCode).IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.WeightUnitMeasureCode).IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.ProductLine).IsFixedLength().HasMaxLength(2)

			builder.Property(Function(t) t.Class).IsFixedLength().HasMaxLength(2)

			builder.Property(Function(t) t.Style).IsFixedLength().HasMaxLength(2)

			' Table & Column Mappings
			builder.ToTable("Product", "Production")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ProductNumber).HasColumnName("ProductNumber")
			builder.Property(Function(t) t.Make).HasColumnName("MakeFlag")
			builder.Property(Function(t) t.FinishedGoods).HasColumnName("FinishedGoodsFlag")
			builder.Property(Function(t) t.Color).HasColumnName("Color")
			builder.Property(Function(t) t.SafetyStockLevel).HasColumnName("SafetyStockLevel")
			builder.Property(Function(t) t.ReorderPoint).HasColumnName("ReorderPoint")
			builder.Property(Function(t) t.StandardCost).HasColumnName("StandardCost")
			builder.Property(Function(t) t.ListPrice).HasColumnName("ListPrice")
			builder.Property(Function(t) t.Size).HasColumnName("Size")
			builder.Property(Function(t) t.SizeUnitMeasureCode).HasColumnName("SizeUnitMeasureCode")
			builder.Property(Function(t) t.WeightUnitMeasureCode).HasColumnName("WeightUnitMeasureCode")
			builder.Property(Function(t) t.Weight).HasColumnName("Weight")
			builder.Property(Function(t) t.DaysToManufacture).HasColumnName("DaysToManufacture")
			builder.Property(Function(t) t.ProductLine).HasColumnName("ProductLine")
			builder.Property(Function(t) t.Class).HasColumnName("Class")
			builder.Property(Function(t) t.Style).HasColumnName("Style")
			builder.Property(Function(t) t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID")
			builder.Property(Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			builder.Property(Function(t) t.SellStartDate).HasColumnName("SellStartDate")
			builder.Property(Function(t) t.SellEndDate).HasColumnName("SellEndDate")
			builder.Property(Function(t) t.DiscontinuedDate).HasColumnName("DiscontinuedDate")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.ProductModel).WithMany(Function(t) t.ProductVariants).HasForeignKey(Function(d) d.ProductModelID)
			builder.HasOne(Function(t) t.ProductSubcategory).WithMany().HasForeignKey(Function(t) t.ProductSubcategoryID)

			builder.HasOne(Function(t) t.SizeUnit).WithMany().HasForeignKey(Function(t) t.SizeUnitMeasureCode)
			builder.HasOne(Function(t) t.WeightUnit).WithMany().HasForeignKey(Function(t) t.WeightUnitMeasureCode)
		End Sub
	End Module
End Namespace