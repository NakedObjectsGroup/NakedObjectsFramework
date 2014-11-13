// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Security {
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