Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class BusinessEntityContactMap
		Inherits EntityTypeConfiguration(Of BusinessEntityContact)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.PersonID,
				Key t.ContactTypeID
			})

			' Table & Column Mappings
			ToTable("BusinessEntityContact", "Person")
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of BusinessEntityContact))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.PersonID,
				Key t.ContactTypeID
			})

			' Table & Column Mappings
			builder.ToTable("BusinessEntityContact", "Person")
		End Sub
	End Module
End Namespace