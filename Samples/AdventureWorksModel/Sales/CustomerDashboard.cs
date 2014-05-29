using NakedObjects;
using System.Collections.Generic;
using System.Linq;

namespace AdventureWorksModel
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerDashboard : ViewModel<Customer>
    {
        #region Injected Services

        public OrderContributedActions OrderContributedActions { set; protected get; }
        #endregion

        
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(Name).Append(" - Dashboard");
            return t.ToString();
        }
      

        public string Name
        {
            get
            {
                return Root.IsIndividual() ? ((Individual)Root).Contact.ToString() : ((Store)Root).Name;
            }
        }

        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders
        {
            get
            {
                return OrderContributedActions.RecentOrders(Root).ToList();
            }
        }

        public decimal TotalOrderValue
        {
            get
            {
                int id = Root.Id;
                return Container.Instances<SalesOrderHeader>().Where(x => x.Customer.Id == id).Sum(x => x.TotalDue);

            }
        }


        public SalesOrderHeader NewOrder()
        {
            var order = OrderContributedActions.CreateNewOrder(Root, true);
            Container.Persist(ref order);
            return order;
        }
    }
}
