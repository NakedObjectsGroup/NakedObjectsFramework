using System.Linq;



namespace AW.Test {
    public static class Test_MenuFunctions {
        internal static T? FirstOf<T>(IContext context) where T : class => context.Instances<T>().FirstOrDefault();

        //Human Resources
        [MemberOrder(1)] public static Department? Department(IContext context) => FirstOf<Department>(context);
        [MemberOrder(2)] public static Employee? Employee(IContext context) => FirstOf<Employee>(context);
        [MemberOrder(4)] public static EmployeeDepartmentHistory? EmployeeDepartmentHistory(IContext context) => FirstOf<EmployeeDepartmentHistory>(context);
        [MemberOrder(5)] public static EmployeePayHistory? EmployeePayHistory(IContext context) => FirstOf<EmployeePayHistory>(context);
        [MemberOrder(6)] public static JobCandidate? JobCandidate(IContext context) => FirstOf<JobCandidate>(context);

        [MemberOrder(7)] public static Shift? Shift(IContext context) => FirstOf<Shift>(context);

        //Person
        [MemberOrder(11)] public static Address? Address(IContext context) => FirstOf<Address>(context);
        [MemberOrder(12)] public static AddressType? AddressType(IContext context) => FirstOf<AddressType>(context);
        [MemberOrder(13)] public static BusinessEntity? BusinessEntity(IContext context) => FirstOf<BusinessEntity>(context);
        [MemberOrder(14)] public static BusinessEntityAddress? BusinessEntityAddress(IContext context) => FirstOf<BusinessEntityAddress>(context);
        [MemberOrder(15)] public static BusinessEntityContact? BusinessEntityContact(IContext context) => FirstOf<BusinessEntityContact>(context);
        [MemberOrder(16)] public static ContactType? ContactType(IContext context) => FirstOf<ContactType>(context);
        [MemberOrder(17)] public static CountryRegion? CountryRegion(IContext context) => FirstOf<CountryRegion>(context);
        [MemberOrder(18)] public static EmailAddress? EmailAddress(IContext context) => FirstOf<EmailAddress>(context);
        [MemberOrder(19)] public static Password? Password(IContext context) => FirstOf<Password>(context);
        [MemberOrder(20)] public static Person? Person(IContext context) => FirstOf<Person>(context);
        [MemberOrder(21)] public static PersonPhone? PersonPhone(IContext context) => FirstOf<PersonPhone>(context);
        [MemberOrder(22)] public static PhoneNumberType? PhoneNumberType(IContext context) => FirstOf<PhoneNumberType>(context);

        [MemberOrder(23)] public static StateProvince? StateProvince(IContext context) => FirstOf<StateProvince>(context);

        //Production
        [MemberOrder(31)] public static BillOfMaterial? BillOfMaterial(IContext context) => FirstOf<BillOfMaterial>(context);
        [MemberOrder(32)] public static Culture? Culture(IContext context) => FirstOf<Culture>(context);
        [MemberOrder(34)] public static Illustration? Illustration(IContext context) => FirstOf<Illustration>(context);
        [MemberOrder(35)] public static Location? Location(IContext context) => FirstOf<Location>(context);
        [MemberOrder(36)] public static Product? Product(IContext context) => FirstOf<Product>(context);
        [MemberOrder(37)] public static ProductCategory? ProductCategory(IContext context) => FirstOf<ProductCategory>(context);
        [MemberOrder(38)] public static ProductCostHistory? ProductCostHistory(IContext context) => FirstOf<ProductCostHistory>(context);
        [MemberOrder(39)] public static ProductDescription? ProductDescription(IContext context) => FirstOf<ProductDescription>(context);
        [MemberOrder(41)] public static ProductInventory? ProductInventory(IContext context) => FirstOf<ProductInventory>(context);
        [MemberOrder(42)] public static ProductListPriceHistory? ProductListPriceHistory(IContext context) => FirstOf<ProductListPriceHistory>(context);
        [MemberOrder(43)] public static ProductModel? ProductModel(IContext context) => FirstOf<ProductModel>(context);
        [MemberOrder(44)] public static ProductModelIllustration? ProductModelIllustration(IContext context) => FirstOf<ProductModelIllustration>(context);
        [MemberOrder(45)] public static ProductModelProductDescriptionCulture? ProductModelProductDescriptionCulture(IContext context) => FirstOf<ProductModelProductDescriptionCulture>(context);
        [MemberOrder(46)] public static ProductPhoto? ProductPhoto(IContext context) => FirstOf<ProductPhoto>(context);
        [MemberOrder(47)] public static ProductProductPhoto? ProductProductPhoto(IContext context) => FirstOf<ProductProductPhoto>(context);
        [MemberOrder(48)] public static ProductReview? ProductReview(IContext context) => FirstOf<ProductReview>(context);
        [MemberOrder(49)] public static ProductSubcategory? ProductSubcategory(IContext context) => FirstOf<ProductSubcategory>(context);
        [MemberOrder(50)] public static ScrapReason? ScrapReason(IContext context) => FirstOf<ScrapReason>(context);
        [MemberOrder(51)] public static TransactionHistory? TransactionHistory(IContext context) => FirstOf<TransactionHistory>(context);
        [MemberOrder(52)] public static UnitMeasure? UnitMeasure(IContext context) => FirstOf<UnitMeasure>(context);
        [MemberOrder(53)] public static WorkOrder? WorkOrder(IContext context) => FirstOf<WorkOrder>(context);

        [MemberOrder(54)] public static WorkOrderRouting? WorkOrderRouting(IContext context) => FirstOf<WorkOrderRouting>(context);

        //Purchasing
        [MemberOrder(61)] public static ProductVendor? ProductVendor(IContext context) => FirstOf<ProductVendor>(context);
        [MemberOrder(62)] public static PurchaseOrderDetail? PurchaseOrderDetail(IContext context) => FirstOf<PurchaseOrderDetail>(context);
        [MemberOrder(63)] public static PurchaseOrderHeader? PurchaseOrderHeader(IContext context) => FirstOf<PurchaseOrderHeader>(context);
        [MemberOrder(64)] public static ShipMethod? ShipMethod(IContext context) => FirstOf<ShipMethod>(context);

        [MemberOrder(65)] public static Vendor? Vendor(IContext context) => FirstOf<Vendor>(context);

        //Sales
        [MemberOrder(71)] public static CountryRegionCurrency? CountryRegionCurrency(IContext context) => FirstOf<CountryRegionCurrency>(context);
        [MemberOrder(72)] public static CreditCard? CreditCard(IContext context) => FirstOf<CreditCard>(context);
        [MemberOrder(73)] public static Currency? Currency(IContext context) => FirstOf<Currency>(context);
        [MemberOrder(74)] public static CurrencyRate? CurrencyRate(IContext context) => FirstOf<CurrencyRate>(context);
        [MemberOrder(75)] public static Customer? Customer(IContext context) => FirstOf<Customer>(context);
        [MemberOrder(76)] public static PersonCreditCard? PersonCreditCard(IContext context) => FirstOf<PersonCreditCard>(context);
        [MemberOrder(77)] public static SalesOrderDetail? SalesOrderDetail(IContext context) => FirstOf<SalesOrderDetail>(context);
        [MemberOrder(78)] public static SalesOrderHeader? SalesOrderHeader(IContext context) => FirstOf<SalesOrderHeader>(context);
        [MemberOrder(78)] public static SalesOrderHeaderSalesReason? SalesOrderHeaderSalesReason(IContext context) => FirstOf<SalesOrderHeaderSalesReason>(context);
        [MemberOrder(79)] public static SalesPerson? SalesPerson(IContext context) => FirstOf<SalesPerson>(context);
        [MemberOrder(80)] public static SalesPersonQuotaHistory? SalesPersonQuotaHistory(IContext context) => FirstOf<SalesPersonQuotaHistory>(context);
        [MemberOrder(81)] public static SalesReason? SalesReason(IContext context) => FirstOf<SalesReason>(context);
        [MemberOrder(82)] public static SalesTaxRate? SalesTaxRate(IContext context) => FirstOf<SalesTaxRate>(context);
        [MemberOrder(83)] public static SalesTerritory? SalesTerritory(IContext context) => FirstOf<SalesTerritory>(context);
        [MemberOrder(84)] public static SalesTerritoryHistory? SalesTerritoryHistory(IContext context) => FirstOf<SalesTerritoryHistory>(context);
        [MemberOrder(85)] public static ShoppingCartItem? ShoppingCartItem(IContext context) => FirstOf<ShoppingCartItem>(context);
        [MemberOrder(86)] public static SpecialOffer? SpecialOffer(IContext context) => FirstOf<SpecialOffer>(context);
        [MemberOrder(87)] public static SpecialOfferProduct? SpecialOfferProduct(IContext context) => FirstOf<SpecialOfferProduct>(context);
        [MemberOrder(88)] public static Store? Store(IContext context) => FirstOf<Store>(context);
    }
}