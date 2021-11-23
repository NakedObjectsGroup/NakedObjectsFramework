






using System.Linq;
using AW.Types;
using NakedFunctions;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Vendors")]
    public static class Vendor_MenuFunctions {
        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public static IQueryable<Vendor> FindVendorByName(string name, IContext context) =>
            context.Instances<Vendor>().Where(v => v.Name == name).OrderBy(v => v.Name);

        public static Vendor? FindVendorByAccountNumber(string accountNumber, IContext context) =>
            context.Instances<Vendor>().FirstOrDefault(x => x.AccountNumber == accountNumber);

        public static Vendor RandomVendor(IContext context) => Random<Vendor>(context);

        public static IQueryable<Vendor> AllVendorsWithWebAddresses(IContext context) =>
            context.Instances<Vendor>().Where(x => x.PurchasingWebServiceURL != null).OrderBy(x => x.Name);
    }
}