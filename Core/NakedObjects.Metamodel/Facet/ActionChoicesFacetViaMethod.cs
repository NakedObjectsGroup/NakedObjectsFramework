// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    public class ActionChoicesFacetViaMethod : ActionChoicesFacetAbstract, IImperativeFacet {
        private readonly MethodInfo choicesMethod;
        private readonly Type choicesType;
        private readonly bool isMultiple;
        private readonly string[] parameterNames;
        private readonly Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes;

        public ActionChoicesFacetViaMethod(MethodInfo choicesMethod, Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes, Type choicesType, ISpecification holder, bool isMultiple = false)
            : base(holder) {
            this.choicesMethod = choicesMethod;
            this.choicesType = choicesType;
            this.isMultiple = isMultiple;
            this.parameterNamesAndTypes = parameterNamesAndTypes;
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.Item1).ToArray();
        }

        public override Tuple<string, IObjectSpecImmutable>[] ParameterNamesAndTypes {
            get { return parameterNamesAndTypes; }
        }

        public override bool IsMultiple {
            get { return isMultiple; }
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return choicesMethod;
        }

        #endregion

        public override object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            INakedObject[] parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);

            try {
                var options = InvokeUtils.Invoke(choicesMethod, nakedObject, parms) as IEnumerable;
                if (options != null) {
                    return options.Cast<object>().ToArray();
                }
                throw new NakedObjectDomainException("Must return IEnumerable from choices method: " + choicesMethod.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("Choices exception: {0} has mismatched (ie type of choices parameter does not match type of action parameter) parameter types", choicesMethod.Name);
                throw new InvokeException(msg, ae);
            }
        }

        protected override string ToStringValues() {
            return "method=" + choicesMethod + ",Type=" + choicesType;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}