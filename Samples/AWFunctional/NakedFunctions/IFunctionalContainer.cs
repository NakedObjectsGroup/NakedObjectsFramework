using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace NakedFunctions
{
    /// <summary>
    /// Subset of IDomainObjectContainer, with only side-effect-free properties.
    /// </summary>
    public interface IFunctionalContainer
    {
        IQueryable<T> Instances<T>() where T : IFunctionalType;
    }
}
