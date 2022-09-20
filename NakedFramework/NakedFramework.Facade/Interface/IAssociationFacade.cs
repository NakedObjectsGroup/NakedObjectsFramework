// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Facade.Interface;

public interface IAssociationFacade : IFieldFacade, IMemberFacade {
    ITypeFacade ElementSpecification { get; }

    bool IsFile { get; }

    bool IsEnum { get; }

    bool DoNotCount { get; }
    bool RenderEagerly { get; }

    bool IsCollection { get; }
    bool IsObject { get; }
    int MemberOrder { get; }
    bool IsASet { get; }
    bool IsInline { get; }

    bool IsConcurrency { get; }

    bool NotNavigable { get; }

    (bool title, string[] columns)? TableViewData { get; }
    IConsentFacade IsUsable(IObjectFacade target);
    IObjectFacade GetValue(IObjectFacade target);
    bool IsVisible(IObjectFacade objectFacade);
    bool IsEager(IObjectFacade objectFacade);

    string GetTitle(IObjectFacade objectFacade);
    int Count(IObjectFacade target);

    bool IsSetToImplicitDefault(IObjectFacade objectFacade);
    (bool, string)? UrlLink();
}