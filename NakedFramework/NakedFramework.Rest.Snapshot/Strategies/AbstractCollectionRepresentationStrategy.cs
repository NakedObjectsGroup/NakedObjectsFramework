// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Facade;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Representation;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Strategies {
    public abstract class AbstractCollectionRepresentationStrategy : MemberRepresentationStrategy {
        private IObjectFacade collection;

        protected AbstractCollectionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, req, propertyContext, flags) { }

        protected IObjectFacade Collection => collection ??= PropertyContext.Property.GetValue(PropertyContext.Target);

        protected override MapRepresentation GetExtensionsForSimple() =>
            RestUtils.GetExtensions(
                PropertyContext.Property.Name,
                PropertyContext.Property.Description,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                PropertyContext.Property.MemberOrder,
                PropertyContext.Property.DataType,
                PropertyContext.Property.PresentationHint,
                GetCustomPropertyExtensions(),
                PropertyContext.Specification,
                PropertyContext.ElementSpecification,
                OidStrategy,
                false);

        private IDictionary<string, object> GetCustomPropertyExtensions() {
            var exts = GetTableViewCustomExtensions(PropertyContext.Property.TableViewData);

            if (PropertyContext.Property.RenderEagerly) {
                exts ??= new Dictionary<string, object>();
                exts[JsonPropertyNames.CustomRenderEagerly] = true;
            }

            return exts;
        }

        public abstract LinkRepresentation[] GetValue();

        protected LinkRepresentation CreateValueLink(IObjectFacade no) =>
            LinkRepresentation.Create(OidStrategy, new ValueRelType(PropertyContext.Property, new UriMtHelper(OidStrategy, Req, no)), Flags,
                                      new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));

        protected LinkRepresentation CreateTableRowValueLink(IObjectFacade no) => RestUtils.CreateTableRowValueLink(no, PropertyContext, OidStrategy, Req, Flags);

        public abstract int? GetSize();

        private static bool InlineDetails(PropertyContextFacade propertyContext, RestControlFlags flags) => flags.InlineDetailsInCollectionMemberRepresentations || propertyContext.Property.RenderEagerly;

        private static bool DoNotCountAndNotEager(PropertyContextFacade propertyContext) => propertyContext.Property.DoNotCount && !propertyContext.Property.RenderEagerly;

        public static AbstractCollectionRepresentationStrategy GetStrategy(bool asTableColumn, bool inline, IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) {
            if (asTableColumn) {
                if (propertyContext.Property.DoNotCount) {
                    return new CollectionMemberNotCountedRepresentationStrategy(oidStrategy, req, propertyContext, flags);
                }

                return new CollectionMemberRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            if (inline && DoNotCountAndNotEager(propertyContext)) {
                return new CollectionMemberNotCountedRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            if (inline && !InlineDetails(propertyContext, flags)) {
                return new CollectionMemberRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            return new CollectionWithDetailsRepresentationStrategy(oidStrategy, req, propertyContext, flags);
        }

        private static ActionContextFacade ActionContext(IActionFacade actionFacade, IObjectFacade target) =>
            new() {
                MenuPath = "",
                Target = target,
                Action = actionFacade,
                VisibleParameters = actionFacade.Parameters.Select(p => new ParameterContextFacade {Parameter = p, Action = actionFacade}).ToArray()
            };

        public virtual InlineActionRepresentation[] GetActions() {
            return !PropertyContext.Target.IsTransient
                ? OidStrategy.FrameworkFacade.GetLocallyContributedActions(PropertyContext).Select(a => InlineActionRepresentation.Create(OidStrategy, Req, a, Flags)).ToArray()
                : Array.Empty<InlineActionRepresentation>();
        }
    }
}