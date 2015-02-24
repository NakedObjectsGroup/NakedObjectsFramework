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
    ///     A <see cref="IFacetFactory" /> implementation that is able to identify a property or collection.
    /// </summary>
    /// <para>
    ///     For example, property  is  used to represent either a property (value or reference) or a collection,
    ///     with the return type indicating which.
    /// </para>
    /// <para>
    ///     Used by <see cref="IFacetFactorySet" /> to determine which facet factories to ask
    ///     whether a <see cref="PropertyInfo" /> represents a property or a collection.
    /// </para>
    public interface IPropertyOrCollectionIdentifyingFacetFactory : IFacetFactory {
        IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy);
        IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy);
    }
}