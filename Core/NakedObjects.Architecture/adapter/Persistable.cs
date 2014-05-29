// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Adapter {
    public class Persistable {
        /// <summary>
        ///     Marks a class as being persistable, but only by under application program control
        /// </summary>
        public static readonly Persistable PROGRAM_PERSISTABLE = new Persistable("Program Persistable");

        /// <summary>
        ///     Marks a class as transient - such an object cannot be persisted
        /// </summary>
        public static readonly Persistable TRANSIENT = new Persistable("Transient");

        /// <summary>
        ///     Marks a class as being persistable by the user (or under application program control)
        /// </summary>
        public static readonly Persistable USER_PERSISTABLE = new Persistable("User Persistable");

        private readonly string name;

        private Persistable(string name) {
            this.name = name;
        }

        public override string ToString() {
            return name;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}