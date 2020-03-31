// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Rest.Snapshot.Utility {
    public class FieldFacadeAdapter {
        private readonly IAssociationFacade association;
        private readonly IActionParameterFacade parameter;

        public FieldFacadeAdapter(IActionParameterFacade parameter) => this.parameter = parameter;

        public FieldFacadeAdapter(IAssociationFacade association) => this.association = association;

        public IFieldFacade AsField => (IFieldFacade) parameter ?? association;

        public string Id => parameter?.Id ?? association?.Id;

        public Choices IsChoicesEnabled => (parameter?.IsChoicesEnabled ?? association?.IsChoicesEnabled).GetValueOrDefault();
        public ITypeFacade Specification => parameter?.Specification ?? association?.Specification;
        public ITypeFacade ElementType => parameter?.ElementType ?? association?.ElementSpecification;
        public bool IsAutoCompleteEnabled => (parameter?.IsAutoCompleteEnabled ?? association?.IsAutoCompleteEnabled).GetValueOrDefault();
        public string Mask => parameter?.Mask ?? association?.Mask;
        public int NumberOfLines => (parameter?.NumberOfLines ?? association?.NumberOfLines).GetValueOrDefault();
        public string Name => parameter?.Name ?? association?.Name;
        public string Description => parameter?.Description ?? association?.Description;
        public bool IsMandatory => (parameter?.IsMandatory ?? association?.IsMandatory).GetValueOrDefault();
        public int? MaxLength => parameter?.MaxLength ?? association?.MaxLength;
        public string Pattern => parameter?.Pattern ?? association?.Pattern;
        public int AutoCompleteMinLength => (parameter?.AutoCompleteMinLength ?? association?.AutoCompleteMinLength).GetValueOrDefault();
        public DataType? DataType => parameter?.DataType ?? association?.DataType;
        public string PresentationHint => parameter?.PresentationHint ?? association?.PresentationHint;

        public Tuple<string, ITypeFacade>[] GetChoicesParameters() => parameter?.GetChoicesParameters() ?? association?.GetChoicesParameters();

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade objectFacade, IDictionary<string, object> parameterNameValues) => parameter?.GetChoicesAndTitles(objectFacade, parameterNameValues) ?? association?.GetChoicesAndTitles(objectFacade, parameterNameValues);

        public object GetChoiceValue(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade item, RestControlFlags flags) => association != null ? RestUtils.GetChoiceValue(oidStrategy, req, item, association, flags) : RestUtils.GetChoiceValue(oidStrategy, req, item, parameter, flags);

        public UriMtHelper GetHelper(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade) {
            if (parameter != null) {
                var parameterContext = new ParameterContextFacade {
                    Action = parameter.Action,
                    Target = objectFacade,
                    Parameter = parameter
                };
                return new UriMtHelper(oidStrategy, req, parameterContext);
            }

            return new UriMtHelper(oidStrategy, req, association, objectFacade);
        }
    }
}