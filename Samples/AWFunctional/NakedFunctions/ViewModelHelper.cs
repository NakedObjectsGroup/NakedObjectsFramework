using System;

namespace NakedFunctions
{
    static class ViewModelHelper
    {
        public static T NewViewModel<T>(IFunctionalContainer container) where T : IFunctionalViewModel
        {
            var vm = Activator.CreateInstance<T>();
            throw new NotImplementedException();
        }
    }
}
