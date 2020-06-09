// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     This factory filters out properties on system types. So for example 'Length' will not show up when displaying a
    ///     string.
    /// </summary>
    public sealed class SystemClassPropertyFilteringFactory : FacetFactoryAbstract, IPropertyFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SystemClassPropertyFilteringFactory));

        public SystemClassPropertyFilteringFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.Properties) { }

        #region IPropertyFilteringFacetFactory Members

        public bool Filters(PropertyInfo property, IClassStrategy classStrategy) => classStrategy.IsSystemClass(property.DeclaringType);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}