'====================================================================================================
'The Free Edition of Instant VB limits conversion output to 100 lines per file.

'To purchase the Premium Edition, visit our website:
'https://www.tangiblesoftwaresolutions.com/order/order-instant-vb.html
'====================================================================================================

Imports AW.Mapping

Imports Microsoft.EntityFrameworkCore

Namespace AW
	Public Class AdventureWorksEFCoreContext
		Inherits DbContext

		Private ReadOnly _nameOrConnectionString As String

		Public Sub New(ByVal nameOrConnectionString As String)
			_nameOrConnectionString = nameOrConnectionString
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

		Protected Overrides Sub OnConfiguring(ByVal optionsBuilder As DbContextOptionsBuilder)
			optionsBuilder.UseSqlServer(_nameOrConnectionString)
			optionsBuilder.UseLazyLoadingProxies()
			'optionsBuilder.LogTo(m => Debug.WriteLine(m), LogLevel.Trace);
		End Sub

		Protected Overrides Sub OnModelCreating(ByVal modelBuilder As ModelBuilder)
			modelBuilder.Entity(Of Department)().Map()
			modelBuilder.Entity(Of Employee)().Map()
			modelBuilder.Entity(Of EmployeeDepartmentHistory)().Map()
			modelBuilder.Entity(Of EmployeePayHistory)().Map()
			modelBuilder.Entity(Of JobCandidate)().Map()
			modelBuilder.Entity(Of Shift)().Map()
			modelBuilder.Entity(Of Address)().Map()
			modelBuilder.Entity(Of AddressType)().Map()
			modelBuilder.Entity(Of BusinessEntity)().Map()
			modelBuilder.Entity(Of BusinessEntityAddress)().Map()
			modelBuilder.Entity(Of BusinessEntityContact)().Map()
			modelBuilder.Entity(Of Person)().Map()
			modelBuilder.Entity(Of ContactType)().Map()
			modelBuilder.Entity(Of CountryRegion)().Map()
			modelBuilder.Entity(Of EmailAddress)().Map()
			modelBuilder.Entity(Of Password)().Map()
			modelBuilder.Entity(Of PersonPhone)().Map()
			modelBuilder.Entity(Of PhoneNumberType)().Map()
			modelBuilder.Entity(Of StateProvince)().Map()
			modelBuilder.Entity(Of BillOfMaterial)().Map()
			modelBuilder.Entity(Of Culture)().Map()
			modelBuilder.Entity(Of Illustration)().Map()
			modelBuilder.Entity(Of Location)().Map()
			modelBuilder.Entity(Of Product)().Map()
			modelBuilder.Entity(Of ProductCategory)().Map()
			modelBuilder.Entity(Of ProductCostHistory)().Map()
			modelBuilder.Entity(Of ProductDescription)().Map()
			modelBuilder.Entity(Of ProductInventory)().Map()
			modelBuilder.Entity(Of ProductListPriceHistory)().Map()
			modelBuilder.Entity(Of ProductModel)().Map()
			modelBuilder.Entity(Of ProductModelIllustration)().Map()
			modelBuilder.Entity(Of ProductModelProductDescriptionCulture)().Map()
			modelBuilder.Entity(Of ProductPhoto)().Map()
			modelBuilder.Entity(Of ProductProductPhoto)().Map()
			modelBuilder.Entity(Of ProductReview)().Map()
			modelBuilder.Entity(Of ProductSubcategory)().Map()
			modelBuilder.Entity(Of ScrapReason)().Map()
			modelBuilder.Entity(Of TransactionHistory)().Map()
			modelBuilder.Entity(Of UnitMeasure)().Map()
			modelBuilder.Entity(Of WorkOrder)().Map()
			modelBuilder.Entity(Of WorkOrderRouting)().Map()
			modelBuilder.Entity(Of ProductVendor)().Map()
			modelBuilder.Entity(Of PurchaseOrderDetail)().Map()
			modelBuilder.Entity(Of PurchaseOrderHeader)().Map()
			modelBuilder.Entity(Of ShipMethod)().Map()
			modelBuilder.Entity(Of Vendor)().Map()
			modelBuilder.Entity(Of PersonCreditCard)().Map()
			modelBuilder.Entity(Of CountryRegionCurrency)().Map()
			modelBuilder.Entity(Of CreditCard)().Map()
			modelBuilder.Entity(Of Currency)().Map()

'====================================================================================================
'End of the allowed output for the Free Edition of Instant VB.

'To purchase the Premium Edition, visit our website:
'https://www.tangiblesoftwaresolutions.com/order/order-instant-vb.html
'====================================================================================================
