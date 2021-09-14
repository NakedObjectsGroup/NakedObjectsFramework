using AW.Mapping;
using AW.Types;
using Microsoft.EntityFrameworkCore;

namespace AW {
    public class AdventureWorksEFCoreContext : DbContext {
        private readonly string _nameOrConnectionString;

        public AdventureWorksEFCoreContext(string nameOrConnectionString) => _nameOrConnectionString = nameOrConnectionString;

        public DbSet<Department> Departments { get; init; }

        //public DbSet<Employee> Employees { get; init; }
        //public DbSet<EmployeeAddress> EmployeeAddresses { get; init; }
        //public DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; init; }
        //public DbSet<EmployeePayHistory> EmployeePayHistories { get; init; }
        //public DbSet<JobCandidate> JobCandidates { get; init; }
        public DbSet<Shift> Shifts { get; init; }

        //public DbSet<Address> Addresses { get; init; }
        public DbSet<AddressType> AddressTypes { get; init; }

        //public DbSet<BusinessEntity> BusinessEntities { get; init; }
        //public DbSet<BusinessEntityAddress> BusinessEntityAddresses { get; init; }
        //public DbSet<BusinessEntityContact> BusinessEntityContacts { get; init; }
        //public DbSet<Person> Contacts { get; init; }
        public DbSet<ContactType> ContactTypes { get; init; }

        public DbSet<CountryRegion> CountryRegions { get; init; }

        //public DbSet<PersonPhone> PersonPhones { get; init; }
        public DbSet<PhoneNumberType> PhoneNumberTypes { get; init; }

        //public DbSet<EmailAddress> EmailAddresses { get; init; }
        public DbSet<Password> Passwords { get; init; }
        public DbSet<StateProvince> StateProvinces { get; init; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; init; }
        public DbSet<Culture> Cultures { get; init; }
        public DbSet<Illustration> Illustrations { get; init; }
        public DbSet<Location> Locations { get; init; }
        public DbSet<Product> Products { get; init; }
        public DbSet<ProductCategory> ProductCategories { get; init; }
        public DbSet<ProductCostHistory> ProductCostHistories { get; init; }
        public DbSet<ProductDescription> ProductDescriptions { get; init; }
        public DbSet<ProductInventory> ProductInventories { get; init; }
        public DbSet<ProductListPriceHistory> ProductListPriceHistories { get; init; }
        public DbSet<ProductModel> ProductModels { get; init; }
        public DbSet<ProductModelIllustration> ProductModelIllustrations { get; init; }
        public DbSet<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures { get; init; }
        public DbSet<ProductPhoto> ProductPhotoes { get; init; }
        public DbSet<ProductProductPhoto> ProductProductPhotoes { get; init; }
        public DbSet<ProductReview> ProductReviews { get; init; }
        public DbSet<ProductSubcategory> ProductSubcategories { get; init; }
        public DbSet<ScrapReason> ScrapReasons { get; init; }
        public DbSet<TransactionHistory> TransactionHistories { get; init; }
        public DbSet<UnitMeasure> UnitMeasures { get; init; }
        public DbSet<WorkOrder> WorkOrders { get; init; }

        public DbSet<WorkOrderRouting> WorkOrderRoutings { get; init; }

        //public DbSet<ProductVendor> ProductVendors { get; init; }
        //public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; init; }
        //public DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; init; }
        //public DbSet<ShipMethod> ShipMethods { get; init; }
        //public DbSet<Vendor> Vendors { get; init; }
        public DbSet<PersonCreditCard> PersonCreditCards { get; init; }

        //public DbSet<CountryRegionCurrency> CountryRegionCurrencies { get; init; }
        public DbSet<CreditCard> CreditCards { get; init; }

        public DbSet<Currency> Currencies { get; init; }

        //public DbSet<CurrencyRate> CurrencyRates { get; init; }
        //public DbSet<Customer> Customers { get; init; }
        //public DbSet<SalesOrderDetail> SalesOrderDetails { get; init; }
        //public DbSet<SalesOrderHeader> SalesOrderHeaders { get; init; }
        //public DbSet<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons { get; init; }
        //public DbSet<SalesPerson> SalesPersons { get; init; }
        //public DbSet<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; init; }
        //public DbSet<SalesReason> SalesReasons { get; init; }
        //public DbSet<SalesTaxRate> SalesTaxRates { get; init; }
        public DbSet<SalesTerritory> SalesTerritories { get; init; }

        //public DbSet<SalesTerritoryHistory> SalesTerritoryHistories { get; init; }
        //public DbSet<ShoppingCartItem> ShoppingCartItems { get; init; }
        public DbSet<SpecialOffer> SpecialOffers { get; init; }

        public DbSet<SpecialOfferProduct> SpecialOfferProducts { get; init; }
        //public DbSet<Store> Stores { get; init; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_nameOrConnectionString);
            optionsBuilder.UseLazyLoadingProxies();
            //optionsBuilder.LogTo(m => Debug.WriteLine(m), LogLevel.Trace);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Department>().Map();
            modelBuilder.Entity<Employee>().Map();
            modelBuilder.Entity<EmployeeDepartmentHistory>().Map();
            modelBuilder.Entity<EmployeePayHistory>().Map();
            modelBuilder.Entity<JobCandidate>().Map();
            modelBuilder.Entity<Shift>().Map();
            modelBuilder.Entity<Address>().Map();
            modelBuilder.Entity<AddressType>().Map();
            modelBuilder.Entity<BusinessEntity>().Map();
            modelBuilder.Entity<BusinessEntityAddress>().Map();
            modelBuilder.Entity<BusinessEntityContact>().Map();
            modelBuilder.Entity<Person>().Map();
            modelBuilder.Entity<ContactType>().Map();
            modelBuilder.Entity<CountryRegion>().Map();
            modelBuilder.Entity<EmailAddress>().Map();
            modelBuilder.Entity<Password>().Map();
            modelBuilder.Entity<PersonPhone>().Map();
            modelBuilder.Entity<PhoneNumberType>().Map();
            modelBuilder.Entity<StateProvince>().Map();
            modelBuilder.Entity<BillOfMaterial>().Map();
            modelBuilder.Entity<Culture>().Map();
            modelBuilder.Entity<Illustration>().Map();
            modelBuilder.Entity<Location>().Map();
            modelBuilder.Entity<Product>().Map();
            modelBuilder.Entity<ProductCategory>().Map();
            modelBuilder.Entity<ProductCostHistory>().Map();
            modelBuilder.Entity<ProductDescription>().Map();
            modelBuilder.Entity<ProductInventory>().Map();
            modelBuilder.Entity<ProductListPriceHistory>().Map();
            modelBuilder.Entity<ProductModel>().Map();
            modelBuilder.Entity<ProductModelIllustration>().Map();
            modelBuilder.Entity<ProductModelProductDescriptionCulture>().Map();
            modelBuilder.Entity<ProductPhoto>().Map();
            modelBuilder.Entity<ProductProductPhoto>().Map();
            modelBuilder.Entity<ProductReview>().Map();
            modelBuilder.Entity<ProductSubcategory>().Map();
            modelBuilder.Entity<ScrapReason>().Map();
            modelBuilder.Entity<TransactionHistory>().Map();
            modelBuilder.Entity<UnitMeasure>().Map();
            modelBuilder.Entity<WorkOrder>().Map();
            modelBuilder.Entity<WorkOrderRouting>().Map();
            modelBuilder.Entity<ProductVendor>().Map();
            modelBuilder.Entity<PurchaseOrderDetail>().Map();
            modelBuilder.Entity<PurchaseOrderHeader>().Map();
            modelBuilder.Entity<ShipMethod>().Map();
            modelBuilder.Entity<Vendor>().Map();
            modelBuilder.Entity<PersonCreditCard>().Map();
            modelBuilder.Entity<CountryRegionCurrency>().Map();
            modelBuilder.Entity<CreditCard>().Map();
            modelBuilder.Entity<Currency>().Map();
            modelBuilder.Entity<CurrencyRate>().Map();
            modelBuilder.Entity<Customer>().Map();
            modelBuilder.Entity<SalesOrderDetail>().Map();
            modelBuilder.Entity<SalesOrderHeader>().Map();
            modelBuilder.Entity<SalesOrderHeaderSalesReason>().Map();
            modelBuilder.Entity<SalesPerson>().Map();
            modelBuilder.Entity<SalesPersonQuotaHistory>().Map();
            modelBuilder.Entity<SalesReason>().Map();
            modelBuilder.Entity<SalesTaxRate>().Map();
            modelBuilder.Entity<SalesTerritory>().Map();
            modelBuilder.Entity<SalesTerritoryHistory>().Map();
            modelBuilder.Entity<ShoppingCartItem>().Map();
            modelBuilder.Entity<SpecialOffer>().Map();
            modelBuilder.Entity<SpecialOfferProduct>().Map();
            modelBuilder.Entity<Store>().Map();
        }
    }
}