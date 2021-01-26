using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class StoreSalesInfo_Functions
    {

        public static string[] DeriveKeys(this StoreSalesInfo vm) =>
            new string[] { vm.AccountNumber, vm.EditMode.ToString() };

        public static StoreSalesInfo CreateFromKeys(string[] keys, IContext context)
        {
            var cus = Customer_MenuFunctions.FindCustomerByAccountNumber(keys[0], context);
            return new StoreSalesInfo {
                AccountNumber = keys[0],
                SalesTerritory = cus.SalesTerritory,
                SalesPerson = cus.Store?.SalesPerson,
                EditMode = bool.Parse(keys[1]) };
        }

        public static StoreSalesInfo Edit(this StoreSalesInfo ssi)
        {
            return ssi with { EditMode = true };
        }

        public static bool HideEdit(this StoreSalesInfo ssi)
        {
            return IsEditView(ssi);
        }

        public static bool IsEditView(this StoreSalesInfo ssi)
        {
            return ssi.EditMode;
        }

        public static (Customer, IContext) Save(this StoreSalesInfo vm, IContext context)
        {
            var cus = Customer_MenuFunctions.FindCustomerByAccountNumber(vm.AccountNumber, context);
            var st = vm.SalesTerritory;
            var sp = vm.SalesPerson;
            var sn = vm.StoreName;
            var store2 = cus.Store with { SalesPerson = sp, Name = sn};
            var cus2 = cus with { SalesTerritory = st, Store = store2}; 
            
            return (cus2, context.WithPendingSave(store2, cus2));
        }

        public static bool HideSave(this StoreSalesInfo ssi)
        {
            return !IsEditView(ssi);
        }
    }
}
