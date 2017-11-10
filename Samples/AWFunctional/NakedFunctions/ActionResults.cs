using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedFunctions
{
    public class QueryResultList
    {
        public QueryResultList(IEnumerable<object> result, string inform= null, string warn = null, string error = null)
        {

        }
    }

    public class QueryResultSingle
    {
        public QueryResultSingle(object result, string inform = null, string warn = null, string error = null)
        {
        }
    }

    public class PotentResultSingle
    {
        public PotentResultSingle(object result, IEnumerable<object> otherNewOrChanged)
        {

        }
    }
}
