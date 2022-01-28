

global using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NOF2.Demo.Model
{
    public class DemoEFCoreContext : DbContext
    {
        private readonly string _cs;

        public DemoEFCoreContext(string cs) {
            _cs = cs;
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeAddress> EmployeeAddresses { get; set; }
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
        //public DbSet<Document> Documents { get; set; }
        public DbSet<Illustration> Illustrations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCostHistory> ProductCostHistories { get; set; }
        public DbSet<ProductDescription> ProductDescriptions { get; set; }
        //public DbSet<ProductDocument> ProductDocuments { get; set; }
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_cs);
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.LogTo(m => Debug.WriteLine(m), LogLevel.Trace);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().Map();
            modelBuilder.Entity<Employee>().Map();
            //modelBuilder.Entity<EmployeeAddress>().Map();
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
            //modelBuilder.Entity<Document>().Map();
            modelBuilder.Entity<Illustration>().Map();
            modelBuilder.Entity<Location>().Map();
            modelBuilder.Entity<Product>().Map();
            modelBuilder.Entity<ProductCategory>().Map();
            modelBuilder.Entity<ProductCostHistory>().Map();
            modelBuilder.Entity<ProductDescription>().Map();
            //modelBuilder.Entity<ProductDocument>().Map();
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
