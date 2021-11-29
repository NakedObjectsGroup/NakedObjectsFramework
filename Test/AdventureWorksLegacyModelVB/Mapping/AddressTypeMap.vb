Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class AddressTypeMap
		Inherits EntityTypeConfiguration(Of AddressType)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.AddressTypeID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("AddressType", "Person")
			[Property](Function(t) t.AddressTypeID).HasColumnName("AddressTypeID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of AddressType))
			builder.HasKey(Function(t) t.AddressTypeID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("AddressType", "Person")
			builder.Property(Function(t) t.AddressTypeID).HasColumnName("AddressTypeID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace