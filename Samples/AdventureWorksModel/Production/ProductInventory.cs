// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Menu;

namespace AdventureWorksModel {
    [IconName("cartons.png")]
    public class ProductInventory  {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [NakedObjectsIgnore]
        public virtual short LocationID { get; set; }

        [MemberOrder(40)]
        public virtual string Shelf { get; set; }

        [MemberOrder(50)]
        public virtual byte Bin { get; set; }

        [MemberOrder(10)]
        public virtual short Quantity { get; set; }

        [MemberOrder(30)]
        public virtual Location Location { get; set; }

        [MemberOrder(20)]
        public virtual Product Product { get; set; }

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Quantity.ToString()).Append(" in", Location).Append(" -", Shelf);
            return t.ToString();
        }

        #endregion

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion


        #region Sub-menu hierarchy for testing only

        public static void Menu(IMenu menu)
        {
            menu.CreateSubMenu("Sub Menu")
                .AddAction(nameof(Action1))
                .CreateSubMenu("Level 2 sub menu")
                .AddAction(nameof(Action2))
                .CreateSubMenu("Level 3 sub menu")
                .AddRemainingNativeActions();
        }
        public void Action1() { }
        public void Action2() { } 
        public void Action3() { }

        public void Action4() { }
        #endregion
    }
}