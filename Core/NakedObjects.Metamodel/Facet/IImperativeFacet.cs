// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Spec;

namespace NakedObjects.Meta.Facet {
    /// <summary>
    ///     A <see cref="IFacet" /> implementation that ultimately wraps a
    ///     <see cref="MethodInfo" />, for a implementation of a <see cref="IMemberSpec" />
    /// </summary>
    /// <para>
    ///     <see cref="IFacet" />s relating to the class itself (ie for <see cref="IObjectSpec" />
    ///     should not implement this interface.
    /// </para>
    public interface IImperativeFacet {
        /// <summary>
        ///     The <see cref="MethodInfo" /> invoked by this Facet.
        /// </summary>
        MethodInfo GetMethod();
    }
}