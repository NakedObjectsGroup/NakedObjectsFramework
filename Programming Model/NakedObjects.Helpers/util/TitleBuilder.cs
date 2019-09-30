// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NakedObjects.Util;

namespace NakedObjects {
    [Obsolete("Use Container.NewTitleBuilder() method")]
    public class TitleBuilder {
        private const string Space = " ";
        private static readonly IDictionary<Type, Title> titleFrom = new Dictionary<Type, Title>();

        protected StringBuilder Title { get; set; }

        public static string ListTitleProperties {
            get {
                var str = new StringBuilder();
                lock (titleFrom) {
                    foreach (Type type in titleFrom.Keys) {
                        str.AppendLine(type + " -> " + titleFrom[type].GetType().Name);
                    }
                }

                return str.ToString();
            }
        }

        /// <summary>
        ///     Determines if the specified object's Title (from its <c>ToString()</c> method) is empty. Will
        ///     return true if either: the specified reference is null; the object's <c>ToString()</c> method
        ///     returns null; or if the <c>ToString()</c> returns an empty string
        /// </summary>
        public static bool IsEmpty(object obj, string format) {
            return obj == null || IsEmpty(TitleOrToString(obj, format));
        }

        /// <summary>
        ///     Determines if the specified text is empty. Will return true if either: the specified reference is null;
        ///     or if the reference is an empty string
        /// </summary>
        public static bool IsEmpty(string text) {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        ///     Append the Title of the specified object
        /// </summary>
        public TitleBuilder Append(object obj) {
            if (!IsEmpty(obj, null)) {
                AppendWithSpace(obj, null);
            }

            return this;
        }

        /// <summary>
        ///     Appends the Title of the specified object, or the specified text if the objects Title is null or empty.
        ///     Prepends a space if there is already some text in this Title object
        /// </summary>
        /// <param name="obj">the object whose Title is to be appended to this Title</param>
        /// <param name="format"></param>
        /// <param name="defaultValue">a textual value to be used if the object's Title is null or empty</param>
        /// <returns>
        ///     a reference to the called object(itself)
        /// </returns>
        public TitleBuilder Append(object obj, string format, string defaultValue) {
            if (!IsEmpty(obj, format)) {
                AppendWithSpace(obj, format);
            }
            else {
                AppendWithSpace(defaultValue);
            }

            return this;
        }

        /// <summary>
        ///     Appends a space (if there is already some text in this Title object) and then the specified text
        /// </summary>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Append(string text) {
            if (!IsEmpty(text)) {
                AppendWithSpace(text);
            }

            return this;
        }

        /// <summary>
        ///     Appends the joining string and the Title of the specified object (from its <c>ToString()</c>
        ///     method). If the object is empty then nothing will be appended
        /// </summary>
        /// <seealso cref="IsEmpty(object, string)" />
        public TitleBuilder Append(string joiner, object obj) {
            if (!IsEmpty(obj, null)) {
                AppendJoiner(joiner);
                AppendWithSpace(obj, null);
            }

            return this;
        }

        /// <summary>
        ///     Append the <c>joiner</c> text, a space, and the Title of the specified naked obj (<c>object</c>)
        ///     (got by calling the objects Title() method) to the text of this TitleString obj. If the Title of the
        ///     specified obj is null then use the <c>defaultValue</c> text. If both the objects Title and
        ///     the default value are null or equate to a zero-length string then no text will be appended ; not even
        ///     the joiner text
        /// </summary>
        /// <param name="joiner">text to Append before the Title</param>
        /// <param name="obj">object whose Title needs to be appended</param>
        /// <param name="format"></param>
        /// <param name="defaultTitle">the text to use if the the object's Title is null</param>
        /// <returns>
        ///     a reference to the called obj (itself)
        /// </returns>
        public TitleBuilder Append(string joiner, object obj, string format, string defaultTitle) {
            if (IsEmpty(obj, format)) {
                Append(joiner, defaultTitle);
            }
            else {
                AppendJoiner(joiner);
                AppendWithSpace(obj, format);
            }

            return this;
        }

        /// <summary>
        ///     Appends the joiner text, a space, and the text to the text of this TitleString object. If no text yet
        ///     exists in the object then the joiner text and space are omitted
        /// </summary>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Append(string joiner, string text) {
            if (!IsEmpty(text)) {
                AppendJoiner(joiner);
                AppendWithSpace(text);
            }

            return this;
        }

        /// <summary>
        ///     Append a space to the text of this TitleString obj if, and only if, there is some existing text
        ///     i.e., a space is only added to existing text and will not create a text entry consisting of only one
        ///     space
        /// </summary>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder AppendSpace() {
            if (Title.Length > 0) {
                Title.Append(Space);
            }

            return this;
        }

        /// <summary>
        ///     Concatenate the the Title value (the result of calling an objects Title() method) to this TitleString
        ///     object. If the value is null then no text is added
        /// </summary>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Concat(object obj) {
            return Concat(obj, null, "");
        }

        /// <summary>
        ///     Concatenate the the Title value (the result of calling an objects Title() method), or the specified
        ///     default value if the Title is equal to null or is empty, to this TitleString object.
        /// </summary>
        /// <param name="obj">the naked obj to get a Title from</param>
        /// <param name="defaultValue">the default text to use when the naked obj is null</param>
        /// <param name="format"></param>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Concat(object obj, string format, string defaultValue) {
            if (IsEmpty(obj, format)) {
                Title.Append(defaultValue);
            }
            else {
                Title.Append(TitleOrToString(obj, format));
            }

            return this;
        }

        /// <summary>
        ///     Concatenate the specified text on to the end of the text of this TitleString
        /// </summary>
        /// <param name="text">text to Append</param>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Concat(string text) {
            Title.Append(text);
            return this;
        }

        /// <summary>
        ///     Concatenate the specified joiner then concat the object using <see cref="Concat(object)" />
        ///     with a null format and no default
        /// </summary>
        /// <param name="joiner">text to Append</param>
        /// <param name="obj">object to Concat</param>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Concat(string joiner, object obj) {
            if (!IsEmpty(obj, null)) {
                AppendJoiner(joiner);
                Concat(obj);
            }

            return this;
        }

        /// <summary>
        ///     Append the specified joiner then Concat the object using <see cref="Concat(object,string,string)" />
        /// </summary>
        /// <param name="joiner">text to Append</param>
        /// <param name="obj">the naked obj to get a Title from</param>
        /// <param name="defaultValue">the default text to use when the naked obj is null</param>
        /// <param name="format"></param>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Concat(string joiner, object obj, string format, string defaultValue) {
            if (!IsEmpty(obj, format)) {
                AppendJoiner(joiner);
            }

            Concat(obj, format, defaultValue);
            return this;
        }

        /// <summary>
        ///     Returns a string that represents the value of this obj
        /// </summary>
        public override string ToString() {
            return Title.ToString();
        }

        /// <summary>
        ///     Truncates this Title so it has a maximum number of words. Spaces are used to determine words, thus two
        ///     spaces in a Title will cause two words to be mistakenly identified
        /// </summary>
        /// <param name="noWords">the number of words to show</param>
        /// <returns>
        ///     a reference to the called object (itself)
        /// </returns>
        public TitleBuilder Truncate(int noWords) {
            if (noWords < 1) {
                throw new ArgumentException("Truncation must be to one or more words");
            }

            string[] words = Title.ToString().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (noWords >= words.Length) {
                return this;
            }

            Title = new StringBuilder();
            for (int i = 0; i < noWords; i++) {
                Title.Append(words[i]).Append(" ");
            }

            Title.Append("...");
            return this;
        }

        #region Constructors

        /// <summary>
        ///     Creates a new, empty, TitleBuilder object
        /// </summary>
        public TitleBuilder() {
            Title = new StringBuilder();
        }

        /// <summary>
        ///     Creates a new TitleBuilder object, containing the Title of the specified object
        /// </summary>
        public TitleBuilder(object obj)
            : this() {
            Concat(obj);
        }

        /// <summary>
        ///     Creates a new TitleBuilder object, containing the Title of the specified object
        /// </summary>
        public TitleBuilder(object obj, string defaultTitle)
            : this() {
            if (IsEmpty(obj, null)) {
                Concat(defaultTitle);
            }
            else {
                Concat(obj);
            }
        }

        /// <summary>
        ///     Creates a new Title object, containing the specified text
        /// </summary>
        public TitleBuilder(string text)
            : this() {
            Concat(text);
        }

        #endregion

        #region Private

        internal static string TitleOrToString(object obj, string format) {
            Type type = obj.GetType();

            lock (titleFrom) {
                if (titleFrom.ContainsKey(type) && titleFrom[type] != null) {
                    return titleFrom[type].GetTitle(obj, format);
                }

                titleFrom[type] = null;

                MethodInfo titleMethod = type.GetMethod("Title", Type.EmptyTypes);
                if (titleMethod != null) {
                    titleFrom[type] = new TitleFromTitleMethod(titleMethod);
                }

                if (titleFrom[type] == null) {
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo property in properties) {
                        if (Attribute.GetCustomAttribute(property, typeof(TitleAttribute)) != null) {
                            titleFrom[type] = new TitleFromProperty(property);
                        }
                    }
                }

                if (titleFrom[type] == null && format != null) {
                    MethodInfo formatMethod = type.GetMethod("ToString", new[] {typeof(string)});
                    if (formatMethod != null) {
                        titleFrom[type] = new TitleFromFormattingToString(formatMethod);
                    }
                }

                if (titleFrom[type] == null) {
                    titleFrom[type] = new TitleFromToString();
                }

                return titleFrom[type].GetTitle(obj, format);
            }
        }

        private void AppendJoiner(string joiner) {
            if (Title.Length > 0) {
                Title.Append(joiner);
            }
        }

        private void AppendWithSpace(string str) {
            AppendSpace();
            Title.Append(str);
        }

        private void AppendWithSpace(object obj, string format) {
            AppendSpace();
            Title.Append(TitleOrToString(obj, format));
        }

        #endregion
    }

    // Title invokers
    internal abstract class Title {
        internal abstract string GetTitle(object obj, string format);
    }

    internal class TitleFromTitleMethod : Title {
        private readonly MethodInfo method;

        internal TitleFromTitleMethod(MethodInfo method) {
            this.method = method;
        }

        internal override string GetTitle(object obj, string format) {
            return (string) method.Invoke(obj, null) ?? obj.ToString();
        }
    }

    internal class TitleFromProperty : Title {
        private readonly PropertyInfo property;

        public TitleFromProperty(PropertyInfo property) {
            this.property = property;
        }

        internal override string GetTitle(object obj, string format) {
            object referencedObject = property.GetValue(obj, new object[0]);
            if (referencedObject == null) {
                return "";
            }
            else {
#pragma warning disable 618
                return TitleBuilder.TitleOrToString(referencedObject, format);
#pragma warning restore 618
            }
        }
    }

    internal class TitleFromToString : Title {
        internal override string GetTitle(object obj, string format) {
            if (typeof(Enum).IsAssignableFrom(obj.GetType())) {
                return NameUtils.NaturalName(obj.ToString());
            }

            return obj.ToString();
        }
    }

    internal class TitleFromFormattingToString : Title {
        private readonly MethodInfo formatMethod;

        public TitleFromFormattingToString(MethodInfo formatMethod) {
            this.formatMethod = formatMethod;
        }

        internal override string GetTitle(object obj, string format) {
            return (string) formatMethod.Invoke(obj, new object[] {format});
        }
    }
}