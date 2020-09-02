// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using System.ComponentModel.DataAnnotations.Schema;
using NakedFunctions;
using static NakedFunctions.Result;

namespace AdventureWorksModel {
    [Bounded]
    [IconName("clock.png")]
    public class Shift : IHasModifiedDate  {

        #region ID

        [NakedObjectsIgnore]
        public virtual byte ShiftID { get; set; }

        #endregion

        #region Name

        [MemberOrder(1)]
        [StringLength(50)]
        [TypicalLength(10)]
        public virtual string Name { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region Complex Types

        private TimePeriod times = new TimePeriod();

        [MemberOrder(2)]
        public virtual TimePeriod Times {
            get { return times; }
            set { times = value; }
        }

        [Mask("T")]
        [MemberOrder(3), NotMapped]
        public virtual TimeSpan StartTime
        {
            get { return Times.StartTime; }
        }

        [Mask("T")]
        [MemberOrder(4), NotMapped]
        public virtual TimeSpan EndTime
        {
            get { return Times.EndTime; }
        }

        #endregion

    }
    public static class ShiftFunctions
    {
        public static string Title(Shift s)
        {
            return s.Name;
        }

        #region Life Cycle Methods
        public static Shift Updating(Shift p, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(p, now);

        }
        #endregion


        public static (Shift,Shift) ChangeTimes(Shift s, TimeSpan startTime, TimeSpan endTime)
        {
            var s2 = s.With(x => x.Times.StartTime, startTime).With(x => x.Times.EndTime,endTime);
            return DisplayAndPersist(s2);
        }

        public static TimeSpan Default0ChangeTimes(Shift s)
        {
            return new TimeSpan(0, 9, 0, 0);
        }
    }

}