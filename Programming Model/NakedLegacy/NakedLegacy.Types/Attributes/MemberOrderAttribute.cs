// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedLegacy {
    /// <summary>
    ///     For specifying the order in which fields and/or actions are presented to
    ///     the user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class MemberOrderAttribute : Attribute {
        public MemberOrderAttribute() => Sequence = "";

        public MemberOrderAttribute(string sequence) => Sequence = sequence;

        public MemberOrderAttribute(double sequence) => Sequence = "" + sequence;

        public string Sequence { get; set; }

        public string Name { get; set; }
    }
}