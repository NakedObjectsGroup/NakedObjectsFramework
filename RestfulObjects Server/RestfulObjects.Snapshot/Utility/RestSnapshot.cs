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
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;

namespace RestfulObjects.Snapshot.Utility {
    public class RestSnapshot {
        private readonly IList<string> allowHeaders = new List<string>();
        private readonly ILog logger = LogManager.GetLogger<RestSnapshot>();
        private readonly Action populator;
        private readonly IOidStrategy oidStrategy;
        private readonly HttpRequestMessage requestMessage;
        private readonly IList<WarningHeaderValue> warningHeaders = new List<WarningHeaderValue>();

        private HttpStatusCode httpStatusCode = HttpStatusCode.OK;
        private Representation representation;

        static RestSnapshot() {
            AcceptHeaderStrict = true;
        }

        private RestSnapshot(IOidStrategy oidStrategy, HttpRequestMessage req, bool validateAsJson) {
            this.oidStrategy = oidStrategy;
            requestMessage = req;
            if (validateAsJson) {
                ValidateIncomingMediaTypeAsJson();
            }
        }

        private RestSnapshot(IOidStrategy oidStrategy, ContextFacade context, HttpRequestMessage req, bool validateAsJson)
            : this(oidStrategy, req, validateAsJson) {
            logger.DebugFormat("RestSnapshot:{0}", context.GetType().FullName);
            CheckForRedirection(oidStrategy, context, req);
        }

        public RestSnapshot(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
            : this(oidStrategy,objectContext, req, true) {
            populator = () => {
                this.httpStatusCode = httpStatusCode;
                representation = ObjectRepresentation.Create(oidStrategy, objectContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,actionResultContext, req, true) {
            populator = () => {
                representation = ActionResultRepresentation.Create(oidStrategy ,req, actionResultContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:ServicesList");
            populator = () => {
                representation = ListRepresentation.Create(oidStrategy ,listContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IMenuFacade[] menus, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            logger.DebugFormat("RestSnapshot:MenuList");
            populator = () => {
                representation = ListRepresentation.Create(oidStrategy, menus, req, flags);
                SetHeaders();
            };
        }


        public RestSnapshot(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:propertyprompt");
            populator = () => {
                representation = PromptRepresentation.Create(oidStrategy ,propertyContext, listContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ParameterContextFacade parmContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:parameterprompt");
            populator = () => {
                representation = PromptRepresentation.Create(oidStrategy, parmContext, listContext, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequestMessage req, RestControlFlags flags, bool value = false)
            : this(oidStrategy,propertyContext, req, false) {
            FilterBlobsAndClobs(propertyContext, flags);
            populator = () => {
                if (value) {
                    representation = CollectionValueRepresentation.Create(oidStrategy ,propertyContext, req, flags);
                }
                else {
                    representation = RequestingAttachment() ? AttachmentRepresentation.Create(oidStrategy ,req, propertyContext, flags) :
                        MemberAbstractRepresentation.Create(oidStrategy ,req, propertyContext, flags);
                }
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, PropertyTypeContextFacade propertyTypeContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:{0}", propertyTypeContext.GetType().FullName);
            populator = () => {
                representation = MemberTypeRepresentation.Create(oidStrategy ,req, propertyTypeContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ActionContextFacade actionContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,actionContext, req, true) {
            populator = () => {
                representation = ActionRepresentation.Create(oidStrategy ,req, actionContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ActionTypeContextFacade actionTypeContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:{0}", actionTypeContext.GetType().FullName);

            populator = () => {
                representation = ActionTypeRepresentation.Create(oidStrategy ,req, actionTypeContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ParameterTypeContextFacade parameterTypeContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:{0}", parameterTypeContext.GetType().FullName);

            populator = () => {
                representation = ParameterTypeRepresentation.Create(oidStrategy ,req, parameterTypeContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:Home");

            populator = () => {
                representation = HomePageRepresentation.Create(oidStrategy, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IDictionary<string, string> capabilities, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:Version");

            populator = () => {
                representation = VersionRepresentation.Create(oidStrategy, req, capabilities, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, IPrincipal user, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:User");


            populator = () => {
                representation = UserRepresentation.Create(oidStrategy, req, user, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ITypeFacade[] specs, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:TypeList");


            populator = () => {
                representation = ListRepresentation.Create(oidStrategy ,specs, req, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, ITypeFacade spec, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:DomainType");


            populator = () => {
                representation = DomainTypeRepresentation.Create(oidStrategy ,req, spec, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, TypeActionInvokeContext typeActionInvokeContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:{0}", typeActionInvokeContext.GetType().FullName);

            populator = () => {
                representation = TypeActionInvokeRepresentation.Create(oidStrategy, req, typeActionInvokeContext, flags);
                SetHeaders();
            };
        }

        public RestSnapshot(IOidStrategy oidStrategy, FilterFromInvokeContext filterFromInvokeContext, HttpRequestMessage req, RestControlFlags flags)
            : this(oidStrategy, req, true) {
            logger.DebugFormat("RestSnapshot:{0}", filterFromInvokeContext.GetType().FullName);

            populator = () => {
                representation = FilterFromInvokeRepresentation.Create(oidStrategy, req, filterFromInvokeContext, flags);
                SetHeaders();
            };
        }


        public RestSnapshot(IOidStrategy oidStrategy, Exception exception, HttpRequestMessage req)
            : this(oidStrategy,req, true) {
            logger.DebugFormat("RestSnapshot:Exception");

            populator = () => {
                MapToHttpError(exception);
                MapToRepresentation(exception, req);
                MapToWarningHeader(exception);
                SetHeaders();
            };
        }


        public static bool AcceptHeaderStrict { get; set; }

        public Representation Representation {
            get { return representation; }
        }

        public Uri Location { get; set; }

        public WarningHeaderValue[] WarningHeaders {
            get { return warningHeaders.ToArray(); }
        }

        public string[] AllowHeaders {
            get { return allowHeaders.ToArray(); }
        }

        public HttpStatusCode HttpStatusCode {
            get { return httpStatusCode; }
            private set { httpStatusCode = value; }
        }

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

        private static void CheckForRedirection(IOidStrategy oidStrategy, ContextFacade context, HttpRequestMessage req) {
            var ocs = context as ObjectContextFacade;
            var arcs = context as ActionResultContextFacade;
            string url = (ocs != null ? ocs.RedirectedUrl : null) ?? (arcs != null && arcs.Result != null ? arcs.Result.RedirectedUrl : null);

            if (url != null) {
                Uri redirectAddress = new UriMtHelper(oidStrategy, req).GetRedirectUri(req, url);
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
            catch (HttpResponseException e) {
                logger.DebugFormat("HttpResponse exception being passed up {0}", e.Message);
                throw;
            }
            catch (Exception e) {
                throw new GeneralErrorNOSException(e);
            }
        }

        public bool RequestingAttachment() {
            HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> incomingMediaTypes = requestMessage.Headers.Accept;

            if (!incomingMediaTypes.Any() || incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType))) {
                return false;
            }

            return true;
        }

        public void ValidateIncomingMediaTypeAsJson() {
            if (AcceptHeaderStrict) {
                HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> incomingMediaTypes = requestMessage.Headers.Accept;

                if (!incomingMediaTypes.Any() || incomingMediaTypes.Any(mt => RestUtils.IsJsonMediaType(mt.MediaType))) {
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

            List<string> incomingMediaTypes = requestMessage.Headers.Accept.Select(a => a.MediaType).ToList();
            string outgoingMediaType = contentType == null ? "" : contentType.MediaType;

            if (!incomingMediaTypes.Contains(outgoingMediaType)) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            }
        }

        private void ValidateOutgoingJsonMediaType() {
            var contentType = Representation.GetContentType();
            List<NameValueHeaderValue> incomingParameters = requestMessage.Headers.Accept.SelectMany(a => a.Parameters).ToList();

            string[] incomingProfiles = incomingParameters.Where(nv => nv.Name == "profile").Select(nv => nv.Value).Distinct().ToArray();
            string[] outgoingProfiles = contentType != null ? contentType.Parameters.Where(nv => nv.Name == "profile").Select(nv => nv.Value).Distinct().ToArray() : new string[] {};

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

        private void MapToRepresentation(Exception e, HttpRequestMessage req) {
            if (e is WithContextNOSException) {
                ArgumentsRepresentation.Format format = e is BadPersistArgumentsException ? ArgumentsRepresentation.Format.Full : ArgumentsRepresentation.Format.MembersOnly;
                RestControlFlags flags = e is BadPersistArgumentsException ? ((BadPersistArgumentsException) e).Flags : RestControlFlags.DefaultFlags();

                var contextNosException = e as WithContextNOSException;

                if (contextNosException.Contexts.Any(c => c.ErrorCause == Cause.Disabled || c.ErrorCause == Cause.Immutable)) {
                    representation = NullRepresentation.Create();
                }
                else if (contextNosException.ContextFacade != null) {
                    representation = ArgumentsRepresentation.Create(oidStrategy , req, contextNosException.ContextFacade, format, flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments));
                }
                else if (contextNosException.Contexts.Any()) {
                    representation = ArgumentsRepresentation.Create(oidStrategy, req, contextNosException.Contexts, format, flags, UriMtHelper.GetJsonMediaType(RepresentationTypes.BadArguments));
                }
                else {
                    representation = NullRepresentation.Create();
                }
            }
            else if (e is ResourceNotFoundNOSException ||
                     e is NotAllowedNOSException ||
                     e is PreconditionFailedNOSException ||
                     e is PreconditionHeaderMissingNOSException ||
                     e is NoContentNOSException) {
                representation = NullRepresentation.Create();
            }
            else {
                representation = ErrorRepresentation.Create(oidStrategy, e);
            }
        }

        private void SetHeaders() {
            if (representation != null) {
                //ContentType = representation.GetContentType();
                Etag = representation.GetEtag();
                //Caching = representation.GetCaching();

                foreach (string w in representation.GetWarnings()) {
                    warningHeaders.Add(new WarningHeaderValue(299, "RestfulObjects", "\"" + w + "\""));
                }

                if (httpStatusCode == HttpStatusCode.Created) {
                    Location = representation.GetLocation();
                }
            }
        }

        private void MapToHttpError(Exception e) {
            const HttpStatusCode unprocessableEntity = (HttpStatusCode) 422;
            const HttpStatusCode preconditionHeaderMissing = (HttpStatusCode) 428;

            if (e is ResourceNotFoundNOSException) {
                httpStatusCode = HttpStatusCode.NotFound;
            }
            else if (e is BadArgumentsNOSException) {
                var bre = e as BadArgumentsNOSException;

                if (bre.Contexts.Any(c => c.ErrorCause == Cause.Immutable)) {
                    httpStatusCode = HttpStatusCode.MethodNotAllowed;
                }
                else if (bre.Contexts.Any(c => c.ErrorCause == Cause.Disabled)) {
                    httpStatusCode = HttpStatusCode.Forbidden;
                }
                else {
                    httpStatusCode = unprocessableEntity;
                }
            }
            else if (e is BadRequestNOSException) {
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            else if (e is NotAllowedNOSException) {
                httpStatusCode = HttpStatusCode.MethodNotAllowed;
            }
            else if (e is NoContentNOSException) {
                httpStatusCode = HttpStatusCode.NoContent;
            }
            else if (e is PreconditionFailedNOSException) {
                httpStatusCode = HttpStatusCode.PreconditionFailed;
            }
            else if (e is PreconditionHeaderMissingNOSException) {
                httpStatusCode = preconditionHeaderMissing;
            }
            else {
                httpStatusCode = HttpStatusCode.InternalServerError;
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
                else if (bae.Contexts.Any()) {
                    foreach (string w in bae.Contexts.Where(c => !String.IsNullOrEmpty(c.Reason)).Select(c => c.Reason)) {
                        warnings.Add(w);
                    }
                }
                else {
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