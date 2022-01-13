// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using NakedFramework.Facade.Contexts;

namespace NakedFramework.Facade.Interface;

public interface IFieldFacade {
    ITypeFacade Specification { get; }
    Choices IsChoicesEnabled { get; }
    bool IsAutoCompleteEnabled { get; }
    string PresentationHint { get; }
    (Regex, string)? RegEx { get; }
    (IConvertible min, IConvertible max, bool isDateRange)? Range { get; }
    int NumberOfLines { get; }
    int Width { get; }
    string Name(IObjectFacade objectFacade);
    string Description(IObjectFacade objectFacade);
    bool IsMandatory { get; }
    int? MaxLength { get; }
    string Pattern { get; }
    string Mask { get; }
    bool IsNullable { get; }
    bool IsPassword { get; }
    bool IsDateOnly { get; }
    string Grouping { get; }

    DataType? DataType { get; }
    int AutoCompleteMinLength { get; }

    bool IsFindMenuEnabled { get; }

    IObjectFacade[] GetChoices(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues);
    (IObjectFacade obj, string title)[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues);
    (string, ITypeFacade)[] GetChoicesParameters();
    IObjectFacade[] GetCompletions(IObjectFacade objectFacade, string autoCompleteParm);

    string GetMaskedValue(IObjectFacade objectFacade);
    bool DefaultTypeIsExplicit(IObjectFacade objectFacade);
}