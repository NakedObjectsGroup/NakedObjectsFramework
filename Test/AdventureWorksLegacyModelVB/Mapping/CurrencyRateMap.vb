Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CurrencyRateMap
		Inherits EntityTypeConfiguration(Of CurrencyRate)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CurrencyRateID)

			' Properties
			[Property](Function(t) t.FromCurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			[Property](Function(t) t.ToCurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			ToTable("CurrencyRate", "Sales")
			[Property](Function(t) t.CurrencyRateID).HasColumnName("CurrencyRateID")
			[Property](Function(t) t.CurrencyRateDate).HasColumnName("CurrencyRateDate")
			[Property](Function(t) t.FromCurrencyCode).HasColumnName("FromCurrencyCode")
			[Property](Function(t) t.ToCurrencyCode).HasColumnName("ToCurrencyCode")
			[Property](Function(t) t.AverageRate).HasColumnName("AverageRate")
			[Property](Function(t) t.EndOfDayRate).HasColumnName("EndOfDayRate")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Currency).WithMany().HasForeignKey(Function(t) t.FromCurrencyCode)
			HasRequired(Function(t) t.Currency1).WithMany().HasForeignKey(Function(t) t.ToCurrencyCode)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of CurrencyRate))
			builder.HasKey(Function(t) t.CurrencyRateID)

			' Properties
			builder.Property(Function(t) t.FromCurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			builder.Property(Function(t) t.ToCurrencyCode).IsRequired().IsFixedLength().HasMaxLength(3)

			' Table & Column Mappings
			builder.ToTable("CurrencyRate", "Sales")
			builder.Property(Function(t) t.CurrencyRateID).HasColumnName("CurrencyRateID")
			builder.Property(Function(t) t.CurrencyRateDate).HasColumnName("CurrencyRateDate")
			builder.Property(Function(t) t.FromCurrencyCode).HasColumnName("FromCurrencyCode")
			builder.Property(Function(t) t.ToCurrencyCode).HasColumnName("ToCurrencyCode")
			builder.Property(Function(t) t.AverageRate).HasColumnName("AverageRate")
			builder.Property(Function(t) t.EndOfDayRate).HasColumnName("EndOfDayRate")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Currency).WithMany().HasForeignKey(Function(t) t.FromCurrencyCode)
			builder.HasOne(Function(t) t.Currency1).WithMany().HasForeignKey(Function(t) t.ToCurrencyCode)
		End Sub
	End Module
End Namespace