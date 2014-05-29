// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Transaction {
    public class TransactionDecoratorInstaller : IReflectionDecoratorInstaller {
        #region IReflectionDecoratorInstaller Members

        public virtual string Name {
            get { return "transaction"; }
        }

        public virtual IFacetDecorator[] CreateDecorators() {
            return new IFacetDecorator[] {new TransactionDecorator()};
        }

        #endregion
    }
}