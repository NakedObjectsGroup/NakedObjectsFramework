// ReSharper disable InconsistentNaming

using System;
using System.Text;
using NakedObjects;

namespace Legacy.NakedObjects.Application {
    public class Title {
        private readonly StringBuilder stringBuilder;

        public Title() => stringBuilder = new StringBuilder();

        public Title(string text) {
            stringBuilder = new StringBuilder();
            concat(text);
        }

        public Title(TitledObject @object) {
            stringBuilder = new StringBuilder();
            concat(@object);
        }

        public Title(TitledObject @object, string defaultValue) {
            stringBuilder = new StringBuilder();
            concat(@object, defaultValue);
        }

        [NakedObjectsIgnore]
        public virtual Title append(int number) {
            append(number.ToString());
            return this;
        }

        [NakedObjectsIgnore]
        public virtual Title append(string text) {
            if (!string.IsNullOrEmpty(text)) {
                appendSpace();
                stringBuilder.Append(text);
            }

            return this;
        }

        [NakedObjectsIgnore]
        public virtual Title append(string joiner, string text) {
            if (!string.IsNullOrEmpty(text)) {
                if (stringBuilder.Length > 0) {
                    concat(joiner);
                }

                appendSpace();
                stringBuilder.Append(text);
            }

            return this;
        }

        [NakedObjectsIgnore]
        public virtual Title append(string joiner, TitledObject @object) {
            append(joiner, @object, "");
            return this;
        }

        [NakedObjectsIgnore]
        public virtual Title append(string joiner, TitledObject @object, string defaultValue) {
            if (stringBuilder.Length > 0 && titleString(@object).Length > 0 || defaultValue is not null && defaultValue.Length > 0) {
                concat(joiner);
                appendSpace();
            }

            concat(@object, defaultValue);
            return this;
        }

        private static string titleString(TitledObject @object) => @object?.title()?.ToString() ?? "";

        [NakedObjectsIgnore]
        public virtual Title append(TitledObject @object) {
            if (titleString(@object) is not "") {
                appendSpace();
                stringBuilder.Append(titleString(@object));
            }

            return this;
        }

        [NakedObjectsIgnore]
        public virtual Title append(TitledObject @object, string defaultValue) {
            appendSpace();
            concat(@object, defaultValue);
            return this;
        }

        public virtual Title appendSpace() {
            if (stringBuilder.Length > 0) {
                stringBuilder.Append(" ");
            }

            return this;
        }

        //[JavaFlags(17)]
        [NakedObjectsIgnore]
        public Title concat(string text) {
            stringBuilder.Append(text);
            return this;
        }

        //[JavaFlags(17)]
        [NakedObjectsIgnore]
        public Title concat(string joiner, TitledObject @object) {
            if (stringBuilder.Length > 0 && titleString(@object).Length > 0) {
                concat(joiner);
            }

            concat(@object, "");
            return this;
        }

        //[JavaFlags(17)]
        [NakedObjectsIgnore]
        public Title concat(TitledObject @object) {
            concat(@object, "");
            return this;
        }

        //[JavaFlags(17)]
        [NakedObjectsIgnore]
        public Title concat(string joiner, TitledObject @object, string defaultValue) {
            if (stringBuilder.Length > 0 && titleString(@object).Length > 0) {
                concat(joiner);
            }

            concat(@object, defaultValue);
            return this;
        }

        //[JavaFlags(17)]
        [NakedObjectsIgnore]
        public Title concat(TitledObject @object, string defaultValue) {
            if (titleString(@object).Length is 0) {
                stringBuilder.Append(defaultValue);
            }
            else {
                stringBuilder.Append(@object.title());
            }

            return this;
        }

        public override string ToString() => stringBuilder.ToString();

        public virtual Title truncate(int noWords) {
            if (noWords < 1) {
                //throw new IllegalArgumentException("Truncation must be to one or more words");
                throw new ArgumentException("Truncation must be to one or more words");
            }

            var num = 0;
            for (var index = 0; num < stringBuilder.Length && index < noWords; ++num) {
                if (stringBuilder[num] == ' ') {
                    ++index;
                }
            }

            if (num < stringBuilder.Length) {
                stringBuilder.Length = num - 1;
                stringBuilder.Append("...");
            }

            return this;
        }

        public static Title title(TitledObject @object) => @object is null ? new Title() : new Title(titleString(@object));

        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        //[JavaFlags(4227077)]
        public new virtual object MemberwiseClone() =>
            //Title title = this;
            //ObjectImpl.clone((object)title);
            //return ((object)title).MemberwiseClone();
            null; // to compile
    }
}