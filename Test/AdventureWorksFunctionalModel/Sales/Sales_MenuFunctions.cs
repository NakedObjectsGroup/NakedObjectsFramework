






using System.Linq;
using AW.Types;
using NakedFunctions;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Sales")]
    public static class Sales_MenuFunctions {
        #region FindSalesPersonByName

        [TableView(true, "SalesTerritory")]
        public static IQueryable<SalesPerson> FindSalesPersonByName(
            [Optionally] string? firstName, string lastName, IContext context) {
            var matchingPersons = Person_MenuFunctions.FindPersonsByName(firstName, lastName, context);
            return from sp in context.Instances<SalesPerson>()
                   from person in matchingPersons
                   where sp.BusinessEntityID == person.BusinessEntityID
                   orderby sp.EmployeeDetails.PersonDetails.LastName, sp.EmployeeDetails.PersonDetails.FirstName
                   select sp;
        }

        #endregion

        public static SalesPerson RandomSalesPerson(IContext context) => Random<SalesPerson>(context);

        [MemberOrder("Sales", 1)]
        [DescribedAs("... from an existing Employee")]
        public static SalesPerson? CreateNewSalesPerson(Employee employee) =>
            //TODO:
            null;

        public static IQueryable<SalesTaxRate> ListSalesTaxRates(IContext context) => context.Instances<SalesTaxRate>();

        [TableView(true, "CurrencyRateDate", "AverageRate", "EndOfDayRate")]
        public static CurrencyRate? FindCurrencyRate(Currency currency1, Currency currency2, IContext context) {
            var name1 = currency1.Name;
            var name2 = currency2.Name;
            return context.Instances<CurrencyRate>().FirstOrDefault(cr => cr.Currency.Name == name1 && cr.Currency1.Name == name2);
        }

        public static Currency Default0FindRate(IContext context) =>
            context.Instances<Currency>().First(c => c.Name == "US Dollar");

        #region ListAccountsForSalesPerson

        [TableView(true)] [MemberOrder("Sales", 1)] //TableView == ListView
        public static IQueryable<Store> ListAccountsForSalesPerson(
            SalesPerson sp,
            IContext context
        ) =>
            from obj in context.Instances<Store>()
            where obj.SalesPerson != null && obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID
            select obj;

        [PageSize(20)]
        public static IQueryable<SalesPerson> AutoComplete0ListAccountsForSalesPerson(
            [MinLength(2)] string name,
            IContext context) {
            return context.Instances<SalesPerson>().Where(sp => sp.EmployeeDetails.PersonDetails.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion
    }
}