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
    [Bounded]
    [Immutable]
    public class AddressType: IHasModifiedDate, IHasRowGuid {
        public AddressType(
            int addressTypeID,
            string name,
            Guid rowguid,
            DateTime modifiedDate
            )
        {
            AddressTypeID = addressTypeID;
            Name = name;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public AddressType()
        {
        }

        [NakedObjectsIgnore]
        public virtual int AddressTypeID { get; set; }

        [NakedObjectsIgnore]
        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [NakedObjectsIgnore]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }

    public static class AddressTypeFunctions
    {
        public static string Title(this AddressType a)
        {
            return a.CreateTitle(a.Name);
        }

        public static AddressType Updating(AddressType a, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(a, now);

        }
    }
}