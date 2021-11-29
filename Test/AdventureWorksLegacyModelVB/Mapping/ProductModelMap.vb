Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductModelMap
		Inherits EntityTypeConfiguration(Of ProductModel)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductModelID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ProductModel", "Production")
			[Property](Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.CatalogDescription).HasColumnName("CatalogDescription")
			[Property](Function(t) t.Instructions).HasColumnName("Instructions")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductModel))
			builder.HasKey(Function(t) t.ProductModelID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ProductModel", "Production")
			builder.Property(Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.CatalogDescription).HasColumnName("CatalogDescription")
			builder.Property(Function(t) t.Instructions).HasColumnName("Instructions")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace