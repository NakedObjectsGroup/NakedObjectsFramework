using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedFunctions
{
    //TODO: Need to work out how these are handled, and how to define the methods to allow queryables to be passed in
    public interface IFunctionalViewModel
    {
    }
    public interface IFunctionalViewModelEdit : IFunctionalViewModel
    {

    }

    public interface IFunctionalViewModelSwitchable : IFunctionalViewModel
    {
    }
}
