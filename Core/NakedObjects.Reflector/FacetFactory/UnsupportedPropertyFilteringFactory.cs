// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    /// add summary here 
    /// </summary>
    public class UnsupportedPropertyFilteringFactory : FacetFactoryAbstract, IPropertyFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UnsupportedPropertyFilteringFactory));

        public UnsupportedPropertyFilteringFactory(int numericOrder)
            : base(numericOrder, FeatureType.Property) {}

        #region IPropertyFilteringFacetFactory Members

        public bool Filters(PropertyInfo property, IClassStrategy classStrategy) {
            string typeName = property.DeclaringType == null ? "Unknown" : property.DeclaringType.FullName;

            //todo rework this so that factories filter actions appropraitely
            // rename this facetfactory and also action one to indicate more clearly what they do so 
            // that they can be replaced as necessary. 
            if (classStrategy.IsSystemClass(property.DeclaringType)) {
                Log.InfoFormat("Skipping fields in {0} (system class according to ClassStrategy)", typeName);
                return true;
            }

            return false;
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}