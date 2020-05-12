// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public abstract class PropertyOrCollectionIdentifyingFacetFactoryAbstract : MethodPrefixBasedFacetFactoryAbstract, IPropertyOrCollectionIdentifyingFacetFactory {
        protected PropertyOrCollectionIdentifyingFacetFactoryAbstract(int numericOrder, FeatureType featureTypes)
            : base(numericOrder, featureTypes) { }

        protected static bool IsCollectionOrArray(Type type) => CollectionUtils.IsCollection(type);

        protected bool IsPropertyIncluded(PropertyInfo property) => property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null;

        protected IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) =>
            candidates.Where(property => property.GetGetMethod() != null &&
                                         classStrategy.IsTypeToBeIntrospected(property.PropertyType) &&
                                         IsPropertyIncluded(property)).ToList();
    }
}