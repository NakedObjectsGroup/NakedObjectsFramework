// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Security; 

/// <summary>
///     Authorizes a list of roles (comma seperated) and/or users to view and/or edit the property. If the EditRoles/Users
///     or ViewRoles/Users parameter is present then
///     access is limited to that list. If it is absent then no restriction is imposed.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property)]
[Obsolete("Use AuthorizationManager")]
public class AuthorizePropertyAttribute : Attribute {
    public string ViewRoles { get; set; }
    public string EditRoles { get; set; }
    public string ViewUsers { get; set; }
    public string EditUsers { get; set; }
}