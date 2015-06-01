// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Interface;
using NakedObjects.Surface.Nof2.Context;
using NakedObjects.Surface.Nof2.Utility;
using NakedObjects.Surface.Nof2.Wrapper;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.reflector.java.control;
using sdm.systems.reflector;
using LogWrapper = NakedObjects.Surface.Utility.LogWrapper;
using User = org.nakedobjects.application.control.User;

namespace NakedObjects.Surface.Nof2.Implementation {
    public class FrameworkFacade : IFrameworkFacade {
        private readonly IOidStrategy oidStrategy;


        public FrameworkFacade(IOidStrategy oidStrategy) {
            oidStrategy.Surface = this;
            this.oidStrategy = oidStrategy;
        }

        #region API

        public IOidTranslator OidTranslator { get; private set; }

        public IOidStrategy OidStrategy {
            get { return oidStrategy; }
        }
        public IMessageBrokerSurface MessageBroker { get; private set; }

        public void Start() {
            SetSession();
            org.nakedobjects.@object.NakedObjects.getObjectPersistor().startTransaction();
        }

        public void End(bool success) {
            if (success) {
                org.nakedobjects.@object.NakedObjects.getObjectPersistor().endTransaction();
            }
            else {
                org.nakedobjects.@object.NakedObjects.getObjectPersistor().abortTransaction();
            }
        }

        public IPrincipal GetUser() {
            return MapErrors(() => {
                                 string userName = org.nakedobjects.@object.NakedObjects.getCurrentSession().getUserName();
                                 return new GenericPrincipal(new GenericIdentity(userName), new string[] {});
                             });
        }

        public ObjectContextSurface GetService(IOidTranslation serviceName) {
            return MapErrors(() => GetServiceInternal(serviceName).ToObjectContextSurface(this));
        }

        ListContextSurface IFrameworkFacade.GetServices() {
            throw new NotImplementedException();
        }

        public IMenuFacade[] GetMainMenus() {
            throw new NotImplementedException();
        }

        public ObjectContextSurface[] GetServices() {
            return MapErrors(() => SurfaceUtils.GetServicesInternal().Select(s => new ObjectContext(s).ToObjectContextSurface(this)).ToArray());
        }

        public ObjectContextSurface GetObject(IObjectFacade nakedObject) {
            throw new NotImplementedException();
        }

        public ObjectContextSurface RefreshObject(IObjectFacade nakedObject, ArgumentsContext arguments) {
            throw new NotImplementedException();
        }

        public ObjectContextSurface GetObject(IOidTranslation oid) {
            return MapErrors(() => GetObjectInternal(oid).ToObjectContextSurface(this));
        }

        public ObjectContextSurface PutObject(IOidTranslation oid, ArgumentsContext arguments) {
            return MapErrors(() => ChangeObject(GetObjectAsNakedObject(oid), arguments));
        }

        public PropertyContextSurface GetProperty(IOidTranslation oid, string propertyName) {
            return MapErrors(() => {
                                 PropertyContext pc = GetProperty(GetObjectAsNakedObject(oid), propertyName);
                                 return new PropertyContextSurface {Target = new NakedObjectWrapper(pc.Target, this), Property = new AssociationWrapper(pc.Property, pc.Target, this)};
                             });
        }

        public ListContextSurface GetPropertyCompletions(IOidTranslation objectId, string propertyName, ArgumentsContext arguments) {
            throw new NotImplementedException();
        }

        public ListContextSurface GetParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContext arguments) {
            throw new NotImplementedException();
        }

        public ListContextSurface GetServiceParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContext arguments) {
            throw new NotImplementedException();
        }

        public ActionContextSurface GetServiceAction(IOidTranslation serviceName, string actionName) {
            return MapErrors(() => {
                                 if (string.IsNullOrEmpty(actionName.Trim())) {
                                     throw new BadRequestNOSException();
                                 }

                                 NakedObject nakedObject = GetServiceAsNakedObject(serviceName);
                                 return GetAction(actionName, nakedObject).ToActionContextSurface(this);
                             });
        }


        public ActionContextSurface GetObjectAction(IOidTranslation objectId, string actionName) {
            return MapErrors(() => {
                                 if (string.IsNullOrEmpty(actionName.Trim())) {
                                     throw new BadRequestNOSException();
                                 }

                                 NakedObject nakedObject = GetObjectAsNakedObject(objectId);
                                 return GetAction(actionName, nakedObject).ToActionContextSurface(this);
                             });
        }

        public PropertyContextSurface PutProperty(IOidTranslation objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }

        public PropertyContextSurface DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));
        }

        public ActionResultContextSurface ExecuteListAction(IOidTranslation[] objectId, ITypeFacade elementSpec, string actionName, ArgumentsContext arguments) {
            throw new NotImplementedException();
        }

        public PropertyContextSurface AddToCollection(IOidTranslation objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => {
                                 PropertyContext context = SetupPropertyContext(GetObjectAsNakedObject(objectId), propertyName, argument.Value);
                                 var property = (OneToManyAssociation) context.Property;
                                 return ChangeCollection(context, property.validToAdd, property.addElement, argument);
                             });
        }

        public PropertyContextSurface DeleteFromCollection(IOidTranslation objectId, string propertyName, ArgumentContext argument) {
            return MapErrors(() => {
                                 PropertyContext context = SetupPropertyContext(GetObjectAsNakedObject(objectId), propertyName, argument.Value);
                                 var property = (OneToManyAssociation) context.Property;
                                 return ChangeCollection(context, property.validToRemove, property.removeElement, argument);
                             });
        }

        public ObjectContextSurface GetImage(string imageId) {
            return null;
        }

        public ITypeFacade[] GetDomainTypes() {
            return MapErrors(() => org.nakedobjects.@object.NakedObjects.getSpecificationLoader().allSpecifications().Select(s => new TypeFacade(s, null, this)).Cast<ITypeFacade>().ToArray());
        }

        public ITypeFacade GetDomainType(string typeName) {
            return MapErrors(() => {
                                 NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeName);
                                 if (spec is NoMemberSpecification) {
                                     throw new TypeResourceNotFoundNOSException(typeName);
                                 }
                                 return new TypeFacade(spec, null, this);
                             });
        }

        public PropertyTypeContextSurface GetPropertyType(string typeName, string propertyName) {
            return MapErrors(() => {
                                 Tuple<NakedObjectField, NakedObjectSpecification> pc = GetPropertyTypeInternal(typeName, propertyName);

                                 return new PropertyTypeContextSurface {
                                                                           Property = new AssociationWrapper(pc.Item1, null, this),
                                                                           OwningSpecification = new TypeFacade(pc.Item2, null, this)
                                                                       };
                             });
        }

        public ActionTypeContextSurface GetActionType(string typeName, string actionName) {
            return MapErrors(() => {
                                 Tuple<ActionContext, NakedObjectSpecification> pc = GetActionTypeInternal(typeName, actionName);

                                 return new ActionTypeContextSurface {
                                                                         ActionContext = pc.Item1.ToActionContextSurface(this),
                                                                         OwningSpecification = new TypeFacade(pc.Item2, null, this)
                                                                     };
                             });
        }

        public ParameterTypeContextSurface GetActionParameterType(string typeName, string actionName, string parmName) {
            return MapErrors(() => {
                                 Tuple<ActionWrapper, NakedObjectSpecification, NakedObjectActionParameter> pc = GetActionParameterTypeInternal(typeName, actionName, parmName);

                                 return new ParameterTypeContextSurface {
                                                                            Action = new ActionFacade(pc.Item1, null, this),
                                                                            OwningSpecification = new TypeFacade(pc.Item2, null, this),
                                                                            Parameter = new ActionParameterWrapper(pc.Item3, null, this)
                                                                        };
                             });
        }

        public ObjectContextSurface Persist(string typeName, ArgumentsContext arguments) {
            return MapErrors(() => CreateObject(typeName, arguments));
        }

        public UserCredentials Validate(string user, string password) {
            // HACK: hard-coded for now...
            return new UserCredentials("WELFARE\\sdmtraining3", password, new List<string>());
        }

        public IObjectFacade GetObject(ITypeFacade spec, object domainObject) {
            throw new NotImplementedException();
        }

        public IObjectFacade GetObject(object domainObject) {
            throw new NotImplementedException();
        }

        public object Wrap(object arm, IObjectFacade oldNakedObject) {
            throw new NotImplementedException();
        }

        public ActionResultContextSurface ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContext arguments) {
            return MapErrors(() => {
                                 ActionContext actionContext = GetInvokeActionOnObject(objectId, actionName);
                                 return ExecuteAction(actionContext, arguments);
                             });
        }

        public ActionResultContextSurface ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContext arguments) {
            return MapErrors(() => {
                                 ActionContext actionContext = GetInvokeActionOnService(serviceName, actionName);
                                 return ExecuteAction(actionContext, arguments);
                             });
        }

        #endregion

        #region helpers

        private PropertyContext SetupPropertyContext(NakedObject nakedObject, string propertyName, object toAdd) {
            PropertyContext context = GetProperty(nakedObject, propertyName);
            context.ProposedValue = toAdd;
            context.ProposedNakedObject = org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(toAdd);
            return context;
        }

        private bool ValidateParameters(ActionContext actionContext, IDictionary<string, object> rawParms) {
            if (rawParms.Any(kvp => !actionContext.Action.GetParameters(actionContext.Target).Select(p => p.getId()).Contains(kvp.Key))) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            bool isValid = true;
            var orderedParms = new Dictionary<string, ParameterContext>();

            // setup orderedparms 
            foreach (NakedObjectActionParameter parm in actionContext.Action.GetParameters(actionContext.Target)) {
                orderedParms[parm.getId()] = new ParameterContext();

                object value = rawParms.ContainsKey(parm.getId()) ? rawParms[parm.getId()] : null;

                orderedParms[parm.getId()].ProposedValue = value;
                orderedParms[parm.getId()].Parameter = parm;
                orderedParms[parm.getId()].Action = actionContext.Action;
            }

            //check for individual parameter validity, including parsing of text input

            foreach (NakedObjectActionParameter parm in actionContext.Action.GetParameters(actionContext.Target)) {
                try {
                    Naked valueNakedObject = GetValue(parm.getSpecification(), rawParms.ContainsKey(parm.getId()) ? rawParms[parm.getId()] : null);

                    orderedParms[parm.getId()].ProposedNakedObject = valueNakedObject;
                }
                catch (Exception) {
                    isValid = false;
                    orderedParms[parm.getId()].Reason = "Invalid Entry"; // i18n 
                }
            }
            actionContext.VisibleParameters = orderedParms.Select(p => p.Value).ToArray();

            // check for validity of whole set, including any 'co-validation' involving multiple parameters
            if (isValid) {
                Consent consent = actionContext.Action.isParameterSetValid(actionContext.Target, orderedParms.Select(kvp => kvp.Value.ProposedNakedObject).ToArray());
                if (!consent.isAllowed()) {
                    foreach (ParameterContext p in actionContext.VisibleParameters) {
                        p.Reason = consent.getReason();
                    }
                    actionContext.Reason = consent.getReason();
                    actionContext.ErrorCause = Cause.Disabled;
                    isValid = false;
                }
            }


            return isValid;
        }


        private static Consent IsCurrentlyImmutable(NakedObject target) {
            if (target.persistable() == Persistable.IMMUTABLE) {
                return new Veto("Is immutable");
            }
            return new Allow();
        }

        private ActionResultContextSurface ExecuteAction(ActionContext actionContext, ArgumentsContext arguments) {
            ValidateConcurrency(actionContext.Target, arguments.Digest);

            var actionResultContext = new ActionResultContext {Target = actionContext.Target, ActionContext = actionContext};
            if (ConsentHandler(actionContext.Action.isAvailable(actionContext.Target), actionResultContext, Cause.Disabled)) {
                if (ValidateParameters(actionContext, arguments.Values) && !arguments.ValidateOnly) {
                    Naked result = actionContext.Action.execute(actionContext.Target, actionContext.VisibleParameters.Select(p => p.ProposedNakedObject).ToArray());
                    actionResultContext.Result = GetObjectContext(result);
                }
            }
            return actionResultContext.ToActionResultContextSurface(this);
        }

        private static Naked GetValue(NakedObjectSpecification specification, object rawValue) {
            if (specification.isValue()) {
                NakedValue value = org.nakedobjects.@object.NakedObjects.getObjectLoader().createValueInstance(specification);
                value.parseTextEntry(rawValue == null ? "" : rawValue.ToString());
                return value;
            }

            return rawValue == null ? null : org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(rawValue);
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

        private NakedObject GetObjectAsNakedObject(IOidTranslation objectId) {
            object obj = oidStrategy.GetDomainObjectByOid(objectId);
            return org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(obj);
        }

        private NakedObject GetServiceAsNakedObject(IOidTranslation serviceName) {
            object obj = oidStrategy.GetServiceByServiceName(serviceName);
            return org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(obj);
        }

        private static ActionContext GetAction(string actionName, NakedObject nakedObject) {
            if (string.IsNullOrWhiteSpace(actionName.Trim())) {
                throw new BadRequestNOSException();
            }

            ActionWrapper action = nakedObject.getSpecification().GetActionLeafNodes().
                Where(p => p.isVisible(nakedObject).isAllowed()).SingleOrDefault(p => p.getId() == actionName);

            if (action == null) {
                throw new ActionResourceNotFoundNOSException(actionName);
            }

            return new ActionContext {Target = nakedObject, Action = action, VisibleParameters = GetContextParms(action, nakedObject)};
        }

        private static ParameterContext[] GetContextParms(ActionWrapper action, NakedObject target) {
            NakedObjectActionParameter[] parms = action.GetParameters(target);
            return parms.Select(p => new ParameterContext {Action = action, Parameter = p}).ToArray();
        }

        private ObjectContext GetObjectContext(Naked naked) {
            if (naked == null) {
                return null;
            }

            var nakedObject = naked as NakedObject;

            if (nakedObject != null) {
                IEnumerable<ActionWrapper> actions = nakedObject.getSpecification().GetActionLeafNodes().Where(p => p.isVisible(nakedObject).isAllowed());
                IEnumerable<NakedObjectField> properties = nakedObject.getSpecification().getFields().Where(p => !p.isHidden() && p.isVisible(nakedObject).isAllowed());
                return new ObjectContext(nakedObject) {
                                                          VisibleActions = actions.Select(a => new ActionContext {
                                                                                                                     Action = a,
                                                                                                                     Target = nakedObject,
                                                                                                                     VisibleParameters = GetContextParms(a, nakedObject)
                                                                                                                 }).ToArray(),
                                                          VisibleProperties = properties.Select(p => new PropertyContext {
                                                                                                                             Property = p,
                                                                                                                             Target = nakedObject
                                                                                                                         }).ToArray()
                                                      };
            }

            return new ObjectContext(naked);
        }


        private ObjectContext GetObjectInternal(IOidTranslation oid) {
            NakedObject nakedObject = GetObjectAsNakedObject(oid);
            return GetObjectContext(nakedObject);
        }

        private ObjectContext GetServiceInternal(IOidTranslation serviceName) {
            NakedObject nakedObject = GetServiceAsNakedObject(serviceName);
            return GetObjectContext(nakedObject);
        }

        private ActionContext GetInvokeActionOnObject(IOidTranslation objectId, string actionName) {
            NakedObject nakedObject = GetObjectAsNakedObject(objectId);
            return GetAction(actionName, nakedObject);
        }

        private ActionContext GetInvokeActionOnService(IOidTranslation serviceName, string actionName) {
            NakedObject nakedObject = GetServiceAsNakedObject(serviceName);
            return GetAction(actionName, nakedObject);
        }

        private Consent IsOfCorrectType(OneToManyAssociation property, PropertyContext context) {
            var collectionNakedObject = (InternalCollectionAdapter) property.get(context.Target);
            NakedObjectSpecification elementSpec = collectionNakedObject.getElementSpecification();


            if (context.ProposedNakedObject.getSpecification().isOfType(elementSpec)) {
                return new Allow();
            }
            return new Veto(string.Format("Not a suitable type; must be a {0}", elementSpec.getFullName()));
        }

        private PropertyContextSurface ChangeCollection(PropertyContext context, Func<NakedObject, NakedObject, Consent> validator, Action<NakedObject, NakedObject> mutator, ArgumentContext args) {
            ValidateConcurrency(context.Target, args.Digest);

            var property = (OneToManyAssociation) context.Property;

            if (ConsentHandler(IsOfCorrectType(property, context), context, Cause.Other)) {
                if (ConsentHandler(IsCurrentlyImmutable(context.Target), context, Cause.Immutable)) {
                    if (ConsentHandler(property.isAvailable(context.Target), context, Cause.Disabled)) {
                        if (ConsentHandler(validator(context.Target, (NakedObject) context.ProposedNakedObject), context, Cause.Other)) {
                            if (!args.ValidateOnly) {
                                mutator(context.Target, (NakedObject) context.ProposedNakedObject);
                            }
                        }
                    }
                }
            }

            context.Mutated = true;
            return context.ToPropertyContextSurface(this);
        }

        private void SetSession() {
            var session = new SimpleSession();
            session.setUser(new User("REST"));

            org.nakedobjects.@object.NakedObjects.getInstance().setSession(session);
        }

        private bool ConsentHandler(Consent consent, Context.Context context, Cause cause) {
            if (consent.isVetoed()) {
                context.Reason = consent.getReason();
                context.ErrorCause = cause;
                return false;
            }
            return true;
        }

        private static Consent CanSetPropertyValue(PropertyContext context) {
            try {
                context.ProposedNakedObject = GetValue(context.Specification, context.ProposedValue);
                return new Allow();
            }
            catch (Exception e) {
                return new Veto(e.Message);
            }
        }

        private static Consent IsAssociationValid(OneToOneAssociation property, NakedObject target, Naked proposed, bool allowDisabled = false) {
            if (allowDisabled && property.isAvailable(target).isVetoed()) {
                return new Allow();
            }

            if (proposed is NakedObject) {
                return property.isAssociationValid(target, (NakedObject) proposed);
            }
            return property.isValueValid(target, (NakedValue) proposed);
        }

        public static void SetValue<T>(OneToOneAssociation property, NakedObject target, Naked proposed) {
            object valueHolder = property.get(target).getObject();
            MethodInfo m = valueHolder.GetType().GetMethod("setValue", new[] {typeof (T)});
            m.Invoke(valueHolder, new[] {proposed.getObject()});
        }

        public static void ClearValue(OneToOneAssociation property, NakedObject target) {
            object valueHolder = property.get(target).getObject();
            MethodInfo m = valueHolder.GetType().GetMethod("clear");
            m.Invoke(valueHolder, new object[] {});
        }


        private void SetAssociation(OneToOneAssociation property, NakedObject target, Naked proposed) {
            if (proposed is NakedObject) {
                property.setAssociation(target, (NakedObject) proposed);
            }
            else {
                MethodInfo m = GetType().GetMethod("SetValue").MakeGenericMethod(proposed.getObject().GetType());
                m.Invoke(null, new object[] {property, target, proposed});
            }
        }

        private static bool ClearAssociation(OneToOneAssociation property, NakedObject target) {
            Naked existingValue = property.get(target);

            if (existingValue != null) {
                if (existingValue is NakedObject) {
                    property.clearAssociation(target, (NakedObject) existingValue);
                }
                else {
                    ClearValue(property, target);
                }
                return true;
            }
            return false;
        }

        private static void ValidateConcurrency(NakedObject nakedObject, string digest) {
            if (!string.IsNullOrEmpty(digest) && new VersionWrapper(nakedObject.getVersion()).IsDifferent(digest)) {
                throw new PreconditionFailedNOSException();
            }
        }

        private ObjectContextSurface ChangeObject(NakedObject nakedObject, ArgumentsContext arguments) {
            ValidateConcurrency(nakedObject, arguments.Digest);

            PropertyContext[] pc;
            try {
                pc = arguments.Values.Select(kvp => CanChangeProperty(nakedObject, kvp.Key, arguments.ValidateOnly, kvp.Value)).ToArray();
            }
            catch (PropertyResourceNotFoundNOSException e) {
                // no matching property for argument - consider this a syntax error 
                throw new BadRequestNOSException(e.Message);
            }
                      
            var objectContext = new ObjectContext(pc.First().Target) {VisibleProperties = pc};

            // if we fail we need to display passed in properties - if OK all visible
            PropertyContext[] propertiesToDisplay = objectContext.VisibleProperties;

            if (pc.All(c => string.IsNullOrEmpty(c.Reason))) {
                propertiesToDisplay = nakedObject.getSpecification().getFields().
                    Where(p => !p.isHidden() && p.isVisible(nakedObject).isAllowed()).
                    Select(p => new PropertyContext {Target = nakedObject, Property = p}).ToArray();
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Mutated = true;
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextSurface(this);
        }

        private ObjectContextSurface SetObject(NakedObject nakedObject, ArgumentsContext arguments) {
            if (nakedObject.getSpecification().getFields().Where(f => !f.isCollection()).Any(p => !arguments.Values.Keys.Select(s => s.ToLower()).Contains(p.getId()))) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            PropertyContext[] pc = arguments.Values.Select(kvp => CanSetProperty(nakedObject, kvp.Key, arguments.ValidateOnly, kvp.Value)).ToArray();
            var objectContext = new ObjectContext(pc.First().Target) {VisibleProperties = pc};

            // if we fail we need to display all - if OK only those that are visible 
            PropertyContext[] propertiesToDisplay = objectContext.VisibleProperties;

            if (pc.All(c => string.IsNullOrEmpty(c.Reason))) {
                if (nakedObject.getResolveState().isTransient()) {
                    if (nakedObject.getSpecification().persistable() == Persistable.USER_PERSISTABLE) {
                        org.nakedobjects.@object.NakedObjects.getObjectPersistor().makePersistent(nakedObject);
                    }
                    else {
                        org.nakedobjects.@object.NakedObjects.getObjectPersistor().objectChanged(nakedObject);
                    }
                }

                propertiesToDisplay = nakedObject.getSpecification().getFields().
                    Where(p => !p.isHidden() && p.isVisible(nakedObject).isAllowed()).
                    Select(p => new PropertyContext {Target = nakedObject, Property = p}).ToArray();
            }

            ObjectContext oc = GetObjectContext(objectContext.Target);
            oc.Mutated = false;
            oc.Reason = objectContext.Reason;
            oc.VisibleProperties = propertiesToDisplay;
            return oc.ToObjectContextSurface(this);
        }


        private PropertyContextSurface ChangeProperty(NakedObject nakedObject, string propertyName, ArgumentContext argument) {
            ValidateConcurrency(nakedObject, argument.Digest);
            return CanChangeProperty(nakedObject, propertyName, argument.ValidateOnly, argument.Value).ToPropertyContextSurface(this);
        }

        private PropertyContext CanChangeProperty(NakedObject nakedObject, string propertyName, bool validateOnly, object toPut = null) {
            PropertyContext context = GetProperty(nakedObject, propertyName);
            context.ProposedValue = toPut;
            var property = (OneToOneAssociation) context.Property;

            if (ConsentHandler(IsCurrentlyImmutable(context.Target), context, Cause.Immutable)) {
                if (ConsentHandler(property.isAvailable(context.Target), context, Cause.Disabled)) {
                    if (toPut == null) {
                        if (!validateOnly) {
                            context.Mutated = ClearAssociation(property, context.Target);
                        }
                    }
                    else {
                        if (ConsentHandler(CanSetPropertyValue(context), context, Cause.Other)) {
                            if (ConsentHandler(IsAssociationValid(property, context.Target, context.ProposedNakedObject), context, Cause.Other)) {
                                if (!validateOnly) {
                                    SetAssociation(property, context.Target, context.ProposedNakedObject);
                                    context.Mutated = true;
                                }
                            }
                        }
                    }
                }
            }
            context.Mutated = true;
            return context;
        }

        private PropertyContext CanSetProperty(NakedObject nakedObject, string propertyName, bool validateOnly, object toPut = null) {
            PropertyContext context = GetProperty(nakedObject, propertyName, false);
            context.ProposedValue = toPut;
            var property = (OneToOneAssociation) context.Property;

            if (ConsentHandler(IsCurrentlyImmutable(context.Target), context, Cause.Immutable)) {
                if (toPut == null) {
                    if (property.isMandatory()) {
                        context.Reason = "Mandatory";
                        context.ErrorCause = Cause.Other;
                    }
                    else if (!validateOnly) {
                        ClearAssociation(property, context.Target);
                    }
                }
                else {
                    if (ConsentHandler(CanSetPropertyValue(context), context, Cause.Other)) {
                        if (ConsentHandler(IsAssociationValid(property, context.Target, context.ProposedNakedObject, true), context, Cause.Other)) {
                            if (!validateOnly) {
                                SetAssociation(property, context.Target, context.ProposedNakedObject);
                            }
                        }
                    }
                }
            }

            return context;
        }

        private PropertyContext GetProperty(NakedObject nakedObject, string propertyName, bool onlyVisible = true) {
            if (string.IsNullOrEmpty(propertyName.Trim())) {
                throw new BadRequestNOSException();
            }
            string nof2Id = propertyName.ToLower();

            IEnumerable<NakedObjectField> fields = nakedObject.getSpecification().getFields();

            if (onlyVisible) {
                fields = fields.Where(p => !p.isHidden() && p.isVisible(nakedObject).isAllowed());
            }


            NakedObjectField property = fields.SingleOrDefault(p => p.getId() == nof2Id);


            if (property == null) {
                throw new PropertyResourceNotFoundNOSException(propertyName);
            }

            if (!property.isCollection()) {
                property.get(nakedObject); // get value so any errors happen inside error mapping code 
            }

            return new PropertyContext {Target = nakedObject, Property = property};
        }


        private Tuple<NakedObjectField, NakedObjectSpecification> GetPropertyTypeInternal(string typeName, string propertyName) {
            if (string.IsNullOrEmpty(typeName.Trim()) || string.IsNullOrEmpty(propertyName.Trim())) {
                throw new BadRequestNOSException();
            }
            NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeName);
            if (spec is NoMemberSpecification) {
                throw new TypeResourceNotFoundNOSException(typeName);
            }

            string nof2Id = propertyName.ToLower();

            NakedObjectField property = spec.getFields().SingleOrDefault(p => p.getId() == nof2Id);

            if (property == null) {
                throw new PropertyResourceNotFoundNOSException(propertyName);
            }

            return new Tuple<NakedObjectField, NakedObjectSpecification>(property, spec);
        }

        private static Tuple<ActionContext, NakedObjectSpecification> GetActionTypeInternal(string typeName, string actionName) {
            if (string.IsNullOrEmpty(typeName.Trim()) || string.IsNullOrWhiteSpace(actionName.Trim())) {
                throw new BadRequestNOSException();
            }

            NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeName);
            if (spec is NoMemberSpecification) {
                throw new TypeResourceNotFoundNOSException(typeName);
            }

            ActionWrapper action = spec.GetActionLeafNodes().SingleOrDefault(p => p.getId() == actionName);

            if (action == null) {
                throw new ActionResourceNotFoundNOSException(actionName);
            }

            var actionContext = new ActionContext {Action = action};

            return new Tuple<ActionContext, NakedObjectSpecification>(actionContext, spec);
        }

        private Tuple<ActionWrapper, NakedObjectSpecification, NakedObjectActionParameter> GetActionParameterTypeInternal(string typeName, string actionName, string parmName) {
            if (string.IsNullOrEmpty(typeName.Trim()) || string.IsNullOrWhiteSpace(actionName.Trim()) || string.IsNullOrWhiteSpace(parmName.Trim())) {
                throw new BadRequestNOSException();
            }

            NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeName);
            if (spec is NoMemberSpecification) {
                throw new TypeResourceNotFoundNOSException(typeName);
            }

            ActionWrapper action = spec.GetActionLeafNodes().SingleOrDefault(p => p.getId() == actionName);

            if (action == null) {
                throw new ActionResourceNotFoundNOSException(actionName);
            }

            NakedObjectActionParameter parm = action.GetParameters().SingleOrDefault(p => p.getId() == parmName);

            if (parm == null) {
                throw new ActionResourceNotFoundNOSException(parmName);
            }

            return new Tuple<ActionWrapper, NakedObjectSpecification, NakedObjectActionParameter>(action, spec, parm);
        }

        private ObjectContextSurface CreateObject(string typeName, ArgumentsContext arguments) {
            if (string.IsNullOrWhiteSpace(typeName)) {
                throw new BadRequestNOSException();
            }

            NakedObject nakedObject;
            try {
                NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeName);
                nakedObject = org.nakedobjects.@object.NakedObjects.getObjectPersistor().createTransientInstance(spec);
            }
            catch (Exception) {
                throw new TypeResourceNotFoundNOSException(typeName);
            }
            return SetObject(nakedObject, arguments);
        }

        #endregion
    }
}