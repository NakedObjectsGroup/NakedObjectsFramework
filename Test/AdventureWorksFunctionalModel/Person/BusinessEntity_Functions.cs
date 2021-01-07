using NakedFunctions;
using AW.Types;

namespace AW.Functions
{

    public static class BusinessEntityFunctions
    {
        public static bool HideContacts(BusinessEntity be)
        {
            return false;
        }

        //TODO: This needs modification to create persisted address with all fields filled.
        public static Address CreateNewAddress(BusinessEntity be)
        {
            var a = new Address();  //TODO add all fields
            //a.AddressFor = be;
            return a;
        }

        #region Life Cycle Methods
        public static BusinessEntity Updating(BusinessEntity be, IContext context) => be with { BusinessEntityModifiedDate = context.Now() };
        #endregion
    }
}
