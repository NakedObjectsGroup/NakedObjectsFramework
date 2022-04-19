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
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedObjects.Reflector.Facet;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class CallbackMethodsFacetFactory : DomainObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.DeletedMethod,
        RecognisedMethodsAndPrefixes.DeletingMethod,
        RecognisedMethodsAndPrefixes.LoadedMethod,
        RecognisedMethodsAndPrefixes.LoadingMethod,
        RecognisedMethodsAndPrefixes.UpdatedMethod,
        RecognisedMethodsAndPrefixes.UpdatingMethod,
        RecognisedMethodsAndPrefixes.PersistedMethod,
        RecognisedMethodsAndPrefixes.PersistingMethod,
        RecognisedMethodsAndPrefixes.CreatedMethod,
        RecognisedMethodsAndPrefixes.OnPersistingErrorMethod,
        RecognisedMethodsAndPrefixes.OnUpdatingErrorMethod
    };

    public CallbackMethodsFacetFactory(IFacetFactoryOrder<CallbackMethodsFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Objects) { }

    public string[] Prefixes => FixedPrefixes;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover remover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var facets = new List<IFacet>();
        var methods = new List<MethodInfo>();

        var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.CreatedMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new CreatedCallbackFacetViaMethod(method, Logger<CreatedCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(CreatedCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.PersistingMethod, typeof(void), Type.EmptyTypes);

        if (method is not null) {
            methods.Add(method);
            facets.Add(new PersistingCallbackFacetViaMethod(method, Logger<PersistingCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(PersistingCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.PersistedMethod, typeof(void), Type.EmptyTypes);

        if (method is not null) {
            methods.Add(method);
            facets.Add(new PersistedCallbackFacetViaMethod(method, Logger<PersistedCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(PersistedCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.UpdatingMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new UpdatingCallbackFacetViaMethod(method, Logger<UpdatingCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(UpdatingCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.UpdatedMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new UpdatedCallbackFacetViaMethod(method, Logger<UpdatedCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(UpdatedCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.LoadingMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new LoadingCallbackFacetViaMethod(method, Logger<LoadingCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(LoadingCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.LoadedMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new LoadedCallbackFacetViaMethod(method, Logger<LoadedCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(LoadedCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DeletingMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new DeletingCallbackFacetViaMethod(method, Logger<DeletingCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(DeletingCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DeletedMethod, typeof(void), Type.EmptyTypes);
        if (method is not null) {
            methods.Add(method);
            facets.Add(new DeletedCallbackFacetViaMethod(method, Logger<DeletedCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(DeletedCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.OnUpdatingErrorMethod, typeof(string), new[] { typeof(Exception) });
        if (method is not null) {
            methods.Add(method);
            facets.Add(new OnUpdatingErrorCallbackFacetViaMethod(method, Logger<OnUpdatingErrorCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(OnUpdatingErrorCallbackFacetNull.Instance);
        }

        method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.OnPersistingErrorMethod, typeof(string), new[] { typeof(Exception) });
        if (method is not null) {
            methods.Add(method);
            facets.Add(new OnPersistingErrorCallbackFacetViaMethod(method, Logger<OnPersistingErrorCallbackFacetViaMethod>()));
        }
        else {
            facets.Add(OnPersistingErrorCallbackFacetNull.Instance);
        }

        remover.RemoveMethods(methods);
        FacetUtils.AddFacets(facets, specification);
        return metamodel;
    }
}