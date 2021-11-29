Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductDescriptionMap
		Inherits EntityTypeConfiguration(Of ProductDescription)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductDescriptionID)

			' Properties
			[Property](Function(t) t.Description).IsRequired().HasMaxLength(400)

			' Table & Column Mappings
			ToTable("ProductDescription", "Production")
			[Property](Function(t) t.ProductDescriptionID).HasColumnName("ProductDescriptionID")
			[Property](Function(t) t.Description).HasColumnName("Description")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductDescription))
			builder.HasKey(Function(t) t.ProductDescriptionID)

			' Properties
			builder.Property(Function(t) t.Description).IsRequired().HasMaxLength(400)

			' Table & Column Mappings
			builder.ToTable("ProductDescription", "Production")
			builder.Property(Function(t) t.ProductDescriptionID).HasColumnName("ProductDescriptionID")
			builder.Property(Function(t) t.Description).HasColumnName("Description")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace