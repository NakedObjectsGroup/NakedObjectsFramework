Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductSubcategoryMap
		Inherits EntityTypeConfiguration(Of ProductSubcategory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductSubcategoryID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ProductSubcategory", "Production")
			[Property](Function(t) t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID")
			[Property](Function(t) t.ProductCategoryID).HasColumnName("ProductCategoryID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.ProductCategory).WithMany(Function(t) t.ProductSubcategory).HasForeignKey(Function(d) d.ProductCategoryID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductSubcategory))
			builder.HasKey(Function(t) t.ProductSubcategoryID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ProductSubcategory", "Production")
			builder.Property(Function(t) t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID")
			builder.Property(Function(t) t.ProductCategoryID).HasColumnName("ProductCategoryID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.ProductCategory).WithMany(Function(t) t.ProductSubcategory).HasForeignKey(Function(d) d.ProductCategoryID)
		End Sub
	End Module
End Namespace