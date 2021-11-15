using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedLegacy.Types.Value_holders
{
    public class MultiLineTextString : TextString
    {
        public MultiLineTextString(string text) : base(text) { }

        public MultiLineTextString(string text, Action<string> callback) : base(text, callback) { }
    }
}
