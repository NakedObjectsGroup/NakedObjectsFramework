// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Core.Context;
using NakedObjects.Snapshot.Xml.Utility;

namespace NakedObjects.Snapshot {
    [Named("XML Snapshot")]
    public class XmlSnapshotService : IXmlSnapshotService {
        #region IXmlSnapshotService Members

        public INakedObjectsFramework Framework { set; protected get; }

        [NotContributedAction]
        public IXmlSnapshot GenerateSnapshot(object domainObject) {
            return new XmlSnapshot(domainObject, Framework.LifecycleManager, Framework.Manager);
        }

        #endregion
    }
}