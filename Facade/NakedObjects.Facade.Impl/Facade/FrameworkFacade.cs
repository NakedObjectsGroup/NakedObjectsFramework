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
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Impl.Contexts;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Util;

namespace NakedObjects.Facade.Impl {
    public class FrameworkFacade : IFrameworkFacade {
        private readonly INakedObjectsFramework framework;
        private readonly IOidStrategy oidStrategy;

        public FrameworkFacade(IOidStrategy oidStrategy, IOidTranslator oidTranslator, INakedObjectsFramework framework) {
            oidStrategy.FrameworkFacade = this;
            this.oidStrategy = oidStrategy;
            OidTranslator = oidTranslator;
            this.framework = framework;
            MessageBroker = new MessageBrokerWrapper(framework.MessageBroker);
        }

        /// <summary>
        ///  mainly for testing
        /// </summary>
        public INakedObjectsFramework Framework => framework;

        #region IFrameworkFacade Members

        public ObjectContextFacade GetImage(string imageId) {
            return null;
        }

        public void Start() {
            framework.TransactionManager.StartTransaction();
        }

        public void End(bool success) {
            try {
                if (success) {
                    framework.TransactionManager.EndTransaction();
                }
                else {
                    framework.TransactionManager.AbortTransaction();
                }
            }
            catch (DataUpdateException e) {
                throw new DataUpdateNOSException(e);
            }
            catch (ConcurrencyException e) {
                throw new PreconditionFailedNOSException(e.Message, e) {
                    SourceNakedObject = ObjectFacade.Wrap(e.SourceNakedObjectAdapter, this, framework)
                };
            }
        }

        public IPrincipal GetUser() {
            return MapErrors(() => framework.Session.Principal);
        }

        public IOidTranslator OidTranslator { get; }

        public IOidStrategy OidStrategy => oidStrategy;

        public IMessageBrokerFacade MessageBroker { get; }

        public ITypeFacade[] GetDomainTypes() {
            return MapErrors(() => framework.MetamodelManager.AllSpecs.
                Where(s => !IsGenericType(s)).
                Select(GetSpecificationWrapper).ToArray());
        }

        public ObjectContextFacade GetService(IOidTranslation serviceName) {
            return MapErrors(() => GetServiceInternal(serviceName).ToObjectContextFacade(this, framework));
        }

        public ListContextFacade GetServices() {
            return MapErrors(() => GetServicesInternal().ToListContextFacade(this, framework));
        }

        public IMenuFacade[] GetMainMenus() {
            var menus = framework.MetamodelManager.MainMenus() ?? framework.ServicesManager.GetServices().Select(s => s.GetServiceSpec().Menu);
            return menus.Select(m => new MenuFacade(m, this, framework)).Cast<IMenuFacade>().ToArray();
        }

        public ObjectContextFacade GetObject(IObjectFacade objectFacade) {
            return MapErrors(() => GetObjectContext(((ObjectFacade) objectFacade).WrappedNakedObject).ToObjectContextFacade(this, framework));
        }

        public ObjectContextFacade RefreshObject(IObjectFacade objectFacade, ArgumentsContextFacade arguments) {
            return MapErrors(() => RefreshObjectInternal(((ObjectFacade) objectFacade).WrappedNakedObject, arguments).ToObjectContextFacade(this, framework));
        }

        public ITypeFacade GetDomainType(string typeName) {
            return MapErrors(() => GetSpecificationWrapper(GetDomainTypeInternal(typeName)));
        }

        public PropertyTypeContextFacade GetPropertyType(string typeName, string propertyName) {
            return MapErrors(() => {
                Tuple<IAssociationSpec, IObjectSpec> pc = GetPropertyTypeInternal(typeName, propertyName);

                return new PropertyTypeContextFacade {
                    Property = new AssociationFacade(pc.Item1, this, framework),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2)
                };
            });
        }

        public ActionTypeContextFacade GetActionType(string typeName, string actionName) {
            return MapErrors(() => {
                Tuple<ActionContext, ITypeSpec> pc = GetActionTypeInternal(typeName, actionName);
                return new ActionTypeContextFacade {
                    ActionContext = pc.Item1.ToActionContextFacade(this, framework),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2)
                };
            });
        }

        public ParameterTypeContextFacade GetActionParameterType(string typeName, string actionName, string parmName) {
            return MapErrors(() => {
                var pc = GetActionParameterTypeInternal(typeName, actionName, parmName);

                return new ParameterTypeContextFacade {
                    Action = new ActionFacade(pc.Item1, this, framework, pc.Item4 ?? ""),
                    OwningSpecification = GetSpecificationWrapper(pc.Item2),
                    Parameter = new ActionParameterFacade(pc.Item3, this, framework, pc.Item4 ?? "")
                };
            });
        }

        public ObjectContextFacade Persist(string typeName, ArgumentsContextFacade arguments) {
            return MapErrors(() => CreateObject(typeName, arguments));
        }

        public ObjectContextFacade PersistObject(IObjectFacade transient, ArgumentsContextFacade arguments) {
            return MapErrors(() => PersistTransientObject(transient, arguments));
        }

        public UserCredentials Validate(string user, string password) {
            return new UserCredentials(user, password, new List<string>());
        }

        public IObjectFacade GetObject(ITypeFacade spec, object value) {
            var s = ((TypeFacade) spec).WrappedValue;

            if (value == null) {
                return null;
            }

            var text = value as string;
            var adapter = text != null ? s.GetFacet<IParseableFacet>().ParseTextEntry(text, Framework.NakedObjectManager) :
                Framework.GetNakedObject(value);

            return ObjectFacade.Wrap(adapter, this, Framework);
        }

        public IObjectFacade GetObject(object domainObject) {
            return ObjectFacade.Wrap(framework.NakedObjectManager.CreateAdapter(domainObject, null, null), this, framework);
        }

        public ObjectContextFacade GetObject(IOidTranslation oid) {
            return MapErrors(() => GetObjectInternal(oid).ToObjectContextFacade(this, framework));
        }

        public ObjectContextFacade PutObject(IOidTranslation oid, ArgumentsContextFacade arguments) {
            return MapErrors(() => ChangeObject(GetObjectAsNakedObject(oid), arguments));
        }

        public PropertyContextFacade GetProperty(IOidTranslation oid, string propertyName) {
            return MapErrors(() => GetProperty(GetObjectAsNakedObject(oid), propertyName).ToPropertyContextFacade(this, framework));
        }

        public ListContextFacade GetPropertyCompletions(IOidTranslation objectId, string propertyName, ArgumentsContextFacade arguments) {
            return MapErrors(() => GetPropertyCompletions(GetObjectAsNakedObject(objectId), propertyName, arguments).ToListContextFacade(this, framework));
        }

        public ListContextFacade GetParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments) {
            return MapErrors(() => GetParameterCompletions(GetObjectAsNakedObject(objectId), actionName, parmName, arguments).ToListContextFacade(this, framework));
        }

        public ListContextFacade GetServiceParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments) {
            return MapErrors(() => GetParameterCompletions(GetServiceAsNakedObject(objectId), actionName, parmName, arguments).ToListContextFacade(this, framework));
        }

        public ActionContextFacade GetServiceAction(IOidTranslation serviceName, string actionName) {
            return MapErrors(() => GetAction(actionName, GetServiceAsNakedObject(serviceName)).ToActionContextFacade(this, framework));
        }

        public ActionContextFacade GetObjectAction(IOidTranslation objectId, string actionName) {
            return MapErrors(() => GetAction(actionName, GetObjectAsNakedObject(objectId)).ToActionContextFacade(this, framework));
        }

        public PropertyContextFacade PutProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }

        public PropertyContextFacade DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }

        public ActionResultContextFacade ExecuteListAction(IOidTranslation[] list, ITypeFacade elementSpec, string actionName, ArgumentsContextFacade arguments) {
            return MapErrors(() => {
                ActionContext actionContext = GetInvokeActionOnList(list, elementSpec, actionName);
                return ExecuteAction(actionContext, arguments);
            });
        }

        public ActionResultContextFacade ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContextFacade arguments) {
            return MapErrors(() => {
                ActionContext actionContext = GetInvokeActionOnObject(objectId, actionName);
                return ExecuteAction(actionContext, arguments);
            });
        }

        public ActionResultContextFacade ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContextFacade arguments) {
            return MapErrors(() => {
                ActionContext actionContext = GetInvokeActionOnService(serviceName, actionName);
                return ExecuteAction(actionContext, arguments);
            });
        }

        public object Wrap(object arm, IObjectFacade objectFacade) {
            var no = ((ObjectFacade) objectFacade).WrappedNakedObject;
            // var oid = framework.OidStrategy.GetOid(arm);
            var noArm = framework.GetNakedObject(arm);
            var currentMemento = (ICollectionMemento) no.Oid;
            var newMemento = currentMemento.NewSelectionMemento(new object[] {}, false);
            noArm.SetATransientOid(newMemento);

            return noArm.Object;
        }

        #endregion

        #region Helpers

        private IAssociationSpec GetPropertyInternal(INakedObjectAdapter nakedObject, string propertyName, bool onlyVisible = true) {
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

        // todo more hacking remove this id stuff 
        private const string InputName = "Input";
        private const string SelectName = "Select";

        private string InputOrSelect(ITypeSpec spec) {
            return (spec.IsParseable ? InputName : SelectName);
        }

        public string GetObjectId(INakedObjectAdapter owner) {
            string postFix = "";

            if (owner.Spec.IsCollection) {
                var elementFacet = owner.Spec.GetFacet<ITypeOfFacet>();
                var elementType = elementFacet.GetValue(owner);

                postFix = "-" + elementType.Name;
            }

            return owner.Spec.ShortName + postFix;
        }

        public string GetInlineFieldId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return parent.Id + "-" + GetObjectId(owner) + "-" + assoc.Id;
        }

        public string GetFieldId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + "-" + assoc.Id;
        }

        private string GetInlineFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetInlineFieldId(parent, owner, assoc) + "-" + InputOrSelect(assoc.ReturnSpec);
        }

        private string GetFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetFieldId(owner, assoc) + "-" + InputOrSelect(assoc.ReturnSpec);
        }

        private string GetFieldInputId(IAssociationSpec parent, INakedObjectAdapter nakedObject, IAssociationSpec assoc) {
            return parent == null ? GetFieldInputId(nakedObject, assoc) : GetInlineFieldInputId(parent, nakedObject, assoc);
        }

        public string GetCollectionItemId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + "-" + assoc.Id + "-" + "Item";
        }

        // endremove

        private ObjectContext
            RefreshObjectInternal(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments, IAssociationSpec parent = null) {
            var oc = new ObjectContext(nakedObject);

            if (nakedObject.Oid.IsTransient) {
                // use oid to catch transient aggregates 
                foreach (IAssociationSpec assoc in (nakedObject.GetObjectSpec()).Properties.Where(p => !p.IsReadOnly)) {
                    var key = GetFieldInputId(parent, nakedObject, assoc);
                    if (arguments.Values.ContainsKey(key)) {
                        object newValue = ((string[]) arguments.Values[key]).First();

                        if (assoc.ReturnSpec.IsParseable) {
                            try {
                                var oneToOneAssoc = ((IOneToOneAssociationSpec) assoc);
                                INakedObjectAdapter value = assoc.ReturnSpec.GetFacet<IParseableFacet>().ParseTextEntry((string) newValue, framework.NakedObjectManager);
                                oneToOneAssoc.SetAssociation(nakedObject, value);
                            }
                            catch (InvalidEntryException) {
                                //ModelState.AddModelError(name, MvcUi.InvalidEntry);
                                oc.Reason = "Invalid Entry";
                                oc.ErrorCause = Cause.Other;
                            }
                        }
                        else if (assoc is IOneToOneAssociationSpec) {
                            INakedObjectAdapter value = framework.GetNakedObjectFromId((string) newValue);
                            var oneToOneAssoc = ((IOneToOneAssociationSpec) assoc);
                            oneToOneAssoc.SetAssociation(nakedObject, value);
                        }
                    }
                }

                foreach (IOneToManyAssociationSpec assoc in (nakedObject.GetObjectSpec()).Properties.OfType<IOneToManyAssociationSpec>()) {
                    string name = GetCollectionItemId(nakedObject, assoc);

                    if (arguments.Values.ContainsKey(name)) {
                        var items = arguments.Values[name] as string[];

                        if (items != null && assoc.Count(nakedObject) == 0) {
                            var values = items.Select(framework.GetNakedObjectFromId).ToArray();
                            var collection = assoc.GetNakedObject(nakedObject);
                            collection.Spec.GetFacet<ICollectionFacet>().Init(collection, values);
                        }
                    }
                }

                foreach (IAssociationSpec assoc in (nakedObject.GetObjectSpec()).Properties.Where(p => p.IsInline)) {
                    var inlineNakedObject = assoc.GetNakedObject(nakedObject);
                    RefreshObjectInternal(inlineNakedObject, arguments, assoc);
                }
            }

            return oc;
        }

        private PropertyContext GetProperty(INakedObjectAdapter nakedObject, string propertyName, bool onlyVisible = true) {
            IAssociationSpec property = GetPropertyInternal(nakedObject, propertyName, onlyVisible);
            return new PropertyContext {Target = nakedObject, Property = property};
        }

        private ListContext GetServicesInternal() {
            INakedObjectAdapter[] services = framework.ServicesManager.GetServicesWithVisibleActions(framework.LifecycleManager);
            var elementType = (IObjectSpec) framework.MetamodelManager.GetSpecification(typeof (object));

            return new ListContext {
                ElementType = elementType,
                List = services,
                IsListOfServices = true
            };
        }

        private ListContext GetCompletions(PropParmAdapter propParm, INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
            INakedObjectAdapter[] list = propParm.GetList(nakedObject, arguments);

            return new ListContext {
                ElementType = propParm.Specification,
                List = list,
                IsListOfServices = false
            };
        }

        private ListContext GetPropertyCompletions(INakedObjectAdapter nakedObject, string propertyName, ArgumentsContextFacade arguments) {
            var property = GetPropertyInternal(nakedObject, propertyName) as IOneToOneAssociationSpec;
            return GetCompletions(new PropParmAdapter(property, this, framework), nakedObject, arguments);
        }

        private ListContext GetParameterCompletions(INakedObjectAdapter nakedObject, string actionName, string parmName, ArgumentsContextFacade arguments) {
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

        private PropertyContext CanChangeProperty(INakedObjectAdapter nakedObject, string propertyName, object toPut = null) {
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

        private PropertyContext CanSetProperty(INakedObjectAdapter nakedObject, string propertyName, object toPut = null) {
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
                var allParms = context.VisibleProperties.Select(pc => new Tuple<string, INakedObjectAdapter>(pc.Id.ToLower(), pc.ProposedNakedObject)).ToArray();

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

        private PropertyContextFacade ChangeProperty(INakedObjectAdapter nakedObject, string propertyName, ArgumentContextFacade argument) {
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
            return context.ToPropertyContextFacade(this, framework);
        }

        private void SetProperty(PropertyContext context) {
            ((IOneToOneAssociationSpec) context.Property).SetAssociation(context.Target, context.ProposedValue == null ? null : context.ProposedNakedObject);
        }

        private static void ValidateConcurrency(INakedObjectAdapter nakedObject, string digest) {
            if (!string.IsNullOrEmpty(digest) && new VersionFacade(nakedObject.Version).IsDifferent(digest)) {
                throw new PreconditionFailedNOSException();
            }
        }

        private ObjectContextFacade ChangeObject(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
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

                        if (nakedObject.Spec.Persistable == PersistableType.UserPersistable && nakedObject.ResolveState.IsTransient()) {
                            framework.LifecycleManager.MakePersistent(nakedObject);
                        }
                        else {
                            framework.Persistor.ObjectChanged(nakedObject, framework.LifecycleManager, framework.MetamodelManager);
                        }
                    }

                    propertiesToDisplay = ((IObjectSpec) nakedObject.Spec).Properties.
                        Where(p => p.IsVisible(nakedObject)).
                        Select(p => new PropertyContext {Target = nakedObject, Property = p}).ToArray();
                }
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Mutated = true;
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextFacade(this, framework);
        }

        private ObjectContextFacade SetTransientObject(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {

            Dictionary<string, PropertyContext> contexts = arguments.Values.ToDictionary(kvp => kvp.Key, kvp => CanSetProperty(nakedObject, kvp.Key, kvp.Value));
            var objectContext = new ObjectContext(contexts.First().Value.Target) { VisibleProperties = contexts.Values.ToArray() };

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
                            Select(p => new PropertyContext { Target = nakedObject, Property = p }).ToArray();
                    }
                }
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextFacade(this, framework);
        }

        private ObjectContextFacade SetObject(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
            // this is for ProtoPersistents where the arguments must contain all values 
            // for standard transients the object may already have values set so no need to check  
            if (((IObjectSpec) nakedObject.Spec).Properties.OfType<IOneToOneAssociationSpec>().Any(p => !arguments.Values.Keys.Contains(p.Id))) {
                throw new BadRequestNOSException("Malformed arguments");
            }
            return SetTransientObject(nakedObject, arguments);
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

                // todo investigate this rawparms should either contain or not contain  target
                if (parm != null) {
                    rawParms[parm.Id] = actionContext.Target.Object;
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
                        var multiParm = parm as IOneToManyActionParameterSpec;
                        INakedObjectAdapter valueNakedObject = GetValue(parm.Spec, multiParm == null ? null : multiParm.ElementSpec, rawParms.ContainsKey(parm.Id) ? rawParms[parm.Id] : null);

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

        private bool ConsentHandler(IConsent consent, Context context, Cause cause) {
            if (consent.IsVetoed) {
                context.Reason = consent.Reason;
                context.ErrorCause = cause;
                return false;
            }
            return true;
        }

        private ActionResultContextFacade ExecuteAction(ActionContext actionContext, ArgumentsContextFacade arguments) {
            ValidateConcurrency(actionContext.Target, arguments.Digest);

            var actionResultContext = new ActionResultContext {Target = actionContext.Target, ActionContext = actionContext};
            if (ConsentHandler(actionContext.Action.IsUsable(actionContext.Target), actionResultContext, Cause.Disabled)) {
                if (ValidateParameters(actionContext, arguments.Values) && !arguments.ValidateOnly) {
                    INakedObjectAdapter result = actionContext.Action.Execute(actionContext.Target, actionContext.VisibleParameters.Select(p => p.ProposedNakedObject).ToArray());
                    actionResultContext.Result = GetObjectContext(result);
                }
            }
            return actionResultContext.ToActionResultContextFacade(this, framework);
        }

        // TODO either move this into framework or (better?) add a VetoCause enum to Veto and use  
        private static IConsent IsCurrentlyMutable(INakedObjectAdapter target) {
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

        private INakedObjectAdapter GetValue(IObjectSpec specification, IObjectSpec elementSpec, object rawValue) {
            if (rawValue == null) {
                return null;
            }

            var fromStreamFacet = specification.GetFacet<IFromStreamFacet>();
            if (fromStreamFacet != null) {
                var httpPostedFileBase = (HttpPostedFileBase) rawValue;
                return fromStreamFacet.ParseFromStream(httpPostedFileBase.InputStream, httpPostedFileBase.ContentType, httpPostedFileBase.FileName, framework.NakedObjectManager);
            }

            if (specification.IsParseable) {
                return specification.GetFacet<IParseableFacet>().ParseTextEntry(rawValue.ToString(), framework.NakedObjectManager);
            }

            if (elementSpec != null) {
                if (elementSpec.IsParseable) {
                    var elements = ((IEnumerable) rawValue).Cast<object>().Select(e => elementSpec.GetFacet<IParseableFacet>().ParseTextEntry(e.ToString(), framework.NakedObjectManager)).ToArray();
                    var elementType = TypeUtils.GetType(elementSpec.FullName);
                    Type collType = typeof (List<>).MakeGenericType(elementType);
                    var list = ((IList) Activator.CreateInstance(collType)).AsQueryable();
                    var collection = framework.NakedObjectManager.CreateAdapter(list, null, null);
                    collection.Spec.GetFacet<ICollectionFacet>().Init(collection, elements);
                    return collection;
                }
            }

            if (specification.IsQueryable) {
                var rawEnumerable = rawValue as IEnumerable;
                rawValue = rawEnumerable == null ? rawValue : rawEnumerable.AsQueryable();
            }

            return framework.NakedObjectManager.CreateAdapter(rawValue, null, null);
        }

        private IConsent CanSetPropertyValue(PropertyContext context) {
            try {
                var coll = context.Property as IOneToManyAssociationSpec;
                context.ProposedNakedObject = GetValue((IObjectSpec) context.Specification, coll == null ? null : coll.ElementSpec, context.ProposedValue);
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
            catch (NakedObjectsFacadeException) {
                throw;
            }
            catch (Exception e) {
                throw FacadeUtils.Map(e);
            }
        }

        private INakedObjectAdapter GetObjectAsNakedObject(IOidTranslation objectId) {
            var obj = oidStrategy.GetObjectFacadeByOid(objectId);
            return obj.WrappedAdapter();
        }

        private INakedObjectAdapter GetServiceAsNakedObject(IOidTranslation serviceName) {
            object obj = oidStrategy.GetServiceByServiceName(serviceName);
            return framework.NakedObjectManager.GetAdapterFor(obj);
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

        private Tuple<IActionSpec, string> GetActionInternal(string actionName, INakedObjectAdapter nakedObject) {
            if (string.IsNullOrWhiteSpace(actionName)) {
                throw new BadRequestNOSException();
            }

            IActionSpec[] actions = nakedObject.Spec.GetActionLeafNodes().Where(p => p.IsVisible(nakedObject)).ToArray();
            IActionSpec action = actions.SingleOrDefault(p => p.Id == actionName) ?? FacadeUtils.GetOverloadedAction(actionName, nakedObject.Spec);

            // todo tidy this 
            if (action == null) {
                var typeOfFacet = nakedObject.Spec.GetFacet<ITypeOfFacet>();

                if (typeOfFacet != null) {
                    var metamodel = framework.MetamodelManager.Metamodel;
                    var elementSpecImmut = typeOfFacet.GetValueSpec(nakedObject, metamodel);
                    var elementSpec = framework.MetamodelManager.GetSpecification(elementSpecImmut);

                    if (elementSpec != null) {
                        actions = elementSpec.GetCollectionContributedActions().Where(p => p.IsVisible(nakedObject)).ToArray();
                        action = actions.SingleOrDefault(p => p.Id == actionName) ?? FacadeUtils.GetOverloadedAction(actionName, nakedObject.Spec);
                    }
                }
            }

            if (action == null) {
                throw new ActionResourceNotFoundNOSException(actionName);
            }

            return new Tuple<IActionSpec, string>(action, FacadeUtils.GetOverloadedUId(action, nakedObject.Spec));
        }

        private IActionParameterSpec GetParameterInternal(string actionName, string parmName, INakedObjectAdapter nakedObject) {
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

        private ActionContext GetAction(string actionName, INakedObjectAdapter nakedObject) {
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
            var actionAndUid = FacadeUtils.GetActionandUidFromSpec(spec, actionName, typeName);

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
            Tuple<IActionSpec, string> actionAndUid = FacadeUtils.GetActionandUidFromSpec(spec, actionName, typeName);

            IActionParameterSpec parm = actionAndUid.Item1.Parameters.SingleOrDefault(p => p.Id == parmName);

            if (parm == null) {
                throw new TypeActionParameterResourceNotFoundNOSException(parmName, actionName, typeName);
            }

            return new Tuple<IActionSpec, ITypeSpec, IActionParameterSpec, string>(actionAndUid.Item1, spec, parm, actionAndUid.Item2);
        }

        private INakedObjectAdapter MakeTypedCollection(Type instanceType, IEnumerable<object> objects) {
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));
            objects.Where(o => o != null).ForEach(o => typedCollection.Add(o));
            return framework.NakedObjectManager.CreateAdapter(typedCollection.AsQueryable(), null, null);
        }

        private ActionContext GetInvokeActionOnList(IOidTranslation[] list, ITypeFacade elementSpec, string actionName) {
            var domainCollection = list.Select(id => oidStrategy.GetDomainObjectByOid(id));
            var nakedObject = MakeTypedCollection(elementSpec.GetUnderlyingType(), domainCollection);
            return GetAction(actionName, nakedObject);
        }

        private ActionContext GetInvokeActionOnObject(IOidTranslation objectId, string actionName) {
            INakedObjectAdapter nakedObject = GetObjectAsNakedObject(objectId);
            return GetAction(actionName, nakedObject);
        }

        private ActionContext GetInvokeActionOnService(IOidTranslation serviceName, string actionName) {
            INakedObjectAdapter nakedObject = GetServiceAsNakedObject(serviceName);
            return GetAction(actionName, nakedObject);
        }

        private static IActionSpec MatchingActionSpecOnService(IActionSpec actionToMatch) {
            var allServiceActions = actionToMatch.OnSpec.GetActionLeafNodes();

            return allServiceActions.SingleOrDefault(sa => sa.Id == actionToMatch.Id &&
                                                           sa.ParameterCount == actionToMatch.ParameterCount &&
                                                           sa.Parameters.Select(p => p.Spec).SequenceEqual(actionToMatch.Parameters.Select(p => p.Spec)));


        }

        private ObjectContext GetObjectContext(INakedObjectAdapter nakedObject) {
            if (nakedObject == null) {
                return null;
            }

            IActionSpec[] actions = nakedObject.Spec.GetActionLeafNodes().Where(p => p.IsVisible(nakedObject)).ToArray();
            var objectSpec = nakedObject.Spec as IObjectSpec;
            IAssociationSpec[] properties = objectSpec == null ? new IAssociationSpec[] {} : objectSpec.Properties.Where(p => p.IsVisible(nakedObject)).ToArray();

            ActionContext[] ccaContexts = {}; 

            if (nakedObject.Spec.IsQueryable) {
                ITypeOfFacet typeOfFacet = nakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.GetValueSpec(nakedObject, framework.MetamodelManager.Metamodel);
                var elementSpec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                IActionSpec[] cca = elementSpec.GetCollectionContributedActions().Where(p => p.IsVisible(nakedObject)).ToArray();
             

                ccaContexts = cca.Select(a => new { action = a, uid = FacadeUtils.GetOverloadedUId(MatchingActionSpecOnService(a) , a.OnSpec) }).Select(a => new ActionContext {
                    Action = a.action,
                    OverloadedUniqueId = a.uid,
                    Target = framework.ServicesManager.GetService(a.action.OnSpec as IServiceSpec),
                    VisibleParameters = a.action.Parameters.Select(p => new ParameterContext {
                        Action = a.action,
                        Parameter = p,
                        OverloadedUniqueId = a.uid
                    }).ToArray()
                }).ToArray();
            }

            var actionContexts = actions.Select(a => new {action = a, uid = FacadeUtils.GetOverloadedUId(a, nakedObject.Spec)}).Select(a => new ActionContext {
                Action = a.action,
                Target = nakedObject,
                VisibleParameters = FilterParmsForContributedActions(a.action, nakedObject.Spec, a.uid),
                OverloadedUniqueId = a.uid
            });

            return new ObjectContext(nakedObject) {
                VisibleActions = actionContexts.Union(ccaContexts).ToArray(),
                VisibleProperties = properties.Select(p => new PropertyContext {
                    Property = p,
                    Target = nakedObject
                }).ToArray()
            };
        }

        private ObjectContext GetObjectInternal(IOidTranslation oid) {
            INakedObjectAdapter nakedObject = GetObjectAsNakedObject(oid);
            return GetObjectContext(nakedObject);
        }

        private ObjectContext GetServiceInternal(IOidTranslation serviceName) {
            INakedObjectAdapter nakedObject = GetServiceAsNakedObject(serviceName);
            return GetObjectContext(nakedObject);
        }

        private ITypeSpec GetDomainTypeInternal(string domainTypeId) {
            try {
                var spec = (TypeFacade) oidStrategy.GetSpecificationByLinkDomainType(domainTypeId);
                return spec.WrappedValue;
            }
            catch (Exception) {
                throw new TypeResourceNotFoundNOSException(domainTypeId);
            }
        }

        private ObjectContextFacade CreateObject(string typeName, ArgumentsContextFacade arguments) {
            if (string.IsNullOrWhiteSpace(typeName)) {
                throw new BadRequestNOSException();
            }

            var spec = (IObjectSpec) GetDomainTypeInternal(typeName);
            INakedObjectAdapter nakedObject = framework.LifecycleManager.CreateInstance(spec);

            return SetObject(nakedObject, arguments);
        }

        private ObjectContextFacade PersistTransientObject(IObjectFacade transient, ArgumentsContextFacade arguments) {

            INakedObjectAdapter nakedObject = transient.WrappedAdapter();
            return SetTransientObject(nakedObject, arguments);
        }


        private ITypeFacade GetSpecificationWrapper(ITypeSpec spec) {
            return new TypeFacade(spec, this, framework);
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
            private readonly IFrameworkFacade frameworkFacade;

            private PropParmAdapter(object p, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework) {
                this.frameworkFacade = frameworkFacade;
                this.framework = framework;
                if (p == null) {
                    throw new BadRequestNOSException();
                }
            }

            public PropParmAdapter(IOneToOneAssociationSpec prop, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework)
                : this((object) prop, frameworkFacade, framework) {
                this.prop = prop;
                CheckAutocompleOrConditional();
            }

            public PropParmAdapter(IActionParameterSpec parm, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework)
                : this((object) parm, frameworkFacade, framework) {
                this.parm = parm;
                CheckAutocompleOrConditional();
            }

            private bool IsAutoCompleteEnabled => prop == null ? parm.IsAutoCompleteEnabled : prop.IsAutoCompleteEnabled;

            public IObjectSpec Specification => prop == null ? parm.Spec : prop.ReturnSpec;

            private Func<Tuple<string, IObjectSpec>[]> GetChoicesParameters => prop == null ? (Func<Tuple<string, IObjectSpec>[]>) parm.GetChoicesParameters : prop.GetChoicesParameters;

            private Func<INakedObjectAdapter, IDictionary<string, INakedObjectAdapter>, INakedObjectAdapter[]> GetChoices => prop == null ? (Func<INakedObjectAdapter, IDictionary<string, INakedObjectAdapter>, INakedObjectAdapter[]>) parm.GetChoices : prop.GetChoices;

            private Func<INakedObjectAdapter, string, INakedObjectAdapter[]> GetCompletions => prop == null ? (Func<INakedObjectAdapter, string, INakedObjectAdapter[]>) parm.GetCompletions : prop.GetCompletions;

            private void CheckAutocompleOrConditional() {
                if (!(IsAutoCompleteEnabled || GetChoicesParameters().Any())) {
                    throw new BadRequestNOSException();
                }
            }

            public INakedObjectAdapter[] GetList(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
                return IsAutoCompleteEnabled ? GetAutocompleteList(nakedObject, arguments) : GetConditionalList(nakedObject, arguments);
            }

            private ITypeFacade GetSpecificationWrapper(IObjectSpec spec) {
                return new TypeFacade(spec, frameworkFacade, framework);
            }

            private INakedObjectAdapter[] GetConditionalList(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
                Tuple<string, IObjectSpec>[] expectedParms = GetChoicesParameters();
                IDictionary<string, object> actualParms = arguments.Values;

                string[] expectedParmNames = expectedParms.Select(t => t.Item1).ToArray();
                string[] actualParmNames = actualParms.Keys.ToArray();

                if (expectedParmNames.Length < actualParmNames.Length) {
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

                var errors = new List<ContextFacade>();

                var mappedArguments = new Dictionary<string, INakedObjectAdapter>();

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

                            errors.Add(new ChoiceContextFacade(key, GetSpecificationWrapper(expectedType)) {
                                ProposedValue = rawValue
                            });
                        }
                        catch (Exception e) {
                            errors.Add(new ChoiceContextFacade(key, GetSpecificationWrapper(expectedType)) {
                                Reason = e.Message,
                                ProposedValue = rawValue
                            });
                        }
                    }
                    else if (actualType != null && !actualType.IsOfType(expectedType)) {
                        errors.Add(new ChoiceContextFacade(key, GetSpecificationWrapper(expectedType)) {
                            Reason = $"Argument is of wrong type is {actualType.FullName} expect {expectedType.FullName}",
                            ProposedValue = actualParms[ep.Item1]
                        });
                    }
                    else {
                        mappedArguments[key] = framework.NakedObjectManager.CreateAdapter(value, null, null);

                        errors.Add(new ChoiceContextFacade(key, GetSpecificationWrapper(expectedType)) {
                            ProposedValue = getValue(ep)
                        });
                    }
                }

                if (errors.Any(e => !string.IsNullOrEmpty(e.Reason))) {
                    throw new BadRequestNOSException("Wrong type of conditional argument(s)", errors);
                }

                return GetChoices(nakedObject, mappedArguments);
            }

            private INakedObjectAdapter[] GetAutocompleteList(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
                if (arguments.SearchTerm == null) {
                    throw new BadRequestNOSException("Missing or malformed search term");
                }
                return GetCompletions(nakedObject, arguments.SearchTerm);
            }
        }

        #endregion
    }
}