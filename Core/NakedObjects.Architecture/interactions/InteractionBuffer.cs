// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Text;

namespace NakedObjects.Architecture.Interactions {
    public class InteractionBuffer {
        private readonly StringBuilder buf = new StringBuilder();

        public virtual bool IsNotEmpty {
            get { return !IsEmpty; }
        }

        public virtual bool IsEmpty {
            get { return buf.Length == 0; }
        }

        public virtual void Append(string reason) {
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
    }


    // Copyright (c) Naked Objects Group Ltd.
}