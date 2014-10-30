// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Service;
using NakedObjects.Core.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Web.Mvc.Html {
    internal static class FrameworkHelper {
     
        public static IEnumerable<IActionSpec> GetActions(this INakedObjectsFramework framework, INakedObject nakedObject) {
            return nakedObject.Spec.GetAllActions().OfType<ActionSpec>().Cast<IActionSpec>().Union(
                        nakedObject.Spec.GetAllActions().OfType<ActionSpecSet>().SelectMany(set => set.Actions)).
                               Where(a => a.IsUsable( nakedObject).IsAllowed).
                               Where(a => a.IsVisible( nakedObject ));
        }

        public static IEnumerable<IActionSpec> GetTopLevelActions(this INakedObjectsFramework framework,INakedObject nakedObject) {
            return nakedObject.Spec.GetAllActions().
                               Where(a => a.IsVisible( nakedObject)).
                               Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible( nakedObject)));
        }

        public static IEnumerable<IActionSpec> GetTopLevelActionsByReturnType(this INakedObjectsFramework framework, INakedObject nakedObject, IObjectSpec spec) {
            return framework.GetTopLevelActions(nakedObject).
                Where(a => a is ActionSpecSet || (IsOfTypeOrCollectionOfType(a, spec) && a.IsFinderMethod)).
                Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible( nakedObject) && IsOfTypeOrCollectionOfType(sa, spec) && sa.IsFinderMethod));
        }

        public static IEnumerable<IActionSpec> GetChildActions(this INakedObjectsFramework framework, ActionContext actionContext) {
            if (actionContext.Action is ActionSpecSet) {
                return actionContext.Action.Actions.
                                     Where(a => a.ActionType == ActionType.User).
                                     Where(a => a.IsVisible(actionContext.Target));
            }

            return new List<IActionSpec>();
        }

        public static IEnumerable<IActionSpec> GetChildActionsByReturnType(this INakedObjectsFramework framework, ActionContext actionContext, IObjectSpec spec) {
            return framework.GetChildActions(actionContext).Where(a => IsOfTypeOrCollectionOfType(a, spec)).
                                                  Where(action => action.Parameters.All(parm => parm.Spec.IsParseable || parm.IsChoicesEnabled || parm.Spec.IsOfType(actionContext.Target.Spec)));
        }

        private static bool IsOfTypeOrCollectionOfType(IActionSpec actionSpec, IObjectSpec spec) {
            var returnType = actionSpec.ReturnType;
            return returnType.IsOfType(spec) || (returnType.IsCollection && actionSpec.ElementSpec.IsOfType(spec));
        }

        public static string GetObjectType(Type type) {
            return type.GetProxiedTypeFullName().Replace('.', '-');
        }

        public static string GetObjectType(object model) {
            return model == null ? string.Empty : GetObjectType(model.GetType());
        }

        private static string Encode(this IEncodedToStrings encoder) {
            return encoder.ToShortEncodedStrings().Aggregate((a, b) => a + ";" + b);
        }

        public static string GetObjectId(INakedObject nakedObject, CollectionMemento memento) {
            return memento.Encode();
        }

        public static string GetObjectId(this INakedObjectsFramework framework, INakedObject nakedObject) {
            if (nakedObject.Spec.IsViewModel) {
                framework.LifecycleManager.PopulateViewModelKeys(nakedObject);
            }
            else if (nakedObject.Oid == null) {
                return "";
            }

            return GetObjectId(nakedObject.Oid);
        }


        public static string GetObjectId(IOid oid) {
            return ((IEncodedToStrings) oid).Encode();
        }

        public static string GetObjectId(this INakedObjectsFramework framework, object model) {
            Assert.AssertFalse("Cannot get Adapter for Adapter", model is INakedObject);
            INakedObject nakedObject = framework.GetNakedObject(model);
            return framework.GetObjectId(nakedObject);
        }

        public static string GetObjectTypeName(this INakedObjectsFramework framework, object model) {
            INakedObject nakedObject = framework.GetNakedObject(model);
            return nakedObject.Spec.ShortName;
        }

        public static string GetServiceId(this INakedObjectsFramework framework, string name) {
            INakedObject nakedObject = framework.GetAdaptedService(name);
            return framework.GetObjectId(nakedObject);
        }

        public static string GetInternalServiceId(object service) {
            return ServiceUtils.GetId(service);
        }

        public static object GetObjectFromId(this INakedObjectsFramework framework, string encodedId) {
            return framework.GetNakedObjectFromId(encodedId).Object;
        }

        // TODO move this onto lifecycle manager
        public static INakedObject GetNakedObjectFromId(this INakedObjectsFramework framework, string encodedId) {
            if (string.IsNullOrEmpty(encodedId)) {
                return null;
            }

            IOid oid = framework.LifecycleManager.RestoreOid(encodedId.Split(';'));

            if (oid is CollectionMemento) {
                return RestoreCollection(oid as CollectionMemento);
            }

            if (oid is AggregateOid) {
                return framework.RestoreInline(oid as AggregateOid);
            }

            if (oid is ViewModelOid) {
                return framework.RestoreViewModel(oid as ViewModelOid);
            }

            return framework.RestoreObject(oid);
        }

        private static IObjectSpec GetSpecificationFromObjectId(this INakedObjectsFramework framework, string encodedId, out string[] restOfArray) {
            string[] asArray = encodedId.Split(';');
            return framework.GetSpecificationFromObjectId(asArray, out restOfArray);
        }

        private static IObjectSpec GetSpecificationFromObjectId(this INakedObjectsFramework framework, string[] asArray, out string[] restOfArray) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(asArray.First()));
            IObjectSpec spec = framework.Metamodel.GetSpecification(typeName);
            restOfArray = asArray.ToArray();
            return spec;
        }

        private static INakedObject RestoreCollection(CollectionMemento memento) {
            return memento.RecoverCollection();
        }

        private static INakedObject RestoreInline(this INakedObjectsFramework framework, AggregateOid aggregateOid) {
            IOid parentOid = aggregateOid.ParentOid;
            INakedObject parent = framework.RestoreObject(parentOid);
            IAssociationSpec assoc = parent.Spec.Properties.Where((p => p.Id == aggregateOid.FieldName)).Single();

            return assoc.GetNakedObject(parent);
        }

        private static INakedObject RestoreViewModel(this INakedObjectsFramework framework, ViewModelOid viewModelOid) {
            return framework.Manager.GetAdapterFor(viewModelOid) ?? framework.LifecycleManager.GetViewModel(viewModelOid);
        }

        public static INakedObject RestoreObject(this INakedObjectsFramework framework, IOid oid) {
            if (oid.IsTransient) {
                return framework.LifecycleManager.RecreateInstance(oid, oid.Spec);
            }
            return framework.LifecycleManager.LoadObject(oid, oid.Spec);
        }

        public static INakedObject GetNakedObject(this INakedObjectsFramework framework, object domainObject) {
            return framework.Manager.CreateAdapter(domainObject, null, null);
        }

        public static INakedObject GetAdaptedService(this INakedObjectsFramework framework, string name) {
            return framework.Services.GetService(name);
        }

        public static object GetService(this INakedObjectsFramework framework, string name) {
            return framework.Services.GetService(name).Object;
        }

        public static T GetService<T>(this INakedObjectsFramework framework, string name) {
            return framework.Services.GetService(name).GetDomainObject<T>();
        }

        public static INakedObject GetAdaptedService<T>(this INakedObjectsFramework framework) {
            return framework.Services.GetServices().FirstOrDefault(no => no.Object is T);
        }

        public static T GetService<T>(this INakedObjectsFramework framework) {
            return framework.Services.GetServices().Select(no => no.Object).OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<object> GetAllServices(this INakedObjectsFramework framework) {
            return framework.Services.GetServices().Where(x => framework.GetActions(x).Any()).Select(x => x.Object);
        }

        public static IEnumerable<object> GetContributingServices(this INakedObjectsFramework framework) {
            return framework.Services.GetServicesWithVisibleActions(ServiceType.Menu | ServiceType.Contributor, framework.LifecycleManager).Where(x => framework.GetActions(x).Any()).Select(x => x.Object);
        }

        public static IEnumerable<object> GetServices(this INakedObjectsFramework framework) {
            framework.GetAllServices();
            return framework.Services.GetServicesWithVisibleActions(ServiceType.Menu, framework.LifecycleManager).Where(x => framework.GetActions(x).Any()).Select(x => x.Object);
        }

        public static string GetActionId(IActionSpec action) {
            return action == null ? string.Empty : string.Format("{0};{1}", action.OnType.FullName, action.Id);
        }

        public static IActionSpec GetActionFromId(this INakedObjectsFramework framework, string actionId) {
            string[] asArray = actionId.Split(';');
            IObjectSpec spec = framework.Metamodel.GetSpecification(asArray.First());
            string id = asArray.Skip(1).First();

            return spec.GetAllActions().Single(a => a.Id == id);
        }

        public static bool IsNotPersistent(this INakedObject nakedObject) {
            return (nakedObject.ResolveState.IsTransient() && nakedObject.Spec.Persistable == PersistableType.ProgramPersistable) ||
                   nakedObject.Spec.Persistable == PersistableType.Transient;
        }

        public static bool IsImage(this IObjectSpec spec, INakedObjectsFramework framework) {
            IObjectSpec imageSpec = framework.Metamodel.GetSpecification(typeof(Image));
            return spec != null && spec.IsOfType(imageSpec);
        }

        private static bool IsFileAttachment(this IObjectSpec spec, INakedObjectsFramework framework) {
            IObjectSpec fileSpec = framework.Metamodel.GetSpecification(typeof(FileAttachment));
            return spec != null && spec.IsOfType(fileSpec);
        }

        public static bool IsFile(this IAssociationSpec assoc, INakedObjectsFramework framework) {
            return assoc.Spec.IsFile(framework);
        }

        public static bool IsFile(this IObjectSpec spec, INakedObjectsFramework framework) {
            return spec != null && (spec.IsImage(framework) || spec.IsFileAttachment(framework) || spec.ContainsFacet<IArrayValueFacet<byte>>());
        }

        public static string IconName(INakedObject nakedObject) {
            string name = nakedObject.Spec.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        public static INakedObject Parse(this IObjectSpec spec, string s, INakedObjectsFramework framework) {
            return s == null ? framework.Manager.CreateAdapter("", null, null) : spec.GetFacet<IParseableFacet>().ParseTextEntry(s, framework.Manager);
        }

        public static bool IsQueryOnly(this IActionSpec action) {
            if (action.ReturnType.IsQueryable) {
                return true;
            }

            return action.ContainsFacet<IQueryOnlyFacet>();
        }

        public static bool IsIdempotent(this IActionSpec action) {
            return action.ContainsFacet<IIdempotentFacet>();
        }

        public static bool IsParseableOrCollectionOfParseable(this INakedObjectsFramework framework, IActionParameterSpec parmSpec) {
            var spec = parmSpec.Spec;
            return spec.IsParseable || (spec.IsCollection && parmSpec.GetFacet<IElementTypeFacet>().ValueSpec.IsParseable);
        }

        public static INakedObject GetTypedCollection(this INakedObjectsFramework framework, IFeatureSpec featureSpec, IEnumerable collectionValue) {

            IObjectSpec collectionitemSpec = framework.Metamodel.GetSpecification(featureSpec.GetFacet<IElementTypeFacet>().ValueSpec);
            string[] rawCollection = collectionValue.Cast<string>().ToArray();
            object[] objCollection;

            Type instanceType = TypeUtils.GetType(collectionitemSpec.FullName);
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));

            if (collectionitemSpec.IsParseable) {
                objCollection = rawCollection.Select(s => string.IsNullOrEmpty(s) ? null : collectionitemSpec.GetFacet<IParseableFacet>().ParseTextEntry(s, framework.Manager).Object).ToArray();
            }
            else {
                // need to check if collection is actually a collection memento 
                if (rawCollection.Count() == 1) {
                    INakedObject firstObj = framework.GetNakedObjectFromId(rawCollection.First());

                    if (firstObj != null && firstObj.Oid is CollectionMemento) {
                        return firstObj;
                    }
                }

                objCollection = rawCollection.Select(s => framework.GetNakedObjectFromId(s).GetDomainObject()).ToArray();
            }

            objCollection.Where(o => o != null).ForEach(o => typedCollection.Add(o));

            return framework.Manager.CreateAdapter(typedCollection.AsQueryable(), null, null);
        }

        public static bool IsViewModelEditView(this INakedObject target) {
            return target.Spec.ContainsFacet<IViewModelFacet>() && target.Spec.GetFacet<IViewModelFacet>().IsEditView(target);
        }
    }
}