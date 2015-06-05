// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Facade {
    public interface IActionParameterFacade : IFacadeHolder {
        ITypeFacade Specification { get; }
        ITypeFacade ElementType { get; }
        IActionFacade Action { get; }
        string Id { get; }
        Choices IsChoicesEnabled { get; }
        bool IsAutoCompleteEnabled { get; }
        string PresentationHint { get; }
        Tuple<Regex, string> RegEx { get; }
        Tuple<IConvertible, IConvertible, bool> Range { get; }
        int NumberOfLines { get; }
        int Width { get; }
        int TypicalLength { get; }
        bool IsAjax { get; }
        bool IsNullable { get; }
        bool IsPassword { get; }
        string Name { get; }
        string Description { get; }
        bool IsMandatory { get; }
        int? MaxLength { get; }
        string Pattern { get; }
        int Number { get; }
        string Mask { get; }
        int AutoCompleteMinLength { get; }
        IDictionary<string, object> ExtensionData { get; }
        bool IsFindMenuEnabled { get; }
        IObjectFacade[] GetChoices(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues);
        Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade nakedObject, IDictionary<string, object> parameterNameValues);
        IObjectFacade[] GetCompletions(IObjectFacade nakedObject, string autoCompleteParm);
        bool DefaultTypeIsExplicit(IObjectFacade nakedObject);
        IObjectFacade GetDefault(IObjectFacade nakedObject);
        Tuple<string, ITypeFacade>[] GetChoicesParameters();
        string GetMaskedValue(IObjectFacade valueNakedObject);
        // todo not really same as other interfaces - more PutValue with validate only ? 
        IConsentFacade IsValid(IObjectFacade target, object value);
    }
}