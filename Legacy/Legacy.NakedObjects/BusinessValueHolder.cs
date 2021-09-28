using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects {
    public abstract class BusinessValueHolder : TitledObject {
        public virtual bool userChangeable() => true;

        public abstract bool isEmpty();

        public abstract bool isSameAs(BusinessValueHolder @object);

        public virtual string titleString() => this.title().ToString();

        public abstract Title title();

        public override string ToString() => this.titleString();

        public virtual object getValue() => (object)this;

        //[JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
        public abstract void parseUserEntry(string text);

        public abstract void restoreFromEncodedString(string data);

        public abstract string asEncodedString();

        public abstract void copyObject(BusinessValueHolder @object);

        public abstract void clear();

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone() {
            //BusinessValueHolder businessValueHolder = this;
            //ObjectImpl.clone((object)businessValueHolder);
            //return ((object)businessValueHolder).MemberwiseClone();
            return null; // to compile
        }
    }
}
