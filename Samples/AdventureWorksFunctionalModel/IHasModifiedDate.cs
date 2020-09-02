using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel
{
    public interface IHasModifiedDate
    {

        DateTime ModifiedDate { get; }
    }
}
