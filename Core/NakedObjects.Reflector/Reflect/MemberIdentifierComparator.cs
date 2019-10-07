// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Reflect {
    /// <summary>
    ///     Compares <see cref="IMemberSpecImmutable" /> by <see cref="ISpecification.Identifier" />
    /// </summary>
    internal class MemberIdentifierComparator<T> : IComparer<T> where T : IMemberSpecImmutable {
        #region IComparer<T> Members

        public int Compare(T o1, T o2) {
            return o1.Identifier.CompareTo(o2.Identifier);
        }

        #endregion
    }
}