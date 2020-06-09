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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    public abstract class FacetFactoryAbstract : IFacetFactory {
        protected FacetFactoryAbstract(int numericOrder, ILoggerFactory loggerFactory, FeatureType featureTypes) {
            NumericOrder = numericOrder;
            LoggerFactory = loggerFactory;
            FeatureTypes = featureTypes;
        }

        #region IFacetFactory Members

        protected ILogger<T> Logger<T>() => LoggerFactory.CreateLogger<T>();

        public int NumericOrder { get; }
        protected ILoggerFactory LoggerFactory { get; }

        public virtual FeatureType FeatureTypes { get; }

        public virtual void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) { }
        public virtual void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) { }
        public virtual void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) { }
        public virtual void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) { }

        // stubs for ParallelReflector
        public IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

        public IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

        public IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

        public IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => metamodel;

        public int CompareTo(IFacetFactory other) => NumericOrder.CompareTo(other.NumericOrder);

        #endregion

        public virtual IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) => new PropertyInfo[] { };

        public virtual IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) => new PropertyInfo[] { };
    }
}