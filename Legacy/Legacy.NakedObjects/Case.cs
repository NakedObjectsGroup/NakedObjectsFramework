using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects {
    public class Case {
        public static readonly Case INSENSITIVE;
        public static readonly Case SENSITIVE;
        private string name;

        private Case(string name) => this.name = name;

        public override string ToString() => this.name;

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[JavaFlags(32778)]
        static Case() {
            // ISSUE: unable to decompile the method.
        }

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone() {
            //Case @case = this;
            //ObjectImpl.clone((object)@case);
            //return ((object)@case).MemberwiseClone();
            return null; // to compile
        }
    }
}
