// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class ParameterRepresentation : Representation {
        protected ParameterRepresentation(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade, FieldFacadeAdapter parameter, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetName(parameter);
            SetExtensions(req, objectFacade, parameter, flags);
            SetLinks(req, objectFacade, parameter);
            SetHeader(objectFacade);
        }

        internal string Name { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader(IObjectFacade objectFacade) => SetEtag(objectFacade);

        private void SetName(FieldFacadeAdapter parameter) => Name = parameter.Id;

        private LinkRepresentation CreatePromptLink(HttpRequest req, IObjectFacade objectFacade, FieldFacadeAdapter parameter) {
            var opts = new List<OptionalProperty>();

            if (parameter.IsAutoCompleteEnabled) {
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object))))));
                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, parameter.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                var parms = parameter.GetChoicesParameters();
                var args = parms.Select(tuple => RestUtils.CreateArgumentProperty(OidStrategy, req, tuple, Flags)).ToArray();
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy, new PromptRelType(parameter.GetHelper(OidStrategy, req, objectFacade)), Flags, opts.ToArray());
        }

        private void SetLinks(HttpRequest req, IObjectFacade objectFacade, FieldFacadeAdapter parameter) {
            var tempLinks = new List<LinkRepresentation>();

            if (parameter.IsAutoCompleteEnabled || parameter.GetChoicesParameters().Any()) {
                tempLinks.Add(CreatePromptLink(req, objectFacade, parameter));
            }

            Links = tempLinks.ToArray();
        }

        private void SetExtensions(HttpRequest req, IObjectFacade objectFacade, FieldFacadeAdapter parameter, RestControlFlags flags) {
            IDictionary<string, object> custom = null;

            if (IsUnconditionalChoices(parameter)) {
                custom = new Dictionary<string, object>();

                (object value, string title)[] choicesArray = parameter.GetChoicesAndTitles(objectFacade, null).Select(choice => (parameter.GetChoiceValue(OidStrategy, req, choice.obj, flags), choice.title)).ToArray();

                var op = choicesArray.Select(choice => new OptionalProperty(choice.title, choice.value)).ToArray();
                var map = MapRepresentation.Create(op);
                custom[JsonPropertyNames.CustomChoices] = map;
            }

            var mask = parameter.Mask;

            if (!string.IsNullOrWhiteSpace(mask)) {
                custom ??= new Dictionary<string, object>();
                custom[JsonPropertyNames.CustomMask] = mask;
            }

            var multipleLines = parameter.NumberOfLines;

            if (multipleLines > 1) {
                custom ??= new Dictionary<string, object>();
                custom[JsonPropertyNames.CustomMultipleLines] = multipleLines;
            }

            if (parameter.IsFindMenuEnabled != null && parameter.IsFindMenuEnabled.Value) {
                custom ??= new Dictionary<string, object>();
                custom[JsonPropertyNames.CustomFindMenu] = true;
            }

            custom = RestUtils.AddRangeExtension(parameter.AsField, custom);

            Extensions = RestUtils.GetExtensions(parameter.Name,
                                                 parameter.Description,
                                                 null,
                                                 null,
                                                 null,
                                                 null,
                                                 !parameter.IsMandatory,
                                                 parameter.MaxLength,
                                                 parameter.Pattern,
                                                 null,
                                                 parameter.DataType,
                                                 parameter.PresentationHint,
                                                 custom,
                                                 parameter.Specification,
                                                 parameter.ElementType,
                                                 OidStrategy,
                                                 true);
        }

        private static bool IsUnconditionalChoices(FieldFacadeAdapter parameter) =>
            parameter.IsChoicesEnabled != Choices.NotEnabled &&
            (parameter.Specification.IsParseable || parameter.Specification.IsCollection && parameter.ElementType.IsParseable) &&
            !parameter.GetChoicesParameters().Any();

        private static LinkRepresentation CreateDefaultLink(IOidStrategy oidStrategy, HttpRequest req, FieldFacadeAdapter parameter, IActionFacade action, IObjectFacade defaultNakedObject, string title, RestControlFlags flags) {
            var helper = new UriMtHelper(oidStrategy, req, defaultNakedObject);
            var relType = new DefaultRelType(action.Id, parameter.Id, helper);

            return LinkRepresentation.Create(oidStrategy, relType, flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        private static object CreateDefaultLinks(IOidStrategy oidStrategy, HttpRequest req, FieldFacadeAdapter parameter, IActionFacade action, IObjectFacade defaultNakedObject, string title, RestControlFlags flags) {
            if (defaultNakedObject.Specification.IsCollection) {
                return defaultNakedObject.ToEnumerable().Select(i => CreateDefaultLink(oidStrategy, req, parameter, action, i, i.TitleString, flags)).ToArray();
            }

            return CreateDefaultLink(oidStrategy, req, parameter, action, defaultNakedObject, title, flags);
        }

        public static ParameterRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade, ParameterContextFacade parameterContext, RestControlFlags flags) {
            var optionals = new List<OptionalProperty>();
            var parameter = parameterContext.Parameter;

            if (parameter.IsChoicesEnabled != Choices.NotEnabled && !parameter.GetChoicesParameters().Any()) {
                var choices = parameter.GetChoices(objectFacade, null);
                var choicesArray = choices.Select(c => RestUtils.GetChoiceValue(oidStrategy, req, c, parameter, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }

            var adapter = new FieldFacadeAdapter(parameter) {MenuId = parameterContext.MenuId};

            // include default value for for non-nullable boolean so we can distinguish from nullable on client 
            if (parameter.DefaultTypeIsExplicit(objectFacade) || parameter.Specification.IsBoolean && !parameter.IsNullable) {
                var defaultNakedObject = parameter.GetDefault(objectFacade);
                if (defaultNakedObject != null) {
                    var title = defaultNakedObject.TitleString;
                    var value = RestUtils.ObjectToPredefinedType(defaultNakedObject.Object, true);
                    var isValue = defaultNakedObject.Specification.IsParseable || defaultNakedObject.Specification.IsCollection && defaultNakedObject.ElementSpecification.IsParseable;
                    var defaultValue = isValue ? value : CreateDefaultLinks(oidStrategy, req, adapter, parameter.Action, defaultNakedObject, title, flags);

                    optionals.Add(new OptionalProperty(JsonPropertyNames.Default, defaultValue));
                }
            }

            return optionals.Any()
                ? CreateWithOptionals<ParameterRepresentation>(new object[] {oidStrategy, req, objectFacade, adapter, flags}, optionals)
                : new ParameterRepresentation(oidStrategy, req, objectFacade, adapter, flags);
        }

        public static ParameterRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade, IAssociationFacade assoc, ActionContextFacade actionContext, RestControlFlags flags) {
            var optionals = new List<OptionalProperty>();

            if (assoc.IsChoicesEnabled != Choices.NotEnabled && !assoc.GetChoicesParameters().Any()) {
                var choices = assoc.GetChoices(objectFacade, null);
                var choicesArray = choices.Select(c => RestUtils.GetChoiceValue(oidStrategy, req, c, assoc, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }

            var adapter = new FieldFacadeAdapter(assoc);

            var defaultNakedObject = assoc.GetValue(objectFacade);
            if (defaultNakedObject != null) {
                var title = defaultNakedObject.TitleString;
                var value = RestUtils.ObjectToPredefinedType(defaultNakedObject.Object, true);
                var isValue = defaultNakedObject.Specification.IsParseable || defaultNakedObject.Specification.IsCollection && defaultNakedObject.ElementSpecification.IsParseable;
                var defaultValue = isValue ? value : CreateDefaultLinks(oidStrategy, req, adapter, actionContext.Action, defaultNakedObject, title, flags);

                optionals.Add(new OptionalProperty(JsonPropertyNames.Default, defaultValue));
            }

            return optionals.Any()
                ? CreateWithOptionals<ParameterRepresentation>(new object[] {oidStrategy, req, objectFacade, adapter, flags}, optionals)
                : new ParameterRepresentation(oidStrategy, req, objectFacade, adapter, flags);
        }
    }
}