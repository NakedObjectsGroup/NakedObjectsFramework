using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedFunctions
{
    /// <summary>
    /// Subset of IDomainObjectContainer, with only side-effect-free methods.
    /// </summary>
    public interface IFunctionalContainer
    {
        IQueryable<T> Instances<T>();
    }
}
