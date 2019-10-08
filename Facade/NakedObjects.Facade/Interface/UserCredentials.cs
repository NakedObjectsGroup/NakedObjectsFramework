// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace NakedObjects.Facade {
    public class UserCredentials {
        public UserCredentials(string user, string password, List<string> roles) {
            User = user;
            Password = password;
            Roles = roles.AsReadOnly();
        }

        public string User { get; private set; }
        public string Password { get; private set; }
        public IList<string> Roles { get; private set; }
    }
}