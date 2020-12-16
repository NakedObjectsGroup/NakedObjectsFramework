// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;

namespace NakedObjects {
    /// <summary>
    ///     A helper class that provides a ViewModel (implementing IViewModel) where all information
    ///     is derived from a single persistent object of type T, where T implements IHasIntegerId.
    /// </summary>
    public abstract class ViewModel<T> : IViewModel where T : class, IHasIntegerId {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public T Root { get; set; }

        public string[] DeriveKeys() {
            return new[] {Root.Id.ToString()};
        }

        public void PopulateUsingKeys(string[] keys) {
            int id = int.Parse(keys.Single());
            try {
                Root = Container.Instances<T>().Where(x => x.Id == id).Single();
            }
            catch {
                throw new DomainException("No instance with Id: " + id);
            }
        }

        public virtual bool HideRoot() {
            return true;
        }

        // Virtual so that a given implementation may wish to render the Root object visible 
        // and perhaps rename it with DisplayName
    }
}