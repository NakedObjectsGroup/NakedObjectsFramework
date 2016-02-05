// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;
using System.Reflection;
using NakedObjects.Core;
using System.Collections.Generic;
using NakedObjects.Architecture.Component;
using System.Linq;

namespace NakedObjects.Reflect.FacetFactory {
    public abstract class PropertyOrCollectionIdentifyingFacetFactoryAbstract : MethodPrefixBasedFacetFactoryAbstract, IPropertyOrCollectionIdentifyingFacetFactory {
        protected PropertyOrCollectionIdentifyingFacetFactoryAbstract(int numericOrder, FeatureType featureTypes)
            : base(numericOrder, featureTypes) {}

        protected static bool IsCollectionOrArray(Type type) {
            return CollectionUtils.IsCollection(type);
        }

        protected bool IsPropertyIncluded(PropertyInfo property) {
            if (property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null) return false;
            return true;
            //var attr = property.DeclaringType.GetCustomAttribute<NakedObjectsTypeAttribute>();
            //if (attr == null) return true;
            //switch (attr.ReflectionScope) {
            //    case ReflectOver.All:
            //        return true; //Because we checked for NakedObjectsIgnore earlier.
            //    case ReflectOver.TypeOnlyNoMembers:
            //        return false;
            //    case ReflectOver.ExplicitlyIncludedMembersOnly:
            //        return property.GetCustomAttribute<NakedObjectsIncludeAttribute>() != null;
            //    case ReflectOver.None:
            //        throw new ReflectionException("Attempting to introspect a class that has been marked with NakedObjectsType with ReflectOver.None");
            //    default:
            //        throw new ReflectionException(String.Format("Unhandled value for ReflectOver: {0}", attr.ReflectionScope));
            //}
        }

        protected IList<PropertyInfo> PropertiesToBeIntrospected(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return candidates.Where(property => property.GetGetMethod() != null &&
                                                classStrategy.IsTypeToBeIntrospected(property.PropertyType) &&
                                                IsPropertyIncluded(property)
                                    ).ToList();
        }
    }
}