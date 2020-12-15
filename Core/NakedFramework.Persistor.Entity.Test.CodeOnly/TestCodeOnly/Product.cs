// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

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

        private void SetupStatus() =>
            callbackStatus ??= new Dictionary<string, int> {
                {"Created", 0},
                {"Updating", 0},
                {"Updated", 0},
                {"Loading", 0},
                {"Loaded", 0},
                {"Persisting", 0},
                {"Persisted", 0}
            };

        public void ResetCallbackStatus() {
            callbackStatus = null;
        }

        [NakedObjectsIgnore]
        public IDictionary<string, int> GetCallbackStatus() => callbackStatus;

        #endregion
    }

    public class Product : AbstractTestCode {
// ReSharper disable once NotAccessedField.Local
        private Category forTest;
        public IDomainObjectContainer Container { protected get; set; }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual Category Owningcategory { get; set; }

        public object ExposeContainerForTest() => Container;

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