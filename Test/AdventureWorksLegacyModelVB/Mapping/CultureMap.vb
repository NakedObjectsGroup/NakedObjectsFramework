Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CultureMap
		Inherits EntityTypeConfiguration(Of Culture)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CultureID)

			' Properties
			[Property](Function(t) t.CultureID).IsRequired().IsFixedLength().HasMaxLength(6)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Culture", "Production")
			[Property](Function(t) t.CultureID).HasColumnName("CultureID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Culture))
			builder.HasKey(Function(t) t.CultureID)

			' Properties
			builder.Property(Function(t) t.CultureID).IsRequired().IsFixedLength().HasMaxLength(6)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Culture", "Production")
			builder.Property(Function(t) t.CultureID).HasColumnName("CultureID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace