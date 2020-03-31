// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    [DataContract]
    public abstract class AbstractPropertyRepresentationStrategy : MemberRepresentationStrategy {
        protected AbstractPropertyRepresentationStrategy(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) :
            base(oidStrategy, req, propertyContext, flags) {}

        protected IDictionary<string, object> CustomExtensions { get; set; }

        protected void AddPrompt(List<LinkRepresentation> links, Func<LinkRepresentation> createPrompt) {
            if (PropertyContext.Property.IsAutoCompleteEnabled || PropertyContext.Property.GetChoicesParameters().Any()) {
                links.Add(createPrompt());
            }
        }

        protected LinkRepresentation CreatePromptLink() {
            var opts = new List<OptionalProperty>();

            if (PropertyContext.Property.IsAutoCompleteEnabled) {
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))));
                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, PropertyContext.Property.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                Tuple<string, ITypeFacade>[] parms = PropertyContext.Property.GetChoicesParameters();
                OptionalProperty[] args = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, Req, pnt, Flags)).ToArray();
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()), Flags, opts.ToArray());
        }

        protected LinkRepresentation CreatePersistPromptLink() {
            var opts = new List<OptionalProperty>();

            KeyValuePair<string, object>[] ids = PropertyContext.Target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline).ToDictionary(p => p.Id, p => GetPropertyValue(OidStrategy, Req, p, PropertyContext.Target, Flags, true, UseDateOverDateTime())).ToArray();
            OptionalProperty[] props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

            var objectMembers = new OptionalProperty(JsonPropertyNames.PromptMembers, MapRepresentation.Create(props));

            if (PropertyContext.Property.IsAutoCompleteEnabled) {
                var searchTerm = new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))));
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(objectMembers, searchTerm));

                var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, PropertyContext.Property.AutoCompleteMinLength)));

                opts.Add(arguments);
                opts.Add(extensions);
            }
            else {
                Tuple<string, ITypeFacade>[] parms = PropertyContext.Property.GetChoicesParameters();
                var conditionalArguments = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, Req, pnt, Flags));
                var args = new List<OptionalProperty> {objectMembers};
                args.AddRange(conditionalArguments);
                var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray()));
                opts.Add(arguments);
            }

            return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()) {Method = RelMethod.Put}, Flags, opts.ToArray());
        }

        protected void AddMutatorLinks(List<LinkRepresentation> links) {
            if (PropertyContext.Property.IsUsable(PropertyContext.Target).IsAllowed) {
                links.Add(CreateModifyLink());

                if (!PropertyContext.Property.IsMandatory) {
                    links.Add(CreateClearLink());
                }
            }
        }

        private LinkRepresentation CreateClearLink() => LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Clear, GetHelper()) {Method = RelMethod.Delete}, Flags);

        private LinkRepresentation CreateModifyLink() =>
            LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Modify, GetHelper()) {Method = RelMethod.Put}, Flags,
                new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object)))));

        private IDictionary<string, object> GetCustomPropertyExtensions() {
            if (CustomExtensions == null) {
                AddChoicesCustomExtension();

                string mask = PropertyContext.Property.Mask;

                if (!string.IsNullOrWhiteSpace(mask)) {
                    CustomExtensions ??= new Dictionary<string, object>();
                    CustomExtensions[JsonPropertyNames.CustomMask] = mask;
                }

                var multipleLines = PropertyContext.Property.NumberOfLines;

                if (multipleLines > 1) {
                    CustomExtensions ??= new Dictionary<string, object>();
                    CustomExtensions[JsonPropertyNames.CustomMultipleLines] = multipleLines;
                }

                if (PropertyContext.Property.NotNavigable) {
                    CustomExtensions ??= new Dictionary<string, object>();
                    CustomExtensions[JsonPropertyNames.CustomNotNavigable] = true;
                }

                CustomExtensions = RestUtils.AddRangeExtension(PropertyContext.Property, CustomExtensions);
            }

            return CustomExtensions;
        }

        public bool UseDateOverDateTime() => PropertyContext.Property.IsDateOnly;

        protected override MapRepresentation GetExtensionsForSimple() =>
            RestUtils.GetExtensions(
                friendlyname: PropertyContext.Property.Name,
                description: PropertyContext.Property.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: null,
                optional: !PropertyContext.Property.IsMandatory,
                maxLength: PropertyContext.Property.MaxLength,
                pattern: PropertyContext.Property.Pattern,
                memberOrder: PropertyContext.Property.MemberOrder,
                dataType: PropertyContext.Property.DataType,
                presentationHint: PropertyContext.Property.PresentationHint,
                customExtensions: GetCustomPropertyExtensions(),
                returnType: PropertyContext.Specification,
                elementType: PropertyContext.ElementSpecification,
                oidStrategy: OidStrategy,
                useDateOverDateTime: UseDateOverDateTime());

        public bool GetHasChoices() => PropertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !PropertyContext.Property.GetChoicesParameters().Any();

        public abstract bool ShowChoices();

        private static bool InlineDetails(PropertyContextFacade propertyContext, RestControlFlags flags) =>
            flags.InlineDetailsInPropertyMemberRepresentations ||
            propertyContext.Property.RenderEagerly ||
            propertyContext.Target.IsViewModelEditView ||
            propertyContext.Target.IsTransient;

        public static AbstractPropertyRepresentationStrategy GetStrategy(bool inline, IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) {

            if (flags.InlineCollectionItems) {
                return new PropertyTableRowRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            if (inline && !InlineDetails(propertyContext, flags)) {
                return new PropertyMemberRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            return new PropertyWithDetailsRepresentationStrategy(inline, oidStrategy, req, propertyContext, flags);
        }

        public abstract LinkRepresentation[] GetLinks();

        protected abstract bool AddChoices();

        protected void AddChoicesCustomExtension() {
            if (AddChoices()) {
                CustomExtensions ??= new Dictionary<string, object>();

                Tuple<IObjectFacade, string>[] choices = PropertyContext.Property.GetChoicesAndTitles(PropertyContext.Target, null);
                Tuple<object, string>[] choicesArray = choices.Select(tuple => new Tuple<object, string>(RestUtils.GetChoiceValue(OidStrategy, Req, tuple.Item1, PropertyContext.Property, Flags), tuple.Item2)).ToArray();

                OptionalProperty[] op = choicesArray.Select(tuple => new OptionalProperty(tuple.Item2, tuple.Item1)).ToArray();
                MapRepresentation map = MapRepresentation.Create(op);

                CustomExtensions[JsonPropertyNames.CustomChoices] = map;
            }
        }

        public virtual object GetPropertyValue(IOidStrategy oidStrategy, HttpRequest req, IAssociationFacade property, IObjectFacade target, RestControlFlags flags, bool valueOnly, bool useDateOverDateTime) => Representation.GetPropertyValue(oidStrategy, req, property, target, flags, valueOnly, useDateOverDateTime);
    }
}