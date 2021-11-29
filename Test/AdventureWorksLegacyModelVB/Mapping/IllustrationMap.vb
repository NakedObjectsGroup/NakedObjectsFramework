Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class IllustrationMap
		Inherits EntityTypeConfiguration(Of Illustration)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.IllustrationID)

			' Properties
			' Table & Column Mappings
			ToTable("Illustration", "Production")
			[Property](Function(t) t.IllustrationID).HasColumnName("IllustrationID")
			[Property](Function(t) t.Diagram).HasColumnName("Diagram")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Illustration))
			builder.HasKey(Function(t) t.IllustrationID)

			' Properties
			' Table & Column Mappings
			builder.ToTable("Illustration", "Production")
			builder.Property(Function(t) t.IllustrationID).HasColumnName("IllustrationID")
			builder.Property(Function(t) t.Diagram).HasColumnName("Diagram")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace