using System.Data.Entity;
using AW.Mapping;


namespace AW {
    public class AdventureWorksContext : DbContext {
        public AdventureWorksContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public DbSet<Department>?  Departments { get; init; }
        public DbSet<Shift>?  Shifts { get; init; }
        public DbSet<AddressType>?  AddressTypes { get; init; }
        public DbSet<ContactType>?  ContactTypes { get; init; }
        public DbSet<CountryRegion>?  CountryRegions { get; init; }
        public DbSet<PhoneNumberType>?  PhoneNumberTypes { get; init; }
        public DbSet<Password>?  Passwords { get; init; }
        public DbSet<StateProvince>?  StateProvinces { get; init; }
        public DbSet<BillOfMaterial>?  BillOfMaterials { get; init; }
        public DbSet<Culture>?  Cultures { get; init; }
        public DbSet<Illustration>?  Illustrations { get; init; }
        public DbSet<Location>?  Locations { get; init; }
        public DbSet<Product>?  Products { get; init; }
        public DbSet<ProductCategory>?  ProductCategories { get; init; }
        public DbSet<ProductCostHistory>?  ProductCostHistories { get; init; }
        public DbSet<ProductDescription>?  ProductDescriptions { get; init; }
        public DbSet<ProductInventory>?  ProductInventories { get; init; }
        public DbSet<ProductListPriceHistory>?  ProductListPriceHistories { get; init; }
        public DbSet<ProductModel>?  ProductModels { get; init; }
        public DbSet<ProductModelIllustration>?  ProductModelIllustrations { get; init; }
        public DbSet<ProductModelProductDescriptionCulture>?  ProductModelProductDescriptionCultures { get; init; }
        public DbSet<ProductPhoto>?  ProductPhotoes { get; init; }
        public DbSet<ProductProductPhoto>?  ProductProductPhotoes { get; init; }
        public DbSet<ProductReview>?  ProductReviews { get; init; }
        public DbSet<ProductSubcategory>?  ProductSubcategories { get; init; }
        public DbSet<ScrapReason>?  ScrapReasons { get; init; }
        public DbSet<TransactionHistory>?  TransactionHistories { get; init; }
        public DbSet<UnitMeasure>?  UnitMeasures { get; init; }
        public DbSet<WorkOrder>?  WorkOrders { get; init; }
        public DbSet<WorkOrderRouting>?  WorkOrderRoutings { get; init; }
        public DbSet<PersonCreditCard>?  PersonCreditCards { get; init; }
        public DbSet<CreditCard>?  CreditCards { get; init; }
        public DbSet<Currency>?  Currencies { get; init; }
        public DbSet<SalesTerritory>?  SalesTerritories { get; init; }
        public DbSet<SpecialOffer>?  SpecialOffers { get; init; }
        public DbSet<SpecialOfferProduct>?  SpecialOfferProducts { get; init; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
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
            modelBuilder.Configurations.Add(new IllustrationMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductCategoryMap());
            modelBuilder.Configurations.Add(new ProductCostHistoryMap());
            modelBuilder.Configurations.Add(new ProductDescriptionMap());
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