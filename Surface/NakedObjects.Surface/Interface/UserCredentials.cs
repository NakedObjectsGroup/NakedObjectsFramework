// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace NakedObjects.Surface {
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