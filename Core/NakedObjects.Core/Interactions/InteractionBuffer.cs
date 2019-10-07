// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Text;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Core.Interactions {
    public sealed class InteractionBuffer : IInteractionBuffer {
        private readonly StringBuilder buf = new StringBuilder();

        #region IInteractionBuffer Members

        public bool IsNotEmpty {
            get { return !IsEmpty; }
        }

        public bool IsEmpty {
            get { return buf.Length == 0; }
        }

        public void Append(string reason) {
            if (reason == null) {
                return;
            }

            if (IsNotEmpty) {
                buf.Append("; ");
            }

            buf.Append(reason);
        }

        public override string ToString() {
            return buf.ToString();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}