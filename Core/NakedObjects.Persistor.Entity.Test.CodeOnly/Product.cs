// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;
using NUnit.Framework;

namespace TestCodeOnly {
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


    public class Product : AbstractTestCode{
        public IDomainObjectContainer Container { protected get; set; }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual Category Owningcategory { get; set; }

        protected Category forTest; 

        public object ExposeContainerForTest() {
            return Container;
        }

        public override void Loading() {
            base.Loading();
            // test scalar access OK 
            Assert.IsNotNull(Name);
        }

        public override void Loaded() {
            base.Loaded();

            try {
                // test lazy load OK  
                forTest = Owningcategory;
            }
            catch (Exception) {
                Assert.Fail("lazy load failed in loaded method");
            }
        }

    }
}