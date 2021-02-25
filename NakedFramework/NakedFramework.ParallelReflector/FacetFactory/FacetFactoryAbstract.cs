// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;

namespace NakedFramework.ParallelReflector.FacetFactory {
    public abstract class FacetFactoryAbstract  {
        protected FacetFactoryAbstract(int numericOrder,
                                       ILoggerFactory loggerFactory,
                                       FeatureType featureTypes) {
            NumericOrder = numericOrder;
            LoggerFactory = loggerFactory;
            FeatureTypes = featureTypes;
        }

        protected ILoggerFactory LoggerFactory { get; }

        protected ILogger<T> Logger<T>() => LoggerFactory.CreateLogger<T>();

        public virtual IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) => new PropertyInfo[] { };

        public virtual IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) => new PropertyInfo[] { };

        #region IFacetFactory Members

        public int NumericOrder { get; }

        public virtual FeatureType FeatureTypes { get; }

        public int CompareTo(IFacetFactory other) => NumericOrder.CompareTo(other.NumericOrder);

        #endregion
    }
}