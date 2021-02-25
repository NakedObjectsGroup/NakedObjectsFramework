// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;

namespace NakedFramework.Core.Exception {
    public class ConcurrencyException : ObjectPersistenceException {
        public ConcurrencyException(INakedObjectAdapter nakedObjectAdapter)
            : this(NakedObjects.Resources.NakedObjects.ConcurrencyMessage, nakedObjectAdapter.Oid) =>
            SourceNakedObjectAdapter = nakedObjectAdapter;

        public ConcurrencyException(string message, IOid source)
            : base(message) =>
            SourceOid = source;

        public ConcurrencyException(string message, System.Exception cause)
            : base(message, cause) { }

        public IOid SourceOid { get; }
        public INakedObjectAdapter SourceNakedObjectAdapter { get; set; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}