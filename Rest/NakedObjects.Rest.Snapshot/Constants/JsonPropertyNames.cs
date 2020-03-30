// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Constants {
    public class JsonPropertyNames {
        public const string Arguments = "arguments";
        public const string Choices = "choices";
        public const string Default = "default";
        public const string Description = "description";
        public const string DisabledReason = "disabledReason";
        public const string DomainType = "domainType";
        public const string ElementType = "elementType";
        public const string Extensions = "extensions";
        public const string Format = "format";
        public const string FriendlyName = "friendlyName";
        public const string HasChoices = "hasChoices";
        public const string HasParams = "hasParams";
        public const string Href = "href";
        public const string Id = "id";
        public const string ImplVersion = "implVersion";
        public const string InstanceId = "instanceId";
        public const string InvalidReason = "invalidReason";
        public const string IsService = "isService";
        public const string Links = "links";
        public const string MaxLength = "maxLength";
        public const string MinLength = "minLength";
        public const string MemberOrder = "memberOrder";
        public const string MemberType = "memberType";
        public const string Members = "members";
        public const string Message = "message";
        public const string Method = "method";
        public const string Name = "name";
        public const string Number = "number";
        public const string Optional = "optional";
        public const string OptionalCapabilities = "optionalCapabilities";
        public const string Parameters = "parameters";
        public const string Pattern = "pattern";
        public const string PluralName = "pluralName";
        public const string Prompt = "prompt";
        public const string Rel = "rel";
        public const string Result = "result";
        public const string ResultType = "resultType";
        public const string ReturnType = "returnType";
        public const string Roles = "roles";
        public const string ServiceId = "serviceId";
        public const string Size = "size";
        public const string SpecVersion = "specVersion";
        public const string StackTrace = "stackTrace";
        public const string SubType = "subtype";
        public const string SuperType = "supertype";
        public const string Title = "title";
        public const string Type = "type";
        public const string TypeActions = "typeActions";
        public const string UserName = "userName";
        public const string Value = "value";
        public const string XRoInvalidReason = "x-ro-invalidReason";
        public const string XRoSearchTerm = RestControlFlags.SearchTermReserved;

        public const string XRoMembers = "x-ro-nof-members";

        // custom 
        public const string CustomMask = "x-ro-nof-mask";
        public const string CustomChoices = "x-ro-nof-choices";
        public const string PresentationHint = "x-ro-nof-presentationHint";
        public const string InteractionMode = "x-ro-nof-interactionMode";
        public const string PromptMembers = "x-ro-nof-members";

        public const string CustomTableViewTitle = "x-ro-nof-tableViewTitle";
        public const string CustomTableViewColumns = "x-ro-nof-tableViewColumns";
        public const string CustomMultipleLines = "x-ro-nof-multipleLines";
        public const string CustomDataType = "x-ro-nof-dataType";
        public const string CustomRange = "x-ro-nof-range";
        public const string CustomNotNavigable = "x-ro-nof-notNavigable";
        public const string CustomRenderEagerly = "x-ro-nof-renderEagerly";
        public const string CustomMenuPath = "x-ro-nof-menuPath";

        public const string CustomWarnings = "x-ro-nof-warnings";
        public const string CustomMessages = "x-ro-nof-messages";

        // extensions
        public const string MenuId = "menuId";
        public const string Pagination = "pagination";

        public const string Page = "page";
        public const string PageSize = "pageSize";
        public const string NumPages = "numPages";
        public const string TotalCount = "totalCount";
    }
}