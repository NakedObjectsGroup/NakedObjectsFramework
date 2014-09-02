// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {
    public interface IPolymorphicLinkWithOid<TRole, TOwner>
        where TRole : class
        where TOwner : class, IHasIntegerId {

        string RoleObjectOid { get; set; }

        TOwner Owner { get; set; }
    }
}