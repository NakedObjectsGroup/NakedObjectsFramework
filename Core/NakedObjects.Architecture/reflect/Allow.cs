// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     An instance of this type is used to allow something
    /// </summary>
    public class Allow : ConsentAbstract {
        /// <summary>
        ///     An Allow object with no reason
        /// </summary>
        public static readonly Allow Default = new Allow();

        public Allow() {}

        public Allow(string reason)
            : base(reason) {}

        /// <summary>
        ///     Returns <c>true</c>
        /// </summary>
        public override bool IsAllowed {
            get { return true; }
        }

        /// <summary>
        ///     Returns <c>false</c>
        /// </summary>
        public override bool IsVetoed {
            get { return false; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}