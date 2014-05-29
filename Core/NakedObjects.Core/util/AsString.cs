// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Text;

namespace NakedObjects.Core.Util {
    public sealed class AsString {
        private readonly StringBuilder buffer;
        private bool addComma;
        private bool useLineBreaks;

        private AsString() {
            buffer = new StringBuilder();
        }

        public AsString(object forObject) {
            buffer = new StringBuilder();
            CreateName(forObject, buffer);
            buffer.Append(" [");
        }

        public AsString(object forObject, int id) {
            buffer = new StringBuilder();
            buffer.Append(Name(forObject));
            buffer.Append("#");
            buffer.Append(id);
            buffer.Append(" [");
        }

        public AsString(object forObject, string text)
            : this(forObject) {
            buffer.Append(text);
            addComma = text.Length > 0;
        }

        public bool UseLineBreaks {
            set { useLineBreaks = value; }
        }

        public static string CreateName(object forObject) {
            var buffer = new StringBuilder();
            CreateName(forObject, buffer);
            return buffer.ToString();
        }

        private static void CreateName(object forObject, StringBuilder buffer) {
            buffer.Append(Name(forObject));
            buffer.Append("@");
            buffer.Append(Convert.ToString(forObject.GetHashCode(), 16));
        }

        public static string Name(object forObject) {
            string name = forObject.GetType().FullName;
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        public static string Timestamp(DateTime? date) {
            return date == null ? "" : date.GetValueOrDefault().ToLongTimeString();
        }

        public static AsString CreateWithIdentifier(object obj) {
            return new AsString(obj);
        }

        public static AsString CreateAnonymous(object obj) {
            var asString = new AsString();
            asString.Append(Name(obj));
            asString.Append("[");
            return asString;
        }

        public AsString Append(string text) {
            buffer.Append(text);
            return this;
        }

        public AsString Append(string name, bool flag) {
            Append(name, flag ? "true" : "false");
            return this;
        }

        public AsString Append(string name, sbyte number) {
            Append(name, number.ToString());
            return this;
        }

        public AsString Append(string name, double number) {
            Append(name, number.ToString());
            return this;
        }

        public AsString Append(string name, float number) {
            Append(name, number.ToString());
            return this;
        }

        public AsString Append(string name, int number) {
            Append(name, Convert.ToString(number));
            return this;
        }

        public AsString Append(string name, long number) {
            Append(name, Convert.ToString(number));
            return this;
        }

        public AsString Append(string name, object obj) {
            Append(name, obj == null ? "null" : obj.ToString());
            return this;
        }

        public AsString Append(string name, short number) {
            Append(name, number.ToString());
            return this;
        }

        public AsString Append(string name, string stringValue) {
            if (addComma) {
                buffer.Append(',');
                if (useLineBreaks) {
                    buffer.Append(Environment.NewLine).Append('\t');
                }
            }
            else {
                addComma = true;
            }
            buffer.Append(name);
            buffer.Append('=');
            buffer.Append(stringValue);

            return this;
        }

        public AsString AppendAsHex(string name, long number) {
            Append(name, "#" + Convert.ToString(number, 16));
            return this;
        }

        public void AppendAsTimestamp(string name, ref DateTime date) {
            string dateString = Timestamp(date);
            Append(name, dateString);
        }

        public void AppendTruncated(string name, string stringValue, int maxLength) {
            if (stringValue.Length > maxLength) {
                Append(name, stringValue.Substring(0, (maxLength) - (0)));
                Append("...");
            }
            else {
                Append(name, stringValue);
            }
        }

        public void AddComma() {
            addComma = true;
        }

        public override string ToString() {
            buffer.Append(']');
            return buffer.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}