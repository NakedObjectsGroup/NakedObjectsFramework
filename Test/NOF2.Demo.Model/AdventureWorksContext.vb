'====================================================================================================
'The Free Edition of Instant VB limits conversion output to 100 lines per file.

'To purchase the Premium Edition, visit our website:
'https://www.tangiblesoftwaresolutions.com/order/order-instant-vb.html
'====================================================================================================

Imports System.Data.Entity
Imports AW.Mapping


Namespace AW
	Public Class AdventureWorksContext
		Inherits DbContext

		Public Sub New(ByVal nameOrConnectionString As String)
			MyBase.New(nameOrConnectionString)
		End Sub
		Public Property Departments() As DbSet(Of Department)?
		Public Property Shifts() As DbSet(Of Shift)?
		Public Property AddressTypes() As DbSet(Of AddressType)?
		Public Property ContactTypes() As DbSet(Of ContactType)?
		Public Property CountryRegions() As DbSet(Of CountryRegion)?
		Public Property PhoneNumberTypes() As DbSet(Of PhoneNumberType)?
		Public Property Passwords() As DbSet(Of Password)?
		Public Property StateProvinces() As DbSet(Of StateProvince)?
		Public Property BillOfMaterials() As DbSet(Of BillOfMaterial)?
		Public Property Cultures() As DbSet(Of Culture)?
		Public Property Illustrations() As DbSet(Of Illustration)?
		Public Property Locations() As DbSet(Of Location)?
		Public Property Products() As DbSet(Of Product)?
		Public Property ProductCategories() As DbSet(Of ProductCategory)?
		Public Property ProductCostHistories() As DbSet(Of ProductCostHistory)?
		Public Property ProductDescriptions() As DbSet(Of ProductDescription)?
		Public Property ProductInventories() As DbSet(Of ProductInventory)?
		Public Property ProductListPriceHistories() As DbSet(Of ProductListPriceHistory)?
		Public Property ProductModels() As DbSet(Of ProductModel)?
		Public Property ProductModelIllustrations() As DbSet(Of ProductModelIllustration)?
		Public Property ProductModelProductDescriptionCultures() As DbSet(Of ProductModelProductDescriptionCulture)?
		Public Property ProductPhotoes() As DbSet(Of ProductPhoto)?
		Public Property ProductProductPhotoes() As DbSet(Of ProductProductPhoto)?
		Public Property ProductReviews() As DbSet(Of ProductReview)?
		Public Property ProductSubcategories() As DbSet(Of ProductSubcategory)?
		Public Property ScrapReasons() As DbSet(Of ScrapReason)?
		Public Property TransactionHistories() As DbSet(Of TransactionHistory)?
		Public Property UnitMeasures() As DbSet(Of UnitMeasure)?
		Public Property WorkOrders() As DbSet(Of WorkOrder)?
		Public Property WorkOrderRoutings() As DbSet(Of WorkOrderRouting)?
		Public Property PersonCreditCards() As DbSet(Of PersonCreditCard)?
		Public Property CreditCards() As DbSet(Of CreditCard)?
		Public Property Currencies() As DbSet(Of Currency)?
		Public Property SalesTerritories() As DbSet(Of SalesTerritory)?
		Public Property SpecialOffers() As DbSet(Of SpecialOffer)?
		Public Property SpecialOfferProducts() As DbSet(Of SpecialOfferProduct)?

		Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
			modelBuilder.Configurations.Add(New DepartmentMap())
			modelBuilder.Configurations.Add(New EmployeeMap())
			modelBuilder.Configurations.Add(New EmployeeDepartmentHistoryMap())
			modelBuilder.Configurations.Add(New EmployeePayHistoryMap())
			modelBuilder.Configurations.Add(New JobCandidateMap())
			modelBuilder.Configurations.Add(New ShiftMap())
			modelBuilder.Configurations.Add(New AddressMap())
			modelBuilder.Configurations.Add(New AddressTypeMap())
			modelBuilder.Configurations.Add(New BusinessEntityMap())
			modelBuilder.Configurations.Add(New BusinessEntityAddressMap())
			modelBuilder.Configurations.Add(New BusinessEntityContactMap())
			modelBuilder.Configurations.Add(New PersonMap())
			modelBuilder.Configurations.Add(New ContactTypeMap())
			modelBuilder.Configurations.Add(New CountryRegionMap())
			modelBuilder.Configurations.Add(New EmailAddressMap())
			modelBuilder.Configurations.Add(New PasswordMap())
			modelBuilder.Configurations.Add(New PersonPhoneMap())
			modelBuilder.Configurations.Add(New PhoneNumberTypeMap())
			modelBuilder.Configurations.Add(New StateProvinceMap())
			modelBuilder.Configurations.Add(New BillOfMaterialMap())
			modelBuilder.Configurations.Add(New CultureMap())
			modelBuilder.Configurations.Add(New IllustrationMap())
			modelBuilder.Configurations.Add(New LocationMap())
			modelBuilder.Configurations.Add(New ProductMap())
			modelBuilder.Configurations.Add(New ProductCategoryMap())
			modelBuilder.Configurations.Add(New ProductCostHistoryMap())
			modelBuilder.Configurations.Add(New ProductDescriptionMap())
			modelBuilder.Configurations.Add(New ProductInventoryMap())
			modelBuilder.Configurations.Add(New ProductListPriceHistoryMap())
			modelBuilder.Configurations.Add(New ProductModelMap())
			modelBuilder.Configurations.Add(New ProductModelIllustrationMap())
			modelBuilder.Configurations.Add(New ProductModelProductDescriptionCultureMap())
			modelBuilder.Configurations.Add(New ProductPhotoMap())
			modelBuilder.Configurations.Add(New ProductProductPhotoMap())
			modelBuilder.Configurations.Add(New ProductReviewMap())
			modelBuilder.Configurations.Add(New ProductSubcategoryMap())
			modelBuilder.Configurations.Add(New ScrapReasonMap())
			modelBuilder.Configurations.Add(New TransactionHistoryMap())
			modelBuilder.Configurations.Add(New UnitMeasureMap())
			modelBuilder.Configurations.Add(New WorkOrderMap())
			modelBuilder.Configurations.Add(New WorkOrderRoutingMap())
			modelBuilder.Configurations.Add(New ProductVendorMap())
			modelBuilder.Configurations.Add(New PurchaseOrderDetailMap())
			modelBuilder.Configurations.Add(New PurchaseOrderHeaderMap())
			modelBuilder.Configurations.Add(New ShipMethodMap())
			modelBuilder.Configurations.Add(New VendorMap())
			modelBuilder.Configurations.Add(New PersonCreditCardMap())
			modelBuilder.Configurations.Add(New CountryRegionCurrencyMap())
			modelBuilder.Configurations.Add(New CreditCardMap())
			modelBuilder.Configurations.Add(New CurrencyMap())
			modelBuilder.Configurations.Add(New CurrencyRateMap())
			modelBuilder.Configurations.Add(New CustomerMap())
			modelBuilder.Configurations.Add(New SalesOrderDetailMap())
			modelBuilder.Configurations.Add(New SalesOrderHeaderMap())
			modelBuilder.Configurations.Add(New SalesOrderHeaderSalesReasonMap())

'====================================================================================================
'End of the allowed output for the Free Edition of Instant VB.

'To purchase the Premium Edition, visit our website:
'https://www.tangiblesoftwaresolutions.com/order/order-instant-vb.html
'====================================================================================================
