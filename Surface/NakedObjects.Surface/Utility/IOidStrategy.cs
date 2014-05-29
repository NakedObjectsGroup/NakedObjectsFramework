// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public interface IOidStrategy {
        INakedObjectsSurface Surface { set; }
        object GetDomainObjectByOid(LinkObjectId objectId);
        object GetServiceByServiceName(LinkObjectId serviceName);
        LinkObjectId GetOid(INakedObjectSurface nakedObject);

        INakedObjectSpecificationSurface GetSpecificationByLinkDomainType(string linkDomainType);
        string GetLinkDomainTypeBySpecification(INakedObjectSpecificationSurface spec);
    }
}