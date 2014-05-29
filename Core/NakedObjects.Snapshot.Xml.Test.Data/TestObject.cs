// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Snapshot.Xml.Test {
    public class TestObject {
        #region Injected Services
        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service; 'injc' to add an injected Container

        #endregion
        #region Life Cycle Methods
        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.


        #endregion

        [Key, Hidden]
        public virtual int TestObjectId { get; set; }

        //Add properties with 'propv', collections with 'coll', actions with 'act' shortcuts

        public virtual int TestInt { get; set; }
        public virtual string TestString { get; set; }

        public virtual TestObject TestReference { get; set; }

        private ICollection<TestObject> testCollection = new List<TestObject>();
        public virtual ICollection<TestObject> TestCollection {
            get { return testCollection; }
            set { testCollection = value; }
        }
    }
}

