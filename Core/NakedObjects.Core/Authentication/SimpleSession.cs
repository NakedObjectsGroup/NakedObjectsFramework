// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Principal;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Authentication {
    public sealed class SimpleSession : WindowsSession {
        private readonly string code;

        public SimpleSession(IPrincipal principal, string code) : base(principal) {
            this.code = code;
        }

        public SimpleSession(IPrincipal principal)
            : this(principal, string.Empty) {}

        public string ValidationCode {
            get { return code; }
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("name", UserName);
            str.Append("code", ValidationCode);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}