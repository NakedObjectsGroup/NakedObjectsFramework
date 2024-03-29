﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Reflect;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Error;
using NakedFramework.Facade.Impl.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Facade.Utility;
using NakedFramework.Metamodel.Menu;

namespace NakedFramework.Facade.Impl.Impl;

public class FrameworkFacade : IFrameworkFacade {
    private readonly ILogger<FrameworkFacade> logger;
    private readonly IServiceProvider provider;
    private readonly IStringHasher stringHasher;

    public FrameworkFacade(IOidStrategy oidStrategy,
                           IOidTranslator oidTranslator,
                           INakedFramework framework,
                           IStringHasher stringHasher,
                           IServiceProvider provider,
                           ILogger<FrameworkFacade> logger) {
        this.stringHasher = stringHasher;
        this.provider = provider;
        this.logger = logger;
        oidStrategy.FrameworkFacade = this;
        OidStrategy = oidStrategy;
        OidTranslator = oidTranslator;
        Framework = framework;
        MessageBroker = new MessageBrokerWrapper(framework.MessageBroker);
    }

    /// <summary>
    ///     mainly for testing
    /// </summary>
    public INakedFramework Framework { get; }

    #region IFrameworkFacade Members

    public string[] ServerTypes => Framework.ServerTypes;

    public void Inject(object toInject) => Framework.DomainObjectInjector.InjectInto(toInject);

    public ObjectContextFacade GetImage(string imageId) => null;

    public void Start() => Framework.TransactionManager.StartTransaction();

    public void End(bool success) {
        try {
            if (success) {
                Framework.TransactionManager.EndTransaction();
            }
            else {
                Framework.TransactionManager.AbortTransaction();
            }
        }
        catch (DataUpdateException e) {
            throw new DataUpdateNOSException(e);
        }
        catch (ConcurrencyException e) {
            throw new PreconditionFailedNOSException(e.Message, e) {
                SourceNakedObject = ObjectFacade.Wrap(e.SourceNakedObjectAdapter, this, Framework)
            };
        }
    }

    public IPrincipal GetUser() => MapErrors(() => Framework.Session.Principal);

    public IOidTranslator OidTranslator { get; }

    public IOidStrategy OidStrategy { get; }

    public IMessageBrokerFacade MessageBroker { get; }

    public ObjectContextFacade GetService(IOidTranslation serviceName) => MapErrors(() => GetServiceInternal(serviceName).ToObjectContextFacade(this, Framework));

    public ListContextFacade GetServices() => MapErrors(() => GetServicesInternal().ToListContextFacade(this, Framework));

    public MenuContextFacade GetStaticServices() => MapErrors(() => GetStaticServicesInternal().ToMenuContextFacade(this, Framework));

    public MenuContextFacade GetMainMenus() => MapErrors(() => GetMenusInternal().ToMenuContextFacade(this, Framework));

    public ObjectContextFacade GetObject(IObjectFacade objectFacade) => MapErrors(() => GetObjectContext(((ObjectFacade)objectFacade).WrappedNakedObject).ToObjectContextFacade(this, Framework));

    public ITypeFacade GetDomainType(string typeName) => MapErrors(() => GetSpecificationWrapper(GetDomainTypeInternal(typeName)));

    public ObjectContextFacade Persist(string typeName, ArgumentsContextFacade arguments) => MapErrors(() => CreateObject(typeName, arguments, true));

    public ObjectContextFacade GetTransient(string typeName, ArgumentsContextFacade arguments) => MapErrors(() => CreateObject(typeName, arguments, false));

    public IObjectFacade GetObject(object domainObject) {
        // make sure object is in sync with framework.
        Framework.DomainObjectInjector.InjectInto(domainObject);
        return ObjectFacade.Wrap(Framework.NakedObjectManager.CreateAdapter(domainObject, null, null), this, Framework);
    }

    public ObjectContextFacade GetObject(IOidTranslation oid) => MapErrors(() => GetObjectInternal(oid).ToObjectContextFacade(this, Framework));

    public ObjectContextFacade PutObject(IOidTranslation oid, ArgumentsContextFacade arguments) => MapErrors(() => ChangeObject(GetObjectAsNakedObject(oid), arguments).ToObjectContextFacade(this, Framework));

    public PropertyContextFacade GetProperty(IOidTranslation oid, string propertyName) => MapErrors(() => GetProperty(GetObjectAsNakedObject(oid), propertyName).ToPropertyContextFacade(this, Framework));

    public PropertyContextFacade GetPropertyWithCompletions(IObjectFacade transient, string propertyName, ArgumentsContextFacade arguments) => MapErrors(() => GetPropertyWithCompletions(transient.WrappedAdapter(), propertyName, arguments).ToPropertyContextFacade(this, Framework));

    public ActionContextFacade GetServiceAction(IOidTranslation serviceName, string actionName) => MapErrors(() => GetActionOnService(serviceName, actionName, null).ToActionContextFacade(this, Framework));

    public ActionContextFacade GetMenuAction(string menuName, string actionName) => MapErrors(() => GetActionOnMenu(menuName, actionName).ToActionContextFacade(this, Framework));

    public ActionContextFacade GetObjectAction(IOidTranslation objectId, string actionName) => MapErrors(() => GetAction(actionName, GetObjectAsNakedObject(objectId)).ToActionContextFacade(this, Framework));

    public ActionContextFacade GetObjectActionWithCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments) => MapErrors(() => GetActionWithCompletions(actionName, GetObjectAsNakedObject(objectId), parmName, arguments).ToActionContextFacade(this, Framework));

    public ActionContextFacade GetServiceActionWithCompletions(IOidTranslation serviceName, string actionName, string parmName, ArgumentsContextFacade arguments) => MapErrors(() => GetActionWithCompletions(actionName, GetServiceAsNakedObject(serviceName), parmName, arguments).ToActionContextFacade(this, Framework));

    public ActionContextFacade GetMenuActionWithCompletions(string menuName, string actionName, string parmName, ArgumentsContextFacade arguments) => MapErrors(() => GetMenuActionWithCompletions1(menuName, actionName, parmName, arguments).ToActionContextFacade(this, Framework));

    public PropertyContextFacade PutProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument) => MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));

    public PropertyContextFacade DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument) => MapErrors(() => ChangeProperty(GetObjectAsNakedObject(objectId), propertyName, argument));

    public ActionResultContextFacade ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContextFacade arguments) =>
        MapErrors(() => {
            var actionContext = GetInvokeActionOnObject(objectId, actionName);
            return ExecuteAction(actionContext, arguments);
        });

    public ActionResultContextFacade ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContextFacade arguments) =>
        MapErrors(() => {
            var actionContext = GetActionOnService(serviceName, actionName, arguments);
            return ExecuteAction(actionContext, arguments);
        });

    public ActionResultContextFacade ExecuteMenuAction(string menuName, string actionName, ArgumentsContextFacade arguments) =>
        MapErrors(() => {
            var actionContext = GetActionOnMenu(menuName, actionName);
            return ExecuteAction(actionContext, arguments);
        });

    public (string, ActionContextFacade)[] GetMenuItem(IMenuItemFacade item, string parent = "") {
        switch (item) {
            case IMenuActionFacade menuActionFacade:
                return new[] { (item.Name, GetActionContext(menuActionFacade, parent)) };
            case IMenuFacade menuFacade:
                parent = parent + (string.IsNullOrEmpty(parent) ? "" : IdConstants.MenuItemDivider) + menuFacade.Name;
                return menuFacade.MenuItems.SelectMany(i => GetMenuItem(i, parent)).ToArray();
            default:
                return Array.Empty<(string, ActionContextFacade)>();
        }
    }

    public ActionContextFacade[] GetLocallyContributedActions(PropertyContextFacade propertyContext) =>
        propertyContext.Target.Specification.GetLocallyContributedActions(propertyContext.Property.ElementSpecification, propertyContext.Property.Id).Select(a => ((ActionFacade)a).WrappedSpec).Select(a => new ActionContext {
            MenuPath = "",
            Target = propertyContext.Target.WrappedAdapter(),
            Action = a,
            VisibleParameters = FilterCCAParms(a)
        }.ToActionContextFacade(this, Framework)).ToArray();

    public NakedObjectsFacadeException MapException(Exception ex) => FacadeUtils.Map(ex);

    public IServiceProvider GetScopedServiceProvider => Framework.ServiceProvider;

    #endregion

    #region Helpers

    private IObjectFacade GetTarget(IMenuActionFacade actionFacade) {
        if (actionFacade.Action.IsStatic) {
            return null;
        }

        return GetServices().List.Single(s => s.Specification.IsOfType(actionFacade.Action.OnType));
    }

    private ActionContextFacade GetActionContext(IMenuActionFacade actionFacade, string menuPath) =>
        new() {
            MenuPath = menuPath,
            Target = GetTarget(actionFacade),
            Action = actionFacade.Action,
            VisibleParameters = FilterMenuParms(actionFacade)
        };

    private static IAssociationSpec GetPropertyInternal(INakedObjectAdapter nakedObject, string propertyName, bool onlyVisible = true) {
        if (string.IsNullOrWhiteSpace(propertyName)) {
            throw new BadRequestNOSException();
        }

        var propertyQuery = ((IObjectSpec)nakedObject.Spec).Properties.Where(p => p.Id == propertyName);

        if (onlyVisible) {
            propertyQuery = propertyQuery.Where(p => p.IsVisible(nakedObject));
        }

        var property = propertyQuery.SingleOrDefault();

        if (property == null) {
            throw new PropertyResourceNotFoundNOSException(propertyName);
        }

        return property;
    }

    private static PropertyContext GetProperty(INakedObjectAdapter nakedObject, string propertyName, bool onlyVisible = true) {
        var property = GetPropertyInternal(nakedObject, propertyName, onlyVisible);
        return new PropertyContext { Target = nakedObject, Property = property };
    }

    private ListContext GetServicesInternal() {
        var services = Framework.ServicesManager.GetServicesWithVisibleActions(Framework.LifecycleManager);
        var elementType = (IObjectSpec)Framework.MetamodelManager.GetSpecification(typeof(object));

        return new ListContext {
            ElementType = elementType,
            List = services,
            IsListOfServices = true
        };
    }

    private static IMenuActionImmutable[] GetMenuActions(IMenuItemImmutable item) =>
        item switch {
            IMenuActionImmutable actionImmutable => new[] { actionImmutable },
            IMenuImmutable menu => menu.MenuItems.SelectMany(GetMenuActions).ToArray(),
            _ => Array.Empty<IMenuActionImmutable>()
        };

    private bool IsVisible(IActionSpecImmutable specIm) {
        var serviceSpec = specIm.GetOwnerSpec(Framework.MetamodelManager.Metamodel);
        var objectSpec = Framework.MetamodelManager.GetSpecification(serviceSpec);
        var no = Framework.ServicesManager.GetServices().SingleOrDefault(s => ReferenceEquals(s.Spec, objectSpec));
        var actionSpec = Framework.MetamodelManager.GetActionSpec(specIm);

        return actionSpec.IsVisible(no);
    }

    private bool HasVisibleAction(IMenuImmutable menu) => menu.MenuItems.SelectMany(GetMenuActions).Any(a => IsVisible(a.Action));

    private IMenuImmutable[] GetMenusWithVisibleActions(IMetamodelManager metamodelManager) {
        var menus = Framework.MetamodelManager.MainMenus();
        return menus?.Where(HasVisibleAction).ToArray();
    }

    private MenuContext GetMenusInternal() {
        var menus = GetMenusWithVisibleActions(Framework.MetamodelManager) ?? Framework.ServicesManager.GetServicesWithVisibleActions(Framework.LifecycleManager).Select(s => s.GetServiceSpec().Menu);
        var elementType = (IObjectSpec)Framework.MetamodelManager.GetSpecification(typeof(object));

        return new MenuContext {
            IsStaticServices = false,
            ElementType = elementType,
            List = menus.ToArray()
        };
    }

    private MenuContext GetStaticServicesInternal() {
        var menus = Framework.ServicesManager.GetStaticServicesAsMenus();
        var elementType = (IObjectSpec)Framework.MetamodelManager.GetSpecification(typeof(object));

        return new MenuContext {
            IsStaticServices = true,
            ElementType = elementType,
            List = menus.ToArray()
        };
    }

    private static ListContext GetCompletions(PropParmAdapter propParm, INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
        var list = propParm.GetList(nakedObject, arguments);

        return new ListContext {
            ElementType = propParm.Specification,
            List = list,
            IsListOfServices = false
        };
    }

    private PropertyContext GetPropertyWithCompletions(INakedObjectAdapter nakedObject, string propertyName, ArgumentsContextFacade arguments) {
        var property = GetPropertyInternal(nakedObject, propertyName) as IOneToOneAssociationSpec;
        var completions = GetCompletions(new PropParmAdapter(property, this, Framework), nakedObject, arguments);
        return new PropertyContext { Target = nakedObject, Property = property, Completions = completions };
    }

    private PropertyContext CanChangeProperty(INakedObjectAdapter nakedObject, string propertyName, object toPut = null) {
        var context = GetProperty(nakedObject, propertyName);
        context.ProposedValue = toPut;
        var property = (IOneToOneAssociationSpec)context.Property;

        if (ConsentHandler(IsCurrentlyMutable(context.Target), context, Cause.Immutable)) {
            if (ConsentHandler(property.IsUsable(context.Target), context, Cause.Disabled)) {
                if (toPut != null && ConsentHandler(CanSetPropertyValue(context), context, Cause.WrongType)) {
                    ConsentHandler(property.IsAssociationValid(context.Target, context.ProposedNakedObject), context, Cause.Other);
                }
            }
        }

        return context;
    }

    private static bool IsVisibleAndUsable(PropertyContext context) {
        var property = (IOneToOneAssociationSpec)context.Property;
        return property.IsVisible(context.Target) && property.IsUsable(context.Target).IsAllowed;
    }

    private PropertyContext CanSetProperty(INakedObjectAdapter nakedObject, string propertyName, object toPut = null) {
        var context = GetProperty(nakedObject, propertyName, false);
        context.ProposedValue = toPut;
        var property = (IOneToOneAssociationSpec)context.Property;

        if (toPut != null && ConsentHandler(CanSetPropertyValue(context), context, Cause.WrongType)) {
            if (IsVisibleAndUsable(context)) {
                ConsentHandler(property.IsAssociationValid(context.Target, context.ProposedNakedObject), context, Cause.Other);
            }
        }
        else if (toPut == null && property.IsMandatory && IsVisibleAndUsable(context)) {
            // only check user editable fields
            context.Reason = "Mandatory";
            context.ErrorCause = Cause.Other;
        }

        return context;
    }

    private IConsent CrossValidate(ObjectContext context) {
        var validateFacet = context.Specification.GetFacet<IValidateObjectFacet>();

        if (validateFacet != null) {
            var allParms = context.VisibleProperties.Select(pc => (pc.Id.ToLower(), pc.ProposedNakedObject)).ToArray();

            var result = validateFacet.ValidateParms(context.Target, allParms, logger);
            if (!string.IsNullOrEmpty(result)) {
                return new Veto(result);
            }
        }

        if (context.Specification.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
            var state = context.Target.ValidToPersist();
            if (state != null) {
                return new Veto(state);
            }
        }

        return new Allow();
    }

    private PropertyContextFacade ChangeProperty(INakedObjectAdapter nakedObject, string propertyName, ArgumentContextFacade argument) {
        ValidateConcurrency(nakedObject, argument.Digest);
        var context = CanChangeProperty(nakedObject, propertyName, argument.Value);
        if (string.IsNullOrEmpty(context.Reason)) {
            var spec = context.Target.Spec as IObjectSpec ?? throw new NakedObjectsFacadeException("context.Target.Spec must be IObjectSpec");

            var existingValues = spec.Properties.Where(p => p.Id != context.Id).Select(p => new { p, no = p.GetNakedObject(context.Target) }).Select(ao => new PropertyContext {
                    Property = ao.p,
                    ProposedNakedObject = ao.no,
                    ProposedValue = ao.no?.Object,
                    Target = context.Target
                }
            ).Union(new[] { context });

            var objectContext = new ObjectContext(context.Target) { VisibleProperties = existingValues.ToArray() };

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
        return context.ToPropertyContextFacade(this, Framework);
    }

    private static void SetProperty(PropertyContext context) => ((IOneToOneAssociationSpec)context.Property).SetAssociation(context.Target, context.ProposedValue == null ? null : context.ProposedNakedObject);

    private static void ValidateConcurrency(INakedObjectAdapter nakedObject, string digest) {
        if (!string.IsNullOrEmpty(nakedObject?.Version.Digest) && string.IsNullOrEmpty(digest)) {
            // expect concurrency 
            throw new PreconditionMissingNOSException();
        }

        if (!string.IsNullOrEmpty(digest) && new VersionFacade(nakedObject?.Version).IsDifferent(digest)) {
            throw new PreconditionFailedNOSException();
        }
    }

    protected string GetPropertyValueForEtag(IOneToOneAssociationSpec property, INakedObjectAdapter target) {
        var valueNakedObject = property.GetNakedObject(target);

        if (valueNakedObject == null) {
            return "";
        }

        if (property.ReturnSpec.IsParseable) {
            return valueNakedObject.Object.ToString();
        }

        var objectFacade = ObjectFacade.Wrap(valueNakedObject, this, Framework);
        return OidTranslator.GetOidTranslation(objectFacade).Encode();
    }

    protected string GetTransientSecurityHash(ObjectContext target, out string rawValue) {
        var propertiesValue = $"UserName:{Framework.Session.Principal.Identity?.Name ?? ""}";

        if (target.Specification is IObjectSpec spec) {
            var nakedObject = target.Target;

            var allProperties = spec.Properties.OfType<IOneToOneAssociationSpec>().Where(p => !p.IsInline && p.IsPersisted);
            var userUnsettableProperties = allProperties.Where(p => p.IsUsable(nakedObject).IsVetoed || !p.IsVisible(nakedObject));
            var propertyValues = userUnsettableProperties.ToDictionary(p => p.Id, p => GetPropertyValueForEtag(p, nakedObject));

            propertiesValue += propertyValues.Aggregate("", (s, kvp) => $"{s}{kvp.Key}:{kvp.Value}");
        }

        rawValue = propertiesValue;
        return stringHasher.GetHash(propertiesValue);
    }

    private void ValidateTransientIntegrity(ObjectContext nakedObject, string digest) {
        var transientHash = GetTransientSecurityHash(nakedObject, out var rawValue);

        if (transientHash != digest) {
            logger.LogError($"Transient Integrity failed for: {nakedObject.Id} bad values: {rawValue} old hash: {digest} new hash {transientHash}");
            var msg = digest is null
                ? "Values provided may not be persisted as an object (expected an etag value in If-Match header)"
                : "Values provided may not be persisted as an object (ensure any derived properties are annotated NotPersisted";

            throw new BadRequestNOSException(msg);
        }
    }

    private ObjectContext ChangeObject(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
        ValidateConcurrency(nakedObject, arguments.Digest);

        Dictionary<string, PropertyContext> contexts;
        try {
            contexts = arguments.Values.ToDictionary(kvp => kvp.Key, kvp => CanChangeProperty(nakedObject, kvp.Key, kvp.Value));
        }
        catch (PropertyResourceNotFoundNOSException e) {
            // no matching property for argument - consider this a syntax error 
            throw new BadRequestNOSException(e.Message);
        }

        var objectContext = new ObjectContext(contexts.First().Value.Target) { VisibleProperties = contexts.Values.ToArray() };

        // if we fail we need to display passed in properties - if OK all visible
        var propertiesToDisplay = objectContext.VisibleProperties;

        if (contexts.Values.All(c => string.IsNullOrEmpty(c.Reason))) {
            if (ConsentHandler(CrossValidate(objectContext), objectContext, Cause.Other)) {
                if (!arguments.ValidateOnly) {
                    Array.ForEach(objectContext.VisibleProperties, SetProperty);

                    if (nakedObject.Spec.Persistable == PersistableType.UserPersistable && nakedObject.ResolveState.IsTransient()) {
                        Framework.LifecycleManager.MakePersistent(nakedObject);
                    }
                    else {
                        Framework.Persistor.ObjectChanged(nakedObject, Framework.LifecycleManager, Framework.MetamodelManager);
                    }
                }

                propertiesToDisplay = ((IObjectSpec)nakedObject.Spec).Properties.Where(p => p.IsVisible(nakedObject)).Select(p => new PropertyContext { Target = nakedObject, Property = p }).ToArray();
            }
        }

        var oc = GetObjectContext(objectContext.Target);
        oc.Mutated = true;
        oc.Reason = objectContext.Reason;
        oc.VisibleProperties = propertiesToDisplay;
        return oc;
    }

    private ObjectContextFacade SetObject(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments, bool save) {
        // this is for ProtoPersistents where the arguments must contain all values 
        // for standard transients the object may already have values set so no need to check  
        if (((IObjectSpec)nakedObject.Spec).Properties.OfType<IOneToOneAssociationSpec>().Any(p => !arguments.Values.ContainsKey(p.Id))) {
            throw new BadRequestNOSException("Malformed arguments");
        }

        var contexts = arguments.Values.ToDictionary(kvp => kvp.Key, kvp => CanSetProperty(nakedObject, kvp.Key, kvp.Value));
        var objectContext = new ObjectContext(contexts.First().Value.Target) { VisibleProperties = contexts.Values.ToArray() };

        // if we fail we need to display all - if OK only those that are visible 
        var propertiesToDisplay = objectContext.VisibleProperties;
        var isPersisted = false;

        if (contexts.Values.All(c => string.IsNullOrEmpty(c.Reason))) {
            if (ConsentHandler(CrossValidate(objectContext), objectContext, Cause.Other)) {
                if (!arguments.ValidateOnly) {
                    Array.ForEach(objectContext.VisibleProperties, SetProperty);

                    ValidateTransientIntegrity(objectContext, arguments.Digest);

                    if (save) {
                        if (nakedObject.Spec.Persistable == PersistableType.UserPersistable) {
                            Save(nakedObject, objectContext);
                        }
                        else {
                            Framework.Persistor.ObjectChanged(nakedObject, Framework.LifecycleManager, Framework.MetamodelManager);
                        }
                    }

                    // return the visible properties when the object is persistent 
                    // it won't actually be until end of transaction
                    propertiesToDisplay = ((IObjectSpec)nakedObject.Spec).Properties.Where(p => p.IsVisibleWhenPersistent(nakedObject)).Select(p => new PropertyContext { Target = nakedObject, Property = p }).ToArray();
                    isPersisted = true;
                }
            }
        }

        // isPersisted flag indicates to return the visible actions when the object is persistent 
        // as it won't actually be until end of transaction
        var oc = GetObjectContext(objectContext.Target, isPersisted);
        oc.Reason = objectContext.Reason;
        oc.VisibleProperties = propertiesToDisplay;
        return oc.ToObjectContextFacade(this, Framework);
    }

    private void Save(INakedObjectAdapter nakedObject, ObjectContext objectContext) {
        var saveFacet = nakedObject.Spec.GetFacet<ISaveFacet>();
        var validate = saveFacet.Save(Framework, nakedObject, logger);
        IConsent consent = validate is not null ? new Veto(validate) : new Allow();
        ConsentHandler(consent, objectContext, Cause.Other);
    }

    private static void SetFirstParmFromTarget(ActionContext actionContext, IDictionary<string, object> rawParms) {
        if (actionContext.Target is not null) {
            var parm = actionContext.Action.Parameters.FirstOrDefault(p => actionContext.Target.Spec.IsOfType(p.Spec));

            if (parm is not null) {
                rawParms[parm.Id] = actionContext.Target.Object;
            }
        }
    }

    private void SetInjectedParms(ActionContext actionContext, IDictionary<string, object> rawParms) {
        foreach (var parameterSpec in actionContext.Action.Parameters) {
            var injectedFacet = parameterSpec.GetFacet<IInjectedParameterFacet>();
            if (injectedFacet is not null) {
                rawParms[parameterSpec.Id] = injectedFacet.GetInjectedValue(Framework, provider);
            }
        }
    }

    private bool ValidateParameters(ActionContext actionContext, IDictionary<string, object> rawParms) {
        if (rawParms.Any(kvp => !actionContext.Action.Parameters.Select(p => p.Id).Contains(kvp.Key))) {
            throw new BadRequestNOSException("Malformed arguments");
        }

        var isValid = true;
        var orderedParms = new Dictionary<string, ParameterContext>();

        if (actionContext.Action.IsStaticFunction) {
            // need to pass in target and injected parms 

            // same as contributed 
            SetFirstParmFromTarget(actionContext, rawParms);
            SetInjectedParms(actionContext, rawParms);
        }

        // handle contributed actions 

        if (actionContext.Action.IsContributedMethod && !actionContext.Action.OnSpec.Equals(actionContext.Target.Spec)) {
            var parm = actionContext.Action.Parameters.FirstOrDefault(p => actionContext.Target.Spec.IsOfType(p.Spec));

            if (parm != null) {
                rawParms[parm.Id] = actionContext.Target.Object;
            }
        }

        // check mandatory fields first as standard NO behaviour is that no validation takes place until 
        // all mandatory fields are set. 
        foreach (var parm in actionContext.Action.Parameters) {
            orderedParms[parm.Id] = new ParameterContext();

            var value = rawParms.ContainsKey(parm.Id) ? rawParms[parm.Id] : null;

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
            foreach (var parm in actionContext.Action.Parameters) {
                try {
                    var multiParm = parm as IOneToManyActionParameterSpec;
                    var valueNakedObject = GetValue(parm.Spec, multiParm?.ElementSpec, rawParms.ContainsKey(parm.Id) ? rawParms[parm.Id] : null);

                    orderedParms[parm.Id].ProposedNakedObject = valueNakedObject;

                    var consent = parm.IsValid(actionContext.Target, valueNakedObject);
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
            var consent = actionContext.Action.IsParameterSetValid(actionContext.Target, orderedParms.Select(kvp => kvp.Value.ProposedNakedObject).ToArray());
            if (!consent.IsAllowed) {
                actionContext.Reason = consent.Reason;
                isValid = false;
            }
        }

        actionContext.VisibleParameters = orderedParms.Select(p => p.Value).ToArray();

        return isValid;
    }

    private static bool ConsentHandler(IConsent consent, Context context, Cause cause) {
        if (consent.IsVetoed) {
            context.Reason = consent.Reason;
            context.ErrorCause = cause;
            return false;
        }

        return true;
    }

    private static void VerifyActionType(IActionSpec action, MethodType methodType) {
        switch (methodType) {
            case MethodType.Idempotent when !(action.IsQueryOnly() || action.IsIdempotent()):
                throw new NotAllowedNOSException("action is not idempotent"); // i18n 
            case MethodType.QueryOnly when !action.IsQueryOnly():
                throw new NotAllowedNOSException("action is not side-effect free"); // i18n 
        }
    }

    private ActionResultContextFacade ExecuteAction(ActionContext actionContext, ArgumentsContextFacade arguments) {
        // validate action type 

        VerifyActionType(actionContext.Action, arguments.ExpectedActionType);

        if (!actionContext.Action.IsQueryOnly()) {
            ValidateConcurrency(actionContext.Target, arguments.Digest);
        }

        var actionResultContext = new ActionResultContext { Target = actionContext.Target, ActionContext = actionContext };
        var errorOnChange = false;

        if (actionContext.Target?.IsViewModelEditView(Framework) == true) {
            // this is a form so we expect to update form with values in arguments 

            var objectContext = ChangeObject(actionContext.Target, arguments);

            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                errorOnChange = true;
                actionResultContext.ActionContext.VisibleProperties = objectContext.VisibleProperties;
            }

            if (!string.IsNullOrEmpty(objectContext.Reason)) {
                errorOnChange = true;
                actionResultContext.Reason = objectContext.Reason;
            }

            if (!errorOnChange) {
                // then clear so that action (which must be zero parms) does not get confused
                arguments.Values = new Dictionary<string, object>();
            }
        }

        if (!errorOnChange) {
            if (ConsentHandler(actionContext.Action.IsUsable(actionContext.Target), actionResultContext, Cause.Disabled)) {
                if (ValidateParameters(actionContext, arguments.Values) && !arguments.ValidateOnly) {
                    var result = actionContext.Action.Execute(actionContext.Target, actionContext.VisibleParameters.Select(p => p.ProposedNakedObject).ToArray());
                    var isProxied = result != null && FasterTypeUtils.IsEF6OrCoreProxy(result.Object.GetType());
                    // if proxied object is known to EF and so is being persisted
                    var oc = GetObjectContext(result, isProxied);

                    if (result != null && result.ResolveState.IsTransient()) {
                        var securityHash = GetTransientSecurityHash(oc, out var rawValue);
                        actionResultContext.TransientSecurityHash = securityHash;
                        logger.LogInformation($"Creating hash for: {oc.Id} raw values: {rawValue} hash: {securityHash}");
                    }

                    actionResultContext.Result = oc;
                }
            }
        }

        if (actionContext.Action.IsStaticFunction) {
            // Filter parameters
            actionContext.VisibleParameters = actionContext.VisibleParameters.Where(p => !IsTargetParm(actionContext.Action, p.Parameter) && !p.Parameter.IsInjected).ToArray();
        }

        return actionResultContext.ToActionResultContextFacade(this, Framework);
    }

    private static IConsent IsCurrentlyMutable(INakedObjectAdapter target) {
        var isPersistent = target.ResolveState.IsPersistent();

        var immutableFacet = target.Spec.GetFacet<IImmutableFacet>();
        if (immutableFacet != null) {
            var whenTo = immutableFacet.Value;
            switch (whenTo) {
                case WhenTo.UntilPersisted when !isPersistent:
                    return new Veto(NakedObjects.Resources.NakedObjects.FieldDisabledUntil);
                case WhenTo.OncePersisted when isPersistent:
                    return new Veto(NakedObjects.Resources.NakedObjects.FieldDisabledOnce);
            }

            var tgtSpec = target.Spec;
            if (tgtSpec.IsAlwaysImmutable() || (tgtSpec.IsImmutableOncePersisted() && isPersistent)) {
                return new Veto(NakedObjects.Resources.NakedObjects.FieldDisabled);
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
            var attachmentFacade = (IAttachmentFacade)rawValue;
            return fromStreamFacet.ParseFromStream(attachmentFacade.InputStream, attachmentFacade.ContentType, attachmentFacade.FileName, Framework.NakedObjectManager);
        }

        if (specification.IsParseable) {
            return specification.GetFacet<IParseableFacet>().ParseTextEntry(rawValue.ToString(), Framework.NakedObjectManager);
        }

        if (elementSpec is { IsParseable: true }) {
            var elements = ((IEnumerable)rawValue).Cast<object>().Select(e => elementSpec.GetFacet<IParseableFacet>().ParseTextEntry(e.ToString(), Framework.NakedObjectManager)).ToArray();
            var elementType = TypeUtils.GetType(elementSpec.FullName);
            var collType = typeof(List<>).MakeGenericType(elementType);
            var list = ((IList)Activator.CreateInstance(collType))?.AsQueryable();
            var collection = Framework.NakedObjectManager.CreateAdapter(list, null, null);
            collection.Spec.GetFacet<ICollectionFacet>().Init(collection, elements);
            return collection;
        }

        if (specification.IsQueryable) {
            rawValue = rawValue is IEnumerable rawEnumerable ? rawEnumerable.AsQueryable() : rawValue;
        }

        return Framework.NakedObjectManager.CreateAdapter(rawValue, null, null);
    }

    private IConsent CanSetPropertyValue(PropertyContext context) {
        try {
            var coll = context.Property as IOneToManyAssociationSpec;
            context.ProposedNakedObject = GetValue((IObjectSpec)context.Specification, coll?.ElementSpec, context.ProposedValue);
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
        var obj = OidStrategy.GetObjectFacadeByOid(objectId);
        return obj.WrappedAdapter();
    }

    private INakedObjectAdapter GetServiceAsNakedObject(IOidTranslation serviceName) {
        var obj = OidStrategy.GetServiceByServiceName(serviceName);
        return Framework.NakedObjectManager.GetAdapterFor(obj);
    }

    private static ParameterContext[] FilterParms(IActionSpec action, ITypeSpec targetSpec) =>
        action.IsStaticFunction
            ? FilterParmsForFunctions(action)
            : FilterParmsForContributedActions(action, targetSpec);

    private static ParameterContextFacade[] FilterMenuParms(IMenuActionFacade actionFacade) {
        var parms = actionFacade.Action.Parameters;
        if (actionFacade.Action.IsStatic) {
            // filter parms for functions 

            parms = parms.Where(p => !p.IsInjected).ToArray();
        }

        return parms.Select(p => new ParameterContextFacade { Parameter = p, Action = actionFacade.Action }).ToArray();
    }

    private static bool IsTargetParm(IActionSpec action, IActionParameterSpec parm) =>
        parm.Number == 0 &&
        action.ContainsFacet<IContributedToObjectFacet>() &&
        !action.ContainsFacet<IContributedToCollectionFacet>();

    private static ParameterContext[] FilterParmsForFunctions(IActionSpec action) =>
        action.Parameters
              .Where(p => !IsTargetParm(action, p))
              .Where(p => !p.IsInjected)
              .Select(p => new ParameterContext {
                  Action = action,
                  Parameter = p
              })
              .ToArray();

    private static ParameterContext[] FilterParmsForContributedActions(IActionSpec action, ITypeSpec targetSpec) {
        IActionParameterSpec[] parms;
        if (action.IsContributedMethod && !action.OnSpec.Equals(targetSpec)) {
            var tempParms = new List<IActionParameterSpec>();

            var skipped = false;
            foreach (var parameter in action.Parameters) {
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
            Parameter = p
        }).ToArray();
    }

    private IActionSpec GetActionInternal(string actionName, INakedObjectAdapter nakedObject) {
        if (string.IsNullOrWhiteSpace(actionName)) {
            throw new BadRequestNOSException();
        }

        var action = nakedObject.Spec.GetActionLeafNodes().Where(p => p.Id == actionName).SingleOrDefault(p => p.IsVisible(nakedObject)) ??
                     GetActionFromElementSpec(actionName, nakedObject) ??
                     throw new ActionResourceNotFoundNOSException(actionName);

        return action;
    }

    private IActionSpec GetActionFromElementSpec(string actionName, INakedObjectAdapter nakedObject) {
        var typeOfFacet = nakedObject.Spec.GetFacet<ITypeOfFacet>();
        IActionSpec action = null;
        if (typeOfFacet != null) {
            var metamodel = Framework.MetamodelManager.Metamodel;
            var elementSpecImmut = typeOfFacet.GetValueSpec(nakedObject, metamodel);
            var elementSpec = Framework.MetamodelManager.GetSpecification(elementSpecImmut);

            if (elementSpec != null) {
                action = elementSpec.GetCollectionContributedActions().Where(p => p.Id == actionName).SingleOrDefault(p => p.IsVisible(nakedObject));
            }
        }

        return action;
    }

    private static IActionParameterSpec GetParameterInternal(IActionSpec action, string parmName) {
        if (string.IsNullOrWhiteSpace(parmName) || string.IsNullOrWhiteSpace(parmName)) {
            throw new BadRequestNOSException();
        }

        var parm = action.Parameters.SingleOrDefault(p => p.Id == parmName);

        if (parm == null) {
            // todo throw something;
        }

        return parm;
    }

    private IEnumerable<IActionSpecImmutable> GetActionsFromMenuItems(IEnumerable<IMenuItemImmutable> menuItems) => menuItems.SelectMany(GetActionFromMenuItem);

    private IEnumerable<IActionSpecImmutable> GetActionFromMenuItem(IMenuItemImmutable menuItemImmutable) =>
        menuItemImmutable switch {
            MenuAction menuAction => new[] { menuAction.Action },
            MenuImmutable menuImmutable => GetActionsFromMenuItems(menuImmutable.MenuItems),
            _ => Array.Empty<IActionSpecImmutable>()
        };

    private ActionContext GetActionOnMenu(string menuName, string actionName) {
        var menu = Framework.MetamodelManager.MainMenus().SingleOrDefault(m => m.Id == menuName);
        var isStatic = menu is null;
        menu ??= Framework.ServicesManager.GetStaticServicesAsMenus().SingleOrDefault(m => m.Id == menuName);

        if (menu is not null) {
            try {
                var action = GetActionsFromMenuItems(menu.MenuItems).SingleOrDefault(a => a.Identifier.MemberName == actionName);
                if (action is not null) {
                    var actionSpec = Framework.MetamodelManager.GetActionSpec(action);

                    return new ActionContext {
                        Target = null,
                        Action = actionSpec,
                        MenuId = menu.Id,
                        IsStaticService = isStatic,
                        VisibleParameters = FilterParms(actionSpec, actionSpec.OnSpec)
                    };
                }
            }
            catch (InvalidOperationException) {
                logger.LogError($"multiple actions with name '{actionName}' found on menu '{menuName}'");
                throw new ActionResourceNotFoundNOSException(actionName);
            }
        }

        throw new ActionResourceNotFoundNOSException(actionName);
    }

    private static bool IsContributedToCollection(IActionSpec actionSpec) => actionSpec.ContainsFacet<IContributedToCollectionFacet>();

    private ActionContext GetActionOnService(IOidTranslation serviceName, string actionName, ArgumentsContextFacade argumentsContextFacade) {
        return Framework.ReflectorType switch {
            ReflectorType.Object => GetAction(actionName, GetServiceAsNakedObject(serviceName)),
            ReflectorType.Functional => GetCollectionContributedActionOnType(serviceName, actionName, argumentsContextFacade),
            _ => throw new NotImplementedException("Hybrid system not yet supported")
        };
    }

    private ActionContext GetCollectionContributedActionOnType(IOidTranslation serviceName, string actionName, ArgumentsContextFacade argumentsContextFacade) {
        try {
            var typeSpec = Framework.MetamodelManager.GetSpecification(serviceName.DomainType);
            var actionSpec = typeSpec?.GetActions().Where(IsContributedToCollection).SingleOrDefault(a => a.Id == actionName);

            if (actionSpec is not null) {
                var targetKey = actionSpec.Parameters.First().Id;
                INakedObjectAdapter target = null;
                if (argumentsContextFacade?.Values.ContainsKey(targetKey) == true) {
                    var rawTarget = ((IEnumerable)argumentsContextFacade.Values[targetKey]).AsQueryable();
                    target = Framework.NakedObjectManager.CreateAdapter(rawTarget, null, null);
                }

                return new ActionContext {
                    Target = target,
                    Action = actionSpec,
                    VisibleParameters = FilterParms(actionSpec, actionSpec.OnSpec)
                };
            }
        }
        catch (InvalidOperationException) {
            logger.LogError($"multiple actions with name '{actionName}' found on type '{serviceName.DomainType}'");
            throw new ActionResourceNotFoundNOSException(actionName);
        }

        throw new ActionResourceNotFoundNOSException(actionName);
    }

    private ActionContext GetAction(string actionName, INakedObjectAdapter nakedObject) {
        var actionSpec = GetActionInternal(actionName, nakedObject);
        return new ActionContext {
            Target = nakedObject,
            Action = actionSpec,
            VisibleParameters = FilterParms(actionSpec, nakedObject.Spec)
        };
    }

    private ActionContext GetActionWithCompletions(string actionName, INakedObjectAdapter nakedObject, string parmName, ArgumentsContextFacade arguments) {
        var actionSpec = GetActionInternal(actionName, nakedObject);
        var parm = GetParameterInternal(actionSpec, parmName);
        var completions = GetCompletions(new PropParmAdapter(parm, this, Framework), nakedObject, arguments);

        var ac = new ActionContext {
            Target = nakedObject,
            Action = actionSpec,
            VisibleParameters = FilterParms(actionSpec, nakedObject.Spec)
        };

        var pc = ac.VisibleParameters.SingleOrDefault(p => p.Id == parmName);

        if (pc != null) {
            pc.Completions = completions;
        }

        return ac;
    }

    private ActionContext GetMenuActionWithCompletions1(string menuName, string actionName, string parmName, ArgumentsContextFacade arguments) {
        var menu = Framework.MetamodelManager.MainMenus().SingleOrDefault(m => m.Id == menuName);

        if (menu is not null) {
            try {
                var action = GetActionsFromMenuItems(menu.MenuItems).SingleOrDefault(a => a.Identifier.MemberName == actionName);
                if (action is not null) {
                    var actionSpec = Framework.MetamodelManager.GetActionSpec(action);
                    var parm = GetParameterInternal(actionSpec, parmName);
                    var completions = GetCompletions(new PropParmAdapter(parm, this, Framework), null, arguments);

                    var ac = new ActionContext {
                        Target = null,
                        Action = actionSpec,
                        MenuId = menu.Id,
                        VisibleParameters = FilterParms(actionSpec, actionSpec.OnSpec)
                    };

                    var pc = ac.VisibleParameters.SingleOrDefault(p => p.Id == parmName);

                    if (pc != null) {
                        pc.Completions = completions;
                    }

                    return ac;
                }
            }
            catch (InvalidOperationException) {
                logger.LogError($"multiple actions with name '{actionName}' found on menu '{menuName}'");
                throw new ActionResourceNotFoundNOSException(actionName);
            }
        }

        throw new ActionResourceNotFoundNOSException(actionName);
    }

    private ActionContext GetInvokeActionOnObject(IOidTranslation objectId, string actionName) {
        var nakedObject = GetObjectAsNakedObject(objectId);
        return GetAction(actionName, nakedObject);
    }

    private static IActionSpec MatchingActionSpecOnService(IActionSpec actionToMatch) {
        var allServiceActions = actionToMatch.OnSpec.GetActionLeafNodes();

        return allServiceActions.SingleOrDefault(sa => sa.Id == actionToMatch.Id &&
                                                       sa.ParameterCount == actionToMatch.ParameterCount &&
                                                       sa.Parameters.Select(p => p.Spec).SequenceEqual(actionToMatch.Parameters.Select(p => p.Spec)));
    }

    private static (string parent, IActionSpecImmutable action)[] GetMenuItem(IMenuItemImmutable item, string parent = "") {
        string MenuItemDivider() => string.IsNullOrEmpty(parent) ? "" : IdConstants.MenuItemDivider;

        return item switch {
            IMenuActionImmutable menuAction => new[] { (parent, menuAction.Action) },
            IMenuImmutable menu => menu.MenuItems.SelectMany(i => GetMenuItem(i, $"{parent}{MenuItemDivider()}{menu.Name}")).ToArray(),
            _ => Array.Empty<(string, IActionSpecImmutable)>()
        };
    }

    private static bool IsVisible(IMemberSpec actionSpec, INakedObjectAdapter nakedObject, bool isPersisted) => isPersisted ? actionSpec.IsVisibleWhenPersistent(nakedObject) : actionSpec.IsVisible(nakedObject);

    private static ParameterContext[] FilterCCAParms(IActionSpec action) {
        if (action.IsStaticFunction) {
            return FilterParmsForFunctions(action);
        }

        return action.Parameters.Select(p => new ParameterContext {
            Action = action,
            Parameter = p
        }).ToArray();
    }

    private ObjectContext GetObjectContext(INakedObjectAdapter nakedObject, bool isPersisted = false) {
        if (nakedObject == null) {
            return null;
        }

        var actionLeafs = nakedObject.Spec.GetActionLeafNodes().Where(p => IsVisible(p, nakedObject, isPersisted)).ToArray();

        IActionSpec GetLeaf(ISpecification action) => actionLeafs.SingleOrDefault(a => a.Identifier.Equals(action.Identifier));

        var menuItems = nakedObject.Spec.Menu?.MenuItems ?? new List<IMenuItemImmutable>();

        var menuActions = menuItems.SelectMany(m => GetMenuItem(m, m.Grouping));

        var actions = menuActions.Select(m => new { m.parent, action = GetLeaf(m.action) }).Where(a => a.action != null);

        var objectSpec = nakedObject.Spec as IObjectSpec;
        var properties = objectSpec?.Properties.Where(p => IsVisible(p, nakedObject, isPersisted)).ToArray() ?? Array.Empty<IAssociationSpec>();

        var ccaContexts = Array.Empty<ActionContext>();

        if (nakedObject.Spec.IsQueryable) {
            var typeOfFacet = nakedObject.GetTypeOfFacetFromSpec();
            var introspectableSpecification = typeOfFacet.GetValueSpec(nakedObject, Framework.MetamodelManager.Metamodel);
            var elementSpec = Framework.MetamodelManager.GetSpecification(introspectableSpecification);
            var cca = elementSpec.GetCollectionContributedActions().Where(p => p.IsVisible(nakedObject)).ToArray();

            ccaContexts = cca.Select(a => new { action = a }).Select(a => new ActionContext {
                Action = a.action,
                Target = Framework.ServicesManager.GetService(a.action.OnSpec as IServiceSpec),
                VisibleParameters = FilterCCAParms(a.action)
            }).ToArray();
        }

        var actionContexts = actions.Select(a => new { a.action, mp = a.parent }).Select(a => new ActionContext {
            Action = a.action,
            Target = nakedObject,
            VisibleParameters = FilterParms(a.action, nakedObject.Spec),
            MenuPath = a.mp
        });

        var oc = new ObjectContext(nakedObject) {
            VisibleActions = actionContexts.Union(ccaContexts).ToArray(),
            VisibleProperties = properties.Select(p => new PropertyContext {
                Property = p,
                Target = nakedObject
            }).ToArray()
        };

        return oc;
    }

    private ObjectContext GetObjectInternal(IOidTranslation oid) {
        var nakedObject = GetObjectAsNakedObject(oid);
        return GetObjectContext(nakedObject);
    }

    private ObjectContext GetServiceInternal(IOidTranslation serviceName) {
        var nakedObject = GetServiceAsNakedObject(serviceName);
        return GetObjectContext(nakedObject);
    }

    private ITypeSpec GetDomainTypeInternal(string domainTypeId) {
        try {
            var spec = (TypeFacade)OidStrategy.GetSpecificationByLinkDomainType(domainTypeId);
            return spec.WrappedValue;
        }
        catch (Exception) {
            throw new TypeResourceNotFoundNOSException(domainTypeId);
        }
    }

    private ObjectContextFacade CreateObject(string typeName, ArgumentsContextFacade arguments, bool save) {
        if (string.IsNullOrWhiteSpace(typeName)) {
            throw new BadRequestNOSException();
        }

        var spec = (IObjectSpec)GetDomainTypeInternal(typeName);
        var nakedObject = Framework.LifecycleManager.CreateInstance(spec);

        return SetObject(nakedObject, arguments, save);
    }

    private ITypeFacade GetSpecificationWrapper(ITypeSpec spec) => new TypeFacade(spec, this, Framework);

    private class PropParmAdapter {
        private readonly INakedFramework framework;
        private readonly IFrameworkFacade frameworkFacade;
        private readonly IActionParameterSpec parm;
        private readonly IOneToOneAssociationSpec prop;

        private PropParmAdapter(object p, IFrameworkFacade frameworkFacade, INakedFramework framework) {
            this.frameworkFacade = frameworkFacade;
            this.framework = framework;
            if (p == null) {
                throw new BadRequestNOSException();
            }
        }

        public PropParmAdapter(IOneToOneAssociationSpec prop, IFrameworkFacade frameworkFacade, INakedFramework framework)
            : this((object)prop, frameworkFacade, framework) {
            this.prop = prop;
            CheckAutocompleOrConditional();
        }

        public PropParmAdapter(IActionParameterSpec parm, IFrameworkFacade frameworkFacade, INakedFramework framework)
            : this((object)parm, frameworkFacade, framework) {
            this.parm = parm;
            CheckAutocompleOrConditional();
        }

        private bool IsAutoCompleteEnabled => prop?.IsAutoCompleteEnabled ?? parm.IsAutoCompleteEnabled;

        public IObjectSpec Specification => prop == null ? parm.Spec : prop.ReturnSpec;

        private Func<(string name, IObjectSpec spec)[]> GetChoicesParameters => prop == null ? (Func<(string, IObjectSpec)[]>)parm.GetChoicesParameters : prop.GetChoicesParameters;

        private Func<INakedObjectAdapter, IDictionary<string, INakedObjectAdapter>, INakedObjectAdapter[]> GetChoices => prop == null ? parm.GetChoices : prop.GetChoices;

        private Func<INakedObjectAdapter, string, INakedObjectAdapter[]> GetCompletions => prop == null ? parm.GetCompletions : prop.GetCompletions;

        private void CheckAutocompleOrConditional() {
            if (!(IsAutoCompleteEnabled || GetChoicesParameters().Any())) {
                throw new BadRequestNOSException();
            }
        }

        public INakedObjectAdapter[] GetList(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) => IsAutoCompleteEnabled ? GetAutocompleteList(nakedObject, arguments) : GetConditionalList(nakedObject, arguments);

        private ITypeFacade GetSpecificationWrapper(IObjectSpec spec) => new TypeFacade(spec, frameworkFacade, framework);

        private INakedObjectAdapter[] GetConditionalList(INakedObjectAdapter nakedObject, ArgumentsContextFacade arguments) {
            var expectedParms = GetChoicesParameters();
            var actualParms = arguments.Values;

            var expectedParmNames = expectedParms.Select(t => t.name).ToArray();
            var actualParmNames = actualParms.Keys.ToArray();

            if (expectedParmNames.Length < actualParmNames.Length) {
                throw new BadRequestNOSException("Wrong number of conditional arguments");
            }

            if (!actualParmNames.All(expectedParmNames.Contains)) {
                throw new BadRequestNOSException("Unrecognised conditional argument(s)");
            }

            object GetValue((string name, IObjectSpec spec) ep) =>
                actualParms.ContainsKey(ep.name)
                    ? actualParms[ep.name]
                    : ep.spec.IsParseable
                        ? ""
                        : null;

            var matchedParms = expectedParms.ToDictionary(ep => ep.name, ep => new {
                expectedType = ep.spec,
                value = GetValue(ep),
                actualType = GetValue(ep) == null ? null : framework.MetamodelManager.GetSpecification(GetValue(ep).GetType())
            });

            var errors = new List<ContextFacade>();

            var mappedArguments = new Dictionary<string, INakedObjectAdapter>();

            foreach (var ep in expectedParms) {
                var key = ep.name;
                var mp = matchedParms[key];
                var value = mp.value;
                var expectedType = mp.expectedType;
                var actualType = mp.actualType;

                if (expectedType.IsParseable && actualType.IsParseable) {
                    var rawValue = value.ToString();

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
                        ProposedValue = actualParms[ep.name]
                    });
                }
                else {
                    mappedArguments[key] = framework.NakedObjectManager.CreateAdapter(value, null, null);

                    errors.Add(new ChoiceContextFacade(key, GetSpecificationWrapper(expectedType)) {
                        ProposedValue = GetValue(ep)
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