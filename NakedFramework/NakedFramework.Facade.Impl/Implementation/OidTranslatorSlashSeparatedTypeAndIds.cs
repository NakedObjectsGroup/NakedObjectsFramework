// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Services;

namespace NakedObjects.Facade.Impl.Implementation {
    public class OidTranslatorSlashSeparatedTypeAndIds : IOidTranslator {
        private readonly INakedObjectsFramework framework;

        public OidTranslatorSlashSeparatedTypeAndIds(INakedObjectsFramework framework) => this.framework = framework;

        #region IOidTranslator Members

        public IOidTranslation GetOidTranslation(params string[] id) =>
            id.Length switch {
                2 => new OidTranslationSlashSeparatedTypeAndIds(id.First(), id.Last()),
                1 => new OidTranslationSlashSeparatedTypeAndIds(id.First()),
                _ => null
            };

        public IOidTranslation GetOidTranslation(IObjectFacade objectFacade) {
            if (objectFacade.IsViewModel) {
                var vm = ((ObjectFacade) objectFacade).WrappedNakedObject;
                framework.LifecycleManager.PopulateViewModelKeys(vm);
            }

            var (code, key) = GetCodeAndKeyAsTuple(objectFacade);
            return new OidTranslationSlashSeparatedTypeAndIds(code, key);
        }

        #endregion

        private string GetCode(ITypeFacade spec) => GetCode(TypeUtils.GetType(spec.FullName));

        protected (string code, string key) GetCodeAndKeyAsTuple(IObjectFacade nakedObject) {
            var code = GetCode(nakedObject.Specification);
            return (code, GetKeyValues(nakedObject));
        }

        private static string KeyRepresentation(object obj) {
            var key = obj switch {
                DateTime time => time.Ticks,
                Guid _ => obj.ToString(),
                _ => obj
            };

            return (string) Convert.ChangeType(key, typeof(string));
        }

        protected string GetKeyValues(IObjectFacade nakedObjectForKey) {
            string[] keys;
            var wrappedNakedObject = ((ObjectFacade) nakedObjectForKey).WrappedNakedObject;

            if (wrappedNakedObject.Spec.IsViewModel) {
                keys = wrappedNakedObject.Spec.GetFacet<IViewModelFacet>().Derive(wrappedNakedObject, framework.NakedObjectManager, framework.DomainObjectInjector, framework.Session, framework.Persistor);
            }
            else {
                var keyPropertyInfo = nakedObjectForKey.GetKeys();
                keys = keyPropertyInfo.Select(pi => KeyRepresentation(pi.GetValue(nakedObjectForKey.Object, null))).ToArray();
            }

            return GetKeyCodeMapper().CodeFromKey(keys, nakedObjectForKey.Object.GetType());
        }

        private ITypeCodeMapper GetTypeCodeMapper() =>
            (ITypeCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
            ?? new DefaultTypeCodeMapper();

        private IKeyCodeMapper GetKeyCodeMapper() =>
            (IKeyCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
            ?? new DefaultKeyCodeMapper();

        private string GetCode(Type type) => GetTypeCodeMapper().CodeFromType(type);
    }
}