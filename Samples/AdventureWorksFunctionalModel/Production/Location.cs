// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("globe.png")]
    [Bounded]
    [Immutable]
    [PresentationHint("Topaz")]
    public class Location {

        public Location(
            short locationID,
            string name,
            decimal costRate,
            decimal availability,
            DateTime modifiedDate
            )
        {
            LocationID = locationID;
            Name = name;
            CostRate = costRate;
            Availability = availability;
            ModifiedDate = modifiedDate;
        }

        public Location() { }

        [NakedObjectsIgnore]
        public virtual short LocationID { get; set; }

        public virtual string Name { get; set; }

        [Mask("C")]
        public virtual decimal CostRate { get; set; }

        [Mask("########.##")]
        public virtual decimal Availability { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
    public static class LocationFunctions
    {
        public static string Title(this Location loc)
        {
            return loc.CreateTitle(loc.Name);
        }
        public static Location Updating(Location loc, [Injected] DateTime now)
        {
            return loc.With(x => x.ModifiedDate, now);
        }
    }
}