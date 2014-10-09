// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Core.Persist {
    /// <summary>
    ///     Generates OIDs based on the system clock
    /// </summary>
    public class TimeBasedOidGenerator : SimpleOidGenerator {
        public TimeBasedOidGenerator(IMetamodel metamodel)
            : base(metamodel, DateTime.Now.Ticks) {}

        public TimeBasedOidGenerator(IMetamodel metamodel, long start)
            : base(metamodel, start) {}

        public override string Name {
            get { return "Time Initialised OID Generator"; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}