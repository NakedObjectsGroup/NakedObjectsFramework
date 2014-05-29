// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
using NakedObjects.Services;

namespace TestData {
    public class TestHelper {
        #region testcode

        private IDictionary<string, int> events;

        [NotMapped]
        public IDomainObjectContainer Container { protected get; set; }

        [NotMapped]
        public SimpleRepository<Person> MenuService { protected get; set; }

        [NotMapped]
        public SimpleRepository<Product> ContributedActions { protected get; set; }

        [NotMapped]
        public SimpleRepository<Address> SystemService { protected get; set; }

        [NotMapped]
        public bool HasContainer {
            get { return Container != null; }
        }

        [NotMapped]
        public bool HasMenuService {
            get { return MenuService != null; }
        }

        [NotMapped]
        public bool HasContributedActions {
            get { return ContributedActions != null; }
        }

        [NotMapped]
        public bool HasSystemService {
            get { return SystemService != null; }
        }

        [NakedObjectsIgnore]
        public IDictionary<string, int> GetEvents() {
            return events;
        }

        public void ResetEvents() {
            events = null;
        }

        private void SetupEvents() {
            if (events == null) {
                events = new Dictionary<string, int> {
                                                         {"Created", 0},
                                                         {"Updating", 0},
                                                         {"Updated", 0},
                                                         {"Loading", 0},
                                                         {"Loaded", 0},
                                                         {"Persisting", 0},
                                                         {"Persisted", 0},
                                                         {"Deleting", 0},
                                                         {"Deleted", 0}
                                                     };
            }
        }

        #endregion

        #region callbacks

        public virtual void Created() {
            SetupEvents();
            events["Created"]++;
        }

        public virtual void Updating() {
            SetupEvents();
            events["Updating"]++;
        }

        public virtual void Updated() {
            SetupEvents();
            events["Updated"]++;
        }

        public virtual void Loading() {
            SetupEvents();
            events["Loading"]++;
        }

        public virtual void Loaded() {
            SetupEvents();
            events["Loaded"]++;
        }

        public virtual void Persisting() {
            SetupEvents();
            events["Persisting"]++;
        }

        public virtual void Persisted() {
            SetupEvents();
            events["Persisted"]++;
        }

        public virtual void Deleting() {
            SetupEvents();
            events["Deleting"]++;
        }

        public virtual void Deleted() {
            SetupEvents();
            events["Deleted"]++;
        }

        #endregion
    }
}