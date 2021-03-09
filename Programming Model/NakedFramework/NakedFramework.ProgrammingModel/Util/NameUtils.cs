// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Text;
using NakedFramework.Resources;

namespace NakedFramework {
    /// <summary>
    ///     Utility methods for manipulating type-name strings.  The Naked Objects framework makes extensive
    ///     use of these utils, but they are provided within the NakedObjects.Helpers
    ///     assembly to permit optional use within domain code.
    /// </summary>
    public static class NameUtils {
        private const char space = ' ';

        /// <summary>
        ///     Return a lower case, non-spaced version of the specified name
        /// </summary>
        public static string SimpleName(string name) {
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name) {
                if (!char.IsWhiteSpace(ch)) {
                    sb.Append(char.ToLower(ch));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Returns a word spaced version of the specified name, so there are spaces between the words, where each
        ///     word starts with a capital letter. E.g., "NextAvailableDate" is returned as "Next Available Date".
        /// </summary>
        public static string NaturalName(string name) {
            var length = name.Length;

            if (length <= 1) {
                return name.ToUpper(); // ensure first character is upper case
            }

            var naturalName = new StringBuilder(length);

            var character = char.ToUpper(name[0]); // ensure first character is upper case
            naturalName.Append(character);
            var nextCharacter = name[1];

            for (var pos = 2; pos < length; pos++) {
                var previousCharacter = character;
                character = nextCharacter;
                nextCharacter = name[pos];

                if (!char.IsWhiteSpace(previousCharacter)) {
                    if (char.IsUpper(character) && !char.IsUpper(previousCharacter)) {
                        naturalName.Append(space);
                    }

                    if (char.IsUpper(character) && char.IsLower(nextCharacter) && char.IsUpper(previousCharacter)) {
                        naturalName.Append(space);
                    }

                    if (char.IsDigit(character) && !char.IsDigit(previousCharacter)) {
                        naturalName.Append(space);
                    }
                }

                naturalName.Append(character);
            }

            naturalName.Append(nextCharacter);
            return naturalName.ToString();
        }

        public static string PluralName(string name) {
            string pluralName;
            if (name.EndsWith("y")) {
                pluralName = name.Substring(0, name.Length - 1 - 0) + "ies";
            }
            else if (name.EndsWith("s") || name.EndsWith("x")) {
                pluralName = name + "es";
            }
            else {
                pluralName = name + 's';
            }

            return pluralName;
        }

        public static string CapitalizeName(string name) => char.ToUpper(name[0]) + name.Substring(1);

        private static bool IsStartOfNewWord(char c, char previousChar) => char.IsUpper(c) || char.IsDigit(c) && !char.IsDigit(previousChar);

        public static string MakeTitle(string name) {
            var pos = 0;

            // find first upper case character
            while (pos < name.Length && char.IsLower(name[pos])) {
                pos++;
            }

            if (pos == name.Length) {
                return ProgrammingModel.InvalidName;
            }

            var s = new StringBuilder(name.Length - pos); // remove is/get/set
            for (var j = pos; j < name.Length; j++) {
                // process english name - add spaces
                if (j > pos && IsStartOfNewWord(name[j], name[j - 1])) {
                    s.Append(' ');
                }

                s.Append(name[j]);
            }

            return s.ToString();
        }

        public static string[] NaturalNames(Type typeOfEnum) => Enum.GetNames(typeOfEnum).Select(NaturalName).ToArray();
    }
}