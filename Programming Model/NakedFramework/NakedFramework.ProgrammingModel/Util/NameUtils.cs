// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Resources;

namespace NakedFramework;

/// <summary>
///     Utility methods for manipulating type-name strings.  The Naked Objects framework makes extensive
///     use of these utils, but they are provided within the NakedObjects.Helpers
///     assembly to permit optional use within domain code.
/// </summary>
public static class NameUtils {
    private const char Space = ' ';

    /// <summary>
    ///     Return a lower case, non-spaced version of the specified name
    /// </summary>
    public static string SimpleName(string name) {
        var sb = new List<char>();
        foreach (var ch in name) {
            if (!char.IsWhiteSpace(ch)) {
                sb.Add(char.ToLower(ch));
            }
        }

        return new string(sb.ToArray());
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

        var character = char.ToUpper(name[0]); // ensure first character is upper case
        var naturalName = new List<char> { character };
        var nextCharacter = name[1];

        for (var pos = 2; pos < length; pos++) {
            var previousCharacter = character;
            character = nextCharacter;
            nextCharacter = name[pos];

            if (!char.IsWhiteSpace(previousCharacter)) {
                if (char.IsUpper(character) && !char.IsUpper(previousCharacter)) {
                    naturalName.Add(Space);
                }

                if (char.IsUpper(character) && char.IsLower(nextCharacter) && char.IsUpper(previousCharacter)) {
                    naturalName.Add(Space);
                }

                if (char.IsDigit(character) && !char.IsDigit(previousCharacter)) {
                    naturalName.Add(Space);
                }
            }

            naturalName.Add(character);
        }

        naturalName.Add(nextCharacter);
        return new string(naturalName.ToArray());
    }

    public static string PluralName(string name) {
        string pluralName;
        if (name.EndsWith("y")) {
            pluralName = name[..(name.Length - 1 - 0)] + "ies";
        }
        else if (name.EndsWith("s") || name.EndsWith("x")) {
            pluralName = name + "es";
        }
        else {
            pluralName = name + 's';
        }

        return pluralName;
    }

    public static string CapitalizeName(string name) => char.ToUpper(name[0]) + name[1..];

    private static bool IsStartOfNewWord(char c, char previousChar) => char.IsUpper(c) || (char.IsDigit(c) && !char.IsDigit(previousChar));

    public static string MakeTitle(string name) {
        var pos = 0;

        // find first upper case character
        while (pos < name.Length && char.IsLower(name[pos])) {
            pos++;
        }

        if (pos == name.Length) {
            return ProgrammingModel.InvalidName;
        }

        var s = new List<char>(); // remove is/get/set
        for (var j = pos; j < name.Length; j++) {
            // process english name - add spaces
            if (j > pos && IsStartOfNewWord(name[j], name[j - 1])) {
                s.Add(' ');
            }

            s.Add(name[j]);
        }

        return new string(s.ToArray());
    }

    public static string[] NaturalNames(Type typeOfEnum) => Enum.GetNames(typeOfEnum).Select(NaturalName).ToArray();
}