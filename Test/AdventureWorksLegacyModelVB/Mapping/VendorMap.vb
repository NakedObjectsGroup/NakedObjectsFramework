Imports System.Data.Entity.ModelConfiguration

Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace AW.Mapping
	Public Class VendorMap
		Inherits EntityTypeConfiguration(Of Vendor)

		Public Sub New()
			' Primary Key
			HasKey(Function(t) t.BusinessEntityID)

			' Properties
			[Property](Function(t) t.AccountNumber).IsRequired().HasMaxLength(15)

			[Property](Function(t) t.Name).IsRequired().HasMaxLength(50)

			[Property](Function(t) t.PurchasingWebServiceURL).HasMaxLength(1024)

			' Table & Column Mappings
			ToTable("Vendor", "Purchasing")
			[Property](Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			[Property](Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			[Property](Function(t) t.Name).HasColumnName("Name")
			[Property](Function(t) t.CreditRating).HasColumnName("CreditRating")
			[Property](Function(t) t.PreferredVendorStatus).HasColumnName("PreferredVendorStatus")
			[Property](Function(t) t.ActiveFlag).HasColumnName("ActiveFlag")
			[Property](Function(t) t.PurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL")
			[Property](Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Class

	Public Module Mapper
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Map(ByVal builder As EntityTypeBuilder(Of Vendor))
			builder.HasKey(Function(t) t.BusinessEntityID)

			' Properties
			builder.Property(Function(t) t.AccountNumber).IsRequired().HasMaxLength(15)

			builder.Property(Function(t) t.Name).IsRequired().HasMaxLength(50)

			builder.Property(Function(t) t.PurchasingWebServiceURL).HasMaxLength(1024)

			' Table & Column Mappings
			builder.ToTable("Vendor", "Purchasing")
			builder.Property(Function(t) t.BusinessEntityID).HasColumnName("BusinessEntityID")
			builder.Property(Function(t) t.AccountNumber).HasColumnName("AccountNumber")
			builder.Property(Function(t) t.Name).HasColumnName("Name")
			builder.Property(Function(t) t.CreditRating).HasColumnName("CreditRating")
			builder.Property(Function(t) t.PreferredVendorStatus).HasColumnName("PreferredVendorStatus")
			builder.Property(Function(t) t.ActiveFlag).HasColumnName("ActiveFlag")
			builder.Property(Function(t) t.PurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL")
			builder.Property(Function(t) t.ModifiedDate).HasColumnName("ModifiedDate") '.IsConcurrencyToken();
		End Sub
	End Module
End Namespace