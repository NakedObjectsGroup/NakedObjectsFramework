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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class CallbackMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
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

        public override string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, Type type, IMethodRemover remover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var facets = new List<IFacet>();
            var methods = new List<MethodInfo>();

            var method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.CreatedMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new CreatedCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new CreatedCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.PersistingMethod, typeof(void), Type.EmptyTypes, classStrategy);

            if (method != null) {
                methods.Add(method);
                facets.Add(new PersistingCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new PersistingCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.PersistedMethod, typeof(void), Type.EmptyTypes, classStrategy);

            if (method != null) {
                methods.Add(method);
                facets.Add(new PersistedCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new PersistedCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.UpdatingMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new UpdatingCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new UpdatingCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.UpdatedMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new UpdatedCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new UpdatedCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.LoadingMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new LoadingCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new LoadingCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.LoadedMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new LoadedCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new LoadedCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DeletingMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new DeletingCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new DeletingCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.DeletedMethod, typeof(void), Type.EmptyTypes, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new DeletedCallbackFacetViaMethod(method, specification));
            }
            else {
                facets.Add(new DeletedCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.OnUpdatingErrorMethod, typeof(string), new[] {typeof(Exception)}, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new OnUpdatingErrorCallbackFacetViaMethod(method, specification, Logger<OnUpdatingErrorCallbackFacetViaMethod>()));
            }
            else {
                facets.Add(new OnUpdatingErrorCallbackFacetNull(specification));
            }

            method = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.OnPersistingErrorMethod, typeof(string), new[] {typeof(Exception)}, classStrategy);
            if (method != null) {
                methods.Add(method);
                facets.Add(new OnPersistingErrorCallbackFacetViaMethod(method, specification, Logger<OnPersistingErrorCallbackFacetViaMethod>()));
            }
            else {
                facets.Add(new OnPersistingErrorCallbackFacetNull(specification));
            }

            remover.RemoveMethods(methods);
            FacetUtils.AddFacets(facets);
            return metamodel;
        }
    }
}