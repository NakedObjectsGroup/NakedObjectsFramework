// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     For base subclasses or, more likely, to help write tests
    /// </summary>
    public class NamedAndDescribedFacetHolderImpl : FacetHolderImpl, INamedAndDescribedFacetHolder {
        private readonly string description;
        private readonly string name;

        public NamedAndDescribedFacetHolderImpl(string name)
            : this(name, null) {}

        public NamedAndDescribedFacetHolderImpl(string name, string description) {
            this.name = name;
            this.description = description;
        }

        #region INamedAndDescribedFacetHolder Members

        public virtual string Name {
            get { return name; }
        }

        public virtual string Description {
            get { return description; }
        }

        #endregion
    }
}