// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;
using AW.Types;

namespace AW.Functions {
    public static class EmployeePayHistoryFunctions
    {
        #region Life Cycle Methods
        public static EmployeePayHistory Updating(this EmployeePayHistory x, IContext context) => x with { ModifiedDate = context.Now() };

        public static EmployeePayHistory Persisting(this EmployeePayHistory x, IContext context) => x with { ModifiedDate = context.Now() };

        //TODO: I don't think this is necessary
        //public static EmployeePayHistory Persisted(this EmployeePayHistory x, IContainer context)
        //{
        //    Employee.PayHistory.Add(this);
        //}
        #endregion
    }
}