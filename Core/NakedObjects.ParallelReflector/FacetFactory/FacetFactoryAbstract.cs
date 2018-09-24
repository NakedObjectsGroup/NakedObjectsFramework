// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    public abstract class FacetFactoryAbstract : IFacetFactory {
        private readonly FeatureType featureTypes;

        protected FacetFactoryAbstract(int numericOrder, FeatureType featureTypes) {
            NumericOrder = numericOrder;
            this.featureTypes = featureTypes;
        }

        #region IFacetFactory Members

        public int NumericOrder { get; private set; }

        public virtual FeatureType FeatureTypes {
            get { return featureTypes; }
        }

        public virtual void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {}

        public virtual void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {}
        public virtual void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {}
        public virtual void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IMetamodelBuilder metamodel) {}

        public virtual ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            return metamodel;
        }

        public virtual ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            return metamodel;
        }

        public virtual ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            return metamodel;
        }

        public virtual ImmutableDictionary<String, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            return metamodel;
        }

        public virtual IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return new PropertyInfo[] {};
        }

        public virtual IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return new PropertyInfo[] {};
        }

        public int CompareTo(IFacetFactory other) {
            return NumericOrder.CompareTo(other.NumericOrder);
        }

        #endregion
    }
}