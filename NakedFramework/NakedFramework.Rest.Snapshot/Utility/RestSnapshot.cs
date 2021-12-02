// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Error;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Error;
using NakedFramework.Rest.Snapshot.Representation;
using EntityTagHeaderValue = Microsoft.Net.Http.Headers.EntityTagHeaderValue;

namespace NakedFramework.Rest.Snapshot.Utility;

public class RestSnapshot {
    private readonly IList<string> allowHeaders = new List<string>();
    private readonly RestControlFlags flags;
    private readonly IOidStrategy oidStrategy;
    private readonly Action<ILogger> populator;
    private readonly HttpRequest requestMessage;
    private readonly IList<WarningHeaderValue> warningHeaders = new List<WarningHeaderValue>();

    private RestSnapshot(IOidStrategy oidStrategy, HttpRequest req, bool validateAsJson, RestControlFlags flags) {
        this.oidStrategy = oidStrategy;
        requestMessage = req;
        this.flags = flags;
        if (validateAsJson) {
            ValidateIncomingMediaTypeAsJson();
        }
    }

    private RestSnapshot(IOidStrategy oidStrategy, ContextFacade context, HttpRequest req, bool validateAsJson, RestControlFlags flags)
        : this(oidStrategy, req, validateAsJson, flags) {
        CheckForRedirection(oidStrategy, context, req);
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        : this(frameworkFacade.OidStrategy, objectContext, req, true, flags) {
        populator = logger => {
            HttpStatusCode = httpStatusCode;
            Representation = ObjectRepresentation.Create(frameworkFacade, objectContext, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, IMenuFacade menu, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        : this(frameworkFacade.OidStrategy, req, true, flags) {
        populator = logger => {
            HttpStatusCode = httpStatusCode;
            Representation = MenuRepresentation.Create(frameworkFacade, menu, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, ActionResultContextFacade actionResultContext, HttpRequest req, RestControlFlags flags)
        : this(frameworkFacade.OidStrategy, actionResultContext, req, true, flags) {
        populator = logger => {
            Representation = ActionResultRepresentation.Create(frameworkFacade, req, actionResultContext, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, ListContextFacade listContext, HttpRequest req, RestControlFlags flags)
        : this(frameworkFacade.OidStrategy, req, true, flags) {
        populator = logger => {
            Representation = ListRepresentation.Create(frameworkFacade, listContext, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, MenuContextFacade menus, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            Representation = ListRepresentation.Create(oidStrategy, menus, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            Representation = PromptRepresentation.Create(oidStrategy, propertyContext, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, ParameterContextFacade parmContext, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            Representation = PromptRepresentation.Create(oidStrategy, parmContext, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags, bool collectionValue)
        : this(frameworkFacade.OidStrategy, propertyContext, req, false, flags) {
        FilterBlobsAndClobs(propertyContext, flags);
        populator = logger => {
            if (collectionValue) {
                Representation = CollectionValueRepresentation.Create(oidStrategy, propertyContext, req, flags);
            }
            else {
                Representation = RequestingAttachment()
                    ? AttachmentRepresentation.Create(oidStrategy, req, propertyContext, flags)
                    : MemberAbstractRepresentation.Create(frameworkFacade, req, propertyContext, flags);
            }

            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, ActionContextFacade actionContext, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, actionContext, req, true, flags) {
        populator = logger => {
            Representation = ActionRepresentation.Create(oidStrategy, req, actionContext, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, HttpRequest req, RestControlFlags flags)
        : this(frameworkFacade.OidStrategy, req, true, flags) {
        populator = logger => {
            Representation = HomePageRepresentation.Create(frameworkFacade, req, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IFrameworkFacade frameworkFacade, IDictionary<string, string> capabilities, HttpRequest req, RestControlFlags flags)
        : this(frameworkFacade.OidStrategy, req, true, flags) {
        populator = logger => {
            Representation = VersionRepresentation.Create(frameworkFacade, req, capabilities, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, IPrincipal user, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            Representation = UserRepresentation.Create(oidStrategy, req, user, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, TypeActionInvokeContext typeActionInvokeContext, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            Representation = TypeActionInvokeRepresentation.Create(oidStrategy, req, typeActionInvokeContext, flags);
            SetHeaders(logger);
        };
    }

    public RestSnapshot(IOidStrategy oidStrategy, IFrameworkFacade frameworkFacade, Exception exception, HttpRequest req, RestControlFlags flags)
        : this(oidStrategy, req, true, flags) {
        populator = logger => {
            MapToHttpError(exception);
            MapToRepresentation(exception, req, frameworkFacade);
            MapToWarningHeader(exception, logger);
            SetHeaders(logger);
        };
    }

    public static Func<Func<string>, string> DebugFilter { get; set; }

    public IRepresentation Representation { get; private set; }

    public Uri Location { get; private set; }

    public WarningHeaderValue[] WarningHeaders => warningHeaders.ToArray();

    public string[] AllowHeaders => allowHeaders.ToArray();

    public HttpStatusCode HttpStatusCode { get; private set; } = HttpStatusCode.OK;

    public EntityTagHeaderValue Etag { get; private set; }

    private static void CheckForRedirection(IOidStrategy oidStrategy, ContextFacade context, HttpRequest req) {
        var ocs = context as ObjectContextFacade;
        var arcs = context as ActionResultContextFacade;
        var redirected = ocs?.Redirected ?? arcs?.Result?.Redirected;

        if (redirected != null) {
            var (serverName, oid) = redirected.Value;
            var redirectAddress = UriMtHelper.GetRedirectUri(req, serverName, oid);
            throw new RedirectionException(redirectAddress);
        }
    }

    private static void FilterBlobsAndClobs(PropertyContextFacade propertyContext, RestControlFlags flags) {
        if (!flags.BlobsClobs) {
            if (RestUtils.IsBlobOrClob(propertyContext.Specification) && !RestUtils.IsAttachment(propertyContext.Specification)) {
                throw new PropertyResourceNotFoundNOSException(propertyContext.Id);
            }
        }
    }

    public RestSnapshot Populate(ILogger logger) {
        try {
            populator(logger);
            return this;
        }
        catch (Exception e) {
            throw new GeneralErrorNOSException(e);
        }
    }

    private bool RequestingAttachment() {
        var headers = new RequestHeaders(requestMessage.Headers);

        var incomingMediaTypes = headers.Accept;

        return incomingMediaTypes.Any() &&
               !incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType.ToString()));
    }

    private void ValidateIncomingMediaTypeAsJson() {
        if (flags.AcceptHeaderStrict) {
            var headers = new RequestHeaders(requestMessage.Headers);
            var incomingMediaTypes = headers.Accept;

            if (!incomingMediaTypes.Any() || incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType.ToString()))) {
                return;
            }

            var msg = DebugFilter(() => $"Failed incoming MT validation: {string.Join(',', incomingMediaTypes.Select(mt => mt.MediaType.ToString()).ToArray())}");

            throw new ValidationException((int)HttpStatusCode.NotAcceptable, msg);
        }
    }

    public void ValidateOutgoingMediaType(bool isAttachment) {
        if (flags.AcceptHeaderStrict) {
            if (isAttachment) {
                ValidateOutgoingAttachmentMediaType();
            }
            else {
                ValidateOutgoingJsonMediaType();
            }
        }
    }

    private void ValidateOutgoingAttachmentMediaType() {
        var contentType = Representation.GetContentType();
        var headers = requestMessage.GetTypedHeaders();

        var incomingMediaTypes = headers.Accept.Select(a => a.MediaType).ToList();
        var outgoingMediaType = contentType?.MediaType ?? "";

        if (!incomingMediaTypes.Contains(outgoingMediaType)) {
            var msg = DebugFilter(() => $"Failed outgoing attachment MT validation ic: {string.Join(',', incomingMediaTypes.ToArray())} og: {outgoingMediaType}");

            throw new ValidationException((int)HttpStatusCode.NotAcceptable, msg);
        }
    }

    private void ValidateOutgoingJsonMediaType() {
        var contentType = Representation.GetContentType();
        var headers = requestMessage.GetTypedHeaders();
        var incomingParameters = headers.Accept.SelectMany(a => a.Parameters).ToList();

        var incomingProfiles = incomingParameters.Where(nv => nv.Name.ToString() == "profile").Select(nv => nv.Value).Distinct().ToArray();
        var outgoingProfiles = contentType != null ? contentType.Parameters.Where(nv => nv.Name == "profile").Select(nv => nv.Value).Distinct().ToArray() : Array.Empty<StringSegment>();

        if (incomingProfiles.Any() && outgoingProfiles.Any() && !outgoingProfiles.Intersect(incomingProfiles).Any()) {
            if (outgoingProfiles.Contains(UriMtHelper.GetJsonMediaType(RepresentationTypes.Error).Parameters.First().Value)) {
                // outgoing error so even though incoming profiles does not include error representation return error anyway but with 406 code
                HttpStatusCode = HttpStatusCode.NotAcceptable;
            }
            else {
                var msg = DebugFilter(() => $"Failed outgoing json MT validation ic: {string.Join(',', incomingProfiles.ToArray())} og: {string.Join(',', outgoingProfiles.ToArray())}");

                // outgoing profile not included in incoming profiles and not already an error so throw a 406
                throw new ValidationException((int)HttpStatusCode.NotAcceptable, msg);
            }
        }
    }

    private void MapToRepresentation(Exception e, HttpRequest req, IFrameworkFacade frameworkFacade) =>
        Representation = e switch {
            WithContextNOSException wce when wce.Contexts.Any(c => c.ErrorCause == Cause.Disabled || c.ErrorCause == Cause.Immutable) => NullRepresentation.Create(),
            BadPersistArgumentsException bpe when bpe.ContextFacade != null && bpe.Contexts.Any() => ArgumentsRepresentation.Create(oidStrategy, frameworkFacade, req, bpe.ContextFacade, bpe.Contexts, ArgumentsRepresentation.Format.Full, bpe.Flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments)),
            WithContextNOSException wce when wce.ContextFacade != null => ArgumentsRepresentation.Create(oidStrategy, frameworkFacade, req, wce.ContextFacade, ArgumentsRepresentation.Format.MembersOnly, RestControlFlags.DefaultFlags(), UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments)),
            WithContextNOSException wce when wce.Contexts.Any() => ArgumentsRepresentation.Create(oidStrategy, frameworkFacade, req, wce.Contexts, ArgumentsRepresentation.Format.MembersOnly, RestControlFlags.DefaultFlags(), UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments)),
            WithContextNOSException => NullRepresentation.Create(),
            ResourceNotFoundNOSException => NullRepresentation.Create(),
            NotAllowedNOSException => NullRepresentation.Create(),
            PreconditionFailedNOSException => NullRepresentation.Create(),
            PreconditionMissingNOSException => NullRepresentation.Create(),
            NoContentNOSException => NullRepresentation.Create(),
            _ => ErrorRepresentation.Create(oidStrategy, e)
        };

    private void SetHeaders(ILogger logger) {
        if (Representation != null) {
            Etag = Representation.GetEtag();

            foreach (var w in Representation.GetWarnings()) {
                warningHeaders.Add(RestUtils.ToWarningHeaderValue(299, w, logger));
            }

            if (HttpStatusCode == HttpStatusCode.Created) {
                Location = Representation.GetLocation();
            }
        }
    }

    private void MapToHttpError(Exception e) {
        const HttpStatusCode unprocessableEntity = (HttpStatusCode)422;
        const HttpStatusCode preconditionHeaderMissing = (HttpStatusCode)428;

        HttpStatusCode = e switch {
            ResourceNotFoundNOSException => HttpStatusCode.NotFound,
            BadArgumentsNOSException bre when bre.Contexts.Any(c => c.ErrorCause == Cause.Immutable) => HttpStatusCode.MethodNotAllowed,
            BadArgumentsNOSException bre when bre.Contexts.Any(c => c.ErrorCause == Cause.Disabled) => HttpStatusCode.Forbidden,
            BadArgumentsNOSException => unprocessableEntity,
            BadRequestNOSException => HttpStatusCode.BadRequest,
            NotAllowedNOSException => HttpStatusCode.MethodNotAllowed,
            NoContentNOSException => HttpStatusCode.NoContent,
            PreconditionFailedNOSException => HttpStatusCode.PreconditionFailed,
            PreconditionMissingNOSException => preconditionHeaderMissing,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private void MapToWarningHeader(Exception e, ILogger logger) {
        IList<string> ImmutableWarning() {
            allowHeaders.Add("GET");
            return new List<string> { "object is immutable" };
        }

        var warnings = e switch {
            ResourceNotFoundNOSException => new List<string> { e.Message },
            WithContextNOSException bae when bae.Contexts.Any(c => c.ErrorCause == Cause.Immutable) => ImmutableWarning(),
            WithContextNOSException bae when bae.Contexts.Any(c => !string.IsNullOrEmpty(c.Reason)) => bae.Contexts.Where(c => !string.IsNullOrEmpty(c.Reason)).Select(c => c.Reason).ToList(),
            WithContextNOSException bae when string.IsNullOrWhiteSpace(bae.Message) => new List<string>(),
            WithContextNOSException bae => new List<string> { bae.Message },
            NoContentNOSException => new List<string>(),
            _ => new List<string> { e.Message }
        };

        foreach (var w in warnings) {
            warningHeaders.Add(RestUtils.ToWarningHeaderValue(199, w, logger));
        }
    }
}