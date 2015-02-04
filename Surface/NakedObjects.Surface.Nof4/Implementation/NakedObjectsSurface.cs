// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Nof4.Context;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Nof4.Wrapper;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Implementation {
    public class NakedObjectsSurface : INakedObjectsSurface {
        private readonly INakedObjectsFramework framework;
        private readonly IOidStrategy oidStrategy;

        public NakedObjectsSurface(IOidStrategy oidStrategy, INakedObjectsFramework framework) {
            oidStrategy.Surface = this;
            this.oidStrategy = oidStrategy;
            this.framework = framework;
            OidStrategyHolder.OidStrategy = oidStrategy;
        }

        #region INakedObjectsSurface Members

        public ObjectContextSurface GetImage(string imageId) {
            return null;
        }

        public void Start() {
            framework.TransactionManager.StartTransaction();
        }

        public void End(bool success) {
            if (success) {
                framework.TransactionManager.EndTransaction();
            }
            else {
                framework.TransactionManager.AbortTransaction();
            }
        }

        public IPrincipal GetUser() {
            return MapErrors(() => framework.Session.Principal);
        }

        public INakedObjectSpecificationSurface[] GetDomainTypes() {
            return MapErrors(() => framework.MetamodelManager.AllSpecs.
                Where(s => !IsGenericType(s)).
                Select(GetSpecificationWrapper).ToArray());
        }

        public ObjectContextSurface GetService(LinkObjectId serviceName) {
            return MapErrors(() => GetServiceInternal(serviceName).ToObjectContextSurface(this, framework));
        }

        public ListContextSurface GetServices() {
            return MapErrors(() => GetServicesInternal().ToListContextSurface(this, framework));
        }

        public ObjectContextSurface GetObject(INakedObjectSurface nakedObject) {
            return MapErrors(() => GetObjectContext(((NakedObjectWrapper) nakedObject).WrappedNakedObject).ToObjectContextSurface(this, framework));
        }

        public INakedObjectSpecificationSurface GetDomainType(string typeName) {
            return MapErrors(() => GetSpecificationWrapper(GetDomainTypeInternal(typeName)));
        }

        public PropertyTypeContextSurface GetPropertyType(string typeName, string propertyName) {
            return MapErrors(() => {
                Tuple<IAssociationSpec, IObjectSpec> pc = GetPropertyTypeInternal(typeName, propertyName);

                return new PropertyTypeContextSurface {
                    Property = new NakedObjectAssociationWrapper(pc.Item1, this, framework),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2)
                };
            });
        }

        public ActionTypeContextSurface GetActionType(string typeName, string actionName) {
            return MapErrors(() => {
                Tuple<ActionContext, ITypeSpec> pc = GetActionTypeInternal(typeName, actionName);
                return new ActionTypeContextSurface {
                    ActionContext = pc.Item1.ToActionContextSurface(this, framework),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2)
                };
            });
        }

        public ParameterTypeContextSurface GetActionParameterType(string typeName, string actionName, string parmName) {
            return MapErrors(() => {
                var pc = GetActionParameterTypeInternal(typeName, actionName, parmName);

                return new ParameterTypeContextSurface {
                    Action = new NakedObjectActionWrapper(pc.Item1, this, framework, pc.Item4 ?? ""),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2),
                    Parameter = new NakedObjectActionParameterWrapper(pc.Item3, this, framework, pc.Item4 ?? "")
                };
            });
        }

        public ObjectContextSurface Persist(string typeName, ArgumentsContext arguments) {
            return MapErrors(() => CreateObject(typeName, arguments));
        }

        public UserCredentials Validate(string user, string password) {
            return new UserCredentials(user, password, new List<string>());
        }

        public ObjectContextSurface GetObject(LinkObjectId oid) {
            return MapErrors(() => GetObjectInternal(oid).ToObjectContextSurface(this, framework));
        }

        public ObjectContextSurface PutObject(LinkObjectId oid, ArgumentsContext arguments) {
            return MapErrors(() => ChangeObject(GetObjectAsNakedObject(oid), arguments));
        }

        public PropertyContextSurface GetProperty(LinkObjectId oid, string propertyName) {
            return MapErrors(() => GetProperty(GetObjectAsNakedObject(oid), propertyName).ToPropertyContextSurface(this, framework));
        }

        public ListContextSurface GetPropertyCompletions(LinkObjectId objectId, string propertyName, ArgumentsContext arguments) {
            return MapErrors(() => GetPropertyCompletions(GetObjectAsNakedObject(objectId), propertyName, arguments).ToListContextSurface(this, framework));
        }

        public ListContextSurface GetParameterCompletions(LinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments) {
            return MapErrors(() => GetParameterCompletions(GetObjectAsNakedObject(objectId), actionName, parmName, arguments).ToListContextSurface(this, framework));
        }

        public ListContextSurface GetServiceParameterCompletions(LinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments) {
            return MapErrors(() => GetParameterCompletions(GetServiceAsNakedObject(objectId), actionName, parmName, arguments).ToListContextSurface(this, framework));
        }

        public ActionContextSurface GetServiceAction(LinkObjectId serviceName, string actionName) {
            return MapErrors(() => GetAction(actionName, GetServiceAsNakedObject(serviceName)).ToActionContextSurface(this, framework));
        }

        public ActionContextSurface GetObjectAction(LinkObjectId objectId, string actionName) {
            return MapErrors(() => GetAction(actionName, GetObjectAsNakedObject(objectId)).ToActionContextSurface(this, framework));
        }

        public PropertyContextSurface PutProperty(LinkObjectId objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }

        public PropertyContextSurface DeleteProperty(LinkObjectId objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }


        public ActionResultContextSurface ExecuteObjectAction(LinkObjectId objectId, string actionName, ArgumentsContext arguments) {
            return MapErrors(() => {
                ActionContext actionContext = GetInvokeActionOnObject(objectId, actionName);
                return ExecuteAction(actionContext, arguments);
            });
        }

        public ActionResultContextSurface ExecuteServiceAction(LinkObjectId serviceName, string actionName, ArgumentsContext arguments) {
            return MapErrors(() => {
                ActionContext actionContext = GetInvokeActionOnService(serviceName, actionName);
                return ExecuteAction(actionContext, arguments);
            });
        }

        #endregion

        #region Helpers

        private IAssociationSpec GetPropertyInternal(INakedObject nakedObject, string propertyName, bool onlyVisible = true) {
            if (string.IsNullOrWhiteSpace(propertyName)) {
                throw new BadRequestNOSException();
            }

            IEnumerable<IAssociationSpec> propertyQuery = ((IObjectSpec) nakedObject.Spec).Properties;

            if (onlyVisible) {
                propertyQuery = propertyQuery.Where(p => p.IsVisible(nakedObject));
            }

            IAssociationSpec property = propertyQuery.SingleOrDefault(p => p.Id == propertyName);

            if (property == null) {
                throw new PropertyResourceNotFoundNOSException(propertyName);
            }

            return property;
        }


        private PropertyContext GetProperty(INakedObject nakedObject, string propertyName, bool onlyVisible = true) {
            IAssociationSpec property = GetPropertyInternal(nakedObject, propertyName, onlyVisible);
            return new PropertyContext {Target = nakedObject, Property = property};
        }

        private ListContext GetServicesInternal() {
            INakedObject[] services = framework.ServicesManager.GetServicesWithVisibleActions(ServiceType.Menu | ServiceType.Contributor, framework.LifecycleManager);
            var elementType = (IObjectSpec) framework.MetamodelManager.GetSpecification(typeof (object));

            return new ListContext {
                ElementType = elementType,
                List = services,
                IsListOfServices = true
            };
        }

        private ListContext GetCompletions(PropParmAdapter propParm, INakedObject nakedObject, ArgumentsContext arguments) {
            INakedObject[] list = propParm.GetList(nakedObject, arguments);

            return new ListContext {
                ElementType = propParm.Specification,
                List = list,
                IsListOfServices = false
            };
        }

        private ListContext GetPropertyCompletions(INakedObject nakedObject, string propertyName, ArgumentsContext arguments) {
            var property = GetPropertyInternal(nakedObject, propertyName) as IOneToOneAssociationSpec;
            return GetCompletions(new PropParmAdapter(property, this, framework), nakedObject, arguments);
        }

        private ListContext GetParameterCompletions(INakedObject nakedObject, string actionName, string parmName, ArgumentsContext arguments) {
            IActionParameterSpec parm = GetParameterInternal(actionName, parmName, nakedObject);
            return GetCompletions(new PropParmAdapter(parm, this, framework), nakedObject, arguments);
        }

        private Tuple<IAssociationSpec, IObjectSpec> GetPropertyTypeInternal(string typeName, string propertyName) {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(propertyName)) {
                throw new BadRequestNOSException();
            }

            var spec = (IObjectSpec) GetDomainTypeInternal(typeName);

            IAssociationSpec property = spec.Properties.SingleOrDefault(p => p.Id == propertyName);

            if (property == null) {
                throw new TypePropertyResourceNotFoundNOSException(propertyName, typeName);
            }

            return new Tuple<IAssociationSpec, IObjectSpec>(property, spec);
        }

        private PropertyContext CanChangeProperty(INakedObject nakedObject, string propertyName, object toPut = null) {
            PropertyContext context = GetProperty(nakedObject, propertyName);
            context.ProposedValue = toPut;
            var property = (IOneToOneAssociationSpec) context.Property;

            if (ConsentHandler(IsCurrentlyMutable(context.Target), context, Cause.Immutable)) {
                if (ConsentHandler(property.IsUsable(context.Target), context, Cause.Disabled)) {
                    if (toPut != null && ConsentHandler(CanSetPropertyValue(context), context, Cause.WrongType)) {
                        ConsentHandler(property.IsAssociationValid(context.Target, context.ProposedNakedObject), context, Cause.Other);
                    }
                }
            }

            return context;
        }

        private PropertyContext CanSetProperty(INakedObject nakedObject, string propertyName, object toPut = null) {
            PropertyContext context = GetProperty(nakedObject, propertyName, false);
            context.ProposedValue = toPut;
            var property = (IOneToOneAssociationSpec) context.Property;

            //if (ConsentHandler(IsCurrentlyMutable(context.Target), context, Cause.Immutable)) {
            if (toPut != null && ConsentHandler(CanSetPropertyValue(context), context, Cause.WrongType)) {
                ConsentHandler(property.IsAssociationValid(context.Target, context.ProposedNakedObject), context, Cause.Other);
            }
            else if (toPut == null && (property.IsMandatory && property.IsUsable(context.Target).IsAllowed)) {
                // only check user editable fields
                context.Reason = "Mandatory";
                context.ErrorCause = Cause.Other;
            }
            //}

            return context;
        }

        private IConsent CrossValidate(ObjectContext context) {
            var validateFacet = context.Specification.GetFacet<IValidateObjectFacet>();

            if (validateFacet != null) {
                var allParms = context.VisibleProperties.Select(pc => new Tuple<string, INakedObject>(pc.Id.ToLower(), pc.ProposedNakedObject)).ToArray();

                string result = validateFacet.ValidateParms(context.Target, allParms);
                if (!string.IsNullOrEmpty(result)) {
                    return new Veto(result);
                }
            }

            if (context.Specification.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                string state = context.Target.ValidToPersist();
                if (state != null) {
                    return new Veto(state);
                }
            }
            return new Allow();
        }


        private PropertyContextSurface ChangeProperty(INakedObject nakedObject, string propertyName, ArgumentContext argument) {
            ValidateConcurrency(nakedObject, argument.Digest);
            PropertyContext context = CanChangeProperty(nakedObject, propertyName, argument.Value);
            if (string.IsNullOrEmpty(context.Reason)) {
                var spec = context.Target.Spec as IObjectSpec;
                Trace.Assert(spec != null);

                IEnumerable<PropertyContext> existingValues = spec.Properties.Where(p => p.Id != context.Id).
                    Select(p => new {p, no = p.GetNakedObject(context.Target)}).
                    Select(ao => new PropertyContext {
                        Property = ao.p,
                        ProposedNakedObject = ao.no,
                        ProposedValue = ao.no == null ? null : ao.no.Object,
                        Target = context.Target
                    }
                    ).Union(new[] {context});

                var objectContext = new ObjectContext(context.Target) {VisibleProperties = existingValues.ToArray()};

                if (ConsentHandler(CrossValidate(objectContext), objectContext, Cause.Other)) {
                    if (!argument.ValidateOnly) {
                        SetProperty(context);
                    }
                }
                else {
                    context.Reason = objectContext.Reason;
                    context.ErrorCause = objectContext.ErrorCause;
                }
            }
            context.Mutated = true; // mark as changed even if property not actually changed to stop self rep
            return context.ToPropertyContextSurface(this, framework);
        }

        private void SetProperty(PropertyContext context) {
            ((IOneToOneAssociationSpec) context.Property).SetAssociation(context.Target, context.ProposedValue == null ? null : context.ProposedNakedObject);
        }

        private static void ValidateConcurrency(INakedObject nakedObject, string digest) {
            if (!string.IsNullOrEmpty(digest) && new VersionWrapper(nakedObject.Version).IsDifferent(digest)) {
                throw new PreconditionFailedNOSException();
            }
        }

        private ObjectContextSurface ChangeObject(INakedObject nakedObject, ArgumentsContext arguments) {
            ValidateConcurrency(nakedObject, arguments.Digest);

            Dictionary<string, PropertyContext> contexts;
            try {
                contexts = arguments.Values.ToDictionary(kvp => kvp.Key, kvp => CanChangeProperty(nakedObject, kvp.Key, kvp.Value));
            }
            catch (PropertyResourceNotFoundNOSException e) {
                // no matching property for argument - consider this a syntax error 
                throw new BadRequestNOSException(e.Message);
            }


            var objectContext = new ObjectContext(contexts.First().Value.Target) {VisibleProperties = contexts.Values.ToArray()};

            // if we fail we need to display passed in properties - if OK all visible
            PropertyContext[] propertiesToDisplay = objectContext.VisibleProperties;

            if (contexts.Values.All(c => string.IsNullOrEmpty(c.Reason))) {
                if (ConsentHandler(CrossValidate(objectContext), objectContext, Cause.Other)) {
                    if (!arguments.ValidateOnly) {
                        Array.ForEach(objectContext.VisibleProperties, SetProperty);
                    }

                    propertiesToDisplay = ((IObjectSpec)nakedObject.Spec).Properties.
                        Where(p => p.IsVisible(nakedObject)).
                        Select(p => new PropertyContext {Target = nakedObject, Property = p}).ToArray();
                }
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Mutated = true;
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextSurface(this, framework);
        }

        private ObjectContextSurface SetObject(INakedObject nakedObject, ArgumentsContext arguments) {
            if (((IObjectSpec) nakedObject.Spec).Properties.OfType<IOneToOneAssociationSpec>().Any(p => !arguments.Values.Keys.Contains(p.Id))) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            Dictionary<string, PropertyContext> contexts = arguments.Values.ToDictionary(kvp => kvp.Key, kvp => CanSetProperty(nakedObject, kvp.Key, kvp.Value));
            var objectContext = new ObjectContext(contexts.First().Value.Target) {VisibleProperties = contexts.Values.ToArray()};

            // if we fail we need to display all - if OK only those that are visible 
            PropertyContext[] propertiesToDisplay = objectContext.VisibleProperties;

            if (contexts.Values.All(c => string.IsNullOrEmpty(c.Reason))) {
                if (ConsentHandler(CrossValidate(objectContext), objectContext, Cause.Other)) {
                    if (!arguments.ValidateOnly) {
                        Array.ForEach(objectContext.VisibleProperties, SetProperty);

                        if (nakedObject.Spec.Persistable == PersistableType.UserPersistable) {
                            framework.LifecycleManager.MakePersistent(nakedObject);
                        }
                        else {
                            framework.Persistor.ObjectChanged(nakedObject, framework.LifecycleManager, framework.MetamodelManager);
                        }
                        propertiesToDisplay = ((IObjectSpec)nakedObject.Spec).Properties.
                            Where(p => p.IsVisible(nakedObject)).
                            Select(p => new PropertyContext {Target = nakedObject, Property = p}).ToArray();
                    }
                }
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextSurface(this, framework);
        }

        private bool ValidateParameters(ActionContext actionContext, IDictionary<string, object> rawParms) {
            if (rawParms.Any(kvp => !actionContext.Action.Parameters.Select(p => p.Id).Contains(kvp.Key))) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            bool isValid = true;
            var orderedParms = new Dictionary<string, ParameterContext>();

            // handle contributed actions 

            if (actionContext.Action.IsContributedMethod && !actionContext.Action.OnSpec.Equals(actionContext.Target.Spec)) {
                IActionParameterSpec parm = actionContext.Action.Parameters.FirstOrDefault(p => actionContext.Target.Spec.IsOfType(p.Spec));

                if (parm != null) {
                    rawParms.Add(parm.Id, actionContext.Target.Object);
                }
            }

            // check mandatory fields first as standard NO behaviour is that no validation takes place until 
            // all mandatory fields are set. 
            foreach (IActionParameterSpec parm in actionContext.Action.Parameters) {
                orderedParms[parm.Id] = new ParameterContext();

                object value = rawParms.ContainsKey(parm.Id) ? rawParms[parm.Id] : null;

                orderedParms[parm.Id].ProposedValue = value;
                orderedParms[parm.Id].Parameter = parm;
                orderedParms[parm.Id].Action = actionContext.Action;

                var stringValue = value as string;

                if (parm.IsMandatory && (value == null || (value is string && string.IsNullOrEmpty(stringValue)))) {
                    isValid = false;
                    orderedParms[parm.Id].Reason = "Mandatory"; // i18n
                }
            }

            //check for individual parameter validity, including parsing of text input
            if (isValid) {
                foreach (IActionParameterSpec parm in actionContext.Action.Parameters) {
                    try {
                        INakedObject valueNakedObject = GetValue(parm.Spec, rawParms.ContainsKey(parm.Id) ? rawParms[parm.Id] : null);

                        orderedParms[parm.Id].ProposedNakedObject = valueNakedObject;

                        IConsent consent = parm.IsValid(actionContext.Target, valueNakedObject);
                        if (!consent.IsAllowed) {
                            orderedParms[parm.Id].Reason = consent.Reason;
                            isValid = false;
                        }
                    }
                    catch (InvalidEntryException) {
                        isValid = false;
                        orderedParms[parm.Id].ErrorCause = Cause.WrongType;
                        orderedParms[parm.Id].Reason = "Invalid Entry"; // i18n 
                    }
                }
            }

            // check for validity of whole set, including any 'co-validation' involving multiple parameters
            if (isValid) {
                IConsent consent = actionContext.Action.IsParameterSetValid(actionContext.Target, orderedParms.Select(kvp => kvp.Value.ProposedNakedObject).ToArray());
                if (!consent.IsAllowed) {
                    actionContext.Reason = consent.Reason;
                    isValid = false;
                }
            }

            actionContext.VisibleParameters = orderedParms.Select(p => p.Value).ToArray();

            return isValid;
        }


        private bool ConsentHandler(IConsent consent, Context.Context context, Cause cause) {
            if (consent.IsVetoed) {
                context.Reason = consent.Reason;
                context.ErrorCause = cause;
                return false;
            }
            return true;
        }

        private ActionResultContextSurface ExecuteAction(ActionContext actionContext, ArgumentsContext arguments) {
            ValidateConcurrency(actionContext.Target, arguments.Digest);

            var actionResultContext = new ActionResultContext {Target = actionContext.Target, ActionContext = actionContext};
            if (ConsentHandler(actionContext.Action.IsUsable(actionContext.Target), actionResultContext, Cause.Disabled)) {
                if (ValidateParameters(actionContext, arguments.Values) && !arguments.ValidateOnly) {
                    INakedObject result = actionContext.Action.Execute(actionContext.Target, actionContext.VisibleParameters.Select(p => p.ProposedNakedObject).ToArray());
                    actionResultContext.Result = GetObjectContext(result);
                }
            }
            return actionResultContext.ToActionResultContextSurface(this, framework);
        }

        // TODO either move this into framework or (better?) add a VetoCause enum to Veto and use  
        private static IConsent IsCurrentlyMutable(INakedObject target) {
            bool isPersistent = target.ResolveState.IsPersistent();

            var immutableFacet = target.Spec.GetFacet<IImmutableFacet>();
            if (immutableFacet != null) {
                WhenTo when = immutableFacet.Value;
                if (when == WhenTo.UntilPersisted && !isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldDisabledUntil);
                }
                if (when == WhenTo.OncePersisted && isPersistent) {
                    return new Veto(Resources.NakedObjects.FieldDisabledOnce);
                }
                ITypeSpec tgtSpec = target.Spec;
                if (tgtSpec.IsAlwaysImmutable() || (tgtSpec.IsImmutableOncePersisted() && isPersistent)) {
                    return new Veto(Resources.NakedObjects.FieldDisabled);
                }
            }
            return new Allow();
        }


        private INakedObject GetValue(IObjectSpec specification, object rawValue) {
            if (rawValue == null) {
                return null;
            }

            if (specification.IsParseable) {
                return specification.GetFacet<IParseableFacet>().ParseTextEntry(rawValue.ToString(), framework.NakedObjectManager);
            }

            var no = framework.NakedObjectManager.CreateAdapter(rawValue, null, null);

            // the rawValue is not necessarily a collection so need extra check here to avoid 
            // a potential error getting the element spec. 
            if (specification.IsCollection && (no.Spec.IsCollection && !no.Spec.IsParseable)) {
                var elementSpec = specification.GetFacet<ITypeOfFacet>().GetValueSpec(no, framework.MetamodelManager.Metamodel);

                if (elementSpec.IsParseable) {
                    var elements = ((IEnumerable) rawValue).Cast<object>().Select(e => elementSpec.GetFacet<IParseableFacet>().ParseTextEntry(e.ToString(), framework.NakedObjectManager)).ToArray();
                    var elementType = TypeUtils.GetType(elementSpec.FullName);
                    Type collType = typeof (List<>).MakeGenericType(elementType);
                    var collection = framework.NakedObjectManager.CreateAdapter(Activator.CreateInstance(collType), null, null);
                    collection.Spec.GetFacet<ICollectionFacet>().Init(collection, elements);
                    return collection;
                }
            }


            return no;
        }

        private IConsent CanSetPropertyValue(PropertyContext context) {
            try {
                context.ProposedNakedObject = GetValue((IObjectSpec) context.Specification, context.ProposedValue);
                return new Allow();
            }
            catch (InvalidEntryException e) {
                return new Veto(e.Message);
            }
        }

        private static T MapErrors<T>(Func<T> f) {
            try {
                return f();
            }
            catch (NakedObjectsSurfaceException) {
                throw;
            }
            catch (Exception e) {
                throw SurfaceUtils.Map(e);
            }
        }

        private INakedObject GetObjectAsNakedObject(LinkObjectId objectId) {
            object obj = oidStrategy.GetDomainObjectByOid(objectId);
            return framework.NakedObjectManager.CreateAdapter(obj, null, null);
        }


        private INakedObject GetServiceAsNakedObject(LinkObjectId serviceName) {
            object obj = oidStrategy.GetServiceByServiceName(serviceName);
            return framework.NakedObjectManager.CreateAdapter(obj, null, null);
        }

        private ParameterContext[] FilterParmsForContributedActions(IActionSpec action, ITypeSpec targetSpec, string uid) {
            IActionParameterSpec[] parms;
            if (action.IsContributedMethod && !action.OnSpec.Equals(targetSpec)) {
                var tempParms = new List<IActionParameterSpec>();

                bool skipped = false;
                foreach (IActionParameterSpec parameter in action.Parameters) {
                    // skip the first parm that matches the target. 
                    if (targetSpec.IsOfType(parameter.Spec) && !skipped) {
                        skipped = true;
                    }
                    else {
                        tempParms.Add(parameter);
                    }
                }

                parms = tempParms.ToArray();
            }
            else {
                parms = action.Parameters;
            }
            return parms.Select(p => new ParameterContext {
                Action = action,
                Parameter = p,
                OverloadedUniqueId = uid
            }).ToArray();
        }


        private Tuple<IActionSpec, string> GetActionInternal(string actionName, INakedObject nakedObject) {
            if (string.IsNullOrWhiteSpace(actionName)) {
                throw new BadRequestNOSException();
            }

            IActionSpec[] actions = nakedObject.Spec.GetActionLeafNodes().Where(p => p.IsVisible(nakedObject)).ToArray();
            IActionSpec action = actions.SingleOrDefault(p => p.Id == actionName) ?? SurfaceUtils.GetOverloadedAction(actionName, nakedObject.Spec);

            if (action == null) {
                throw new ActionResourceNotFoundNOSException(actionName);
            }

            return new Tuple<IActionSpec, string>(action, SurfaceUtils.GetOverloadedUId(action, nakedObject.Spec));
        }


        private IActionParameterSpec GetParameterInternal(string actionName, string parmName, INakedObject nakedObject) {
            var actionAndUid = GetActionInternal(actionName, nakedObject);

            if (string.IsNullOrWhiteSpace(parmName) || string.IsNullOrWhiteSpace(parmName)) {
                throw new BadRequestNOSException();
            }
            IActionParameterSpec parm = actionAndUid.Item1.Parameters.SingleOrDefault(p => p.Id == parmName);

            if (parm == null) {
                // throw something;
            }

            return parm;
        }


        private ActionContext GetAction(string actionName, INakedObject nakedObject) {
            var actionAndUid = GetActionInternal(actionName, nakedObject);
            return new ActionContext {
                Target = nakedObject,
                Action = actionAndUid.Item1,
                VisibleParameters = FilterParmsForContributedActions(actionAndUid.Item1, nakedObject.Spec, actionAndUid.Item2),
                OverloadedUniqueId = actionAndUid.Item2
            };
        }


        private Tuple<ActionContext, ITypeSpec> GetActionTypeInternal(string typeName, string actionName) {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(actionName)) {
                throw new BadRequestNOSException();
            }

            ITypeSpec spec = GetDomainTypeInternal(typeName);
            var actionAndUid = SurfaceUtils.GetActionandUidFromSpec(spec, actionName, typeName);

            var actionContext = new ActionContext {
                Action = actionAndUid.Item1,
                VisibleParameters = FilterParmsForContributedActions(actionAndUid.Item1, spec, actionAndUid.Item2),
                OverloadedUniqueId = actionAndUid.Item2

            };

            return new Tuple<ActionContext, ITypeSpec>(actionContext, spec);
        }

        private Tuple<IActionSpec, ITypeSpec, IActionParameterSpec, string> GetActionParameterTypeInternal(string typeName, string actionName, string parmName) {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(actionName) || string.IsNullOrWhiteSpace(parmName)) {
                throw new BadRequestNOSException();
            }

            ITypeSpec spec = GetDomainTypeInternal(typeName);
            Tuple<IActionSpec, string> actionAndUid = SurfaceUtils.GetActionandUidFromSpec(spec, actionName, typeName);

            IActionParameterSpec parm = actionAndUid.Item1.Parameters.SingleOrDefault(p => p.Id == parmName);

            if (parm == null) {
                throw new TypeActionParameterResourceNotFoundNOSException(parmName, actionName, typeName);
            }

            return new Tuple<IActionSpec, ITypeSpec, IActionParameterSpec, string>(actionAndUid.Item1, spec, parm, actionAndUid.Item2);
        }


        private ActionContext GetInvokeActionOnObject(LinkObjectId objectId, string actionName) {
            INakedObject nakedObject = GetObjectAsNakedObject(objectId);
            return GetAction(actionName, nakedObject);
        }

        private ActionContext GetInvokeActionOnService(LinkObjectId serviceName, string actionName) {
            INakedObject nakedObject = GetServiceAsNakedObject(serviceName);
            return GetAction(actionName, nakedObject);
        }


        private ObjectContext GetObjectContext(INakedObject nakedObject) {
            if (nakedObject == null) {
                return null;
            }

            IActionSpec[] actions = nakedObject.Spec.GetActionLeafNodes().Where(p => p.IsVisible(nakedObject)).ToArray();
            var objectSpec = nakedObject.Spec as IObjectSpec;
            IAssociationSpec[] properties = objectSpec == null ? new IAssociationSpec[] { } : objectSpec.Properties.Where(p => p.IsVisible(nakedObject)).ToArray();

            return new ObjectContext(nakedObject) {
                VisibleActions = actions.Select(a => new {action = a, uid = SurfaceUtils.GetOverloadedUId(a, nakedObject.Spec)}).Select(a => new ActionContext {
                    Action = a.action,
                    Target = nakedObject,
                    VisibleParameters = FilterParmsForContributedActions(a.action, nakedObject.Spec, a.uid),
                    OverloadedUniqueId = a.uid
                }).ToArray(),
                VisibleProperties = properties.Select(p => new PropertyContext {
                    Property = p,
                    Target = nakedObject
                }).ToArray()
            };
        }


        private ObjectContext GetObjectInternal(LinkObjectId oid) {
            INakedObject nakedObject = GetObjectAsNakedObject(oid);
            return GetObjectContext(nakedObject);
        }

        private ObjectContext GetServiceInternal(LinkObjectId serviceName) {
            INakedObject nakedObject = GetServiceAsNakedObject(serviceName);
            return GetObjectContext(nakedObject);
        }

        private ITypeSpec GetDomainTypeInternal(string domainTypeId) {
            try {
                var spec = (NakedObjectSpecificationWrapper) oidStrategy.GetSpecificationByLinkDomainType(domainTypeId);
                return spec.WrappedValue;
            }
            catch (Exception) {
                throw new TypeResourceNotFoundNOSException(domainTypeId);
            }
        }


        private ObjectContextSurface CreateObject(string typeName, ArgumentsContext arguments) {
            if (string.IsNullOrWhiteSpace(typeName)) {
                throw new BadRequestNOSException();
            }

            var spec = (IObjectSpec) GetDomainTypeInternal(typeName);
            INakedObject nakedObject = framework.LifecycleManager.CreateInstance(spec);

            return SetObject(nakedObject, arguments);
        }


        private INakedObjectSpecificationSurface GetSpecificationWrapper(ITypeSpec spec) {
            return new NakedObjectSpecificationWrapper(spec, this, framework);
        }

        private static bool IsGenericType(ITypeSpec spec) {
            Type type = TypeUtils.GetType(spec.FullName);

            if (type != null) {
                return type.IsGenericType;
            }

            return false;
        }

        private class PropParmAdapter {
            private readonly INakedObjectsFramework framework;
            private readonly IActionParameterSpec parm;
            private readonly IOneToOneAssociationSpec prop;
            private readonly INakedObjectsSurface surface;

            private PropParmAdapter(object p, INakedObjectsSurface surface, INakedObjectsFramework framework) {
                this.surface = surface;
                this.framework = framework;
                if (p == null) {
                    throw new BadRequestNOSException();
                }
            }

            public PropParmAdapter(IOneToOneAssociationSpec prop, INakedObjectsSurface surface, INakedObjectsFramework framework)
                : this((object) prop, surface, framework) {
                this.prop = prop;
                CheckAutocompleOrConditional();
            }

            public PropParmAdapter(IActionParameterSpec parm, INakedObjectsSurface surface, INakedObjectsFramework framework)
                : this((object) parm, surface, framework) {
                this.parm = parm;
                CheckAutocompleOrConditional();
            }

            private bool IsAutoCompleteEnabled {
                get { return prop == null ? parm.IsAutoCompleteEnabled : prop.IsAutoCompleteEnabled; }
            }

            public IObjectSpec Specification {
                get { return prop == null ? parm.Spec : prop.ReturnSpec; }
            }

            private Func<Tuple<string, IObjectSpec>[]> GetChoicesParameters {
                get { return prop == null ? (Func<Tuple<string, IObjectSpec>[]>) parm.GetChoicesParameters : prop.GetChoicesParameters; }
            }

            private Func<INakedObject, IDictionary<string, INakedObject>, INakedObject[]> GetChoices {
                get { return prop == null ? (Func<INakedObject, IDictionary<string, INakedObject>, INakedObject[]>) parm.GetChoices : prop.GetChoices; }
            }

            private Func<INakedObject, string, INakedObject[]> GetCompletions {
                get { return prop == null ? (Func<INakedObject, string, INakedObject[]>) parm.GetCompletions : prop.GetCompletions; }
            }

            private void CheckAutocompleOrConditional() {
                if (!(IsAutoCompleteEnabled || GetChoicesParameters().Any())) {
                    throw new BadRequestNOSException();
                }
            }

            public INakedObject[] GetList(INakedObject nakedObject, ArgumentsContext arguments) {
                return IsAutoCompleteEnabled ? GetAutocompleteList(nakedObject, arguments) : GetConditionalList(nakedObject, arguments);
            }

            private INakedObjectSpecificationSurface GetSpecificationWrapper(IObjectSpec spec) {
                return new NakedObjectSpecificationWrapper(spec, surface, framework);
            }

            private INakedObject[] GetConditionalList(INakedObject nakedObject, ArgumentsContext arguments) {
                Tuple<string, IObjectSpec>[] expectedParms = GetChoicesParameters();
                IDictionary<string, object> actualParms = arguments.Values;

                string[] expectedParmNames = expectedParms.Select(t => t.Item1).ToArray();
                string[] actualParmNames = actualParms.Keys.ToArray();

                if (expectedParmNames.Count() < actualParmNames.Count()) {
                    throw new BadRequestNOSException("Wrong number of conditional arguments");
                }

                if (!actualParmNames.All(expectedParmNames.Contains)) {
                    throw new BadRequestNOSException("Unrecognised conditional argument(s)");
                }

                Func<Tuple<string, IObjectSpec>, object> getValue = ep => {
                    if (actualParms.ContainsKey(ep.Item1)) {
                        return actualParms[ep.Item1];
                    }
                    return ep.Item2.IsParseable ? "" : null;
                };


                var matchedParms = expectedParms.ToDictionary(ep => ep.Item1, ep => new {
                    expectedType = ep.Item2,
                    value = getValue(ep),
                    actualType = getValue(ep) == null ? null : framework.MetamodelManager.GetSpecification(getValue(ep).GetType())
                });

                var errors = new List<ContextSurface>();

                var mappedArguments = new Dictionary<string, INakedObject>();

                foreach (var ep in expectedParms) {
                    string key = ep.Item1;
                    var mp = matchedParms[key];
                    object value = mp.value;
                    IObjectSpec expectedType = mp.expectedType;
                    ITypeSpec actualType = mp.actualType;

                    if (expectedType.IsParseable && actualType.IsParseable) {
                        string rawValue = value.ToString();

                        try {
                            mappedArguments[key] = expectedType.GetFacet<IParseableFacet>().ParseTextEntry(rawValue, framework.NakedObjectManager);

                            errors.Add(new ChoiceContextSurface(key, GetSpecificationWrapper(expectedType)) {
                                ProposedValue = rawValue
                            });
                        }
                        catch (Exception e) {
                            errors.Add(new ChoiceContextSurface(key, GetSpecificationWrapper(expectedType)) {
                                Reason = e.Message,
                                ProposedValue = rawValue
                            });
                        }
                    }
                    else if (actualType != null && !actualType.IsOfType(expectedType)) {
                        errors.Add(new ChoiceContextSurface(key, GetSpecificationWrapper(expectedType)) {
                            Reason = string.Format("Argument is of wrong type is {0} expect {1}", actualType.FullName, expectedType.FullName),
                            ProposedValue = actualParms[ep.Item1]
                        });
                    }
                    else {
                        mappedArguments[key] = framework.NakedObjectManager.CreateAdapter(value, null, null);

                        errors.Add(new ChoiceContextSurface(key, GetSpecificationWrapper(expectedType)) {
                            ProposedValue = getValue(ep)
                        });
                    }
                }

                if (errors.Any(e => !string.IsNullOrEmpty(e.Reason))) {
                    throw new BadRequestNOSException("Wrong type of conditional argument(s)", errors);
                }

                return GetChoices(nakedObject, mappedArguments);
            }

            private INakedObject[] GetAutocompleteList(INakedObject nakedObject, ArgumentsContext arguments) {
                if (arguments.SearchTerm == null) {
                    throw new BadRequestNOSException("Missing or malformed search term");
                }
                return GetCompletions(nakedObject, arguments.SearchTerm);
            }
        }

        #endregion
    }
}