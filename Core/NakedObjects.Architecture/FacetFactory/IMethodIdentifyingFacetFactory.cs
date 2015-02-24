// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.FacetFactory {
    /// <summary>
    ///     A <see cref="IFacetFactory" /> implementation that is able to identify an action
    /// </summary>
    /// <para>
    ///     Used by <see cref="IFacetFactorySet" /> to determine which facet factories to ask
    ///     whether a <see cref="MethodInfo" /> represents an action.
    /// </para>
    public interface IMethodIdentifyingFacetFactory : IFacetFactory {
        IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy);
    }
}