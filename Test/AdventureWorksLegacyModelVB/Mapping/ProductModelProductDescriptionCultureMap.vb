Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductModelProductDescriptionCultureMap
		Inherits EntityTypeConfiguration(Of ProductModelProductDescriptionCulture)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductModelID,
				Key t.ProductDescriptionID,
				Key t.CultureID
			})

			' Properties
			[Property](Function(t) t.ProductModelID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.ProductDescriptionID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.CultureID).IsRequired().IsFixedLength().HasMaxLength(6)

			' Table & Column Mappings
			ToTable("ProductModelProductDescriptionCulture", "Production")
			[Property](Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			[Property](Function(t) t.ProductDescriptionID).HasColumnName("ProductDescriptionID")
			[Property](Function(t) t.CultureID).HasColumnName("CultureID")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Culture).WithMany().HasForeignKey(Function(t) t.CultureID)
			HasRequired(Function(t) t.ProductDescription).WithMany().HasForeignKey(Function(t) t.ProductDescriptionID)
			HasRequired(Function(t) t.ProductModel).WithMany(Function(t) t.ProductModelProductDescriptionCulture).HasForeignKey(Function(d) d.ProductModelID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductModelProductDescriptionCulture))
			builder.HasKey(Function(t) New With {
				Key t.ProductModelID,
				Key t.ProductDescriptionID,
				Key t.CultureID
			})

			' Properties
			builder.Property(Function(t) t.ProductModelID).ValueGeneratedNever()

			builder.Property(Function(t) t.ProductDescriptionID).ValueGeneratedNever()

			builder.Property(Function(t) t.CultureID).IsRequired().IsFixedLength().HasMaxLength(6)

			' Table & Column Mappings
			builder.ToTable("ProductModelProductDescriptionCulture", "Production")
			builder.Property(Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			builder.Property(Function(t) t.ProductDescriptionID).HasColumnName("ProductDescriptionID")
			builder.Property(Function(t) t.CultureID).HasColumnName("CultureID")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Culture).WithMany().HasForeignKey(Function(t) t.CultureID)
			builder.HasOne(Function(t) t.ProductDescription).WithMany().HasForeignKey(Function(t) t.ProductDescriptionID)
			builder.HasOne(Function(t) t.ProductModel).WithMany(Function(t) t.ProductModelProductDescriptionCulture).HasForeignKey(Function(d) d.ProductModelID)
		End Sub
	End Module
End Namespace