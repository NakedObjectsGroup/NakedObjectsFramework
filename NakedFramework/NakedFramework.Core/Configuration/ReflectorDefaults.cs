// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Drawing;
using System.Linq;
using NakedFramework.Value;

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
}