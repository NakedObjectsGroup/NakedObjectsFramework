using AW.Functions;

namespace AW.Types;

    [ViewModel(typeof(CustomerDashboard_Functions))]
    public class CustomerDashboard {
        [Hidden]
        public virtual Customer Root { get; init; }

        public string Name => $"{(Root.IsIndividual() ? Root.Person : Root.Store?.Name)}";

        //Empty field, not - to test that fields are not editable in a VM
        public string Comments { get; init; } = "";

        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders(IContext context) =>
            Root.RecentOrders(context).Take(5).ToList();

        public override string ToString() => $"{Name} - Dashboard";
    }