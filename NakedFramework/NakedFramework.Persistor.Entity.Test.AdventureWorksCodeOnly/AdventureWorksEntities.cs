// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    public class AdventureWorksEntities : DbContext {
        public AdventureWorksEntities(string name)
            : base(name) { }

        public virtual DbSet<AWBuildVersion> AWBuildVersions { get; set; }
        public virtual DbSet<DatabaseLog> DatabaseLogs { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; }
        public virtual DbSet<EmployeePayHistory> EmployeePayHistories { get; set; }
        public virtual DbSet<JobCandidate> JobCandidates { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<CountryRegion> CountryRegions { get; set; }
        public virtual DbSet<StateProvince> StateProvinces { get; set; }
        public virtual DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public virtual DbSet<Culture> Cultures { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Illustration> Illustrations { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCostHistory> ProductCostHistories { get; set; }
        public virtual DbSet<ProductDescription> ProductDescriptions { get; set; }
        public virtual DbSet<ProductDocument> ProductDocuments { get; set; }
        public virtual DbSet<ProductInventory> ProductInventories { get; set; }
        public virtual DbSet<ProductListPriceHistory> ProductListPriceHistories { get; set; }
        public virtual DbSet<ProductModel> ProductModels { get; set; }
        public virtual DbSet<ProductModelIllustration> ProductModelIllustrations { get; set; }
        public virtual DbSet<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures { get; set; }
        public virtual DbSet<ProductPhoto> ProductPhotoes { get; set; }
        public virtual DbSet<ProductProductPhoto> ProductProductPhotoes { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<ProductSubcategory> ProductSubcategories { get; set; }
        public virtual DbSet<ScrapReason> ScrapReasons { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }
        public virtual DbSet<TransactionHistoryArchive> TransactionHistoryArchives { get; set; }
        public virtual DbSet<UnitMeasure> UnitMeasures { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }
        public virtual DbSet<WorkOrderRouting> WorkOrderRoutings { get; set; }
        public virtual DbSet<ProductVendor> ProductVendors { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual DbSet<ShipMethod> ShipMethods { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<VendorAddress> VendorAddresses { get; set; }
        public virtual DbSet<VendorContact> VendorContacts { get; set; }
        public virtual DbSet<ContactCreditCard> ContactCreditCards { get; set; }
        public virtual DbSet<CountryRegionCurrency> CountryRegionCurrencies { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyRate> CurrencyRates { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<Individual> Individuals { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual DbSet<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons { get; set; }
        public virtual DbSet<SalesPerson> SalesPersons { get; set; }
        public virtual DbSet<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; set; }
        public virtual DbSet<SalesReason> SalesReasons { get; set; }
        public virtual DbSet<SalesTaxRate> SalesTaxRates { get; set; }
        public virtual DbSet<SalesTerritory> SalesTerritories { get; set; }
        public virtual DbSet<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual DbSet<SpecialOffer> SpecialOffers { get; set; }
        public virtual DbSet<SpecialOfferProduct> SpecialOfferProducts { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreContact> StoreContacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.EmployeeDepartmentHistories)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.MaritalStatus)
                .IsFixedLength();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Gender)
                .IsFixedLength();

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Employee1)
                .WithOptional(e => e.Employee2)
                .HasForeignKey(e => e.ManagerID);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeAddresses)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeDepartmentHistories)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeePayHistories)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PurchaseOrderHeaders)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasOptional(e => e.SalesPerson)
                .WithRequired(e => e.Employee);

            modelBuilder.Entity<EmployeePayHistory>()
                .Property(e => e.Rate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Shift>()
                .HasMany(e => e.EmployeeDepartmentHistories)
                .WithRequired(e => e.Shift)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.EmployeeAddresses)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.CustomerAddresses)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.SalesOrderHeaders)
                .WithRequired(e => e.Address)
                .HasForeignKey(e => e.BillToAddressID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.SalesOrderHeaders1)
                .WithRequired(e => e.Address1)
                .HasForeignKey(e => e.ShipToAddressID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.VendorAddresses)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AddressType>()
                .HasMany(e => e.CustomerAddresses)
                .WithRequired(e => e.AddressType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AddressType>()
                .HasMany(e => e.VendorAddresses)
                .WithRequired(e => e.AddressType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .Property(e => e.PasswordHash)
                .IsUnicode(false);

            modelBuilder.Entity<Contact>()
                .Property(e => e.PasswordSalt)
                .IsUnicode(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.Employees)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.ContactCreditCards)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.Individuals)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.SalesOrderHeaders)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.StoreContacts)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasMany(e => e.VendorContacts)
                .WithRequired(e => e.Contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ContactType>()
                .HasMany(e => e.StoreContacts)
                .WithRequired(e => e.ContactType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ContactType>()
                .HasMany(e => e.VendorContacts)
                .WithRequired(e => e.ContactType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CountryRegion>()
                .HasMany(e => e.CountryRegionCurrencies)
                .WithRequired(e => e.CountryRegion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CountryRegion>()
                .HasMany(e => e.StateProvinces)
                .WithRequired(e => e.CountryRegion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StateProvince>()
                .Property(e => e.StateProvinceCode)
                .IsFixedLength();

            modelBuilder.Entity<StateProvince>()
                .HasMany(e => e.Addresses)
                .WithRequired(e => e.StateProvince)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StateProvince>()
                .HasMany(e => e.SalesTaxRates)
                .WithRequired(e => e.StateProvince)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BillOfMaterial>()
                .Property(e => e.UnitMeasureCode)
                .IsFixedLength();

            modelBuilder.Entity<BillOfMaterial>()
                .Property(e => e.PerAssemblyQty)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Culture>()
                .Property(e => e.CultureID)
                .IsFixedLength();

            modelBuilder.Entity<Culture>()
                .HasMany(e => e.ProductModelProductDescriptionCultures)
                .WithRequired(e => e.Culture)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Document>()
                .Property(e => e.Revision)
                .IsFixedLength();

            modelBuilder.Entity<Document>()
                .HasMany(e => e.ProductDocuments)
                .WithRequired(e => e.Document)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Illustration>()
                .HasMany(e => e.ProductModelIllustrations)
                .WithRequired(e => e.Illustration)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.CostRate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Location>()
                .Property(e => e.Availability)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.ProductInventories)
                .WithRequired(e => e.Location)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.WorkOrderRoutings)
                .WithRequired(e => e.Location)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.StandardCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.ListPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.SizeUnitMeasureCode)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .Property(e => e.WeightUnitMeasureCode)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .Property(e => e.Weight)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductLine)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .Property(e => e.Class)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .Property(e => e.Style)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.BillOfMaterials)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.ComponentID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.BillOfMaterials1)
                .WithOptional(e => e.Product1)
                .HasForeignKey(e => e.ProductAssemblyID);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductCostHistories)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductDocuments)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductInventories)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductListPriceHistories)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductProductPhotoes)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductReviews)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductVendors)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.PurchaseOrderDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ShoppingCartItems)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.SpecialOfferProducts)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.TransactionHistories)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.WorkOrders)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.ProductSubcategories)
                .WithRequired(e => e.ProductCategory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCostHistory>()
                .Property(e => e.StandardCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductDescription>()
                .HasMany(e => e.ProductModelProductDescriptionCultures)
                .WithRequired(e => e.ProductDescription)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductListPriceHistory>()
                .Property(e => e.ListPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductModel>()
                .HasMany(e => e.ProductModelIllustrations)
                .WithRequired(e => e.ProductModel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductModel>()
                .HasMany(e => e.ProductModelProductDescriptionCultures)
                .WithRequired(e => e.ProductModel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductModelProductDescriptionCulture>()
                .Property(e => e.CultureID)
                .IsFixedLength();

            modelBuilder.Entity<ProductPhoto>()
                .HasMany(e => e.ProductProductPhotoes)
                .WithRequired(e => e.ProductPhoto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionHistory>()
                .Property(e => e.TransactionType)
                .IsFixedLength();

            modelBuilder.Entity<TransactionHistory>()
                .Property(e => e.ActualCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TransactionHistoryArchive>()
                .Property(e => e.TransactionType)
                .IsFixedLength();

            modelBuilder.Entity<TransactionHistoryArchive>()
                .Property(e => e.ActualCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<UnitMeasure>()
                .Property(e => e.UnitMeasureCode)
                .IsFixedLength();

            modelBuilder.Entity<UnitMeasure>()
                .HasMany(e => e.BillOfMaterials)
                .WithRequired(e => e.UnitMeasure)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UnitMeasure>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.UnitMeasure)
                .HasForeignKey(e => e.SizeUnitMeasureCode);

            modelBuilder.Entity<UnitMeasure>()
                .HasMany(e => e.Products1)
                .WithOptional(e => e.UnitMeasure1)
                .HasForeignKey(e => e.WeightUnitMeasureCode);

            modelBuilder.Entity<UnitMeasure>()
                .HasMany(e => e.ProductVendors)
                .WithRequired(e => e.UnitMeasure)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkOrder>()
                .HasMany(e => e.WorkOrderRoutings)
                .WithRequired(e => e.WorkOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkOrderRouting>()
                .Property(e => e.ActualResourceHrs)
                .HasPrecision(9, 4);

            modelBuilder.Entity<WorkOrderRouting>()
                .Property(e => e.PlannedCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<WorkOrderRouting>()
                .Property(e => e.ActualCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductVendor>()
                .Property(e => e.StandardPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductVendor>()
                .Property(e => e.LastReceiptCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProductVendor>()
                .Property(e => e.UnitMeasureCode)
                .IsFixedLength();

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.LineTotal)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.ReceivedQty)
                .HasPrecision(8, 2);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.RejectedQty)
                .HasPrecision(8, 2);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(e => e.StockedQty)
                .HasPrecision(9, 2);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .Property(e => e.SubTotal)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .Property(e => e.TaxAmt)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .Property(e => e.Freight)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .Property(e => e.TotalDue)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .HasMany(e => e.PurchaseOrderDetails)
                .WithRequired(e => e.PurchaseOrderHeader)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ShipMethod>()
                .Property(e => e.ShipBase)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ShipMethod>()
                .Property(e => e.ShipRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ShipMethod>()
                .HasMany(e => e.PurchaseOrderHeaders)
                .WithRequired(e => e.ShipMethod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ShipMethod>()
                .HasMany(e => e.SalesOrderHeaders)
                .WithRequired(e => e.ShipMethod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vendor>()
                .HasMany(e => e.ProductVendors)
                .WithRequired(e => e.Vendor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vendor>()
                .HasMany(e => e.PurchaseOrderHeaders)
                .WithRequired(e => e.Vendor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vendor>()
                .HasMany(e => e.VendorAddresses)
                .WithRequired(e => e.Vendor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vendor>()
                .HasMany(e => e.VendorContacts)
                .WithRequired(e => e.Vendor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CountryRegionCurrency>()
                .Property(e => e.CurrencyCode)
                .IsFixedLength();

            modelBuilder.Entity<CreditCard>()
                .HasMany(e => e.ContactCreditCards)
                .WithRequired(e => e.CreditCard)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .Property(e => e.CurrencyCode)
                .IsFixedLength();

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CountryRegionCurrencies)
                .WithRequired(e => e.Currency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CurrencyRates)
                .WithRequired(e => e.Currency)
                .HasForeignKey(e => e.FromCurrencyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CurrencyRates1)
                .WithRequired(e => e.Currency1)
                .HasForeignKey(e => e.ToCurrencyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CurrencyRate>()
                .Property(e => e.FromCurrencyCode)
                .IsFixedLength();

            modelBuilder.Entity<CurrencyRate>()
                .Property(e => e.ToCurrencyCode)
                .IsFixedLength();

            modelBuilder.Entity<CurrencyRate>()
                .Property(e => e.AverageRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CurrencyRate>()
                .Property(e => e.EndOfDayRate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Customer>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.CustomerType)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.CustomerAddresses)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasOptional(e => e.Individual)
                .WithRequired(e => e.Customer);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.SalesOrderHeaders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .HasOptional(e => e.Store)
                .WithRequired(e => e.Customer);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(e => e.UnitPriceDiscount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(e => e.LineTotal)
                .HasPrecision(38, 6);

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.CreditCardApprovalCode)
                .IsUnicode(false);

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.SubTotal)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.TaxAmt)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.Freight)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.TotalDue)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesPerson>()
                .Property(e => e.SalesQuota)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesPerson>()
                .Property(e => e.Bonus)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesPerson>()
                .Property(e => e.CommissionPct)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SalesPerson>()
                .Property(e => e.SalesYTD)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesPerson>()
                .Property(e => e.SalesLastYear)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesPerson>()
                .HasMany(e => e.SalesPersonQuotaHistories)
                .WithRequired(e => e.SalesPerson)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SalesPerson>()
                .HasMany(e => e.SalesTerritoryHistories)
                .WithRequired(e => e.SalesPerson)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SalesPersonQuotaHistory>()
                .Property(e => e.SalesQuota)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesReason>()
                .HasMany(e => e.SalesOrderHeaderSalesReasons)
                .WithRequired(e => e.SalesReason)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SalesTaxRate>()
                .Property(e => e.TaxRate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SalesTerritory>()
                .Property(e => e.SalesYTD)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesTerritory>()
                .Property(e => e.SalesLastYear)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesTerritory>()
                .Property(e => e.CostYTD)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesTerritory>()
                .Property(e => e.CostLastYear)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SalesTerritory>()
                .HasMany(e => e.StateProvinces)
                .WithRequired(e => e.SalesTerritory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SalesTerritory>()
                .HasMany(e => e.SalesTerritoryHistories)
                .WithRequired(e => e.SalesTerritory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SpecialOffer>()
                .Property(e => e.DiscountPct)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SpecialOffer>()
                .HasMany(e => e.SpecialOfferProducts)
                .WithRequired(e => e.SpecialOffer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SpecialOfferProduct>()
                .HasMany(e => e.SalesOrderDetails)
                .WithRequired(e => e.SpecialOfferProduct)
                .HasForeignKey(e => new {e.SpecialOfferID, e.ProductID})
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.StoreContacts)
                .WithRequired(e => e.Store)
                .WillCascadeOnDelete(false);
        }
    }
}