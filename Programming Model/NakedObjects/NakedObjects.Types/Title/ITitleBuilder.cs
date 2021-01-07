// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects {
    /// <summary>
    ///     Interface for the utility class created by the IDomainObjectContainer#NewTitleBuilder
    ///     to help produce titles for objects without having to add lots of guard
    ///     code.
    ///     It provides two basic method: one to concatenate a title to the buffer;
    ///     another to append a title with a joiner string, taking care adding in necessary
    ///     spaces. The benefits of using this class is that <c>null</c> references are
    ///     safely ignored (rather than appearing as 'null'), and joiners (a space by default) are only
    ///     added when needed
    /// </summary>
    public interface ITitleBuilder {
        ITitleBuilder Append(object obj);
        ITitleBuilder Append(object obj, string format, string defaultValue);
        ITitleBuilder Append(string joiner, object obj);
        ITitleBuilder Append(string joiner, object obj, string format, string defaultTitle);
        ITitleBuilder Append(string joiner, string text);
        ITitleBuilder Append(string text);
        ITitleBuilder AppendSpace();
        ITitleBuilder Concat(object obj);
        ITitleBuilder Concat(object obj, string format, string defaultValue);
        ITitleBuilder Concat(string joiner, object obj);
        ITitleBuilder Concat(string joiner, object obj, string format, string defaultValue);
        ITitleBuilder Concat(string text);
        string ToString();
        ITitleBuilder Truncate(int noWords);
    }
}