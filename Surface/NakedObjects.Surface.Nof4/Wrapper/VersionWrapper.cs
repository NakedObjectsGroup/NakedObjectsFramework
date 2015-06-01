// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Surface.Nof4.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class VersionWrapper : IVersionFacade {
        private readonly IVersion version;

        public VersionWrapper(IVersion version) {
            SurfaceUtils.AssertNotNull(version, "Version is null");

            this.version = version;
        }

        public DateTime? Time {
            get { return version.Time; }
        }

        #region IVersionFacade Members

        public string Digest {
            get { return version.Digest; }
        }

        public bool IsDifferent(string digest) {
            return version.IsDifferent(digest);
        }

        #endregion
    }
}