// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;

namespace NakedFunctions.Reflector.FacetFactory {
    /// <summary>
    ///     This factory filters out properties on system types. So for example 'Length' will not show up when displaying a
    ///     string.
    /// </summary>
    public sealed class SystemClassPropertyFilteringFactory : FunctionalFacetFactoryProcessor, IPropertyFilteringFacetFactory {
        private ILogger<SystemClassPropertyFilteringFactory> logger;

        public SystemClassPropertyFilteringFactory(IFacetFactoryOrder<SystemClassPropertyFilteringFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Properties) =>
            logger = loggerFactory.CreateLogger<SystemClassPropertyFilteringFactory>();

        #region IPropertyFilteringFacetFactory Members

        public bool Filters(PropertyInfo property, IClassStrategy classStrategy) => TypeKeyUtils.IsSystemClass(property.DeclaringType);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}