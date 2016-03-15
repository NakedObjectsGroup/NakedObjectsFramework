// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    [DataContract]
    public class PropertyRepresentationStrategy : MemberRepresentationStrategy {
        public PropertyRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags) :
            base(oidStrategy, req, propertyContext, flags) {}

        private IDictionary<string, object> customExtensions;

        private void AddPrompt(List<LinkRepresentation> links, Func<LinkRepresentation> createPrompt) {
            if (propertyContext.Property.IsAutoCompleteEnabled || propertyContext.Property.GetChoicesParameters().Any()) {
                links.Add(createPrompt());
            }
        }

        private LinkRepresentation CreatePromptLink() {
            var opts = new List<OptionalProperty>();

            if (propertyContext.Property.IsAutoCompleteEnabled) {
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))));
                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, propertyContext.Property.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                Tuple<string, ITypeFacade>[] parms = propertyContext.Property.GetChoicesParameters();
                OptionalProperty[] args = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, req, pnt, Flags)).ToArray();
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()), Flags, opts.ToArray());
        }

        private LinkRepresentation CreatePersistPromptLink() {
            var opts = new List<OptionalProperty>();

            KeyValuePair<string, object>[] ids = propertyContext.Target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline).ToDictionary(p => p.Id, p => Representation.GetPropertyValue(OidStrategy, req, p, propertyContext.Target, Flags, true, UseDateOverDateTime())).ToArray();
            OptionalProperty[] props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

            var objectMembers = new OptionalProperty(JsonPropertyNames.PromptMembers, MapRepresentation.Create(props));

            if (propertyContext.Property.IsAutoCompleteEnabled) {
                var searchTerm = new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))));
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(objectMembers, searchTerm));

                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, propertyContext.Property.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                Tuple<string, ITypeFacade>[] parms = propertyContext.Property.GetChoicesParameters();
                var conditionalArguments = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, req, pnt, Flags));
                var args = new List<OptionalProperty> {objectMembers};
                args.AddRange(conditionalArguments);
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray()));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()) {Method = RelMethod.Put}, Flags, opts.ToArray());
        }

        private void AddMutatorLinks(List<LinkRepresentation> links) {
            if (propertyContext.Property.IsUsable(propertyContext.Target).IsAllowed) {
                links.Add(CreateModifyLink());

                if (!propertyContext.Property.IsMandatory) {
                    links.Add(CreateClearLink());
                }
            }
        }

        private LinkRepresentation CreateClearLink() {
            return LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Clear, GetHelper()) {Method = RelMethod.Delete}, Flags);
        }

        private LinkRepresentation CreateModifyLink() {
            return LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Modify, GetHelper()) {Method = RelMethod.Put}, Flags,
                new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object)))));
        }

        public new LinkRepresentation[] GetLinks(bool inline) {
            var links = new List<LinkRepresentation>(base.GetLinks(inline));

            if (!propertyContext.Target.IsTransient) {
                AddMutatorLinks(links);
            }

            AddPrompt(links, propertyContext.Target.IsTransient ? (Func<LinkRepresentation>) CreatePersistPromptLink : CreatePromptLink);

            return links.ToArray();
        }

        private IDictionary<string, object> GetCustomPropertyExtensions() {

            if (customExtensions == null) {


                if (IsUnconditionalChoices()) {
                    customExtensions = new Dictionary<string, object>();

                    Tuple<IObjectFacade, string>[] choices = propertyContext.Property.GetChoicesAndTitles(propertyContext.Target, null);
                    Tuple<object, string>[] choicesArray = choices.Select(tuple => new Tuple<object, string>(RestUtils.GetChoiceValue(OidStrategy, req, tuple.Item1, propertyContext.Property, Flags), tuple.Item2)).ToArray();

                    OptionalProperty[] op = choicesArray.Select(tuple => new OptionalProperty(tuple.Item2, tuple.Item1)).ToArray();
                    MapRepresentation map = MapRepresentation.Create(op);

                   
                    customExtensions[JsonPropertyNames.CustomChoices] = map;
                }

                string mask = propertyContext.Property.Mask;

                if (!string.IsNullOrWhiteSpace(mask)) {
                   

                    customExtensions = customExtensions ?? new Dictionary<string, object>();
                    customExtensions[JsonPropertyNames.CustomMask] = mask;
                }

                var multipleLines = propertyContext.Property.NumberOfLines;

                if (multipleLines > 1) {        
                    customExtensions = customExtensions ?? new Dictionary<string, object>();
                    customExtensions[JsonPropertyNames.CustomMultipleLines] = multipleLines;
                }

                customExtensions = RestUtils.AddRangeExtension(propertyContext.Property, customExtensions);
            }

            return customExtensions;
        }

        private bool IsUnconditionalChoices() {
            return propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled &&
                   (propertyContext.Property.Specification.IsParseable || (propertyContext.Property.Specification.IsCollection && propertyContext.Property.ElementSpecification.IsParseable)) &&
                   !propertyContext.Property.GetChoicesParameters().Any();
        }

        public bool UseDateOverDateTime() {
            return propertyContext.Property.IsDateOnly;
        }

        protected override MapRepresentation GetExtensionsForSimple() {

            return RestUtils.GetExtensions(
                friendlyname: propertyContext.Property.Name,
                description: propertyContext.Property.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: null,
                optional: !propertyContext.Property.IsMandatory,
                maxLength: propertyContext.Property.MaxLength,
                pattern: propertyContext.Property.Pattern,
                memberOrder: propertyContext.Property.MemberOrder,
                dataType: propertyContext.Property.DataType,
                presentationHint: propertyContext.Property.PresentationHint,
                customExtensions: GetCustomPropertyExtensions(),
                returnType: propertyContext.Specification,
                elementType: propertyContext.ElementSpecification,
                oidStrategy: OidStrategy,
                useDateOverDateTime: UseDateOverDateTime());
        }

        public bool GetHasChoices() {
            return propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !propertyContext.Property.GetChoicesParameters().Any();
        }
    }
}