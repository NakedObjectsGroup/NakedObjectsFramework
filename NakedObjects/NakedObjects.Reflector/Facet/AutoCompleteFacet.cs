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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class AutoCompleteFacet : FacetAbstract, IAutoCompleteFacet, IImperativeFacet {
    private const int DefaultPageSize = 50;

    private readonly MethodSerializationWrapper methodWrapper;

    public AutoCompleteFacet(MethodInfo autoCompleteMethod, int pageSize, int minLength, ILogger<AutoCompleteFacet> logger) {
        methodWrapper = MethodSerializationWrapper.Wrap(autoCompleteMethod, logger);

        PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
        MinLength = minLength;
    }

    public int PageSize { get; }

    public override Type FacetType => typeof(IAutoCompleteFacet);

    #region IAutoCompleteFacet Members

    public int MinLength { get; }

    public object[] GetCompletions(INakedObjectAdapter inObjectAdapter, string autoCompleteParm, INakedFramework framework) {
        try {
            var autoComplete = methodWrapper.Invoke<object>(inObjectAdapter.GetDomainObject(), new object[] { autoCompleteParm });
            return autoComplete switch {
                IQueryable queryable => queryable.Take(PageSize).ToArray(),
                IEnumerable<string> strings => strings.Cast<object>().ToArray(),
                _ when !CollectionUtils.IsCollection(autoComplete.GetType()) => new[] { autoComplete },
                _ => throw new NakedObjectDomainException($"Must return IQueryable or a single object from autoComplete method: {GetMethod().Name}")
            };
        }
        catch (ArgumentException ae) {
            throw new InvokeException($"autoComplete exception: {GetMethod().Name} has mismatched parameter type - must be string", ae);
        }
    }

    #endregion

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.