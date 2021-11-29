Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class CreditCardMap
		Inherits EntityTypeConfiguration(Of CreditCard)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.CreditCardID)

			'Ignores

			' Properties
			[Property](Function(t) t.CardType).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.CardNumber).IsRequired().HasMaxLength(25)

			' Table & Column Mappings
			ToTable("CreditCard", "Sales")
			[Property](Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			[Property](Function(t) t.CardType).HasColumnName("CardType")
			[Property](Function(t) t.CardNumber).HasColumnName("CardNumber")
			[Property](Function(t) t.ExpMonth).HasColumnName("ExpMonth")
			[Property](Function(t) t.ExpYear).HasColumnName("ExpYear")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			HasMany(Function(t) t.PersonLinks).WithRequired(Function(t) t.CreditCard)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of CreditCard))
			builder.HasKey(Function(t) t.CreditCardID)

			'Ignores

			' Properties
			builder.Property(Function(t) t.CardType).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.CardNumber).IsRequired().HasMaxLength(25)

			' Table & Column Mappings
			builder.ToTable("CreditCard", "Sales")
			builder.Property(Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			builder.Property(Function(t) t.CardType).HasColumnName("CardType")
			builder.Property(Function(t) t.CardNumber).HasColumnName("CardNumber")
			builder.Property(Function(t) t.ExpMonth).HasColumnName("ExpMonth")
			builder.Property(Function(t) t.ExpYear).HasColumnName("ExpYear")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			builder.HasMany(Function(t) t.PersonLinks).WithOne(Function(t) t.CreditCard)
		End Sub
	End Module
End Namespace