using System.Linq;
using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class StoreSalesInfo_Functions
    {
        public static string Title(this StoreSalesInfo vm,
                                   IQueryable<Customer> customers)
        {
            return $"{(vm.EditMode ? "Editing - " : "")} Sales Info for: {vm.StoreName}";
        }

        public static string[] DeriveKeys(StoreSalesInfo vm)
        {
            return new string[] { vm.AccountNumber, vm.EditMode.ToString() };
        }

        public static StoreSalesInfo PopulateUsingKeys(StoreSalesInfo vm,
                                                       string[] keys,
                                                       IContext context)
        {
            var cus = Customer_MenuFunctions.FindCustomerByAccountNumber(keys[0], context);
            return vm with { AccountNumber = keys[0] }
                 with
            { SalesTerritory = cus.SalesTerritory }
                 with
            { SalesPerson = cus.Store?.SalesPerson }
                 with
            { EditMode = bool.Parse(keys[1]) };
        }

        public static StoreSalesInfo Edit(this StoreSalesInfo ssi)
        {
            return ssi with { EditMode = true };
        }

        public static bool HideEdit(this StoreSalesInfo ssi)
        {
            return IsEditView(ssi);
        }

        public static bool IsEditView(StoreSalesInfo ssi)
        {
            return ssi.EditMode;
        }

        public static (Customer, IContext) Save(this StoreSalesInfo vm, IContext context)
        {
            var cus = Customer_MenuFunctions.FindCustomerByAccountNumber(vm.AccountNumber, context);
            var st = vm.SalesTerritory;
            var sp = vm.SalesPerson;
            var sn = vm.StoreName;
            var cus2 = cus with { SalesTerritory = st }; //TODO:   Store.SalesPerson = sp, Store.Name = sn);
            return context.SaveAndDisplay(cus2);
        }

        public static bool HideSave(this StoreSalesInfo ssi)
        {
            return !IsEditView(ssi);
        }
    }
}
