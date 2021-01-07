using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class CurrencyRate_Functions
    {
        #region Life Cycle Methods

        public static CurrencyRate Persisting(CurrencyRate pv, IContext context) => pv with { ModifiedDate = context.Now() };

        public static CurrencyRate Updating(CurrencyRate pv, IContext context) => pv with { ModifiedDate = context.Now() };
        #endregion
    }
}
