using System.Data.Entity;

namespace AdventureWorksModel
{
    public class AdventureWorksContext : DbContext
    {
        static AdventureWorksContext()
        {
            Database.SetInitializer<AdventureWorksContext>(null);
        }

        public AdventureWorksContext()
            : base("Name=AdventureWorksContext")
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAddress> EmployeeAddresses { get; set; }
        public DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; }
        public DbSet<EmployeePayHistory> EmployeePayHistories { get; set; }
        public DbSet<JobCandidate> JobCandidates { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<BusinessEntity> BusinessEntities { get; set; }
        public DbSet<BusinessEntityAddress> BusinessEntityAddresses { get; set; }
        public DbSet<BusinessEntityContact> BusinessEntityContacts { get; set; }
        public DbSet<Person> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<CountryRegion> CountryRegions { get; set; }
        public DbSet<PersonPhone> PersonPhones { get; set; }
        public DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<StateProvince> StateProvinces { get; set; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Illustration> Illustrations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCostHistory> ProductCostHistories { get; set; }
        public DbSet<ProductDescription> ProductDescriptions { get; set; }
        public DbSet<ProductDocument> ProductDocuments { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<ProductListPriceHistory> ProductListPriceHistories { get; set; }
        public DbSet<ProductModel> ProductModels { get; set; }
        public DbSet<ProductModelIllustration> ProductModelIllustrations { get; set; }
        public DbSet<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures { get; set; }
        public DbSet<ProductPhoto> ProductPhotoes { get; set; }
        public DbSet<ProductProductPhoto> ProductProductPhotoes { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductSubcategory> ProductSubcategories { get; set; }
        public DbSet<ScrapReason> ScrapReasons { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        public DbSet<UnitMeasure> UnitMeasures { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderRouting> WorkOrderRoutings { get; set; }
        public DbSet<ProductVendor> ProductVendors { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public DbSet<ShipMethod> ShipMethods { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<PersonCreditCard> PersonCreditCards { get; set; }
        public DbSet<CountryRegionCurrency> CountryRegionCurrencies { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public DbSet<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons { get; set; }
        public DbSet<SalesPerson> SalesPersons { get; set; }
        public DbSet<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; set; }
        public DbSet<SalesReason> SalesReasons { get; set; }
        public DbSet<SalesTaxRate> SalesTaxRates { get; set; }
        public DbSet<SalesTerritory> SalesTerritories { get; set; }
        public DbSet<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<SpecialOffer> SpecialOffers { get; set; }
        public DbSet<SpecialOfferProduct> SpecialOfferProducts { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new EmployeeAddressMap());
            modelBuilder.Configurations.Add(new EmployeeDepartmentHistoryMap());
            modelBuilder.Configurations.Add(new EmployeePayHistoryMap());
            modelBuilder.Configurations.Add(new JobCandidateMap());
            modelBuilder.Configurations.Add(new ShiftMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AddressTypeMap());
            modelBuilder.Configurations.Add(new BusinessEntityMap());
            modelBuilder.Configurations.Add(new BusinessEntityAddressMap());
            modelBuilder.Configurations.Add(new BusinessEntityContactMap());
            modelBuilder.Configurations.Add(new PersonMap());
            modelBuilder.Configurations.Add(new ContactTypeMap());
            modelBuilder.Configurations.Add(new CountryRegionMap());
            modelBuilder.Configurations.Add(new EmailAddressMap());
            modelBuilder.Configurations.Add(new PasswordMap());
            modelBuilder.Configurations.Add(new PersonPhoneMap());
            modelBuilder.Configurations.Add(new PhoneNumberTypeMap());

            modelBuilder.Configurations.Add(new StateProvinceMap());
            modelBuilder.Configurations.Add(new BillOfMaterialMap());
            modelBuilder.Configurations.Add(new CultureMap());
            modelBuilder.Configurations.Add(new DocumentMap());
            modelBuilder.Configurations.Add(new IllustrationMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductCategoryMap());
            modelBuilder.Configurations.Add(new ProductCostHistoryMap());
            modelBuilder.Configurations.Add(new ProductDescriptionMap());
            modelBuilder.Configurations.Add(new ProductDocumentMap());
            modelBuilder.Configurations.Add(new ProductInventoryMap());
            modelBuilder.Configurations.Add(new ProductListPriceHistoryMap());
            modelBuilder.Configurations.Add(new ProductModelMap());
            modelBuilder.Configurations.Add(new ProductModelIllustrationMap());
            modelBuilder.Configurations.Add(new ProductModelProductDescriptionCultureMap());
            modelBuilder.Configurations.Add(new ProductPhotoMap());
            modelBuilder.Configurations.Add(new ProductProductPhotoMap());
            modelBuilder.Configurations.Add(new ProductReviewMap());
            modelBuilder.Configurations.Add(new ProductSubcategoryMap());
            modelBuilder.Configurations.Add(new ScrapReasonMap());
            modelBuilder.Configurations.Add(new TransactionHistoryMap());
            modelBuilder.Configurations.Add(new UnitMeasureMap());
            modelBuilder.Configurations.Add(new WorkOrderMap());
            modelBuilder.Configurations.Add(new WorkOrderRoutingMap());
            modelBuilder.Configurations.Add(new ProductVendorMap());
            modelBuilder.Configurations.Add(new PurchaseOrderDetailMap());
            modelBuilder.Configurations.Add(new PurchaseOrderHeaderMap());
            modelBuilder.Configurations.Add(new ShipMethodMap());
            modelBuilder.Configurations.Add(new VendorMap());
            modelBuilder.Configurations.Add(new PersonCreditCardMap());
            modelBuilder.Configurations.Add(new CountryRegionCurrencyMap());
            modelBuilder.Configurations.Add(new CreditCardMap());
            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new CurrencyRateMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new SalesOrderDetailMap());
            modelBuilder.Configurations.Add(new SalesOrderHeaderMap());
            modelBuilder.Configurations.Add(new SalesOrderHeaderSalesReasonMap());
            modelBuilder.Configurations.Add(new SalesPersonMap());
            modelBuilder.Configurations.Add(new SalesPersonQuotaHistoryMap());
            modelBuilder.Configurations.Add(new SalesReasonMap());
            modelBuilder.Configurations.Add(new SalesTaxRateMap());
            modelBuilder.Configurations.Add(new SalesTerritoryMap());
            modelBuilder.Configurations.Add(new SalesTerritoryHistoryMap());
            modelBuilder.Configurations.Add(new ShoppingCartItemMap());
            modelBuilder.Configurations.Add(new SpecialOfferMap());
            modelBuilder.Configurations.Add(new SpecialOfferProductMap());
            modelBuilder.Configurations.Add(new StoreMap());
        }
    }
}
