using System;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application.ValueHolder {
    public class TextString : BusinessValueHolder {
        //private static readonly Logger logger;
        private const long serialVersionUID = 1;
        private int maximumLength;
        private int minimumLength;
        private string text;

        public TextString() {
            this.maximumLength = 0;
            this.minimumLength = 0;
            this.setValue((string)null);
        }

        public TextString(string text) {
            this.maximumLength = 0;
            this.minimumLength = 0;
            this.setValue(text);
        }

        public TextString(TextString textString) {
            this.maximumLength = 0;
            this.minimumLength = 0;
            this.setValue(textString);
        }

        [NakedObjectsIgnore]
        public override string asEncodedString() => this.isEmpty() ? "NULL" : this.text;

        private void checkForInvalidCharacters() {
            if (this.text == null)
                return;
            //for (int index = 0; index < StringImpl.length(this.text); ++index) {
            //    if (this.isCharDisallowed(StringImpl.charAt(this.text, index)))
            //        throw new RuntimeException(new StringBuffer().append((object)ObjectImpl.getClass((object)this)).append(" cannot contain the character code 0x").append(Integer.toHexString((int)StringImpl.charAt(this.text, index))).ToString());
            //}
        }

        [NakedObjectsIgnore]
        public override void clear() => this.text = (string)null;

        [NakedObjectsIgnore]
        public virtual bool contains(string text) => this.contains(text, Case.SENSITIVE);

        [NakedObjectsIgnore]
        public virtual bool contains(string text, Case caseSensitive) {
            if (this.text == null)
                return false;
            //return caseSensitive == Case.SENSITIVE ? StringImpl.indexOf(this.text, text) >= 0 : StringImpl.indexOf(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text)) >= 0;

            return this.text.Contains(text, caseSensitive == Case.SENSITIVE ? StringComparison.InvariantCulture  : StringComparison.InvariantCultureIgnoreCase);
        }

        [NakedObjectsIgnore]
        public override void copyObject(BusinessValueHolder @object) {
            //if (!(@object is TextString))
            //    throw new IllegalArgumentException("Can only copy the value of  a TextString object");
            this.setValue((TextString)@object);
        }

        [NakedObjectsIgnore]
        public virtual bool endsWith(string text) => this.endsWith(text, Case.SENSITIVE);

        [NakedObjectsIgnore]
        public virtual bool endsWith(string text, Case caseSensitive) {
            if (this.text == null)
                return false;
            // return caseSensitive == Case.SENSITIVE ? StringImpl.endsWith(this.text, text) : StringImpl.endsWith(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text));

            return this.text.EndsWith(text, caseSensitive == Case.SENSITIVE ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
        }

        //[Obsolete(null, false)]
        //public override bool Equals(object @object) {
        //    if (!(@object is TextString))
        //        return base.Equals(@object);
        //    TextString textString = (TextString)@object;
        //    if (this.text != null)
        //        //return StringImpl.equals(this.text, (object)textString.text);
        //        return string.Equals(this.text, (object)textString.text);
        //    return textString.text == null;
        //}

        //[JavaFlags(4)]
        //public virtual Logger getLogger() => TextString.logger;

        [NakedObjectsIgnore]
        public virtual int getMaximumLength() => this.maximumLength;

        [NakedObjectsIgnore]
        public virtual int getMinimumLength() => this.minimumLength;

        [NakedObjectsIgnore]
        public virtual string getObjectHelpText() => "A TextString object.";

        //[JavaFlags(4)]
        [NakedObjectsIgnore]
        public virtual bool isCharDisallowed(char c) => c == '\n' || c == '\r' || c == '\t';

        [NakedObjectsIgnore]
        public override bool isEmpty() {
            //return this.text == null || StringImpl.length(this.text) == 0;
            return string.IsNullOrEmpty(this.text);
        }

        [NakedObjectsIgnore]
        public override bool isSameAs(BusinessValueHolder @object) => @object is TextString && this.isSameAs((TextString)@object);

        [NakedObjectsIgnore]
        public virtual bool isSameAs(string text) => this.isSameAs(text, Case.SENSITIVE);

        [NakedObjectsIgnore]
        public virtual bool isSameAs(string text, Case caseSensitive) {
            if (this.text == null)
                return false;
            //return caseSensitive == Case.SENSITIVE ? StringImpl.equals(this.text, (object)text) : StringImpl.equalsIgnoreCase(this.text, text);
            return this.text.Equals(text, caseSensitive == Case.SENSITIVE ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
        }

        [NakedObjectsIgnore]
        public virtual bool isSameAs(TextString text) => this.isSameAs(text, Case.SENSITIVE);

        [NakedObjectsIgnore]
        public virtual bool isSameAs(TextString text, Case caseSensitive) {
            //return this.text == null ? (object)this.text == (object)text.text : (caseSensitive == Case.SENSITIVE ? StringImpl.@equals(this.text, (object)text.text) : StringImpl.equalsIgnoreCase(this.text, text.text));
            return this.text == null
                ? (object)this.text == (object)text.text
                : caseSensitive == Case.SENSITIVE
                    ? this.text.Equals(text.text, StringComparison.InvariantCulture)
                    : this.text.Equals(text.text, StringComparison.InvariantCultureIgnoreCase);
        }

        [NakedObjectsIgnore]
        public virtual bool isValid() => false;

        [NakedObjectsIgnore]
        //[JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
        public override void parseUserEntry(string text) => this.setValue(text);

        [NakedObjectsIgnore]
        public virtual void reset() => this.setValue((string)null);

        [NakedObjectsIgnore]
        public override void restoreFromEncodedString(string data) {
            //if (data == null || StringImpl.equals(data, (object)"NULL")) { 
            if (data == null || data.Equals("NULL", StringComparison.InvariantCulture)) {
                this.clear();
            } else {
                this.text = data;
                this.checkForInvalidCharacters();
            }
        }

        [NakedObjectsIgnore]
        public virtual void setMaximumLength(int maximumLength) => this.maximumLength = maximumLength;

        [NakedObjectsIgnore]
        public virtual void setMinimumLength(int minimumLength) => this.minimumLength = minimumLength;

        [NakedObjectsIgnore]
        public virtual void setValue(string text) {
            this.text = text != null ? text : (string)null;
            this.checkForInvalidCharacters();
        }

        [NakedObjectsIgnore]
        public virtual void setValue(TextString text) {
            if (text == null || text.isEmpty())
                this.clear();
            else
                this.setValue(text.text);
        }

        [NakedObjectsIgnore]
        public virtual bool startsWith(string text) => this.startsWith(text, Case.SENSITIVE);

        [NakedObjectsIgnore]
        public virtual bool startsWith(string text, Case caseSensitive) {
            if (this.text == null)
                return false;
            //return caseSensitive == Case.SENSITIVE ? StringImpl.startsWith(this.text, text) : StringImpl.startsWith(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text));

            return this.text.StartsWith(text, caseSensitive == Case.SENSITIVE ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);

        }

        [NakedObjectsIgnore]
        public virtual string stringValue() => this.isEmpty() ? "" : this.text;

        [NakedObjectsIgnore]
        public override Title title() => new Title(this.stringValue());

        //[JavaFlags(32778)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        static TextString() {
            // ISSUE: unable to decompile the method.
        }


        // NEW code 

        public override string ToString() {
            return stringValue();
        }
    }
}
