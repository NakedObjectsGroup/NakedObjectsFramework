// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace Expenses.Services {
    /// <summary> Defines a service for getting hold of the current user as an object.  Will typically
    /// be implemented by a service that manages a type of object that represents users
    /// (e.g. an EmployeeRepository or OrganisationRepository).
    /// 
    /// </summary>
    /// <author>  Richard
    /// 
    /// </author>
    public interface IUserFinder {
        object CurrentUserAsObject();
    }
}