Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductModelIllustrationMap
		Inherits EntityTypeConfiguration(Of ProductModelIllustration)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.ProductModelID,
				Key t.IllustrationID
			})

			' Properties
			[Property](Function(t) t.ProductModelID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.IllustrationID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("ProductModelIllustration", "Production")
			[Property](Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			[Property](Function(t) t.IllustrationID).HasColumnName("IllustrationID")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Illustration).WithMany(Function(t) t.ProductModelIllustration).HasForeignKey(Function(d) d.IllustrationID)
			HasRequired(Function(t) t.ProductModel).WithMany(Function(t) t.ProductModelIllustration).HasForeignKey(Function(d) d.ProductModelID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductModelIllustration))
			builder.HasKey(Function(t) New With {
				Key t.ProductModelID,
				Key t.IllustrationID
			})

			' Properties
			builder.Property(Function(t) t.ProductModelID).ValueGeneratedNever()

			builder.Property(Function(t) t.IllustrationID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("ProductModelIllustration", "Production")
			builder.Property(Function(t) t.ProductModelID).HasColumnName("ProductModelID")
			builder.Property(Function(t) t.IllustrationID).HasColumnName("IllustrationID")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Illustration).WithMany(Function(t) t.ProductModelIllustration).HasForeignKey(Function(d) d.IllustrationID)
			builder.HasOne(Function(t) t.ProductModel).WithMany(Function(t) t.ProductModelIllustration).HasForeignKey(Function(d) d.ProductModelID)
		End Sub
	End Module
End Namespace