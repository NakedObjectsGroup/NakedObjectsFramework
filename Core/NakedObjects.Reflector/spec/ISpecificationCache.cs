// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.Spec {
    public interface ISpecificationCache {
        IIntrospectableSpecification GetSpecification(string className);

        void Clear();

        IIntrospectableSpecification[] AllSpecifications();

        void Cache(string className, IIntrospectableSpecification spec);
    }

    // Copyright (c) Naked Objects Group Ltd.
}