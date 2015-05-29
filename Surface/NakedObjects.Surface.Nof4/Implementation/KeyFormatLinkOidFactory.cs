// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Services;
using NakedObjects.Surface.Interface;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Nof4.Wrapper;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Implementation {
    public class KeyFormatLinkOidFactory : ILinkOidFactory {
        private readonly INakedObjectsFramework framework;

        public KeyFormatLinkOidFactory(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        public ILinkObjectId GetLinkOid(params string[] id) {
            if (id.Count() == 2) {
                return new LinkObjectId(id.First(), id.Last());
            }
            if (id.Count() == 1) {
                return new LinkObjectId(id.First());
            }

            return null;
        }

        public ILinkObjectId GetLinkOid(INakedObjectSurface nakedObject) {
            if (nakedObject.IsViewModel) {
                var vm = ((NakedObjectWrapper)nakedObject).WrappedNakedObject;
                framework.LifecycleManager.PopulateViewModelKeys(vm);
            }

            Tuple<string, string> codeAndKey = GetCodeAndKeyAsTuple(nakedObject);
            return new LinkObjectId(codeAndKey.Item1, codeAndKey.Item2);
        }

        private string GetCode(INakedObjectSpecificationSurface spec) {
            return GetCode(TypeUtils.GetType(spec.FullName));
        }

        protected Tuple<string, string> GetCodeAndKeyAsTuple(INakedObjectSurface nakedObject) {
            string code = GetCode(nakedObject.Specification);
            return new Tuple<string, string>(code, GetKeyValues(nakedObject));
        }

        private string KeyRepresentation(object obj) {
            if (obj is DateTime) {
                obj = ((DateTime)obj).Ticks;
            }
            return (string)Convert.ChangeType(obj, typeof(string)); // better ? 
        }

        protected string GetKeyValues(INakedObjectSurface nakedObjectForKey) {
            string[] keys;
            INakedObjectAdapter wrappedNakedObject = ((NakedObjectWrapper)nakedObjectForKey).WrappedNakedObject;

            if (wrappedNakedObject.Spec.IsViewModel) {
                keys = wrappedNakedObject.Spec.GetFacet<IViewModelFacet>().Derive(wrappedNakedObject, framework.NakedObjectManager, framework.DomainObjectInjector);
            }
            else {
                PropertyInfo[] keyPropertyInfo = nakedObjectForKey.GetKeys();
                keys = keyPropertyInfo.Select(pi => KeyRepresentation(pi.GetValue(nakedObjectForKey.Object, null))).ToArray();
            }

            return GetKeyCodeMapper().CodeFromKey(keys, nakedObjectForKey.Object.GetType());
        }

        private static object CoerceType(Type type, string value) {
            if (type == typeof(DateTime)) {
                long ticks = long.Parse(value);
                return new DateTime(ticks);
            }

            return Convert.ChangeType(value, type);
        }

        private IDictionary<string, object> CreateKeyDictionary(string[] keys, Type type) {
            PropertyInfo[] keyProperties = framework.Persistor.GetKeys(type);
            int index = 0;
            return keyProperties.ToDictionary(kp => kp.Name, kp => CoerceType(kp.PropertyType, keys[index++]));
        }


        private ITypeCodeMapper GetTypeCodeMapper() {
            return (ITypeCodeMapper)framework.ServicesManager.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultTypeCodeMapper();
        }

        private IKeyCodeMapper GetKeyCodeMapper() {
            return (IKeyCodeMapper)framework.ServicesManager.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultKeyCodeMapper();
        }

        private string[] GetKeys(string instanceId, Type type) {
            return GetKeyCodeMapper().KeyFromCode(instanceId, type);
        }

        private string GetCode(Type type) {
            return GetTypeCodeMapper().CodeFromType(type);
        }
    }
}