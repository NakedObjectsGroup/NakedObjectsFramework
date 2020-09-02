// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Bounded]
    public class ContactType : IHasModifiedDate {

        public ContactType(int contactTypeID, string name, DateTime modifiedDate)
        {
            ContactTypeID = contactTypeID;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public ContactType() { }

        [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }

        public virtual string Name { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class ContactTypeFunctions
    {
        public static string Title(this ContactType ct)
        {
            return ct.CreateTitle(ct.Name);
        }

        public static ContactType Updating(ContactType ct, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(ct, now);

        }
    }
}