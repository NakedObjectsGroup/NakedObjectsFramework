namespace AW.Functions;
public static class CustomerDashboard_Functions
{
    public static string[] DeriveKeys(this CustomerDashboard cd) =>
        new[] { cd.Root.CustomerID.ToString() };

    public static CustomerDashboard CreateFromKeys(string[] keys, IContext context)
    {
        var customerId = int.Parse(keys[0]);
        return new CustomerDashboard
        {
            Root = context.Instances<Customer>().Single(c => c.CustomerID == customerId)
        };
    }
}