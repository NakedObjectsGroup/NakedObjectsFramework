// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Services;
using NakedObjects.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public class TestTypeCodeMapper : ITypeCodeMapper {
        private const string DefaultPrefix = "RestfulObjects.Test.Data.";
        private static readonly IDictionary<string, string> PrefixDictionary = new Dictionary<string, string>();

        #region ITypeCodeMapper Members

        public Type TypeFromCode(string code) {
            var fullCode = TypeStringFromCode(code);
            return TypeUtils.GetType(fullCode);
        }

        public string CodeFromType(Type type) => CodeFromTypeString(type.FullName);

        #endregion

        public string TypeStringFromCode(string code) {
            if (PrefixDictionary.ContainsKey(code)) {
                return PrefixDictionary[code];
            }

            return DefaultPrefix + code;
        }

        public string CodeFromTypeString(string typeString) {
            var code = typeString.Split('.').Last();
            PrefixDictionary[code] = typeString;
            return code;
        }
    }
}