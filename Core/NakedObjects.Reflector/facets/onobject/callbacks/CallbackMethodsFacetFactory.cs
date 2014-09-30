// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    public class CallbackMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (CallbackMethodsFacetFactory));

        private static readonly string[] prefixes;

        static CallbackMethodsFacetFactory() {
            prefixes = new[] {
                PrefixesAndRecognisedMethods.DeletedMethod,
                PrefixesAndRecognisedMethods.DeletingMethod,
                PrefixesAndRecognisedMethods.LoadedMethod,
                PrefixesAndRecognisedMethods.LoadingMethod,
                PrefixesAndRecognisedMethods.UpdatedMethod,
                PrefixesAndRecognisedMethods.UpdatingMethod,
                PrefixesAndRecognisedMethods.PersistedMethod,
                PrefixesAndRecognisedMethods.PersistingMethod,
                PrefixesAndRecognisedMethods.SavedMethod,
                PrefixesAndRecognisedMethods.SavingMethod,
                PrefixesAndRecognisedMethods.CreatedMethod,
                PrefixesAndRecognisedMethods.OnPersistingErrorMethod,
                PrefixesAndRecognisedMethods.OnUpdatingErrorMethod
            };
        }

        public CallbackMethodsFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override string[] Prefixes {
            get { return prefixes; }
        }

        public override bool Process(Type type, IMethodRemover remover, IFacetHolder facetHolder) {
            var facets = new List<IFacet>();
            var methods = new List<MethodInfo>();

            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.CreatedMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new CreatedCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new CreatedCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.PersistingMethod, typeof (void), Type.EmptyTypes);
            MethodInfo oldMethod = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.SavingMethod, typeof (void), Type.EmptyTypes);

            if (method != null && oldMethod != null) {
                // cannot have both old and new method types 
                throw new ModelException(Resources.NakedObjects.PersistingSavingError);
            }

            if (method == null && oldMethod != null) {
                Log.WarnFormat("Class {0} still has Saving method - replace with Persisting", type);
                method = oldMethod;
            }

            if (method != null) {
                methods.Add(method);
                facets.Add(new PersistingCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new PersistingCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.PersistedMethod, typeof (void), Type.EmptyTypes);
            oldMethod = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.SavedMethod, typeof (void), Type.EmptyTypes);

            if (method != null && oldMethod != null) {
                // cannot have both old and new method types 
                throw new ModelException("Cannot have both Persisted and Saved methods - please remove Saved");
            }

            if (method == null && oldMethod != null) {
                Log.WarnFormat("Class {0} still has Saved method - replace with Persisted", type.ToString());
                method = oldMethod;
            }

            if (method != null) {
                methods.Add(method);
                facets.Add(new PersistedCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new PersistedCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.UpdatingMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new UpdatingCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new UpdatingCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.UpdatedMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new UpdatedCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new UpdatedCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.LoadingMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new LoadingCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new LoadingCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.LoadedMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new LoadedCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new LoadedCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.DeletingMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new DeletingCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new DeletingCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.DeletedMethod, typeof (void), Type.EmptyTypes);
            if (method != null) {
                methods.Add(method);
                facets.Add(new DeletedCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new DeletedCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.OnUpdatingErrorMethod, typeof (string), new[] {typeof (Exception)});
            if (method != null) {
                methods.Add(method);
                facets.Add(new OnUpdatingErrorCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new OnUpdatingErrorCallbackFacetNull(facetHolder));
            }

            method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.OnPersistingErrorMethod, typeof (string), new[] {typeof (Exception)});
            if (method != null) {
                methods.Add(method);
                facets.Add(new OnPersistingErrorCallbackFacetViaMethod(method, facetHolder));
            }
            else {
                facets.Add(new OnPersistingErrorCallbackFacetNull(facetHolder));
            }


            remover.RemoveMethods(methods);
            return FacetUtils.AddFacets(facets);
        }
    }
}