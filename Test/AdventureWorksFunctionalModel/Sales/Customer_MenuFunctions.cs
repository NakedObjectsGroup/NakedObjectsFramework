namespace AW.Functions;

[Named("Customers")]
public static class Customer_MenuFunctions
{
    [MemberOrder(10)]
    public static Customer? FindCustomerByAccountNumber(
        [DefaultValue("AW")] [RegEx(@"^AW\d{8}$")]
            string accountNumber, IContext context) =>
        context.Instances<Customer>().FirstOrDefault(x => x.AccountNumber == accountNumber);

    public static IQueryable<Customer> ListCustomersForSalesTerritory(SalesTerritory territory, IContext context)
    {
        var id = territory.TerritoryID;
        return context.Instances<Customer>().Where(c => c.SalesTerritory != null && c.SalesTerritory.TerritoryID == id);
    }

    #region Stores Menu

    [PageSize(2)]
    [MemberOrder("Stores", 1)]
    [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
    public static IQueryable<Customer> FindStoreByName(
        [DescribedAs("partial match")] string name, IContext context)
    {
        var customers = context.Instances<Customer>();
        var stores = context.Instances<Store>();
        return from c in customers
               from s in stores
               where s.Name.ToUpper().Contains(name.ToUpper()) &&
                     c.StoreID == s.BusinessEntityID
               select c;
    }

    [MemberOrder("Stores", 2)]
    public static (Customer, IContext) CreateNewStoreCustomer(
        string name, IContext context)
    {
        var s = new Store { Name = name, rowguid = context.NewGuid(), BusinessEntityRowguid = context.NewGuid(), ModifiedDate = context.Now(), BusinessEntityModifiedDate = context.Now() };
        var c = new Customer { Store = s, CustomerRowguid = context.NewGuid(), CustomerModifiedDate = context.Now() };
        return (c, context.WithNew(c).WithNew(s));
    }

    [MemberOrder("Stores", 3)]
    public static Customer RandomStore(IContext context)
    {
        var stores = context.Instances<Customer>().Where(t => t.StoreID != null).OrderBy(t => "");
        var random = context.RandomSeed().ValueInRange(stores.Count());
        return stores.Skip(random).First();
    }

    #endregion

    #region Individuals Menu

    [MemberOrder("Individuals", 1)]
    [TableView(true)] //Table view == List View
    public static IQueryable<Customer> FindIndividualCustomerByName(
        [Optionally] string firstName, string lastName, IContext context)
    {
        var matchingPersons = Person_MenuFunctions.FindPersonsByName(firstName, lastName, context);
        return from c in context.Instances<Customer>()
               from p in matchingPersons
               where c.PersonID == p.BusinessEntityID
               select c;
    }

    [MemberOrder("Individuals", 2)]
    public static (Customer, IContext) CreateNewIndividualCustomer(
        string firstName,
        string lastName,
        [Optionally][Password] string password,
        IContext context)
    {
        var p = new Person
        {
            PersonType = "SC",
            FirstName = firstName,
            LastName = lastName,
            NameStyle = false,
            EmailPromotion = 0,
            rowguid = context.NewGuid(),
            BusinessEntityRowguid = context.NewGuid(),
            ModifiedDate = context.Now(),
            BusinessEntityModifiedDate = context.Now()
        };
        var c = new Customer { Person = p, CustomerRowguid = context.NewGuid(), CustomerModifiedDate = context.Now() };
        var context2 = context.WithNew(c).WithNew(p);
        return string.IsNullOrEmpty(password)
            ? (c, context2)
            : (c, context2.WithNew(Password_Functions.CreateNewPassword(password, p, context)));
    }

    [MemberOrder("Individuals", 3)]
    public static Customer RandomIndividual(IContext context)
    {
        var indivs = context.Instances<Customer>().Where(t => t.PersonID != null).OrderBy(t => "");
        var random = context.RandomSeed().ValueInRange(indivs.Count());
        return indivs.Skip(random).First();
    }

    [MemberOrder("Individuals", 4)]
    public static IQueryable<Customer> RecentIndividualCustomers(IContext context) =>
        context.Instances<Customer>().Where(t => t.PersonID != null).OrderByDescending(t => t.CustomerModifiedDate);

    #endregion
}