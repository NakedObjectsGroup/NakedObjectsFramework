Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class PhoneNumberTypeMap
		Inherits EntityTypeConfiguration(Of PhoneNumberType)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.PhoneNumberTypeID)

			' Table & Column Mappings
			ToTable("PhoneNumberType", "Person")
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of PhoneNumberType))
			builder.HasKey(Function(t) t.PhoneNumberTypeID)

			' Table & Column Mappings
			builder.ToTable("PhoneNumberType", "Person")
		End Sub
	End Module
End Namespace