// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Specify the minimum and maximum allowed values for a numeric field, or the minimum and maximum
    ///     number of characters for a string field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute {
        public RangeAttribute(int minimum, int maximum = int.MaxValue) : base(minimum, maximum) { }

        public double MinInt { get; private set; }
        public double MaxInt { get; private set; }
    }
}