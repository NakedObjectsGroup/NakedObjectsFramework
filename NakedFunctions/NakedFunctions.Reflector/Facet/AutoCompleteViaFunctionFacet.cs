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
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class AutoCompleteViaFunctionFacet : FacetAbstract, IAutoCompleteFacet, IImperativeFacet {
    private const int DefaultPageSize = 50;
    private readonly MethodSerializationWrapper methodWrapper;

    public AutoCompleteViaFunctionFacet(MethodInfo method,
                                        int pageSize,
                                        int minLength,
                                        ILogger<AutoCompleteViaFunctionFacet> logger) {
        PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
        MinLength = minLength;
        methodWrapper = MethodSerializationWrapper.Wrap(method, logger);
    }

    public int PageSize { get; }

    public override Type FacetType => typeof(IAutoCompleteFacet);

    #region IAutoCompleteFacet Members

    public int MinLength { get; }

    public object[] GetCompletions(INakedObjectAdapter inObjectAdapter, string autoCompleteParm, INakedFramework framework) {
        try {
            var autoComplete = methodWrapper.Invoke<object>(GetMethod().GetParameterValues(inObjectAdapter, autoCompleteParm, framework));

            switch (autoComplete) {
                //returning an IQueryable
                case IQueryable queryable:
                    return queryable.Take(PageSize).ToArray();
                //returning an IEnumerable (of string only)
                case IEnumerable<string> strings:
                    return strings.Cast<object>().ToArray();
                default: {
                    //return type is a single object
                    if (!CollectionUtils.IsCollection(autoComplete.GetType())) {
                        return new[] { autoComplete };
                    }

                    throw new NakedObjectDomainException($"Must return IQueryable or a single object from autoComplete method: {GetMethod().Name}");
                }
            }
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