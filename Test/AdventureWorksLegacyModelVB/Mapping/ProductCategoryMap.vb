Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductCategoryMap
		Inherits EntityTypeConfiguration(Of ProductCategory)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductCategoryID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ProductCategory", "Production")
			[Property](Function(t) t.ProductCategoryID).HasColumnName("ProductCategoryID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductCategory))
			builder.HasKey(Function(t) t.ProductCategoryID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ProductCategory", "Production")
			builder.Property(Function(t) t.ProductCategoryID).HasColumnName("ProductCategoryID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace