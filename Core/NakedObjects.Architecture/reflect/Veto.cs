// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Reflect {
    public class Veto : ConsentAbstract {
        /// <summary>
        ///     A Veto object with no reason
        /// </summary>
        public static readonly Veto Default = new Veto();

        public Veto() {}

        public Veto(string reason)
            : base(reason) {}

        public Veto(Exception exception)
            : base(exception) {}

        /// <summary>
        ///     Returns <c>false</c>
        /// </summary>
        public override bool IsAllowed {
            get { return false; }
        }

        /// <summary>
        ///     Returns <c>true</c>
        /// </summary>
        public override bool IsVetoed {
            get { return true; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}