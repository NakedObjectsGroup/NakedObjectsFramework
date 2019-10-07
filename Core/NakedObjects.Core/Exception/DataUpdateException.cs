// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core {
    public class DataUpdateException : ObjectPersistenceException {
        private readonly IOid sourceOid;

        public DataUpdateException(INakedObjectAdapter nakedObjectAdapter, IVersion updated)
            : this(string.Format(Resources.NakedObjects.DataUpdateMessage, nakedObjectAdapter.Version.User, nakedObjectAdapter.TitleString(), DateTime.Now.ToLongTimeString(), Environment.NewLine, Environment.NewLine, nakedObjectAdapter.Version, updated), nakedObjectAdapter.Oid) {}

        public DataUpdateException(string message, IOid source)
            : base(message) {
            sourceOid = source;
        }

        public DataUpdateException(string message, Exception cause)
            : base(message, cause) {}

        public virtual IOid SourceOid {
            get { return sourceOid; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}