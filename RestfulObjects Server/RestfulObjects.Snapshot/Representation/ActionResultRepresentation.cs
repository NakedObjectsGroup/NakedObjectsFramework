// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ActionResultRepresentation : Representation {
        protected ActionResultRepresentation(HttpRequestMessage req, ActionResultContextSurface actionResult, RestControlFlags flags) : base(flags) {
            SelfRelType = new ActionResultRelType(RelValues.Self, new UriMtHelper(req, actionResult.ActionContext));
            SetResultType(actionResult);
            SetLinks(req, actionResult);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.ResultType)]
        public string ResultType { get; set; }

        private void SetHeader() {
            caching = CacheType.Transactional;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req, ActionResultContextSurface actionResult) {
            if (actionResult.ActionContext.Action.IsQueryOnly()) {
                Links = new[] {LinkRepresentation.Create(SelfRelType, Flags, new OptionalProperty(JsonPropertyNames.Arguments, CreateArguments(req, actionResult)))};
            }
            else {
                Links = new LinkRepresentation[] {};
            }
        }

        private void SetResultType(ActionResultContextSurface actionResult) {
            if (actionResult.Specification.IsParseable()) {
                ResultType = ResultTypes.Scalar;
            }
            else if (actionResult.Specification.IsCollection()) {
                ResultType = ResultTypes.List;
            }
            else {
                ResultType = actionResult.Specification.IsVoid() ? ResultTypes.Void : ResultTypes.Object;
            }
        }

        private MapRepresentation CreateArguments(HttpRequestMessage req, ActionResultContextSurface actionResult) {
            var optionalProperties = new List<OptionalProperty>();

            foreach (ParameterContextSurface visibleParamContext in actionResult.ActionContext.VisibleParameters) {
                IRepresentation value;

                if (visibleParamContext.Specification.IsParseable()) {
                    object proposedObj = visibleParamContext.ProposedNakedObject == null ? visibleParamContext.ProposedValue : visibleParamContext.ProposedNakedObject.Object;
                    object valueObj = RestUtils.ObjectToPredefinedType(proposedObj);
                    value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueObj));
                }
                else if (visibleParamContext.Specification.IsCollection()) {
                    if (visibleParamContext.ElementSpecification.IsParseable()) {
                        var proposedCollection = ((IEnumerable) (visibleParamContext.ProposedNakedObject == null ? visibleParamContext.ProposedValue : visibleParamContext.ProposedNakedObject.Object)).Cast<object>();
                        var valueObjs = proposedCollection.Select(RestUtils.ObjectToPredefinedType).ToArray();
                        value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueObjs));
                    }
                    else {
                        var refNos = visibleParamContext.ProposedNakedObject.ToEnumerable().Select(no => no).ToArray();
                        var refs = refNos.Select(no => RefValueRepresentation.Create(new ObjectRelType(RelValues.Self, new UriMtHelper(req, no)), Flags)).ToArray();

                        value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, refs));
                    }
                }
                else {
                    var valueRef = RefValueRepresentation.Create(new ObjectRelType(RelValues.Self, new UriMtHelper(req, visibleParamContext.ProposedNakedObject)), Flags);
                    value = MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, valueRef));
                }

                optionalProperties.Add(new OptionalProperty(visibleParamContext.Id, value));
            }
            return MapRepresentation.Create(optionalProperties.ToArray());
        }

        public static ActionResultRepresentation Create(HttpRequestMessage req, ActionResultContextSurface actionResult, RestControlFlags flags) {
            if (actionResult.HasResult) {
                IRepresentation result;

                if (actionResult.Result == null) {
                    result = null;
                }
                else if (actionResult.Specification.IsParseable()) {
                    result = ScalarRepresentation.Create(actionResult.Result, req, flags);
                }
                else if (actionResult.Specification.IsObject()) {
                    result = ObjectRepresentation.Create(actionResult.Result, req, flags);
                }
                else {
                    result = ListRepresentation.Create(actionResult, req, flags);
                }

                return CreateWithOptionals<ActionResultRepresentation>(new object[] {req, actionResult, flags}, new[] {new OptionalProperty(JsonPropertyNames.Result, result)});
            }
            return new ActionResultRepresentation(req, actionResult, flags);
        }
    }
}