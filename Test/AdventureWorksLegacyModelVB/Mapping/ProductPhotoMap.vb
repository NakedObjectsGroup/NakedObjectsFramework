Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ProductPhotoMap
		Inherits EntityTypeConfiguration(Of ProductPhoto)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ProductPhotoID)

			' Properties
			[Property](Function(t) t.ThumbnailPhotoFileName).HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ProductPhoto", "Production")
			[Property](Function(t) t.ProductPhotoID).HasColumnName("ProductPhotoID")
			[Property](Function(t) t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto")
			[Property](Function(t) t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName")
			[Property](Function(t) t.LargePhoto).HasColumnName("LargePhoto")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ProductPhoto))
			builder.HasKey(Function(t) t.ProductPhotoID)

			' Properties
			builder.Property(Function(t) t.ThumbnailPhotoFileName).HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ProductPhoto", "Production")
			builder.Property(Function(t) t.ProductPhotoID).HasColumnName("ProductPhotoID")
			builder.Property(Function(t) t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto")
			builder.Property(Function(t) t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName")
			builder.Property(Function(t) t.LargePhoto).HasColumnName("LargePhoto")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace