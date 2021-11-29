Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CurrencyMap
		Inherits EntityTypeConfiguration(Of Currency)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CurrencyCode)

			' Properties
			[Property](Function(t) t.CurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			ToTable("Currency", "Sales")
			[Property](Function(t) t.CurrencyCode).HasColumnName("CurrencyCode")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Currency))
			builder.HasKey(Function(t) t.CurrencyCode)

			' Properties
			builder.Property(Function(t) t.CurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			' Table & Column Mappings
			builder.ToTable("Currency", "Sales")
			builder.Property(Function(t) t.CurrencyCode).HasColumnName("CurrencyCode")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace