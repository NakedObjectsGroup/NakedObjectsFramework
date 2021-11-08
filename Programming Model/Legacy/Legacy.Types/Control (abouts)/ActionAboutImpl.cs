using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Types {
    public class ActionAboutImpl : ActionAbout {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Visible { get; set; }
        public bool Usable { get; set; }
        public string UnusableReason { get; set; }
        public bool[] ParamsRequired { get; set; }
        public string[] ParamLabels { get; set; }
        public object[] ParamDefaultValues { get; set; }
        public object[][] ParamOptions { get; set; }
        public AboutTypeCodes TypeCode { get; }
    }
}
