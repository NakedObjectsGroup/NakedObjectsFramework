// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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

        // Virtual so that a given implementation may wish to render the Root object visible 
        // and perhaps rename it with DisplayName

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
    }
}