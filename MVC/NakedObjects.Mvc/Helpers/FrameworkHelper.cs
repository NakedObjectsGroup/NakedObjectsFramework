// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Actions.Potency;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Service;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Web.Mvc.Html {
    internal static class FrameworkHelper {
     
        public static IEnumerable<INakedObjectAction> GetActions(this INakedObjectsFramework framework, INakedObject nakedObject) {
            return nakedObject.Specification.GetAllActions().OfType<NakedObjectActionImpl>().Cast<INakedObjectAction>().Union(
                        nakedObject.Specification.GetAllActions().OfType<NakedObjectActionSet>().SelectMany(set => set.Actions)).
                               Where(a => a.IsUsable( nakedObject).IsAllowed).
                               Where(a => a.IsVisible( nakedObject ));
        }

        public static IEnumerable<INakedObjectAction> GetTopLevelActions(this INakedObjectsFramework framework,INakedObject nakedObject) {
            return nakedObject.Specification.GetAllActions().
                               Where(a => a.IsVisible( nakedObject)).
                               Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible( nakedObject)));
        }

        public static IEnumerable<INakedObjectAction> GetTopLevelActionsByReturnType(this INakedObjectsFramework framework, INakedObject nakedObject, INakedObjectSpecification spec) {
            return framework.GetTopLevelActions(nakedObject).
                Where(a => a is NakedObjectActionSet || (framework.IsOfTypeOrCollectionOfType(a.ReturnType, spec) && a.IsFinderMethod)).
                Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible( nakedObject) && framework.IsOfTypeOrCollectionOfType(sa.ReturnType, spec) && sa.IsFinderMethod));
        }

        public static IEnumerable<INakedObjectAction> GetChildActions(this INakedObjectsFramework framework, ActionContext actionContext) {
            if (actionContext.Action is NakedObjectActionSet) {
                return actionContext.Action.Actions.
                                     Where(a => a.ActionType == NakedObjectActionType.User).
                                     Where(a => a.IsVisible(actionContext.Target));
            }

            return new List<INakedObjectAction>();
        }

        public static IEnumerable<INakedObjectAction> GetChildActionsByReturnType(this INakedObjectsFramework framework, ActionContext actionContext, INakedObjectSpecification spec) {
            return framework.GetChildActions(actionContext).Where(a => framework.IsOfTypeOrCollectionOfType(a.ReturnType, spec)).
                                                  Where(action => action.Parameters.All(parm => parm.Specification.IsParseable || parm.IsChoicesEnabled || parm.Specification.IsOfType(actionContext.Target.Specification)));
        }

        private static bool IsOfTypeOrCollectionOfType(this INakedObjectsFramework framework, INakedObjectSpecification returnType, INakedObjectSpecification spec) {
            if (returnType.IsOfType(spec)) {
                return true;
            }
            return returnType.IsCollection && framework.Metamodel.GetSpecification( returnType.GetFacet<ITypeOfFacet>().ValueSpec).IsOfType(spec);
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
            if (nakedObject.Specification.IsViewModel) {
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
            return nakedObject.Specification.ShortName;
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

        public static INakedObject GetNakedObjectFromId(this INakedObjectsFramework framework, string encodedId) {
            if (string.IsNullOrEmpty(encodedId)) {
                return null;
            }

            IOid oid = framework.LifecycleManager.OidGenerator.RestoreOid(framework.LifecycleManager, encodedId.Split(';'));

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

        private static INakedObjectSpecification GetSpecificationFromObjectId(this INakedObjectsFramework framework, string encodedId, out string[] restOfArray) {
            string[] asArray = encodedId.Split(';');
            return framework.GetSpecificationFromObjectId(asArray, out restOfArray);
        }

        private static INakedObjectSpecification GetSpecificationFromObjectId(this INakedObjectsFramework framework, string[] asArray, out string[] restOfArray) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(asArray.First()));
            INakedObjectSpecification spec = framework.Metamodel.GetSpecification(typeName);
            restOfArray = asArray.ToArray();
            return spec;
        }

        private static INakedObject RestoreCollection(CollectionMemento memento) {
            return memento.RecoverCollection();
        }

        private static INakedObject RestoreInline(this INakedObjectsFramework framework, AggregateOid aggregateOid) {
            IOid parentOid = aggregateOid.ParentOid;
            INakedObject parent = framework.RestoreObject(parentOid);
            INakedObjectAssociation assoc = parent.Specification.Properties.Where((p => p.Id == aggregateOid.FieldName)).Single();

            return assoc.GetNakedObject(parent);
        }

        private static INakedObject RestoreViewModel(this INakedObjectsFramework framework, ViewModelOid viewModelOid) {
            return framework.LifecycleManager.GetAdapterFor(viewModelOid) ?? framework.LifecycleManager.GetViewModel(viewModelOid);
        }

        public static INakedObject RestoreObject(this INakedObjectsFramework framework, IOid oid) {
            if (oid.IsTransient) {
                return framework.LifecycleManager.RecreateInstance(oid, oid.Specification);
            }
            return framework.Persistor.LoadObject(oid, oid.Specification);
        }

        public static INakedObject GetNakedObject(this INakedObjectsFramework framework, object domainObject) {
            return framework.LifecycleManager.CreateAdapter(domainObject, null, null);
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
            return framework.Services.GetServicesWithVisibleActions(ServiceTypes.Menu | ServiceTypes.Contributor, framework.LifecycleManager).Where(x => framework.GetActions(x).Any()).Select(x => x.Object);
        }

        public static IEnumerable<object> GetServices(this INakedObjectsFramework framework) {
            framework.GetAllServices();
            return framework.Services.GetServicesWithVisibleActions(ServiceTypes.Menu, framework.LifecycleManager).Where(x => framework.GetActions(x).Any()).Select(x => x.Object);
        }

        public static string GetActionId(INakedObjectAction action) {
            return action == null ? string.Empty : string.Format("{0};{1}", action.OnType.FullName, action.Id);
        }

        public static INakedObjectAction GetActionFromId(this INakedObjectsFramework framework, string actionId) {
            string[] asArray = actionId.Split(';');
            INakedObjectSpecification spec = framework.Metamodel.GetSpecification(asArray.First());
            string id = asArray.Skip(1).First();

            return spec.GetAllActions().Single(a => a.Id == id);
        }

        public static bool IsNotPersistent(this INakedObject nakedObject) {
            return (nakedObject.ResolveState.IsTransient() && nakedObject.Specification.Persistable == PersistableType.ProgramPersistable) ||
                   nakedObject.Specification.Persistable == PersistableType.Transient;
        }

        public static bool IsImage(this INakedObjectSpecification spec, INakedObjectsFramework framework) {
            INakedObjectSpecification imageSpec = framework.Metamodel.GetSpecification(typeof(Image));
            return spec != null && spec.IsOfType(imageSpec);
        }

        private static bool IsFileAttachment(this INakedObjectSpecification spec, INakedObjectsFramework framework) {
            INakedObjectSpecification fileSpec = framework.Metamodel.GetSpecification(typeof(FileAttachment));
            return spec != null && spec.IsOfType(fileSpec);
        }

        public static bool IsFile(this INakedObjectAssociation assoc, INakedObjectsFramework framework) {
            return assoc.Specification.IsFile(framework);
        }

        public static bool IsFile(this INakedObjectSpecification spec, INakedObjectsFramework framework) {
            return spec != null && (spec.IsImage(framework) || spec.IsFileAttachment(framework) || spec.ContainsFacet<IArrayValueFacet<byte>>());
        }

        public static string IconName(INakedObject nakedObject) {
            string name = nakedObject.Specification.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        public static INakedObject Parse(this INakedObjectSpecification spec, string s, INakedObjectsFramework framework) {
            return s == null ? framework.LifecycleManager.CreateAdapter("", null, null) : spec.GetFacet<IParseableFacet>().ParseTextEntry(s, framework.LifecycleManager);
        }

        public static bool IsQueryOnly(this INakedObjectAction action) {
            if (action.ReturnType.IsQueryable) {
                return true;
            }

            return action.ContainsFacet<IQueryOnlyFacet>();
        }

        public static bool IsIdempotent(this INakedObjectAction action) {
            return action.ContainsFacet<IIdempotentFacet>();
        }

        public static bool IsParseableOrCollectionOfParseable(this INakedObjectsFramework framework, INakedObjectSpecification spec) {
            return spec.IsParseable || (spec.IsCollection && spec.GetFacet<ITypeOfFacet>().ValueSpec.IsParseable);
        }

        public static INakedObject GetTypedCollection(this INakedObjectsFramework framework, INakedObjectSpecification spec, IEnumerable collectionValue) {
            INakedObjectSpecification collectionitemSpec = framework.Metamodel.GetSpecification(spec.GetFacet<ITypeOfFacet>().ValueSpec);
            string[] rawCollection = collectionValue.Cast<string>().ToArray();
            object[] objCollection;

            Type instanceType = TypeUtils.GetType(collectionitemSpec.FullName);
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));

            if (collectionitemSpec.IsParseable) {
                objCollection = rawCollection.Select(s => string.IsNullOrEmpty(s) ? null : collectionitemSpec.GetFacet<IParseableFacet>().ParseTextEntry(s, framework.LifecycleManager).Object).ToArray();
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

            return framework.LifecycleManager.CreateAdapter(typedCollection.AsQueryable(), null, null);
        }

        public static bool IsViewModelEditView(this INakedObject target) {
            return target.Specification.ContainsFacet<IViewModelFacet>() && target.Specification.GetFacet<IViewModelFacet>().IsEditView(target);
        }
    }
}