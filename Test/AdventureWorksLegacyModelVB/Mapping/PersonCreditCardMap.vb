Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class PersonCreditCardMap
		Inherits EntityTypeConfiguration(Of PersonCreditCard)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) New With {
				Key t.PersonID,
				Key t.CreditCardID
			})

			' Properties
			[Property](Function(t) t.PersonID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			[Property](Function(t) t.CreditCardID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)

			' Table & Column Mappings
			ToTable("PersonCreditCard", "Sales")
			[Property](Function(t) t.PersonID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			HasRequired(Function(t) t.Person).WithMany().HasForeignKey(Function(t) t.PersonID)
			HasRequired(Function(t) t.CreditCard).WithMany().HasForeignKey(Function(t) t.CreditCardID)
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of PersonCreditCard))
			builder.HasKey(Function(t) New With {
				Key t.PersonID,
				Key t.CreditCardID
			})

			' Properties
			builder.Property(Function(t) t.PersonID).ValueGeneratedNever()

			builder.Property(Function(t) t.CreditCardID).ValueGeneratedNever()

			' Table & Column Mappings
			builder.ToTable("PersonCreditCard", "Sales")
			builder.Property(Function(t) t.PersonID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.CreditCardID).HasColumnName("CreditCardID")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();

			' Relationships
			builder.HasOne(Function(t) t.Person).WithMany().HasForeignKey(Function(t) t.PersonID)
			builder.HasOne(Function(t) t.CreditCard).WithMany().HasForeignKey(Function(t) t.CreditCardID)
		End Sub
	End Module
End Namespace