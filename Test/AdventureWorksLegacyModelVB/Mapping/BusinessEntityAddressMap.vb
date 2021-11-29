Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class BusinessEntityAddressMap
		Inherits EntityTypeConfiguration(Of BusinessEntityAddress)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.AddressTypeID,
				Key t.AddressID
			})

			' Table & Column Mappings
			ToTable("BusinessEntityAddress", "Person")
			[Property](Function(t) t.AddressID).HasColumnName("AddressID")
			[Property](Function(t) t.AddressTypeID).HasColumnName("AddressTypeID")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			'Relationships
			HasRequired(Function(t) t.Address).WithMany().HasForeignKey(Function(t) t.AddressID)
			HasRequired(Function(t) t.AddressType).WithMany().HasForeignKey(Function(t) t.AddressTypeID)
			HasRequired(Function(t) t.BusinessEntity).WithMany().HasForeignKey(Function(t) t.BusinessEntityID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of BusinessEntityAddress))
			builder.HasKey(Function(t) New With {
				Key t.BusinessEntityID,
				Key t.AddressTypeID,
				Key t.AddressID
			})

			' Table & Column Mappings
			builder.ToTable("BusinessEntityAddress", "Person")
			builder.Property(Function(t) t.AddressID).HasColumnName("AddressID")
			builder.Property(Function(t) t.AddressTypeID).HasColumnName("AddressTypeID")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			'Relationships
			builder.HasOne(Function(t) t.Address).WithMany().HasForeignKey(Function(t) t.AddressID)
			builder.HasOne(Function(t) t.AddressType).WithMany().HasForeignKey(Function(t) t.AddressTypeID)
			builder.HasOne(Function(t) t.BusinessEntity).WithMany(Function(t) t.Addresses).HasForeignKey(Function(t) t.BusinessEntityID)
		End Sub
	End Module
End Namespace