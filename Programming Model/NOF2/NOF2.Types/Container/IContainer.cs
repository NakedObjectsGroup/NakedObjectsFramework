// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NOF2.Container {
    public interface IContainer {
        void AddMessageToBroker(string message);
        void AddWarningToBroker(string message);
        void ClearWarnings();
        IEnumerable AllInstances(Type ofType);
        IQueryable<T> AllInstances<T>() where T : class;
        object DomainService(Type ofType);
        T DomainService<T>();
        object CreateTransientInstance(Type ofType);
        T CreateTransientInstance<T>() where T : new();
        void MakePersistent<T>(ref T transientObject);
        bool IsPersistent(object obj);
        IList<Type> AllTypes();
        void Resolve(object obj);
        T SystemService<T>();
    }
}