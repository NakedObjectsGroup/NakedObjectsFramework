Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ContactTypeMap
		Inherits EntityTypeConfiguration(Of ContactType)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ContactTypeID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ContactType", "Person")
			[Property](Function(t) t.ContactTypeID).HasColumnName("ContactTypeID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ContactType))
			builder.HasKey(Function(t) t.ContactTypeID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ContactType", "Person")
			builder.Property(Function(t) t.ContactTypeID).HasColumnName("ContactTypeID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace