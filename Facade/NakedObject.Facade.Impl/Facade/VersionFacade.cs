// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class VersionFacade : IVersionFacade {
        private readonly IVersion version;

        public VersionFacade(IVersion version) {
            FacadeUtils.AssertNotNull(version, "Version is null");

            this.version = version;
        }

        public DateTime? Time => version.Time;

        #region IVersionFacade Members

        public string Digest => version.Digest;

        public bool IsDifferent(string digest) {
            return version.IsDifferent(digest);
        }

        #endregion
    }
}