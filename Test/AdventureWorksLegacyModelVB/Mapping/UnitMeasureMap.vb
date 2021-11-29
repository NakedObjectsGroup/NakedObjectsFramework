Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class UnitMeasureMap
		Inherits EntityTypeConfiguration(Of UnitMeasure)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.UnitMeasureCode)

			' Properties
			[Property](Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("UnitMeasure", "Production")
			[Property](Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of UnitMeasure))
			builder.HasKey(Function(t) t.UnitMeasureCode)

			' Properties
			builder.Property(Function(t) t.UnitMeasureCode).IsRequired().IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("UnitMeasure", "Production")
			builder.Property(Function(t) t.UnitMeasureCode).HasColumnName("UnitMeasureCode")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace