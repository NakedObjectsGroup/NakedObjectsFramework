﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Error;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Configuration;
using NakedFramework.Rest.Model;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Error;
using NakedFramework.Rest.Snapshot.Representation;
using NakedFramework.Rest.Snapshot.Utility;
using static NakedFramework.Rest.API.ControllerHelpers;

[assembly: InternalsVisibleTo("NakedFramework.Rest.Test")]
[assembly: InternalsVisibleTo("NakedFramework.Rest.Test.Impl1")]
[assembly: InternalsVisibleTo("NakedFramework.Rest.Test.Impl2")]
[assembly: InternalsVisibleTo("NakedFramework.Rest.Test.Impl3")]
[assembly: InternalsVisibleTo("NakedFramework.Rest.Test.Impl4")]

namespace NakedFramework.Rest.API;

public class RestfulObjectsControllerBase : ControllerBase {
    private readonly ILogger logger;
    private readonly ILoggerFactory loggerFactory;
    private IRestfulObjectsConfiguration config;

    #region constructor and properties

    protected RestfulObjectsControllerBase(IFrameworkFacade frameworkFacade, ILogger logger, ILoggerFactory loggerFactory, IRestfulObjectsConfiguration config) {
        this.logger = logger;
        this.loggerFactory = loggerFactory;
        ResetConfig(config);
        FrameworkFacade = frameworkFacade;
        OidStrategy = frameworkFacade.OidStrategy;
    }

    // internal for testing
    internal void ResetConfig(IRestfulObjectsConfiguration newConfig) {
        config = newConfig;
        RestControlFlags.ConfiguredPageSize = config.DefaultPageSize;
        RestSnapshot.DebugFilter = config.DebugWarnings
            ? f => f()
            : _ => "Enable DebugWarnings to see message";
    }

    internal IRestfulObjectsConfiguration GetConfig() => config;

    public bool IsReadOnly => config.IsReadOnly;

    public bool InlineDetailsInActionMemberRepresentations => config.InlineDetailsInActionMemberRepresentations;

    public bool InlineDetailsInCollectionMemberRepresentations => config.InlineDetailsInCollectionMemberRepresentations;

    public bool InlineDetailsInPropertyMemberRepresentations => config.InlineDetailsInPropertyMemberRepresentations;

    // cache settings in seconds, 0 = no cache, "no, short, long")   
    public (int, int, int) CacheSettings => config.CacheSettings;

    public int DefaultPageSize => config.DefaultPageSize;

    public bool AcceptHeaderStrict => config.AcceptHeaderStrict;

    public bool BlobsClobs => config.BlobsClobs;

    public bool DebugWarnings => config.DebugWarnings;

    protected IFrameworkFacade FrameworkFacade { get; set; }
    public IOidStrategy OidStrategy { get; set; }
    public bool AllowMutatingActionOnImmutableObject => config.AllowMutatingActionOnImmutableObject;

    #endregion

    #region api

    [FromQuery(Name = RestControlFlags.ValidateOnlyReserved)]
    public bool ValidateOnly { get; set; }

    [FromQuery(Name = RestControlFlags.DomainTypeReserved)]
    public string DomainType { get; set; }

    [FromQuery(Name = RestControlFlags.ElementTypeReserved)]
    public string ElementType { get; set; }

    [FromQuery(Name = RestControlFlags.DomainModelReserved)]
    public string DomainModel { get; set; }

    [FromQuery(Name = RestControlFlags.FollowLinksReserved)]
    public bool? FollowLinks { get; set; }

    [FromQuery(Name = RestControlFlags.SortByReserved)]
    public bool SortBy { get; set; }

    [FromQuery(Name = RestControlFlags.SearchTermReserved)]
    public string SearchTerm { get; set; }

    [FromQuery(Name = RestControlFlags.PageReserved)]
    public int Page { get; set; }

    [FromQuery(Name = RestControlFlags.PageSizeReserved)]
    public int PageSize { get; set; }

    [FromQuery(Name = RestControlFlags.InlinePropertyDetailsReserved)]
    public bool? InlinePropertyDetails { get; set; }

    [FromQuery(Name = RestControlFlags.InlineCollectionItemsReserved)]
    public bool? InlineCollectionItems { get; set; }

    public virtual ActionResult GetHome() => InitAndHandleErrors(SnapshotFactory.HomeSnapshot(FrameworkFacade, Request, GetFlags(this)));

    public virtual ActionResult GetUser() => InitAndHandleErrors(SnapshotFactory.UserSnapshot(FrameworkFacade, FrameworkFacade.GetUser, Request, GetFlags(this)));

    public virtual ActionResult GetServices() => InitAndHandleErrors(SnapshotFactory.ServicesSnapshot(FrameworkFacade, Request, GetFlags(this)));

    public virtual ActionResult GetMenus() => InitAndHandleErrors(SnapshotFactory.MenusSnapshot(FrameworkFacade, FrameworkFacade.GetMainMenus, Request, GetFlags(this)));

    public virtual ActionResult GetVersion() => InitAndHandleErrors(SnapshotFactory.VersionSnapshot(FrameworkFacade, GetOptionalCapabilities(), Request, GetFlags(this)));

    public virtual ActionResult GetService(string serviceName) => InitAndHandleErrors(SnapshotFactory.ServiceSnapshot(FrameworkFacade, serviceName, Request, GetFlags(this)));

    public virtual ActionResult GetMenu(string menuName) => InitAndHandleErrors(SnapshotFactory.MenuSnapshot(FrameworkFacade, menuName, Request, GetFlags(this)));

    public virtual ActionResult GetServiceAction(string serviceName, string actionName) => InitAndHandleErrors(SnapshotFactory.ActionSnapshot(FrameworkFacade, serviceName, actionName, Request, GetFlags(this)));

    public virtual ActionResult GetMenuAction(string menuName, string actionName) => InitAndHandleErrors(SnapshotFactory.MenuActionSnapshot(FrameworkFacade, menuName, actionName, Request, GetFlags(this)));

    public virtual ActionResult GetImage(string imageId) => InitAndHandleErrors(SnapshotFactory.ObjectSnapshot(FrameworkFacade, () => FrameworkFacade.GetImage(imageId), Request, GetFlags(this)));

    public virtual ActionResult GetObject(string domainType, string instanceId) => InitAndHandleErrors(SnapshotFactory.ObjectSnapshot(FrameworkFacade, () => FrameworkFacade.GetObjectByName(domainType, instanceId), Request, GetFlags(this)));

    public virtual ActionResult GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
        Func<RestSnapshot> PromptSnapshot() {
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            PropertyContextFacade PropertyContext() => FrameworkFacade.GetPropertyByName(domainType, instanceId, propertyName, argsContext);
            return SnapshotFactory.PromptSnaphot(FrameworkFacade, PropertyContext, Request, flags);
        }

        return InitAndHandleErrors(PromptSnapshot());
    }

    public virtual ActionResult PutPersistPropertyPrompt(string domainType, string propertyName, PromptArgumentMap promptArguments) {
        Func<RestSnapshot> PromptSnapshot() {
            var persistArgs = ProcessPromptArguments(promptArguments);
            var (promptArgs, flags) = ProcessArgumentMap(promptArguments, false, false);
            PropertyContextFacade PropertyContext() => FrameworkFacade.GetTransientPropertyByName(domainType, propertyName, persistArgs, promptArgs);
            return SnapshotFactory.PromptSnaphot(FrameworkFacade, PropertyContext, Request, flags);
        }

        return InitAndHandleErrors(PromptSnapshot());
    }

    public virtual ActionResult GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
        Func<RestSnapshot> PromptSnapshot() {
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            ParameterContextFacade ParameterContext() => FrameworkFacade.GetObjectParameterByName(domainType, instanceId, actionName, parmName, argsContext);
            return SnapshotFactory.PromptSnaphot(FrameworkFacade, ParameterContext, Request, flags);
        }

        return InitAndHandleErrors(PromptSnapshot());
    }

    public virtual ActionResult GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
        Func<RestSnapshot> PromptSnapshot() {
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            ParameterContextFacade ParameterContext() => FrameworkFacade.GetServiceParameterByName(serviceName, actionName, parmName, argsContext);
            return SnapshotFactory.PromptSnaphot(FrameworkFacade, ParameterContext, Request, flags);
        }

        return InitAndHandleErrors(PromptSnapshot());
    }

    public virtual ActionResult GetParameterPromptOnMenu(string menuName, string actionName, string parmName, ArgumentMap arguments) {
        Func<RestSnapshot> PromptSnapshot() {
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            ParameterContextFacade ParameterContext() => FrameworkFacade.GetMenuParameterByName(menuName, actionName, parmName, argsContext);
            return SnapshotFactory.PromptSnaphot(FrameworkFacade, ParameterContext, Request, flags);
        }

        return InitAndHandleErrors(PromptSnapshot());
    }

    public virtual ActionResult PutObject(string domainType, string instanceId, ArgumentMap arguments) {
        (Func<RestSnapshot>, bool) PutObject() {
            RejectRequestIfReadOnly(this);
            var (argsContext, flags) = ProcessArgumentMap(arguments, true, false);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.PutObjectAndValidate(domainType, instanceId, argsContext);
            return (SnapshotFactory.ObjectSnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(PutObject()));
    }

    public virtual ActionResult PostPersist(string domainType, PersistArgumentMap arguments) {
        (Func<RestSnapshot>, bool) PersistObject() {
            RejectRequestIfReadOnly(this);
            var (argsContext, flags) = ProcessPersistArguments(arguments);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.PersistObjectAndValidate(domainType, argsContext, flags);
            return (SnapshotFactory.ObjectSnapshot(FrameworkFacade, () => context, Request, flags, HttpStatusCode.Created), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(PersistObject()));
    }

    public virtual ActionResult GetProperty(string domainType, string instanceId, string propertyName) => InitAndHandleErrors(SnapshotFactory.PropertySnapshot(FrameworkFacade, () => FrameworkFacade.GetPropertyByName(domainType, instanceId, propertyName), Request, GetFlags(this)));

    public virtual ActionResult GetCollection(string domainType, string instanceId, string propertyName) => InitAndHandleErrors(SnapshotFactory.PropertySnapshot(FrameworkFacade, () => FrameworkFacade.GetCollectionPropertyByName(domainType, instanceId, propertyName), Request, GetFlags(this)));

    public virtual ActionResult GetCollectionValue(string domainType, string instanceId, string propertyName) => InitAndHandleErrors(SnapshotFactory.CollectionValueSnapshot(FrameworkFacade, () => FrameworkFacade.GetCollectionPropertyByName(domainType, instanceId, propertyName), Request, GetFlags(this)));

    public virtual ActionResult GetAction(string domainType, string instanceId, string actionName) => InitAndHandleErrors(SnapshotFactory.ActionSnapshot(FrameworkFacade, () => FrameworkFacade.GetObjectActionByName(domainType, instanceId, actionName), Request, GetFlags(this)));

    public virtual ActionResult PutProperty(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
        (Func<RestSnapshot>, bool) PutProperty() {
            RejectRequestIfReadOnly(this);
            var (argContext, flags) = ProcessArgument(argument);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.PutPropertyAndValidate(domainType, instanceId, propertyName, argContext);
            return (SnapshotFactory.PropertySnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(PutProperty()));
    }

    public virtual ActionResult DeleteProperty(string domainType, string instanceId, string propertyName) {
        (Func<RestSnapshot>, bool) DeleteProperty() {
            RejectRequestIfReadOnly(this);
            var (argContext, flags) = ProcessDeleteArgument();
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.DeletePropertyAndValidate(domainType, instanceId, propertyName, argContext);
            return (SnapshotFactory.PropertySnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(DeleteProperty()));
    }

    public virtual ActionResult PostCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) => StatusCode((int)HttpStatusCode.Forbidden);

    public virtual ActionResult DeleteCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) => StatusCode((int)HttpStatusCode.Forbidden);

    private ActionResult Invoke(string domainType, string instanceId, string actionName, ArgumentMap arguments, bool queryOnly) {
        (Func<RestSnapshot>, bool) Execute() {
            if (!queryOnly) {
                RejectRequestIfReadOnly(this);
            }

            var (argsContext, flags) = ProcessArgumentMap(arguments, false, queryOnly);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.ExecuteActionAndValidate(domainType, instanceId, actionName, argsContext);
            return (SnapshotFactory.ActionResultSnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(Execute()));
    }

    public virtual ActionResult GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => Invoke(domainType, instanceId, actionName, arguments, true);

    public virtual ActionResult PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => Invoke(domainType, instanceId, actionName, arguments, false);

    public virtual ActionResult PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => Invoke(domainType, instanceId, actionName, arguments, false);

    private ActionResult InvokeOnService(string serviceName, string actionName, ArgumentMap arguments, bool queryOnly) {
        (Func<RestSnapshot>, bool) Execute() {
            if (!queryOnly) {
                RejectRequestIfReadOnly(this);
            }

            // ignore concurrency always true here
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.ExecuteServiceActionAndValidate(serviceName, actionName, argsContext);
            return (SnapshotFactory.ActionResultSnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(Execute()));
    }

    private ActionResult InvokeOnMenu(string menuName, string actionName, ArgumentMap arguments, bool queryOnly) {
        (Func<RestSnapshot>, bool) Execute() {
            if (!queryOnly) {
                RejectRequestIfReadOnly(this);
            }

            // ignore concurrency always true here
            var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
            // seems strange to call and then wrap in lambda but need to validate here not when snapshot created
            var context = FrameworkFacade.ExecuteMenuActionAndValidate(menuName, actionName, argsContext);
            return (SnapshotFactory.ActionResultSnapshot(FrameworkFacade, () => context, Request, flags), flags.ValidateOnly);
        }

        return InitAndHandleErrors(() => SnapshotOrNoContent(Execute()));
    }

    public virtual ActionResult GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => InvokeOnService(serviceName, actionName, arguments, true);

    public virtual ActionResult PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => InvokeOnService(serviceName, actionName, arguments, false);

    public virtual ActionResult PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => InvokeOnService(serviceName, actionName, arguments, true);

    public virtual ActionResult GetInvokeOnMenu(string menuName, string actionName, ArgumentMap arguments) => InvokeOnMenu(menuName, actionName, arguments, true);

    public virtual ActionResult PutInvokeOnMenu(string menuName, string actionName, ArgumentMap arguments) => InvokeOnMenu(menuName, actionName, arguments, false);

    public virtual ActionResult PostInvokeOnMenu(string menuName, string actionName, ArgumentMap arguments) => InvokeOnMenu(menuName, actionName, arguments, false);

    public virtual ActionResult GetInvokeTypeActions(string typeName, string actionName, ArgumentMap arguments) {
        Func<RestSnapshot> GetTypeAction() => SnapshotFactory.TypeActionSnapshot(FrameworkFacade, () => GetIsTypeOf(actionName, typeName, arguments), Request, GetFlags(this));
        Func<RestSnapshot> NoSuchTypeAction() => () => throw new TypeActionResourceNotFoundException(actionName, typeName);

        Func<RestSnapshot> TypeAction() =>
            actionName switch {
                WellKnownIds.IsSubtypeOf => GetTypeAction(),
                WellKnownIds.IsSupertypeOf => GetTypeAction(),
                _ => NoSuchTypeAction()
            };

        return InitAndHandleErrors(TypeAction());
    }

    public virtual ActionResult InvalidMethod() => StatusCode((int)HttpStatusCode.MethodNotAllowed);

    #endregion

    #region helpers

    private Func<IDictionary<string, string>> GetOptionalCapabilities() => () => ControllerHelpers.GetOptionalCapabilities(config);

    private static string GetIfMatchTag(HttpRequest request) {
        var headers = request.GetTypedHeaders();

        if (headers.IfMatch.Any()) {
            var quotedTag = headers.IfMatch.First().Tag.ToString();
            return quotedTag.Replace("\"", "");
        }

        return null;
    }

    private void SetHeaders(RestSnapshot ss) {
        var msg = ControllerContext.HttpContext.Response;
        var responseHeaders = GetResponseHeaders();

        foreach (var w in ss.WarningHeaders) {
            AppendWarningHeader(responseHeaders, w.ToString());
        }

        foreach (var allowHeader in ss.AllowHeaders) {
            responseHeaders.Append(HeaderNames.Allow, allowHeader);
        }

        if (ss.Location != null) {
            responseHeaders.Location = ss.Location;
        }

        if (ss.Etag != null) {
            responseHeaders.ETag = ss.Etag;
        }

        var contentType = ss.Representation.GetContentType();

        var configuration = FrameworkFacade.GetScopedServiceProvider.GetService<IConfiguration>();
        var appVersion = configuration["AppVersion"];

        if (!string.IsNullOrEmpty(appVersion)) {
            contentType?.Parameters.Add(new NameValueHeaderValue("version", appVersion));
        }

        responseHeaders.ContentType = contentType;

        SetCaching(responseHeaders, ss, CacheSettings);

        ss.ValidateOutgoingMediaType(ss.Representation is AttachmentRepresentation);
        msg.StatusCode = (int)ss.HttpStatusCode;
    }

    private void Validate() {
        ValidateBinding(ModelState);
        ValidateDomainModel(DomainModel);
    }

    private ActionResult InitAndHandleErrors(Func<RestSnapshot> f) {
        var success = false;
        Exception endTransactionError = null;
        RestSnapshot ss;
        try {
            Validate();
            FrameworkFacade.Start();
            ss = f();
            success = true;
        }
        catch (ValidationException validationException) {
            logger.LogInformation(validationException, DisplayState);
            var warning = RestUtils.ToWarningHeaderValue(199, validationException.Message, logger);
            AppendWarningHeader(GetResponseHeaders(), warning.ToString());
            return StatusCode(validationException.StatusCode);
        }
        catch (RedirectionException redirectionException) {
            logger.LogInformation(redirectionException, DisplayState);
            var responseHeaders = ControllerContext.HttpContext.Response.GetTypedHeaders();
            responseHeaders.Location = redirectionException.RedirectAddress;
            return StatusCode(RedirectionException.StatusCode);
        }
        catch (NakedObjectsFacadeException e) {
            LogFacadeException(e)(DisplayState);
            return ErrorResult(e);
        }
        catch (Exception e) {
            logger.LogError(e, DisplayState);
            return ErrorResult(e);
        }
        finally {
            try {
                FrameworkFacade.End(success);
            }
            catch (Exception e) {
                // can't return from finally 
                endTransactionError = e;
            }
        }

        if (endTransactionError != null) {
            logger.LogError(endTransactionError, $"End transaction error : {DisplayState}");
            return ErrorResult(endTransactionError);
        }

        try {
            return RepresentationResult(ss);
        }
        catch (ValidationException validationException) {
            logger.LogInformation(validationException, DisplayState);
            var warning = RestUtils.ToWarningHeaderValue(199, validationException.Message, logger);
            AppendWarningHeader(GetResponseHeaders(), warning.ToString());
            return StatusCode(validationException.StatusCode);
        }
        catch (NakedObjectsFacadeException e) {
            LogFacadeException(e)(DisplayState);
            return ErrorResult(e);
        }
        catch (Exception e) {
            logger.LogError(e, DisplayState);
            return ErrorResult(e);
        }
    }

    private string DisplayName =>
        ControllerContext?.ActionDescriptor?.DisplayName ?? "Unknown Context";

    private string DisplayModelState =>
        ModelState?.Values.Select(v => v.AttemptedValue).Aggregate("", (a, s) => $"{a}{s};");

    private string DisplayState => $"Context: {DisplayName} State {DisplayModelState}";

    private Action<string> LogFacadeException(NakedObjectsFacadeException e) {
        return e switch {
            DataUpdateNOSException => s => logger.LogError(e, s),
            GeneralErrorNOSException => s => logger.LogError(e, s),
            NoContentNOSException => s => logger.LogInformation(e, s),
            NotAllowedNOSException => s => logger.LogWarning(e, s),
            PreconditionFailedNOSException => s => logger.LogWarning(e, s),
            PreconditionMissingNOSException => s => logger.LogError(e, s),
            ActionResourceNotFoundNOSException => s => logger.LogError(e, s),
            BadArgumentsNOSException => s => logger.LogWarning(e, s),
            BadRequestNOSException => s => logger.LogWarning(e, s),
            CollectionResourceNotFoundNOSException => s => logger.LogError(e, s),
            MenuResourceNotFoundNOSException => s => logger.LogError(e, s),
            ObjectResourceNotFoundNOSException => s => logger.LogError(e, s),
            PropertyResourceNotFoundNOSException => s => logger.LogError(e, s),
            ServiceResourceNotFoundNOSException => s => logger.LogError(e, s),
            TypeActionResourceNotFoundException => s => logger.LogError(e, s),
            TypeResourceNotFoundNOSException => s => logger.LogError(e, s),
            _ => s => logger.LogError(e, s)
        };
    }

    private ActionResult ErrorResult(Exception e) => RepresentationResult(SnapshotFactory.ErrorSnapshot(FrameworkFacade, e, Request, GetFlags(this))());

    private ResponseHeaders GetResponseHeaders() => ControllerContext.HttpContext.Response.GetTypedHeaders();

    private ActionResult RepresentationResult(RestSnapshot ss) {
        // there maybe better way of doing
        ActionResult FileResult(AttachmentRepresentation attachmentRepresentation) {
            var responseHeaders = GetResponseHeaders();
            responseHeaders.Append(HeaderNames.ContentDisposition, attachmentRepresentation.ContentDisposition.ToString());
            return File(attachmentRepresentation.AsStream, attachmentRepresentation.GetContentType().ToString());
        }

        ss.Populate(loggerFactory.CreateLogger<RestSnapshot>());
        SetHeaders(ss);

        return ss.Representation switch {
            NullRepresentation => new StatusCodeResult((int)ss.HttpStatusCode),
            AttachmentRepresentation attachmentRepresentation => FileResult(attachmentRepresentation),
            _ => new JsonResult(ss.Representation)
        };
    }

    private (object, RestControlFlags) ExtractValueAndFlags(SingleValueArgument argument, RestfulObjectsControllerBase controller) =>
        HandleMalformed(() => {
            ValidateArguments(argument);
            var flags = argument.ReservedArguments == null ? GetFlags(this) : GetFlags(argument, controller);
            var parm = argument.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy);
            return (parm, flags);
        });

    private IDictionary<string, object> ExtractValues(ArgumentMap arguments, bool errorIfNone) =>
        HandleMalformed(() => {
            ValidateArguments(arguments, errorIfNone);
            return arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
        });

    private (IDictionary<string, object>, RestControlFlags) ExtractValuesAndFlags(ArgumentMap arguments, bool errorIfNone) =>
        HandleMalformed(() => {
            ValidateArguments(arguments, errorIfNone);
            var dictionary = arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
            return (dictionary, GetFlags(arguments, this));
        });

    private (IDictionary<string, object>, RestControlFlags) ExtractValuesAndFlags(PromptArgumentMap arguments, bool errorIfNone) =>
        HandleMalformed(() => {
            ValidateArguments(arguments, errorIfNone);
            var dictionary = arguments.MemberMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
            return (dictionary, GetFlags(arguments, this));
        });

    private TypeActionInvokeContext GetIsTypeOf(string actionName, string typeName, ArgumentMap arguments) {
        ValidateArguments(arguments);
        var context = new TypeActionInvokeContext(actionName, typeName);
        if (!arguments.Map.ContainsKey(context.ParameterId)) {
            throw new BadRequestNOSException("Malformed arguments");
        }

        var thisSpecification = FrameworkFacade.GetDomainType(typeName);
        var parameter = arguments.Map[context.ParameterId];
        var value = parameter.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy);
        var otherSpecification = (ITypeFacade)(value is ITypeFacade ? value : FrameworkFacade.GetDomainType((string)value));
        context.ThisSpecification = thisSpecification;
        context.OtherSpecification = otherSpecification;
        return context;
    }

    private (ArgumentsContextFacade, RestControlFlags) ProcessPersistArguments(PersistArgumentMap persistArgumentMap) {
        var (dictionary, flags) = ExtractValuesAndFlags(persistArgumentMap, true);

        return (new ArgumentsContextFacade {
            Digest = GetIfMatchTag(Request),
            Values = dictionary,
            Page = flags.Page,
            PageSize = flags.PageSize,
            ValidateOnly = flags.ValidateOnly
        }, flags);
    }

    private ArgumentsContextFacade ProcessPromptArguments(PromptArgumentMap promptArgumentMap) {
        var (dictionary, flags) = ExtractValuesAndFlags(promptArgumentMap, false);

        return new ArgumentsContextFacade {
            Digest = GetIfMatchTag(Request),
            Values = dictionary,
            Page = flags.Page,
            PageSize = flags.PageSize,
            ValidateOnly = flags.ValidateOnly
        };
    }

    private (ArgumentsContextFacade, RestControlFlags) ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, bool ignoreConcurrency) {
        var (dictionary, flags) = arguments.ReservedArguments == null
            ? (ExtractValues(arguments, errorIfNone), GetFlags(this))
            : ExtractValuesAndFlags(arguments, errorIfNone);

        var facade = new ArgumentsContextFacade {
            Digest = ignoreConcurrency ? null : GetIfMatchTag(Request),
            Values = dictionary,
            ValidateOnly = flags.ValidateOnly,
            Page = flags.Page,
            PageSize = flags.PageSize,
            SearchTerm = arguments.ReservedArguments?.SearchTerm,
            ExpectedActionType = GetExpectedMethodType(new HttpMethod(Request.Method))
        };
        return (facade, flags);
    }

    private (ArgumentContextFacade, RestControlFlags) ProcessArgument(SingleValueArgument argument) {
        var (value, flags) = ExtractValueAndFlags(argument, this);
        return (new ArgumentContextFacade {
            Digest = GetIfMatchTag(Request),
            Value = value,
            ValidateOnly = flags.ValidateOnly
        }, flags);
    }

    private (ArgumentContextFacade, RestControlFlags) ProcessDeleteArgument() {
        var flags = GetFlags(this);
        return (new ArgumentContextFacade {
            Digest = GetIfMatchTag(Request),
            Value = null,
            ValidateOnly = flags.ValidateOnly
        }, flags);
    }

    #endregion
}