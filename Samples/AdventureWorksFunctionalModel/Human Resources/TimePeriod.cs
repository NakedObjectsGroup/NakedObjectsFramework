// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace AdventureWorksModel {

    [ComplexType]
    public class TimePeriod {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(StartTime.ToString().Substring(0,5)).Append(" ~ ").Append(EndTime.ToString().Substring(0,5));
            return t.ToString();
        }

        public virtual TimeSpan StartTime { get; set; }
        public virtual TimeSpan EndTime { get; set; }
    }
}