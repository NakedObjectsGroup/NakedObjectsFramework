// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedObjects.Facade.Translation;

namespace NakedObjects.Facade.Impl.Implementation {
    public class OidTranslatorSemiColonSeparatedList : IOidTranslator {
        private readonly INakedObjectsFramework framework;

        public OidTranslatorSemiColonSeparatedList(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IOidTranslator Members

        public IOidTranslation GetOidTranslation(params string[] id) {
            if (id.Length != 1) {
                throw new ObjectResourceNotFoundNOSException($"{id.Aggregate((s, t) => $"{s} {t}")}: Parsing Id error");
            }

            return string.IsNullOrEmpty(id.First())
                ? null
                : new OidTranslationSemiColonSeparatedList(id.First());
        }

        public IOidTranslation GetOidTranslation(IObjectFacade objectFacade) {
            if (objectFacade.IsViewModel) {
                var vm = ((ObjectFacade) objectFacade).WrappedNakedObject;
                framework.LifecycleManager.PopulateViewModelKeys(vm, framework);
            }

            var oid = objectFacade.Oid;
            var id = GetObjectId(oid);
            return GetOidTranslation(id);
        }

        #endregion

        private static string Encode(IEncodedToStrings encoder) => encoder.ToShortEncodedStrings().Aggregate((a, b) => $"{a};{b}");

        private static string GetObjectId(IOidFacade oid) => Encode((IEncodedToStrings) oid.Value);
    }
}