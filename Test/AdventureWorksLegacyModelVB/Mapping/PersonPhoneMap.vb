Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class PersonPhoneMap
		Inherits EntityTypeConfiguration(Of PersonPhone)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.PhoneNumber,
				Key t.PhoneNumberTypeID
			})

			' Table & Column Mappings
			ToTable("PersonPhone", "Person")
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of PersonPhone))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.PhoneNumber,
				Key t.PhoneNumberTypeID
			})

			' Table & Column Mappings
			builder.ToTable("PersonPhone", "Person")
		End Sub
	End Module
End Namespace