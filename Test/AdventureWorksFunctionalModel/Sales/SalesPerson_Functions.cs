namespace AW.Functions;
public static class SalesPerson_Functions
{

    [MemberOrder(1)]
    public static IContext RecalulateSalesYTD(this SalesPerson sp, IContext context)
    {
        var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
        var query = from obj in context.Instances<SalesOrderHeader>()
                    where obj.SalesPerson != null && obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID &&
                          obj.StatusByte == 5 &&
                          obj.OrderDate >= startOfYear
                    select obj;
        var newYTD = query.Any() ? query.Sum(n => n.SubTotal) : 0;

        return context.WithUpdated(sp, new(sp)
        {
            SalesYTD = newYTD,
            ModifiedDate = context.Now()
        });
    }

    [MemberOrder(2)]
    public static IContext ChangeSalesQuota(
        this SalesPerson sp, decimal newQuota, IContext context) =>
        context.WithUpdated(sp, new(sp) { SalesQuota = newQuota, ModifiedDate = context.Now() })
            .WithNew(new SalesPersonQuotaHistory { SalesPerson = sp, SalesQuota = newQuota, QuotaDate = context.Now() });

    [MemberOrder(1)]
    public static IContext ChangeSalesTerritory(this SalesPerson sp, SalesTerritory newTerritory, IContext context)
    {
        var newHist = new SalesTerritoryHistory { SalesPerson = sp, SalesTerritory = newTerritory, StartDate = context.Now() };
        var prev = sp.TerritoryHistory.First(n => n.EndDate == null);
        SalesTerritoryHistory uPrev = new(prev) { EndDate = context.Now() };
        SalesPerson uSp = new(sp) { SalesTerritory = newTerritory, ModifiedDate = context.Now() };
        return context.WithNew(newHist).WithUpdated(sp, uSp).WithUpdated(prev, uPrev);
    }
}
    