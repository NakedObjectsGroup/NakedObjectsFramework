Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class LocationMap
		Inherits EntityTypeConfiguration(Of Location)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.LocationID)

			' Properties
			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Location", "Production")
			[Property](Function(t) t.LocationID).HasColumnName("LocationID")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.CostRate).HasColumnName("CostRate")
			[Property](Function(t) t.Availability).HasColumnName("Availability")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Location))
			builder.HasKey(Function(t) t.LocationID)

			' Properties
			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Location", "Production")
			builder.Property(Function(t) t.LocationID).HasColumnName("LocationID")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.CostRate).HasColumnName("CostRate")
			builder.Property(Function(t) t.Availability).HasColumnName("Availability")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace