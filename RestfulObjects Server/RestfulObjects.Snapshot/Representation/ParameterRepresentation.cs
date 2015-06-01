// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ParameterRepresentation : Representation {
        protected ParameterRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade nakedObject, INakedObjectActionParameterSurface parameter, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetName(parameter);
            SetExtensions(req, nakedObject, parameter, flags);
            SetLinks(req, nakedObject, parameter);
            SetHeader(nakedObject);
        }

        internal string Name { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader(IObjectFacade nakedObject) {
            SetEtag(nakedObject);
        }

        private void SetName(INakedObjectActionParameterSurface parameter) {
            Name = parameter.Id;
        }

        private LinkRepresentation CreatePromptLink(HttpRequestMessage req, IObjectFacade nakedObject, INakedObjectActionParameterSurface parameter) {
            var opts = new List<OptionalProperty>();

            var parameterContext = new ParameterContextSurface {
                Action = parameter.Action,
                Target = nakedObject,
                Parameter = parameter
            };

            if (parameter.IsAutoCompleteEnabled) {
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))));
                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, parameter.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                Tuple<string, INakedObjectSpecificationSurface>[] parms = parameter.GetChoicesParameters();
                OptionalProperty[] args = parms.Select(tuple => RestUtils.CreateArgumentProperty(OidStrategy ,req, tuple, Flags)).ToArray();
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy ,new PromptRelType(new UriMtHelper(OidStrategy ,req, parameterContext)), Flags, opts.ToArray());
        }

        private void SetLinks(HttpRequestMessage req, IObjectFacade nakedObject, INakedObjectActionParameterSurface parameter) {
            var tempLinks = new List<LinkRepresentation>();


            if (Flags.FormalDomainModel) {
                var parameterTypeContextSurface = new ParameterTypeContextSurface {
                    Action = parameter.Action,
                    OwningSpecification = nakedObject.Specification,
                    Parameter = parameter
                };
                LinkRepresentation describedBy = LinkRepresentation.Create(OidStrategy ,new ParamTypeRelType(RelValues.DescribedBy, new UriMtHelper(OidStrategy, req, parameterTypeContextSurface)), Flags);
                tempLinks.Add(describedBy);
            }

            if (parameter.IsAutoCompleteEnabled || parameter.GetChoicesParameters().Any()) {
                tempLinks.Add(CreatePromptLink(req, nakedObject, parameter));
            }

            Links = tempLinks.ToArray();
        }

        private void SetExtensions(HttpRequestMessage req, IObjectFacade nakedObject, INakedObjectActionParameterSurface parameter, RestControlFlags flags) {
            IDictionary<string, object> custom = parameter.ExtensionData;

            if (IsUnconditionalChoices(parameter)) {
                Tuple<IObjectFacade, string>[] choices = parameter.GetChoicesAndTitles(nakedObject, null);
                Tuple<object, string>[] choicesArray = choices.Select(tuple => new Tuple<object, string>(RestUtils.GetChoiceValue(OidStrategy ,req, tuple.Item1, parameter, flags), tuple.Item2)).ToArray();

                OptionalProperty[] op = choicesArray.Select(tuple => new OptionalProperty(tuple.Item2, tuple.Item1)).ToArray();
                MapRepresentation map = MapRepresentation.Create(op);
                custom = custom ?? new Dictionary<string, object>();
                custom[JsonPropertyNames.CustomChoices] = map;
            }

            string mask = parameter.Mask;

            if (!string.IsNullOrWhiteSpace(mask)) {
                custom = custom ?? new Dictionary<string, object>();
                custom[JsonPropertyNames.CustomMask] = mask;
            }

            if (Flags.SimpleDomainModel) {
                Extensions = RestUtils.GetExtensions(friendlyname: parameter.Name,
                    description: parameter.Description,
                    pluralName: null,
                    domainType: null,
                    isService: null,
                    hasParams: null,
                    optional: !parameter.IsMandatory,
                    maxLength: parameter.MaxLength,
                    pattern: parameter.Pattern,
                    memberOrder: null,
                    customExtensions: custom,
                    returnType: parameter.Specification,
                    elementType: parameter.ElementType,
                    oidStrategy: OidStrategy);
            }
            else {
                Extensions = MapRepresentation.Create();
            }
        }

        private static bool IsUnconditionalChoices(INakedObjectActionParameterSurface parameter) {
            return parameter.IsChoicesEnabled != Choices.NotEnabled  &&
                   (parameter.Specification.IsParseable || (parameter.Specification.IsCollection && parameter.ElementType.IsParseable)) &&
                   !parameter.GetChoicesParameters().Any();
        }

        private static LinkRepresentation CreateDefaultLink(IOidStrategy oidStrategy, HttpRequestMessage req, INakedObjectActionParameterSurface parameter, IObjectFacade defaultNakedObject, string title, RestControlFlags flags) {
            return LinkRepresentation.Create(oidStrategy ,new DefaultRelType(parameter, new UriMtHelper(oidStrategy ,req, defaultNakedObject)), flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        private static object CreateDefaultLinks(IOidStrategy oidStrategy, HttpRequestMessage req, INakedObjectActionParameterSurface parameter, IObjectFacade defaultNakedObject, string title, RestControlFlags flags) {
            if (defaultNakedObject.Specification.IsCollection) {
                return defaultNakedObject.ToEnumerable().Select(i => CreateDefaultLink(oidStrategy, req, parameter, i, i.TitleString, flags)).ToArray();
            }
            return CreateDefaultLink(oidStrategy, req, parameter, defaultNakedObject, title, flags);
        }

        public static ParameterRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade nakedObject, INakedObjectActionParameterSurface parameter, RestControlFlags flags) {
            var optionals = new List<OptionalProperty>();

            if (parameter.IsChoicesEnabled != Choices.NotEnabled  && !parameter.GetChoicesParameters().Any()) {
                IObjectFacade[] choices = parameter.GetChoices(nakedObject, null);
                object[] choicesArray = choices.Select(c => RestUtils.GetChoiceValue(oidStrategy ,req, c, parameter, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }

            if (parameter.DefaultTypeIsExplicit(nakedObject)) {
                IObjectFacade defaultNakedObject = parameter.GetDefault(nakedObject);
                if (defaultNakedObject != null) {
                    string title = defaultNakedObject.TitleString;
                    object value = RestUtils.ObjectToPredefinedType(defaultNakedObject.Object);
                    var isValue = defaultNakedObject.Specification.IsParseable || (defaultNakedObject.Specification.IsCollection && defaultNakedObject.ElementSpecification.IsParseable);
                    object defaultValue = isValue ? value : CreateDefaultLinks(oidStrategy ,req, parameter, defaultNakedObject, title, flags);

                    optionals.Add(new OptionalProperty(JsonPropertyNames.Default, defaultValue));
                }
            }

            if (optionals.Count == 0) {
                return new ParameterRepresentation(oidStrategy ,req, nakedObject, parameter, flags);
            }
            return CreateWithOptionals<ParameterRepresentation>(new object[] {oidStrategy ,req, nakedObject, parameter, flags}, optionals);
        }
    }
}