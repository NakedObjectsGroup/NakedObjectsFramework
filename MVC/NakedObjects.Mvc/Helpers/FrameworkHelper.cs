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
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Service;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Web.Mvc.Html {
    internal static class FrameworkHelper {
        private static ISession CurrentSession {
            get { return NakedObjectsContext.Session; }
        }

        public static IEnumerable<INakedObjectAction> GetActions(INakedObject nakedObject) {
            return nakedObject.Specification.GetObjectActions().OfType<NakedObjectActionImpl>().Cast<INakedObjectAction>().Union(
                        nakedObject.Specification.GetObjectActions().OfType<NakedObjectActionSet>().SelectMany(set => set.Actions)).
                               Where(a => a.IsUsable(CurrentSession, nakedObject).IsAllowed).
                               Where(a => a.IsVisible(CurrentSession, nakedObject));
        }

        public static IEnumerable<INakedObjectAction> GetTopLevelActions(INakedObject nakedObject) {
            return nakedObject.Specification.GetObjectActions().
                               Where(a => a.IsVisible(CurrentSession, nakedObject)).
                               Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible(CurrentSession, nakedObject)));
        }

        public static IEnumerable<INakedObjectAction> GetTopLevelActionsByReturnType(INakedObject nakedObject, INakedObjectSpecification spec) {
            return GetTopLevelActions(nakedObject).
                Where(a => a is NakedObjectActionSet || (IsOfTypeOrCollectionOfType(a.ReturnType, spec) && a.IsFinderMethod)).
                Where(a => !a.Actions.Any() || a.Actions.Any(sa => sa.IsVisible(CurrentSession, nakedObject) && IsOfTypeOrCollectionOfType(sa.ReturnType, spec) && sa.IsFinderMethod));
        }

        public static IEnumerable<INakedObjectAction> GetChildActions(ActionContext actionContext) {
            if (actionContext.Action is NakedObjectActionSet) {
                return actionContext.Action.Actions.
                                     Where(a => a.ActionType == NakedObjectActionType.User).
                                     Where(a => a.IsVisible(CurrentSession, actionContext.Target));
            }

            return new List<INakedObjectAction>();
        }

        public static IEnumerable<INakedObjectAction> GetChildActionsByReturnType(ActionContext actionContext, INakedObjectSpecification spec) {
            return GetChildActions(actionContext).Where(a => IsOfTypeOrCollectionOfType(a.ReturnType, spec)).
                                                  Where(action => action.Parameters.All(parm => parm.Specification.IsParseable || parm.IsChoicesEnabled || parm.Specification.IsOfType(actionContext.Target.Specification)));
        }

        private static bool IsOfTypeOrCollectionOfType(INakedObjectSpecification returnType, INakedObjectSpecification spec) {
            if (returnType.IsOfType(spec)) {
                return true;
            }
            return returnType.IsCollection && returnType.GetFacet<ITypeOfFacet>().ValueSpec.IsOfType(spec);
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

        public static string GetObjectId(INakedObject nakedObject) {
            if (nakedObject.Specification.IsViewModel) {
                PersistorUtils.PopulateViewModelKeys(nakedObject);
            }
            else if (nakedObject.Oid == null) {
                return "";
            }

            return GetObjectId(nakedObject.Oid);
        }


        public static string GetObjectId(IOid oid) {
            return ((IEncodedToStrings) oid).Encode();
        }

        public static string GetObjectId(object model) {
            Assert.AssertFalse("Cannot get Adapter for Adapter", model is INakedObject);
            INakedObject nakedObject = GetNakedObject(model);
            return GetObjectId(nakedObject);
        }

        public static string GetObjectTypeName(object model) {
            INakedObject nakedObject = GetNakedObject(model);
            return nakedObject.Specification.ShortName;
        }

        public static string GetServiceId(string name) {
            INakedObject nakedObject = GetAdaptedService(name);
            return GetObjectId(nakedObject);
        }

        public static string GetInternalServiceId(object service) {
            return ServiceUtils.GetId(service);
        }

        public static object GetObjectFromId(string encodedId) {
            return GetNakedObjectFromId(encodedId).Object;
        }

        public static INakedObject GetNakedObjectFromId(string encodedId) {
            if (string.IsNullOrEmpty(encodedId)) {
                return null;
            }

            IOid oid = NakedObjectsContext.ObjectPersistor.RestoreOid(encodedId.Split(';'));

            if (oid is CollectionMemento) {
                return RestoreCollection(oid as CollectionMemento);
            }

            if (oid is AggregateOid) {
                return RestoreInline(oid as AggregateOid);
            }

            if (oid is ViewModelOid) {
                return RestoreViewModel(oid as ViewModelOid);
            }

            return RestoreObject(oid);
        }

        private static INakedObjectSpecification GetSpecificationFromObjectId(string encodedId, out string[] restOfArray) {
            string[] asArray = encodedId.Split(';');
            return GetSpecificationFromObjectId(asArray, out restOfArray);
        }

        private static INakedObjectSpecification GetSpecificationFromObjectId(string[] asArray, out string[] restOfArray) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(asArray.First()));
            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(typeName);
            restOfArray = asArray.ToArray();
            return spec;
        }

        private static INakedObject RestoreCollection(CollectionMemento memento) {
            return memento.RecoverCollection();
        }

        private static INakedObject RestoreInline(AggregateOid aggregateOid) {
            IOid parentOid = aggregateOid.ParentOid;
            INakedObject parent = RestoreObject(parentOid);
            INakedObjectAssociation assoc = parent.Specification.Properties.Where((p => p.Id == aggregateOid.FieldName)).Single();

            return assoc.GetNakedObject(parent);
        }

        private static INakedObject RestoreViewModel(ViewModelOid viewModelOid) {
            return NakedObjectsContext.ObjectPersistor.GetAdapterFor(viewModelOid) ?? PersistorUtils.GetViewModel(viewModelOid);
        }

        public static INakedObject RestoreObject(IOid oid) {
            if (oid.IsTransient) {
                return PersistorUtils.RecreateInstance(oid, oid.Specification);
            }
            return NakedObjectsContext.ObjectPersistor.LoadObject(oid, oid.Specification);
        }

        public static INakedObject GetNakedObject(object domainObject) {
            return PersistorUtils.CreateAdapter(domainObject);
        }

        public static INakedObject GetAdaptedService(string name) {
            return NakedObjectsContext.ObjectPersistor.GetService(name);
        }

        public static object GetService(string name) {
            return NakedObjectsContext.ObjectPersistor.GetService(name).Object;
        }

        public static T GetService<T>(string name) {
            return NakedObjectsContext.ObjectPersistor.GetService(name).GetDomainObject<T>();
        }

        public static INakedObject GetAdaptedService<T>() {
            return NakedObjectsContext.ObjectPersistor.GetServices().FirstOrDefault(no => no.Object is T);
        }

        public static T GetService<T>() {
            return NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<object> GetAllServices() {
            return NakedObjectsContext.ObjectPersistor.GetServices().Where(x => GetActions(x).Any()).Select(x => x.Object);
        }

        public static IEnumerable<object> GetContributingServices() {
            return NakedObjectsContext.ObjectPersistor.GetServicesWithVisibleActions(ServiceTypes.Menu | ServiceTypes.Contributor).Where(x => GetActions(x).Any()).Select(x => x.Object);
        }

        public static IEnumerable<object> GetServices() {
            GetAllServices();
            return NakedObjectsContext.ObjectPersistor.GetServicesWithVisibleActions(ServiceTypes.Menu).Where(x => GetActions(x).Any()).Select(x => x.Object);
        }

        public static string GetActionId(INakedObjectAction action) {
            return action == null ? string.Empty : string.Format("{0};{1}", action.OnType.FullName, action.Id);
        }

        public static INakedObjectAction GetActionFromId(string actionId) {
            string[] asArray = actionId.Split(';');
            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(asArray.First());
            string id = asArray.Skip(1).First();

            return spec.GetObjectActions().Single(a => a.Id == id);
        }

        public static bool IsNotPersistent(this INakedObject nakedObject) {
            return (nakedObject.ResolveState.IsTransient() && nakedObject.Specification.Persistable == Persistable.PROGRAM_PERSISTABLE) ||
                   nakedObject.Specification.Persistable == Persistable.TRANSIENT;
        }

        public static bool IsImage(this INakedObjectSpecification spec) {
            INakedObjectSpecification imageSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (Image));
            return spec != null && spec.IsOfType(imageSpec);
        }

        private static bool IsFileAttachment(this INakedObjectSpecification spec) {
            INakedObjectSpecification fileSpec = NakedObjectsContext.Reflector.LoadSpecification(typeof (FileAttachment));
            return spec != null && spec.IsOfType(fileSpec);
        }

        public static bool IsFile(this INakedObjectAssociation assoc) {
            return assoc.Specification.IsFile();
        }

        public static bool IsFile(this INakedObjectSpecification spec) {
            return spec != null && (spec.IsImage() || spec.IsFileAttachment() || spec.ContainsFacet<IArrayValueFacet<byte>>());
        }

        public static string IconName(INakedObject nakedObject) {
            string name = nakedObject.Specification.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        public static INakedObject Parse(this INakedObjectSpecification spec, string s) {
            return s == null ? PersistorUtils.CreateAdapter("") : spec.GetFacet<IParseableFacet>().ParseTextEntry(s);
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

        public static bool IsParseableOrCollectionOfParseable(INakedObjectSpecification spec) {
            return spec.IsParseable || (spec.IsCollection && spec.GetFacet<ITypeOfFacet>().ValueSpec.IsParseable);
        }

        public static INakedObject GetTypedCollection(INakedObjectSpecification spec, IEnumerable collectionValue) {
            INakedObjectSpecification collectionitemSpec = spec.GetFacet<ITypeOfFacet>().ValueSpec;
            string[] rawCollection = collectionValue.Cast<string>().ToArray();
            object[] objCollection;

            Type instanceType = TypeUtils.GetType(collectionitemSpec.FullName);
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));

            if (collectionitemSpec.IsParseable) {
                objCollection = rawCollection.Select(s => string.IsNullOrEmpty(s) ? null : collectionitemSpec.GetFacet<IParseableFacet>().ParseTextEntry(s).Object).ToArray();
            }
            else {
                // need to check if collection is actually a collection memento 
                if (rawCollection.Count() == 1) {
                    INakedObject firstObj = GetNakedObjectFromId(rawCollection.First());

                    if (firstObj != null && firstObj.Oid is CollectionMemento) {
                        return firstObj;
                    }
                }

                objCollection = rawCollection.Select(s => GetNakedObjectFromId(s).GetDomainObject()).ToArray();
            }

            objCollection.Where(o => o != null).ForEach(o => typedCollection.Add(o));

            return PersistorUtils.CreateAdapter(typedCollection.AsQueryable());
        }

        public static bool IsViewModelEditView(this INakedObject target) {
            return target.Specification.ContainsFacet<IViewModelFacet>() && target.Specification.GetFacet<IViewModelFacet>().IsEditView(target);
        }
    }
}