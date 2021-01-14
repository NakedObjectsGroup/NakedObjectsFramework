// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Applied to a string parameter, specifies the minimum & maximum string length that the user may enter.
    ///     When applied to the string parameter of an AutoComplete method, the Minimum specifies the number of
    ///     characters the user must type before the method will be invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MaxLengthAttribute : Attribute {
        public MaxLengthAttribute(int value) => Value = value;

        public int Value { get; }
    }
}