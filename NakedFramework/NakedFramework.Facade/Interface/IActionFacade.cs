// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Facade.Interface; 

public interface IActionFacade : IMemberFacade {
    ITypeFacade ReturnType { get; }
    ITypeFacade ElementType { get; }
    int ParameterCount { get; }
    IActionParameterFacade[] Parameters { get; }
    ITypeFacade OnType { get; }
    string PresentationHint { get; }
    int PageSize { get; }
    string Name { get; }
    string Description { get; }
    bool IsQueryOnly { get; }
    bool IsIdempotent { get; }
    string[] CreateNewProperties { get; }
    bool IsContributed { get; }
    int MemberOrder { get; }
    (bool title, string[] columns)? TableViewData { get; }
    bool RenderEagerly { get; }
    int? NumberOfLines { get; }
    string MemberOrderName { get; }
    bool IsStatic { get; }
    bool IsQueryContributedAction { get; }
    string[] EditProperties { get; }
    string FinderMethodPrefix { get; }
    bool IsVisible(IObjectFacade objectFacade);
    IConsentFacade IsUsable(IObjectFacade objectFacade);

    bool IsStaticObjectMenu { get; }
}