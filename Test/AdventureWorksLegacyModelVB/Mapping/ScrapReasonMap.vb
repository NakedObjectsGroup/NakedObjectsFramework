Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ScrapReasonMap
		Inherits EntityTypeConfiguration(Of ScrapReason)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ScrapReasonID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ScrapReason", "Production")
			[Property](Function(t) t.ScrapReasonID).HasColumnName("ScrapReasonID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ScrapReason))
			builder.HasKey(Function(t) t.ScrapReasonID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ScrapReason", "Production")
			builder.Property(Function(t) t.ScrapReasonID).HasColumnName("ScrapReasonID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace