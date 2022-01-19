// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedLegacy;

namespace NakedLegacy.Reflector.Facet;

public static class AboutHelpers {
    public enum AboutType {
        Action,
        Field
    }

    public static object[] GetParameters(this MethodInfo method, object about, params object[] proposedValues) {
        var parameterCount = method.GetParameters().Length;
        var parameters = new List<object> { about };

        if (parameterCount > 1) {
            var placeholders = proposedValues?.Any() == true ? proposedValues : new object[parameterCount - 1];
            parameters.AddRange(placeholders);
        }

        return parameters.ToArray();
    }


    public static IAbout AboutFactory(this AboutType aboutType, AboutTypeCodes aboutTypeCode) =>
        aboutType is AboutType.Action
            ? new ActionAboutImpl(aboutTypeCode)
            : new FieldAboutImpl(aboutTypeCode);

}