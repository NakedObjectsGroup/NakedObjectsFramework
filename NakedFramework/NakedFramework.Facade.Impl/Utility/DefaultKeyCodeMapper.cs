// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Facade.Translation;
using NakedObjects.Services;

namespace NakedFramework.Facade.Impl.Utility {
    public class DefaultKeyCodeMapper : IKeyCodeMapper {
        private readonly string keySeparator;

        public DefaultKeyCodeMapper() => keySeparator = OidTranslationSlashSeparatedTypeAndIds.KeySeparator;

        #region IKeyCodeMapper Members

        public string[] KeyFromCode(string code, Type type) => code.Split(new[] {keySeparator}, StringSplitOptions.None);

        public string CodeFromKey(string[] key, Type type) => key.Length == 0 ? "" : key.Aggregate((s, t) => $"{s}{keySeparator}{t}");

        #endregion
    }
}