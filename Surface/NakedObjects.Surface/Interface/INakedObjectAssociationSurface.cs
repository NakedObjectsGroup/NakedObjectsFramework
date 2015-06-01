// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NakedObjects.Surface.Context;

namespace NakedObjects.Surface {
    public interface INakedObjectAssociationSurface : INakedObjectMemberSurface {
        ITypeFacade Specification { get; }
        ITypeFacade ElementSpecification { get; }
        Choices IsChoicesEnabled { get; }
        bool IsAutoCompleteEnabled { get; }
        bool IsFile { get; }
        bool IsEnum { get; }
        Tuple<Regex, string> RegEx { get; }
        Tuple<IConvertible, IConvertible, bool> Range { get; }
        bool IsFindMenuEnabled { get; }
        bool IsAjax { get; }
        bool IsNullable { get; }
        bool IsPassword { get; }
        bool DoNotCount { get; }
        bool RenderEagerly { get; }
        int NumberOfLines { get; }
        int Width { get; }
        int TypicalLength { get; }
        int? MaxLength { get; }
        string PresentationHint { get; }
        string Pattern { get; }
        bool IsMandatory { get; }
        string Name { get; }
        string Description { get; }
        bool IsCollection { get; }
        bool IsObject { get; }
        int MemberOrder { get; }
        bool IsASet { get; }
        bool IsInline { get; }
        string Mask { get; }
        int AutoCompleteMinLength { get; }
        bool IsConcurrency { get; }
        IDictionary<string, object> ExtensionData { get; }
        Tuple<bool, string[]> TableViewData { get; }
        IConsentSurface IsUsable(IObjectFacade target);
        IObjectFacade GetNakedObject(IObjectFacade target);
        bool IsVisible(IObjectFacade nakedObject);
        bool IsEager(IObjectFacade nakedObject);
        IObjectFacade[] GetChoices(IObjectFacade target, IDictionary<string, object> parameterNameValues);
        Tuple<string, ITypeFacade>[] GetChoicesParameters();
        Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade target, IDictionary<string, object> parameterNameValues);
        IObjectFacade[] GetCompletions(IObjectFacade target, string autoCompleteParm);
        string GetTitle(IObjectFacade nakedObject);
        int Count(IObjectFacade target);
        string GetMaskedValue(IObjectFacade valueNakedObject);
        bool DefaultTypeIsExplicit(IObjectFacade nakedObject);
    }
}