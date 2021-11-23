namespace AW.Functions;

public static class CustomerCollectionViewModel_Functions
{
    public static string[] DeriveKeys(this CustomerCollectionViewModel vm) =>
        vm.Customers.Select(c => c.CustomerID.ToString()).ToArray();

    public static CustomerCollectionViewModel CreateFromKeys(string[] keys, IContext context)
    {
        var ids = keys == null ? new int[] { } : keys.Select(int.Parse).ToArray();
        return new CustomerCollectionViewModel((from c in context.Instances<Customer>()
                                                from id in ids
                                                where c.CustomerID == id
                                                select c).ToList());
    }
}