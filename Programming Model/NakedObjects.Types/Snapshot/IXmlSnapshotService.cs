// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Snapshot {
    /// <summary>
    /// API for the XmlSnapshotService (defined in the Naked Objects Framework), allowing it to be injected into 
    /// domain code without the need for a dependence on the framework. The service can generate an XML 'snapshot'
    /// of any domain object.
    /// </summary>
    public interface IXmlSnapshotService {
        IXmlSnapshot GenerateSnapshot(object domainObject);
    }
}