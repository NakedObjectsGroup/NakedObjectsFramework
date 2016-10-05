// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    public abstract class AbstractCollectionRepresentationStrategy : MemberRepresentationStrategy {
        protected AbstractCollectionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, req, propertyContext, flags) {           
        }

        private IObjectFacade collection;

        protected IObjectFacade Collection {
            get {
                if (collection == null) {
                    collection = PropertyContext.Property.GetValue(PropertyContext.Target);
                }
                return collection;
            }
        }

        protected override MapRepresentation GetExtensionsForSimple() {
            return RestUtils.GetExtensions(
                friendlyname: PropertyContext.Property.Name,
                description: PropertyContext.Property.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: null,
                optional: null,
                maxLength: null,
                pattern: null,
                memberOrder: PropertyContext.Property.MemberOrder,
                dataType: PropertyContext.Property.DataType,
                presentationHint: PropertyContext.Property.PresentationHint,
                customExtensions: GetCustomPropertyExtensions(),
                returnType: PropertyContext.Specification,
                elementType: PropertyContext.ElementSpecification,
                oidStrategy: OidStrategy,
                useDateOverDateTime: false);
        }

        private IDictionary<string, object> GetCustomPropertyExtensions() {
            var exts = GetTableViewCustomExtensions(PropertyContext.Property.TableViewData);

            if (PropertyContext.Property.RenderEagerly) {
                exts = exts ?? new Dictionary<string, object>();
                exts[JsonPropertyNames.CustomRenderEagerly] = true;
            }
            return exts;
        }

        public abstract LinkRepresentation[] GetValue();

        protected LinkRepresentation CreateValueLink(IObjectFacade no) {
            return LinkRepresentation.Create(OidStrategy, new ValueRelType(PropertyContext.Property, new UriMtHelper(OidStrategy, Req, no)), Flags,
                new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

       
        protected LinkRepresentation CreateTableRowValueLink(IObjectFacade no) {
            return RestUtils.CreateTableRowValueLink(no, PropertyContext, OidStrategy, Req, Flags);
        }

        public abstract int? GetSize();

        private static bool InlineDetails(PropertyContextFacade propertyContext, RestControlFlags flags) {
            return flags.InlineDetailsInCollectionMemberRepresentations || propertyContext.Property.RenderEagerly;
        }

        private static bool DoNotCount(PropertyContextFacade propertyContext) {
            return propertyContext.Property.DoNotCount && !propertyContext.Property.RenderEagerly;
        }

        public static AbstractCollectionRepresentationStrategy GetStrategy(bool asTableColumn,  bool inline, IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags) {
         
            if (asTableColumn) {
                if (propertyContext.Property.DoNotCount) {
                     return new CollectionMemberNotCountedRepresentationStrategy(oidStrategy, req, propertyContext, flags);
                }

                return new CollectionMemberRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            if (inline && DoNotCount(propertyContext)) {
                return new CollectionMemberNotCountedRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            if (inline && !InlineDetails(propertyContext, flags)) {
                return new CollectionMemberRepresentationStrategy(oidStrategy, req, propertyContext, flags);
            }

            return new CollectionWithDetailsRepresentationStrategy(oidStrategy, req, propertyContext, flags);
        }

        private ActionContextFacade ActionContext(IActionFacade actionFacade, IObjectFacade target) {
            return new ActionContextFacade {
                MenuPath = "",
                Target = target,
                Action = actionFacade,
                VisibleParameters = actionFacade.Parameters.Select(p => new ParameterContextFacade { Parameter = p, Action = actionFacade }).ToArray()
            };
        }

        public virtual InlineActionRepresentation[] GetActions() {

            if (!PropertyContext.Target.IsTransient) {
                var lcas = PropertyContext.Target.Specification.GetLocallyContributedActions(PropertyContext.Property.ElementSpecification, PropertyContext.Property.Id);

                if (lcas.Length > 0) {
                    return lcas.Select(a => InlineActionRepresentation.Create(OidStrategy, Req, ActionContext(a, PropertyContext.Target), Flags)).ToArray();
                }
            }

            return new InlineActionRepresentation[] { };
        }
    }
}