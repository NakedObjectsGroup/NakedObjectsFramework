// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace ModelFirst
{
    public abstract class AbstractTestCode {
        #region test code

        private IDictionary<string, int> callbackStatus;

        public virtual void Created() {
            SetupStatus();
            callbackStatus["Created"]++;
        }

        public virtual void Updating() {
            SetupStatus();
            callbackStatus["Updating"]++;
        }

        public virtual void Updated() {
            SetupStatus();
            callbackStatus["Updated"]++;
        }

        public virtual void Loading() {
            SetupStatus();
            callbackStatus["Loading"]++;
        }

        public virtual void Loaded() {
            SetupStatus();
            callbackStatus["Loaded"]++;
        }

        public virtual void Persisting() {
            SetupStatus();
            callbackStatus["Persisting"]++;
        }

        public virtual void Persisted() {
            SetupStatus();
            callbackStatus["Persisted"]++;
        }

        private void SetupStatus() {
            if (callbackStatus == null) {
                callbackStatus = new Dictionary<string, int> {                                                              
                                                                 {"Created", 0},
                                                                 {"Updating", 0},
                                                                 {"Updated", 0},
                                                                 {"Loading", 0},
                                                                 {"Loaded", 0},
                                                                 {"Persisting", 0},
                                                                 {"Persisted", 0}
                                                             };
            }
        }

        public void ResetCallbackStatus() {
            callbackStatus = null;
        }

        [NakedObjectsIgnore]
        public IDictionary<string, int> GetCallbackStatus() {
            return callbackStatus;
        }

        #endregion
    }


    public  class Person : AbstractTestCode
    {
        [Root]
        public object Parent { get; set; }

        public IDomainObjectContainer Container { protected get; set; }

        #region Primitive Properties
    
        public virtual int Id { get; set; }

        #endregion
    
        #region Complex Properties
    
        public virtual NameType ComplexProperty
        {
            get
            {
                return _complexProperty;
            }
            set
            {
                _complexProperty = value;
            }
    
        }
        private NameType _complexProperty = new NameType();
       
    
        public virtual ComplexType1 ComplexProperty_1
        {
            get
            {
                return _complexProperty_1;
            }
            set
            {
                _complexProperty_1 = value;
            }
    
        }
        private ComplexType1 _complexProperty_1 = new ComplexType1();
       

        #endregion
        #region Navigation Properties

        public virtual ICollection<Food> Food {
            get {
                return _food;
            }
        }

        private List<Food> _food = new List<Food>();

        #endregion

        public object ExposeContainerForTest() {
            return Container;
        }
    }
}
