Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductProductPhotoMap
		Inherits EntityTypeConfiguration(Of ProductProductPhoto)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.ProductPhotoID
			})

			' Properties
			[Property](Function(t) t.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.ProductPhotoID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("ProductProductPhoto", "Production")
			[Property](Function(t) t.ProductID).HasColumnName("ProductID")
			[Property](Function(t) t.ProductPhotoID).HasColumnName("ProductPhotoID")
			[Property](Function(t) t.Primary).HasColumnName("Primary")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Product).WithMany(Function(t) t.ProductProductPhoto).HasForeignKey(Function(d) d.ProductID)
			HasRequired(Function(t) t.ProductPhoto).WithMany(Function(t) t.ProductProductPhoto).HasForeignKey(Function(d) d.ProductPhotoID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductProductPhoto))
			builder.HasKey(Function(t) New With {
				Key t.ProductID,
				Key t.ProductPhotoID
			})

			' Properties
			builder.Property(Function(t) t.ProductID).ValueGeneratedNever()

			builder.Property(Function(t) t.ProductPhotoID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("ProductProductPhoto", "Production")
			builder.Property(Function(t) t.ProductID).HasColumnName("ProductID")
			builder.Property(Function(t) t.ProductPhotoID).HasColumnName("ProductPhotoID")
			builder.Property(Function(t) t.Primary).HasColumnName("Primary")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Product).WithMany(Function(t) t.ProductProductPhoto).HasForeignKey(Function(d) d.ProductID)
			builder.HasOne(Function(t) t.ProductPhoto).WithMany(Function(t) t.ProductProductPhoto).HasForeignKey(Function(d) d.ProductPhotoID)
		End Sub
	End Module
End Namespace