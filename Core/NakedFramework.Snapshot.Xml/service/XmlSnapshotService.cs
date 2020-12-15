﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedObjects.Snapshot.Xml.Utility;

namespace NakedObjects.Snapshot.Xml.Service {
    [Named("XML Snapshot")]
    public class XmlSnapshotService : IXmlSnapshotService {
        public INakedObjectsFramework Framework { set; protected get; }
        public ILoggerFactory LoggerFactory { set; protected get; }

        #region IXmlSnapshotService Members

        public IXmlSnapshot GenerateSnapshot(object domainObject) => new XmlSnapshot(domainObject, Framework.NakedObjectManager, Framework.MetamodelManager, LoggerFactory);

        #endregion
    }
}