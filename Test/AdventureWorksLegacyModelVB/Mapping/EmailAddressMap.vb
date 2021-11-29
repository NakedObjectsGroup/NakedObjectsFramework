Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class EmailAddressMap
		Inherits EntityTypeConfiguration(Of EmailAddress)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.EmailAddressID
			})

			' Table & Column Mappings
			ToTable("EmailAddress", "Person")
			[Property](Function(t) t.EmailAddress1).HasColumnName("EmailAddress")
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of EmailAddress))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.EmailAddressID
			})

			' Table & Column Mappings
			builder.ToTable("EmailAddress", "Person")
			builder.Property(Function(t) t.EmailAddress1).HasColumnName("EmailAddress")
		End Sub
	End Module
End Namespace