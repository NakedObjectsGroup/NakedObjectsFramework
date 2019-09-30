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

namespace NakedObjects {
    [Obsolete("Use Container.NewTitleBuilder() method")]
    public class NewTitleBuilder {
        private const string Space = " ";
        private static readonly IDictionary<Type, Title> titleFrom = new Dictionary<Type, Title>();
        private bool appendWithSpace;
        private string defaultValue;
        private string joiner;
        private object obj;
        private string pattern;
        private string separator;
        private StringBuilder title;

        public static string ListTitleProperties {
            get {
                var str = new StringBuilder();
                foreach (Type type in titleFrom.Keys) {
                    str.AppendLine(type + " -> " + titleFrom[type].GetType().Name);
                }

                return str.ToString();
            }
        }

        #region Private

        internal static string TitleOrToString(object obj, string format) {
            Type type = obj.GetType();

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

            if (titleFrom[type] == null) {
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

        #endregion

        /// <summary>
        ///     Determines if the specified object's Title (from its <c>ToString()</c> method) is empty. Will
        ///     return true if either: the specified reference is null; the object's <c>ToString()</c> method
        ///     returns null; or if the <c>ToString()</c> returns an empty string
        /// </summary>
        private static bool IsEmpty(object obj, string format) {
            return obj == null || IsEmpty(TitleOrToString(obj, format));
        }

        /// <summary>
        ///     Determines if the specified text is empty. Will return true if either: the specified reference is null;
        ///     or if the reference is an empty string
        /// </summary>
        private static bool IsEmpty(string text) {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        ///     Returns a string that represents the value of this obj
        /// </summary>
        public override string ToString() {
            ProcessCurrent();
            return title.ToString();
        }

        private void ProcessCurrent() {
            if (obj == null && defaultValue != null) {
                obj = defaultValue;
            }

            if (obj != null) {
                if (joiner != null && title.Length > 0) {
                    title.Append(joiner);
                }

                if (appendWithSpace && title.Length > 0) {
                    title.Append(" ");
                }

                title.Append(TitleOrToString(obj, pattern));
            }

            joiner = separator;
            appendWithSpace = false;
            pattern = null;
            defaultValue = null;
            separator = null;
            obj = null;
        }

        /// <summary>
        ///     Adds the title of the parameter to the title builder, with no
        ///     spaces.  If the paramater is null then no text is added.
        /// </summary>
        public NewTitleBuilder Concat(object obj) {
            ProcessCurrent();
            this.obj = obj;
            return this;
        }

        /// <summary>
        ///     Adds the title of the parameter to the title builder with a single space
        ///     between.  If the paramater is null then no text is added.
        /// </summary>
        public NewTitleBuilder Append(object obj) {
            ProcessCurrent();
            this.obj = obj;
            appendWithSpace = true;
            return this;
        }

        /// <summary>
        ///     Applies the specified format (e.g. "dd/MM/yy" for a date) to the object
        ///     previously just appended or concatenated.
        /// </summary>
        public NewTitleBuilder Format(string pattern) {
            this.pattern = pattern;
            return this;
        }

        /// <summary>
        ///     Specifies a default value to use if the object previously just appended or
        ///     concatenated is null.  If the object is not null, then this method does
        ///     nothing.
        /// </summary>
        public NewTitleBuilder Default(string value) {
            defaultValue = value;
            return this;
        }

        /// <summary>
        ///     Adds a separator string (e.g. ",") for use between two elements of the title.
        ///     If either the preceding or following object being appended or concatenated is
        ///     null then the separator string is not added.
        /// </summary>
        public NewTitleBuilder Separator(string s) {
            separator = s;
            return this;
        }

        /// <summary>
        ///     Truncates the title to a specified number of characters
        /// </summary>
        /// <param name="toCharacters"></param>
        /// <param name="breakOnWords">If set to true, the truncated title will include only whole words (defined by a space)</param>
        /// <param name="continuation">
        ///     If not null, the continuation string is added to the end of the title with
        ///     the other characters reduced to accommodate this
        /// </param>
        public void Truncate(int toCharacters, bool breakOnWords = false, string continuation = null) {
            int start = title.Length;
            ProcessCurrent();
            int last = title.Length;

            int length = toCharacters;
            if (continuation != null) {
                length -= continuation.Length;
            }

            int end = Math.Min(start + length, last - 1);
            if (end >= length) {
                title = new StringBuilder(title.ToString(0, end));

                if (breakOnWords) {
                    for (; end > 5; end--) {
                        if (title[end - 1] == ' ') {
                            break;
                        }
                    }

                    end--;
                }
                else {
                    for (; end > 5; end--) {
                        if (title[end - 1] != ' ') {
                            break;
                        }
                    }
                }

                title.Length = end;
                title.Append(continuation);
            }
        }

        #region Constructors

        /// <summary>
        ///     Creates a new, empty, TitleBuilder object
        /// </summary>
        public NewTitleBuilder() {
            title = new StringBuilder();
        }

        /// <summary>
        ///     Creates a new TitleBuilder object, containing the Title of the specified object
        /// </summary>
        public NewTitleBuilder(object obj)
            : this() {
            this.obj = obj;
        }

        #endregion
    }
}