// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Services;
using NakedObjects.Util;

namespace NakedObjects.Facade.Impl.Implementation {
    public class OidTranslatorSlashSeparatedTypeAndIds : IOidTranslator {
        private readonly INakedObjectsFramework framework;

        public OidTranslatorSlashSeparatedTypeAndIds(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IOidTranslator Members

        public IOidTranslation GetOidTranslation(params string[] id) {
            if (id.Length == 2) {
                return new OidTranslationSlashSeparatedTypeAndIds(id.First(), id.Last());
            }
            if (id.Length == 1) {
                return new OidTranslationSlashSeparatedTypeAndIds(id.First());
            }

            return null;
        }

        public IOidTranslation GetOidTranslation(IObjectFacade objectFacade) {
            if (objectFacade.IsViewModel) {
                var vm = ((ObjectFacade) objectFacade).WrappedNakedObject;
                framework.LifecycleManager.PopulateViewModelKeys(vm);
            }

            Tuple<string, string> codeAndKey = GetCodeAndKeyAsTuple(objectFacade);
            return new OidTranslationSlashSeparatedTypeAndIds(codeAndKey.Item1, codeAndKey.Item2);
        }

        #endregion

        private string GetCode(ITypeFacade spec) {
            return GetCode(TypeUtils.GetType(spec.FullName));
        }

        protected Tuple<string, string> GetCodeAndKeyAsTuple(IObjectFacade nakedObject) {
            string code = GetCode(nakedObject.Specification);
            return new Tuple<string, string>(code, GetKeyValues(nakedObject));
        }

        private string KeyRepresentation(object obj) {
            if (obj is DateTime) {
                obj = ((DateTime) obj).Ticks;
            }
            if (obj is Guid)
            {
                obj = obj.ToString();
            }
            return (string) Convert.ChangeType(obj, typeof(string)); 
        }

        protected string GetKeyValues(IObjectFacade nakedObjectForKey) {
            string[] keys;
            INakedObjectAdapter wrappedNakedObject = ((ObjectFacade) nakedObjectForKey).WrappedNakedObject;

            if (wrappedNakedObject.Spec.IsViewModel) {
                keys = wrappedNakedObject.Spec.GetFacet<IViewModelFacet>().Derive(wrappedNakedObject, framework.NakedObjectManager, framework.DomainObjectInjector);
            }
            else {
                PropertyInfo[] keyPropertyInfo = nakedObjectForKey.GetKeys();
                keys = keyPropertyInfo.Select(pi => KeyRepresentation(pi.GetValue(nakedObjectForKey.Object, null))).ToArray();
            }

            return GetKeyCodeMapper().CodeFromKey(keys, nakedObjectForKey.Object.GetType());
        }

        private ITypeCodeMapper GetTypeCodeMapper() {
            return (ITypeCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultTypeCodeMapper();
        }

        private IKeyCodeMapper GetKeyCodeMapper() {
            return (IKeyCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultKeyCodeMapper();
        }

        private string GetCode(Type type) {
            return GetTypeCodeMapper().CodeFromType(type);
        }
    }
}