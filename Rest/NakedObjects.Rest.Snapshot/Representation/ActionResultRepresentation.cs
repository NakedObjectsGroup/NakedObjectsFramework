// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class ActionResultRepresentation : Representation {
        protected ActionResultRepresentation(IOidStrategy oidStrategy, HttpRequest req, ActionResultContextFacade actionResult, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new ActionResultRelType(RelValues.Self, new UriMtHelper(OidStrategy, req, actionResult.ActionContext));
            SetResultType(actionResult);
            SetLinks(req, actionResult);
            SetExtensions(actionResult);
            SetHeader(actionResult);
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.ResultType)]
        public string ResultType { get; set; }

        private void SetHeader(ActionResultContextFacade actionResult) {
            Caching = CacheType.Transactional;

            if (actionResult.Specification.IsObject && actionResult.Result != null) {
                if (actionResult.Result.Target.IsTransient) {
                    SetEtag(actionResult.TransientSecurityHash);
                }
                else {
                    SetEtag(actionResult.Result.Target);
                }
            }
        }

        private static void AddIfPresent(Dictionary<string, object> exts, string[] warningOrMessage, string type) {
            if (warningOrMessage?.Length > 0) {
                exts.Add(type, warningOrMessage);
            }
        }

        private void SetExtensions(ActionResultContextFacade actionResult) {
            var exts = new Dictionary<string, object>();

            AddIfPresent(exts, actionResult.Warnings, JsonPropertyNames.CustomWarnings);
            AddIfPresent(exts, actionResult.Messages, JsonPropertyNames.CustomMessages);

            Extensions = exts.Count > 0 ? RestUtils.CreateMap(exts) : new MapRepresentation();
        }

        private void SetLinks(HttpRequest req, ActionResultContextFacade actionResult) {
            Links = actionResult.ActionContext.Action.IsQueryOnly ? new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags, new OptionalProperty(JsonPropertyNames.Arguments, CreateArguments(req, actionResult)))} : new LinkRepresentation[] { };
        }

        private void SetResultType(ActionResultContextFacade actionResult) {
            if (actionResult.Specification.IsParseable) {
                ResultType = ResultTypes.Scalar;
            }
            else if (actionResult.Specification.IsCollection) {
                ResultType = ResultTypes.List;
            }
            else {
                ResultType = actionResult.Specification.IsVoid ? ResultTypes.Void : ResultTypes.Object;
            }
        }

        private MapRepresentation CreateArguments(HttpRequest req, ActionResultContextFacade actionResult) {
            var optionalProperties = new List<OptionalProperty>();

            foreach (var visibleParamContext in actionResult.ActionContext.VisibleParameters) {
                IRepresentation value;

                if (visibleParamContext.Specification.IsParseable) {
                    var proposedObj = visibleParamContext.ProposedObjectFacade == null ? visibleParamContext.ProposedValue : visibleParamContext.ProposedObjectFacade?.Object;
                    var valueObj = RestUtils.ObjectToPredefinedType(proposedObj);
                    value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueObj));
                }
                else if (visibleParamContext.Specification.IsCollection) {
                    if (visibleParamContext.ElementSpecification.IsParseable) {
                        var proposedEnumerable = (visibleParamContext.ProposedObjectFacade == null
                            ? visibleParamContext.ProposedValue
                            : visibleParamContext.ProposedObjectFacade?.Object) as IEnumerable;

                        var proposedCollection = proposedEnumerable == null ? new object[] { } : proposedEnumerable.Cast<object>();

                        var valueObjs = proposedCollection.Select(i => RestUtils.ObjectToPredefinedType(i)).ToArray();
                        value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueObjs));
                    }
                    else {
                        var refNos = visibleParamContext.ProposedObjectFacade.ToEnumerable().Select(no => no).ToArray();
                        var refs = refNos.Select(no => RefValueRepresentation.Create(OidStrategy, new ObjectRelType(RelValues.Self, new UriMtHelper(OidStrategy, req, no)), Flags)).ToArray();

                        value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, refs));
                    }
                }
                else {
                    RefValueRepresentation valueRef = null;
                    if (visibleParamContext.ProposedObjectFacade != null) {
                        valueRef = RefValueRepresentation.Create(OidStrategy, new ObjectRelType(RelValues.Self, new UriMtHelper(OidStrategy, req, visibleParamContext.ProposedObjectFacade)), Flags);
                    }

                    value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueRef));
                }

                optionalProperties.Add(new OptionalProperty(visibleParamContext.Id, value));
            }

            return MapRepresentation.Create(optionalProperties.ToArray());
        }

        public static ActionResultRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, ActionResultContextFacade actionResult, RestControlFlags flags) {
            if (!actionResult.HasResult) {
                return new ActionResultRepresentation(oidStrategy, req, actionResult, flags);
            }

            IRepresentation result = actionResult switch {
                _ when actionResult.Result == null => null,
                _ when actionResult.Specification.IsParseable => ScalarRepresentation.Create(oidStrategy, actionResult.Result, req, flags),
                _ when actionResult.Specification.IsObject => ObjectRepresentation.Create(oidStrategy, actionResult.Result, req, flags),
                _ => PagedListRepresentation.Create(oidStrategy, actionResult, req, flags)
            };

            return CreateWithOptionals<ActionResultRepresentation>(new object[] {oidStrategy, req, actionResult, flags}, new[] {new OptionalProperty(JsonPropertyNames.Result, result)});
        }
    }
}