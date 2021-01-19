// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using static AW.Helpers;
using AW.Types;

namespace AW.Functions {

    public static class ShiftFunctions {


        public static (Shift, IContext) ChangeTimes(this Shift s, TimeSpan startTime, TimeSpan endTime, IContext context)
        {
            var s2 = s with { StartTime = startTime } with { EndTime = endTime };
            return DisplayAndSave(s2, context);
        }

        public static TimeSpan Default1ChangeTimes(this Shift s) => new(0, 9, 0, 0);

    }
}