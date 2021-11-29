Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class ShipMethodMap
		Inherits EntityTypeConfiguration(Of ShipMethod)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.ShipMethodID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("ShipMethod", "Purchasing")
			[Property](Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ShipBase).HasColumnName("ShipBase")
			[Property](Function(t) t.ShipRate).HasColumnName("ShipRate")
			[Property](Function(t) t.rowguid).HasColumnName("rowguid")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of ShipMethod))
			builder.HasKey(Function(t) t.ShipMethodID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("ShipMethod", "Purchasing")
			builder.Property(Function(t) t.ShipMethodID).HasColumnName("ShipMethodID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ShipBase).HasColumnName("ShipBase")
			builder.Property(Function(t) t.ShipRate).HasColumnName("ShipRate")
			builder.Property(Function(t) t.rowguid).HasColumnName("rowguid")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace