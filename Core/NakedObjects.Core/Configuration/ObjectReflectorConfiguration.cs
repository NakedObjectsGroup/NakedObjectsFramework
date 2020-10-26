// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Drawing;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Menu;
using NakedObjects.Value;

namespace NakedObjects.Core.Configuration {

    public static class ReflectorDefaults {
        public static readonly Type[] DefaultSystemTypes = {
            typeof(bool),
            typeof(byte),
            typeof(char),
            typeof(Color),
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            typeof(FileAttachment),
            typeof(float),
            typeof(Guid),
            typeof(Image),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(short),
            typeof(string),
            typeof(TimeSpan),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(bool[]),
            typeof(byte[]),
            typeof(char[]),
            typeof(Color[]),
            typeof(DateTime[]),
            typeof(decimal[]),
            typeof(double[]),
            typeof(FileAttachment[]),
            typeof(float[]),
            typeof(Guid[]),
            typeof(Image[]),
            typeof(int[]),
            typeof(long[]),
            typeof(sbyte[]),
            typeof(short[]),
            typeof(string[]),
            typeof(TimeSpan[]),
            typeof(uint[]),
            typeof(ulong[]),
            typeof(ushort[]),
            typeof(object),
            typeof(void),
            typeof(List<>),
            typeof(ObjectQuery<>),
            typeof(EnumerableQuery<>),
            typeof(ISet<>),
            typeof(IList<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IQueryable<>),
            typeof(HashSet<>),
            typeof(EntityCollection<>),
            // WhereEnumerableIterator
            new List<int>().Where(i => true).GetType().GetGenericTypeDefinition(),
            // WhereSelectEnumerableIterator
            new List<int>().Where(i => true).Select(i => i).GetType().GetGenericTypeDefinition(),
            // UnionIterator
            new List<int>().Union(new List<int>()).GetType().GetGenericTypeDefinition(),
            typeof(Action<>)
        };
    }


    [Serializable]
    public class ObjectReflectorConfiguration : IObjectReflectorConfiguration {
        public ObjectReflectorConfiguration(Type[] typesToIntrospect,
                                            Type[] services,
                                            string[] modelNamespaces,
                                            bool concurrencyChecking = true) {
            ModelNamespaces = modelNamespaces;
            SupportedSystemTypes = ReflectorDefaults.DefaultSystemTypes.ToList();
            TypesToIntrospect = typesToIntrospect;
            Services = services;
            IgnoreCase = false;
            ConcurrencyChecking = concurrencyChecking;
            
            ValidateConfig();

        }

        // for testing
        public static bool NoValidate { get; set; }

        private void ValidateConfig() {
            if (NoValidate) {
                return;
            }

            var msg = "Reflector configuration errors;\r\n";
            var configError = false;

            if (Services == null || !Services.Any()) {
                configError = true;
                msg += "No services specified;\r\n";
            }

            if (ModelNamespaces == null || !ModelNamespaces.Any()) {
                configError = true;
                msg += "No Namespaces specified;\r\n";
            }

            if (configError) {
                throw new InitialisationException(msg);
            }
        }

        private static Type EnsureGenericTypeIsComplete(Type type)
        {
            if (type.IsGenericType && !type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }

            return type;
        }

        private Type[] GetObjectTypesToIntrospect()
        {
            var types = TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            var oSystemTypes = SupportedSystemTypes.Select(EnsureGenericTypeIsComplete);
   
            var systemTypes = oSystemTypes;
            return types.Union(systemTypes).ToArray();
        }

        #region IObjectReflectorConfiguration Members

        public Type[] TypesToIntrospect { get; }
        public bool IgnoreCase { get; }
        public bool ConcurrencyChecking { get; }
        public bool HasConfig() => TypesToIntrospect.Any() && Services.Any();

        public Type[] Services { get; }
        public string[] ModelNamespaces { get; }
        public List<Type> SupportedSystemTypes { get; }


        public Type[] ObjectTypes => Services.Union(GetObjectTypesToIntrospect()).ToArray();


        #endregion
    }
}