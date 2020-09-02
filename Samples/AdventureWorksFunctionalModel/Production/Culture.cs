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
    public class Culture : IHasModifiedDate {

        public Culture(string cultureID, string name, DateTime modifiedDate)
        {
            CultureID = cultureID;
            Name = name;
            ModifiedDate = modifiedDate;
        }

        public Culture() { }

        [NakedObjectsIgnore]
        public virtual string CultureID { get; set; }

        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

    }

    public static class CultureFunctions
    {
        public static string Title(this Culture c)
        {
            return c.CreateTitle(c.Name);
        }
        public static Culture Updating(Culture c, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(c, now);

        }
    }
}