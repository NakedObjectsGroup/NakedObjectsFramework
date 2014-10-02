// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.DotNet.Facets.Ordering;

namespace NakedObjects.Reflector.Peer {
    public interface IOrderableElement<T> where T : IOrderableElement<T>, IFacetHolder {
        T Peer { get; }
        OrderSet<T> Set { get; }  
    }
}