// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Core.Util;

namespace NakedFramework.ParallelReflector.TypeFacetFactory {
    /// <summary>
    ///     This factory filters out actions on system types. So for example 'GetHashCode' will not show up when displaying a
    ///     string.
    /// </summary>
    public sealed class SystemClassMethodFilteringFactory : SystemTypeFacetFactoryProcessor, IMethodFilteringFacetFactory {
        public SystemClassMethodFilteringFactory(IFacetFactoryOrder<SystemClassMethodFilteringFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) { }

        #region IMethodFilteringFacetFactory Members

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => TypeKeyUtils.IsSystemClass(method.DeclaringType);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}