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
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Representation;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Strategies;

[DataContract]
public abstract class AbstractPropertyRepresentationStrategy : MemberRepresentationStrategy {
    private readonly IFrameworkFacade frameworkFacade;

    protected AbstractPropertyRepresentationStrategy(IFrameworkFacade frameworkFacade, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) :
        base(frameworkFacade, req, propertyContext, flags) =>
        this.frameworkFacade = frameworkFacade;

    private IDictionary<string, object> CustomExtensions { get; set; }

    protected void AddPrompt(List<LinkRepresentation> links, Func<LinkRepresentation> createPrompt) {
        if (PropertyContext.Property.IsAutoCompleteEnabled || PropertyContext.Property.GetChoicesParameters().Any()) {
            links.Add(createPrompt());
        }
    }

    protected LinkRepresentation CreatePromptLink() {
        var opts = new List<OptionalProperty>();

        if (PropertyContext.Property.IsAutoCompleteEnabled) {
            var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object))))));
            var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, PropertyContext.Property.AutoCompleteMinLength)));

            opts.Add(arguments);
            opts.Add(extensions);
        }
        else {
            var parms = PropertyContext.Property.GetChoicesParameters();
            var args = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, Req, pnt, Flags)).ToArray();
            var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args));
            opts.Add(arguments);
        }

        return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()), Flags, opts.ToArray());
    }

    protected LinkRepresentation CreatePersistPromptLink() {
        var opts = new List<OptionalProperty>();

        var ids = PropertyContext.Target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline).ToDictionary(p => p.Id, p => GetPropertyValue(frameworkFacade, Req, p, PropertyContext.Target, Flags, true, UseDateOverDateTime())).ToArray();
        var props = ids.Select(kvp => new OptionalProperty(kvp.Key, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, kvp.Value)))).ToArray();

        var objectMembers = new OptionalProperty(JsonPropertyNames.PromptMembers, MapRepresentation.Create(props));

        if (PropertyContext.Property.IsAutoCompleteEnabled) {
            var searchTerm = new OptionalProperty(JsonPropertyNames.XRoSearchTerm, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object))));
            var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(objectMembers, searchTerm));

            var extensions = new OptionalProperty(JsonPropertyNames.Extensions, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.MinLength, PropertyContext.Property.AutoCompleteMinLength)));

            opts.Add(arguments);
            opts.Add(extensions);
        }
        else {
            var parms = PropertyContext.Property.GetChoicesParameters();
            var conditionalArguments = parms.Select(pnt => RestUtils.CreateArgumentProperty(OidStrategy, Req, pnt, Flags));
            var args = new List<OptionalProperty> { objectMembers };
            args.AddRange(conditionalArguments);
            var arguments = new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(args.ToArray()));
            opts.Add(arguments);
        }

        return LinkRepresentation.Create(OidStrategy, new PromptRelType(GetHelper()) { Method = RelMethod.Put }, Flags, opts.ToArray());
    }

    protected void AddMutatorLinks(List<LinkRepresentation> links) {
        if (PropertyContext.Property.IsUsable(PropertyContext.Target).IsAllowed) {
            links.Add(CreateModifyLink());

            if (!PropertyContext.Property.IsMandatory) {
                links.Add(CreateClearLink());
            }
        }
    }

    private LinkRepresentation CreateClearLink() => LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Clear, GetHelper()) { Method = RelMethod.Delete }, Flags);

    private LinkRepresentation CreateModifyLink() =>
        LinkRepresentation.Create(OidStrategy, new MemberRelType(RelValues.Modify, GetHelper()) { Method = RelMethod.Put }, Flags,
                                  new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object)))));

    private void AddCustomExtension(string name, object value) {
        CustomExtensions ??= new Dictionary<string, object>();
        CustomExtensions[name] = value;
    }

    private IDictionary<string, object> GetCustomPropertyExtensions() {
        if (CustomExtensions == null) {
            AddChoicesCustomExtension();

            var mask = PropertyContext.Property.Mask;

            if (!string.IsNullOrWhiteSpace(mask)) {
                AddCustomExtension(JsonPropertyNames.CustomMask, mask);
            }

            var multipleLines = PropertyContext.Property.NumberOfLines;

            if (multipleLines > 1) {
                AddCustomExtension(JsonPropertyNames.CustomMultipleLines, multipleLines);
            }

            if (PropertyContext.Property.NotNavigable) {
                AddCustomExtension(JsonPropertyNames.CustomNotNavigable, true);
            }

            if (PropertyContext.Property.IsFindMenuEnabled) {
                AddCustomExtension(JsonPropertyNames.CustomFindMenu, true);
            }

            var grouping = PropertyContext.Property.Grouping;

            if (!string.IsNullOrEmpty(grouping)) {
                AddCustomExtension(JsonPropertyNames.CustomPropertyGrouping, grouping);
            }

            CustomExtensions = RestUtils.AddRangeExtension(PropertyContext.Property, CustomExtensions);
        }

        return CustomExtensions;
    }

    public bool UseDateOverDateTime() => PropertyContext.Property.IsDateOnly;

    protected override MapRepresentation GetExtensionsForSimple() =>
        RestUtils.GetExtensions(
            PropertyContext.Property.Name,
            PropertyContext.Property.Description,
            null,
            null,
            null,
            null,
            !PropertyContext.Property.IsMandatory,
            PropertyContext.Property.MaxLength,
            PropertyContext.Property.Pattern,
            PropertyContext.Property.MemberOrder,
            PropertyContext.Property.DataType,
            PropertyContext.Property.PresentationHint,
            GetCustomPropertyExtensions(),
            PropertyContext.Specification,
            PropertyContext.ElementSpecification,
            OidStrategy,
            UseDateOverDateTime());

    public bool GetHasChoices() => PropertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !PropertyContext.Property.GetChoicesParameters().Any();

    public abstract bool ShowChoices();

    private static bool InlineDetails(PropertyContextFacade propertyContext, RestControlFlags flags) =>
        flags.InlineDetailsInPropertyMemberRepresentations ||
        propertyContext.Property.RenderEagerly ||
        propertyContext.Target.IsViewModelEditView ||
        propertyContext.Target.IsTransient;

    public static AbstractPropertyRepresentationStrategy GetStrategy(bool inline, IFrameworkFacade frameworkFacade, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) {
        if (flags.InlineCollectionItems) {
            return new PropertyTableRowRepresentationStrategy(frameworkFacade, req, propertyContext, flags);
        }

        if (inline && !InlineDetails(propertyContext, flags)) {
            return new PropertyMemberRepresentationStrategy(frameworkFacade, req, propertyContext, flags);
        }

        return new PropertyWithDetailsRepresentationStrategy(inline, frameworkFacade, req, propertyContext, flags);
    }

    public abstract LinkRepresentation[] GetLinks();

    protected abstract bool AddChoices();

    private void AddChoicesCustomExtension() {
        if (AddChoices()) {
            CustomExtensions ??= new Dictionary<string, object>();

            (object value, string title)[] choicesArray = PropertyContext.Property.GetChoicesAndTitles(PropertyContext.Target, null).Select(choice => (RestUtils.GetChoiceValue(OidStrategy, Req, choice.obj, PropertyContext.Property, Flags), choice.title)).ToArray();

            var op = choicesArray.Select(tuple => new OptionalProperty(tuple.title, tuple.value)).ToArray();
            var map = MapRepresentation.Create(op);

            CustomExtensions[JsonPropertyNames.CustomChoices] = map;
        }
    }

    public virtual object GetPropertyValue(IFrameworkFacade frameworkFacade, HttpRequest req, IAssociationFacade property, IObjectFacade target, RestControlFlags flags, bool valueOnly, bool useDateOverDateTime) => Representation.Representation.GetPropertyValue(frameworkFacade, req, property, target, flags, valueOnly, useDateOverDateTime);
}