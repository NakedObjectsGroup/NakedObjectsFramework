// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class ShiftFunctions {
        internal static IContext UpdateShift(Shift original, Shift updated, IContext context) =>
            context.WithUpdated(original, updated with { ModifiedDate = context.Now() });

        [Edit]
        public static IContext EditTimes(this Shift s,
                                         TimeSpan startTime, TimeSpan endTime, IContext context) =>
            UpdateShift(s, s with { StartTime = startTime, EndTime = endTime }, context);

        public static string ValidateEditTimes(
            this Shift s, TimeSpan startTime, TimeSpan endTime) =>
            endTime > startTime ? null : "End time must be after start time";

        [Edit]
        public static IContext EditName(this Shift s,
                                        string name, IContext context) =>
            UpdateShift(s, s with { Name = name }, context);
    }
}