using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects {
    public abstract class BusinessValueHolder : TitledObject {
        [NakedObjectsIgnore]
        public virtual bool userChangeable() => true;

        [NakedObjectsIgnore]
        public abstract bool isEmpty();

        [NakedObjectsIgnore]
        public abstract bool isSameAs(BusinessValueHolder @object);

        [NakedObjectsIgnore]
        public virtual string titleString() => this.title().ToString();

        [NakedObjectsIgnore]
        public abstract Title title();

        public override string ToString() => this.titleString();

        [NakedObjectsIgnore]
        public virtual object getValue() => (object)this;

        [NakedObjectsIgnore]
        //[JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
        public abstract void parseUserEntry(string text);

        [NakedObjectsIgnore]
        public abstract void restoreFromEncodedString(string data);

        [NakedObjectsIgnore]
        public abstract string asEncodedString();

        [NakedObjectsIgnore]
        public abstract void copyObject(BusinessValueHolder @object);

        [NakedObjectsIgnore]
        public abstract void clear();

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        [NakedObjectsIgnore]
        public new virtual object MemberwiseClone() {
            //BusinessValueHolder businessValueHolder = this;
            //ObjectImpl.clone((object)businessValueHolder);
            //return ((object)businessValueHolder).MemberwiseClone();
            return null; // to compile
        }
    }
}
