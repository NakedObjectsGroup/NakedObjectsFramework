// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that a service action should be made available on the Find menu
    ///     for a reference object property or parameter -  where the return type matches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FinderActionAttribute : Attribute {
        /// <summary>
        ///     The specified prefix will be renderered in front of the action name for the user
        /// </summary>
        /// <param name="prefix"></param>
        public FinderActionAttribute(string prefix) => Prefix = prefix;

        public FinderActionAttribute() => Prefix = null;

        public string Prefix { get; set; }
    }
}