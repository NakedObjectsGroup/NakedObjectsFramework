Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class BusinessEntityMap
		Inherits EntityTypeConfiguration(Of BusinessEntity)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			' Table & Column Mappings
			ToTable("BusinessEntity", "Person")
			[Property](Function(t) t.BusinessEntityRowguid).HasColumnName("rowguid")
			[Property](Function(t) t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			HasMany(Function(t) t.Addresses).WithRequired(Function(t) t.BusinessEntity)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of BusinessEntity))
			builder.HasKey(Function(t) t.BusinessEntityID)

			' Table & Column Mappings
			builder.ToTable("BusinessEntity", "Person")
			builder.Property(Function(t) t.BusinessEntityRowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			builder.HasMany(Function(t) t.Addresses).WithOne(Function(t) t.BusinessEntity)
		End Sub
	End Module
End Namespace