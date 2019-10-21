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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web.Http;
using Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;

namespace NakedObjects.Rest.Snapshot.Utility {
    public class RestSnapshot {
        private readonly IList<string> allowHeaders = new List<string>();
        private readonly ILog logger = LogManager.GetLogger<RestSnapshot>();
        private readonly IOidStrategy oidStrategy;
        private readonly Action populator;
        private readonly HttpRequest requestMessage;
        private readonly IList<WarningHeaderValue> warningHeaders = new List<WarningHeaderValue>();

        static RestSnapshot() {
            AcceptHeaderStrict = true;
        }

        private RestSnapshot(IOidStrategy oidStrategy, HttpRequest req, bool validateAsJson) {
            this.oidStrategy = oidStrategy;
            requestMessage = req;
            if (validateAsJson) {
                ValidateIncomingMediaTypeAsJson();
            }
        }

        private RestSnapshot(IOidStrategy oidStrategy, ContextFacade context, HttpRequest req, bool validateAsJson)
            : this(oidStrategy, req, validateAsJson) {
            CheckForRedirection(oidStrategy, context, req);
        }

        public RestSnapshot(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
            : this(oidStrategy, objectContext, req, true) {
            populator = () => {
                HttpStatusCode = httpStatusCode;
                Representation = ObjectRepresentation.Create(oidStrategy, objectContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IMenuFacade menu, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
            : this(oidStrategy, req, true) {
            populator = () => {
                HttpStatusCode = httpStatusCode;
                Representation = MenuRepresentation.Create(oidStrategy, menu, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, actionResultContext, req, true) {
            populator = () => {
                Representation = ActionResultRepresentation.Create(oidStrategy, req, actionResultContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            populator = () => {
                Representation = ListRepresentation.Create(oidStrategy, listContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, MenuContextFacade menus, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            populator = () => {
                Representation = ListRepresentation.Create(oidStrategy, menus, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            populator = () => {
                Representation = PromptRepresentation.Create(oidStrategy, propertyContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ParameterContextFacade parmContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            populator = () => {
                Representation = PromptRepresentation.Create(oidStrategy, parmContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags, bool value)
            : this(oidStrategy, propertyContext, req, false) {
            FilterBlobsAndClobs(propertyContext, flags);
            populator = () => {
                if (value) {
                    Representation = CollectionValueRepresentation.Create(oidStrategy, propertyContext, req, flags);
                }
                else {
                    Representation = RequestingAttachment() ? AttachmentRepresentation.Create(oidStrategy, req, propertyContext, flags) :
                        MemberAbstractRepresentation.Create(oidStrategy, req, propertyContext, flags);
                }
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ActionContextFacade actionContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, actionContext, req, true) {
            populator = () => {
                Representation = ActionRepresentation.Create(oidStrategy, req, actionContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {

            populator = () => {
                Representation = HomePageRepresentation.Create(oidStrategy, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IDictionary<string, string> capabilities, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {

            populator = () => {
                Representation = VersionRepresentation.Create(oidStrategy, req, capabilities, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IPrincipal user, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {

            populator = () => {
                Representation = UserRepresentation.Create(oidStrategy, req, user, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, TypeActionInvokeContext typeActionInvokeContext, HttpRequest req, RestControlFlags flags)
            : this(oidStrategy, req, true) {

            populator = () => {
                Representation = TypeActionInvokeRepresentation.Create(oidStrategy, req, typeActionInvokeContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, Exception exception, HttpRequest req)
            : this(oidStrategy, req, true) {

            populator = () => {
                MapToHttpError(exception);
                MapToRepresentation(exception, req);
                MapToWarningHeader(exception);
                SetHeaders();
            };
        }

        public static bool AcceptHeaderStrict { get; set; }

        public Representation Representation { get; private set; }

        public Uri Location { get; set; }

        public WarningHeaderValue[] WarningHeaders => warningHeaders.ToArray();

        public string[] AllowHeaders => allowHeaders.ToArray();

        public HttpStatusCode HttpStatusCode { get; private set; } = HttpStatusCode.OK;

        public EntityTagHeaderValue Etag { get; set; }

        public HttpResponseMessage ConfigureMsg(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings) {
            HttpResponseMessage msg = Representation.GetAsMessage(formatter, cacheSettings);

            foreach (WarningHeaderValue w in WarningHeaders) {
                msg.Headers.Warning.Add(w);
            }

            foreach (string a in AllowHeaders) {
                msg.Content.Headers.Allow.Add(a);
            }

            if (Location != null) {
                msg.Headers.Location = Location;
            }

            if (Etag != null) {
                msg.Headers.ETag = Etag;
            }

            ValidateOutgoingMediaType(Representation is AttachmentRepresentation);
            msg.StatusCode = HttpStatusCode;

            return msg;
        }

        private static void CheckForRedirection(IOidStrategy oidStrategy, ContextFacade context, HttpRequest req) {
            var ocs = context as ObjectContextFacade;
            var arcs = context as ActionResultContextFacade;
            Tuple<string, string> redirected = (ocs != null ? ocs.Redirected : null) ?? (arcs?.Result != null ? arcs.Result.Redirected : null);

            if (redirected != null) {
                Uri redirectAddress = new UriMtHelper(oidStrategy, req).GetRedirectUri(req, redirected.Item1, redirected.Item2);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MovedPermanently) {Headers = {Location = redirectAddress}});
            }
        }

        private static void FilterBlobsAndClobs(PropertyContextFacade propertyContext, RestControlFlags flags) {
            if (!flags.BlobsClobs) {
                if (RestUtils.IsBlobOrClob(propertyContext.Specification) && !RestUtils.IsAttachment(propertyContext.Specification)) {
                    throw new PropertyResourceNotFoundNOSException(propertyContext.Id);
                }
            }
        }

        public RestSnapshot Populate() {
            try {
                populator();
                return this;
            }
            catch (HttpResponseException) {
                throw;
            }
            catch (Exception e) {
                throw new GeneralErrorNOSException(e);
            }
        }

        public bool RequestingAttachment() {
            var headers = new RequestHeaders(requestMessage.Headers);

            var incomingMediaTypes = headers.Accept;

            if (!incomingMediaTypes.Any() || incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType.ToString()))) {
                return false;
            }

            return true;
        }

        public void ValidateIncomingMediaTypeAsJson() {
            if (AcceptHeaderStrict) {
                var headers = new RequestHeaders(requestMessage.Headers);
                var incomingMediaTypes = headers.Accept;

                if (!incomingMediaTypes.Any() || incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType.ToString()))) {
                    return;
                }

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            }
        }

        public void ValidateOutgoingMediaType(bool isAttachment) {
            if (AcceptHeaderStrict) {
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
            var headers = new RequestHeaders(requestMessage.Headers);

            var incomingMediaTypes = headers.Accept.Select(a => a.MediaType).ToList();
            string outgoingMediaType = contentType == null ? "" : contentType.MediaType;

            if (!incomingMediaTypes.Contains(outgoingMediaType)) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            }
        }

        private void ValidateOutgoingJsonMediaType() {
            var contentType = Representation.GetContentType();
            var headers = new RequestHeaders(requestMessage.Headers);
            var incomingParameters = headers.Accept.SelectMany(a => a.Parameters).ToList();

            string[] incomingProfiles = incomingParameters.Where(nv => nv.Name.ToString() == "profile").Select(nv => nv.Value.ToString()).Distinct().ToArray();
            string[] outgoingProfiles = contentType != null ? contentType.Parameters.Where(nv => nv.Name == "profile").Select(nv => nv.Value).Distinct().ToArray() : new string[] { };

            if (incomingProfiles.Any() && outgoingProfiles.Any() && !outgoingProfiles.Intersect(incomingProfiles).Any()) {
                if (outgoingProfiles.Contains(UriMtHelper.GetJsonMediaType(RepresentationTypes.Error).Parameters.First().Value)) {
                    // outgoing error so even though incoming profiles does not include error representation return error anyway but with 406 code
                    HttpStatusCode = HttpStatusCode.NotAcceptable;
                }
                else {
                    // outgoing profile not included in incoming profiles and not already an error so throw a 406
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                }
            }
        }

        private void MapToRepresentation(Exception e, HttpRequest req) {
            if (e is WithContextNOSException) {
                ArgumentsRepresentation.Format format = e is BadPersistArgumentsException ? ArgumentsRepresentation.Format.Full : ArgumentsRepresentation.Format.MembersOnly;
                RestControlFlags flags = e is BadPersistArgumentsException ? ((BadPersistArgumentsException) e).Flags : RestControlFlags.DefaultFlags();

                var contextNosException = e as WithContextNOSException;

                if (contextNosException.Contexts.Any(c => c.ErrorCause == Cause.Disabled || c.ErrorCause == Cause.Immutable)) {
                    Representation = NullRepresentation.Create();
                }
                else if (e is BadPersistArgumentsException && contextNosException.ContextFacade != null && contextNosException.Contexts.Any()) {
                    Representation = ArgumentsRepresentation.Create(oidStrategy, req, contextNosException.ContextFacade, contextNosException.Contexts, format, flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments));
                }
                else if (contextNosException.ContextFacade != null) {
                    Representation = ArgumentsRepresentation.Create(oidStrategy, req, contextNosException.ContextFacade, format, flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments));
                }
                else if (contextNosException.Contexts.Any()) {
                    Representation = ArgumentsRepresentation.Create(oidStrategy, req, contextNosException.Contexts, format, flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments));
                }
                else {
                    Representation = NullRepresentation.Create();
                }
            }
            else if (e is ResourceNotFoundNOSException ||
                     e is NotAllowedNOSException ||
                     e is PreconditionFailedNOSException ||
                     e is PreconditionMissingNOSException ||
                     e is NoContentNOSException) {
                Representation = NullRepresentation.Create();
            }
            else {
                Representation = ErrorRepresentation.Create(oidStrategy, e);
            }
        }

        private void SetHeaders() {
            if (Representation != null) {
                //ContentType = representation.GetContentType();
                Etag = Representation.GetEtag();
                //Caching = representation.GetCaching();

                foreach (string w in Representation.GetWarnings()) {
                    warningHeaders.Add(new WarningHeaderValue(299, "RestfulObjects", "\"" + w + "\""));
                }

                if (HttpStatusCode == HttpStatusCode.Created) {
                    Location = Representation.GetLocation();
                }
            }
        }

        private void MapToHttpError(Exception e) {
            const HttpStatusCode unprocessableEntity = (HttpStatusCode) 422;
            const HttpStatusCode preconditionHeaderMissing = (HttpStatusCode) 428;

            if (e is ResourceNotFoundNOSException) {
                HttpStatusCode = HttpStatusCode.NotFound;
            }
            else if (e is BadArgumentsNOSException) {
                var bre = e as BadArgumentsNOSException;

                if (bre.Contexts.Any(c => c.ErrorCause == Cause.Immutable)) {
                    HttpStatusCode = HttpStatusCode.MethodNotAllowed;
                }
                else if (bre.Contexts.Any(c => c.ErrorCause == Cause.Disabled)) {
                    HttpStatusCode = HttpStatusCode.Forbidden;
                }
                else {
                    HttpStatusCode = unprocessableEntity;
                }
            }
            else if (e is BadRequestNOSException) {
                HttpStatusCode = HttpStatusCode.BadRequest;
            }
            else if (e is NotAllowedNOSException) {
                HttpStatusCode = HttpStatusCode.MethodNotAllowed;
            }
            else if (e is NoContentNOSException) {
                HttpStatusCode = HttpStatusCode.NoContent;
            }
            else if (e is PreconditionFailedNOSException) {
                HttpStatusCode = HttpStatusCode.PreconditionFailed;
            }
            else if (e is PreconditionMissingNOSException) {
                HttpStatusCode = preconditionHeaderMissing;
            }
            else {
                HttpStatusCode = HttpStatusCode.InternalServerError;
            }
        }

        private void MapToWarningHeader(Exception e) {
            IList<string> warnings = new List<string>();

            if (e is ResourceNotFoundNOSException) {
                warnings.Add(e.Message);
            }
            else if (e is WithContextNOSException) {
                var bae = e as WithContextNOSException;

                if (bae.Contexts.Any(c => c.ErrorCause == Cause.Immutable)) {
                    warnings.Add("object is immutable");
                    allowHeaders.Add("GET");
                }
                else if (bae.Contexts.Any(c => !string.IsNullOrEmpty(c.Reason))) {
                    foreach (string w in bae.Contexts.Where(c => !string.IsNullOrEmpty(c.Reason)).Select(c => c.Reason)) {
                        warnings.Add(w);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(bae.Message)) {
                    warnings.Add(bae.Message);
                }
            }
            else if (e is NoContentNOSException) {
                // do nothing 
            }
            else {
                warnings.Add(e.Message);
            }

            foreach (string w in warnings) {
                try {
                    // remove all \" within warning message as they cause format exception 
                    warningHeaders.Add(new WarningHeaderValue(199, "RestfulObjects", "\"" + w.Replace('"', ' ') + "\""));
                }
                catch (FormatException fe) {
                    logger.WarnFormat("Failed to parse warning message: {0} : {1}", w, fe.Message);
                    warningHeaders.Add(new WarningHeaderValue(199, "RestfulObjects", "\"" + "Failed to parse warning message" + "\""));
                }
            }
        }
    }
}