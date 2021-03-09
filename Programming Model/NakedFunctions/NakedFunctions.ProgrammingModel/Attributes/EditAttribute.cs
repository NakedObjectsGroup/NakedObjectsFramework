// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Applied to a function, indicating that the function is intended to allow the user to edit the value of one or more
    ///     properties
    ///     on the type the function is contributed to.  The function's paramaters (after the first, 'contributee, parameter)
    ///     must match
    ///     properties on the contributee type, both in type and name (except for casing).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EditAttribute : Attribute { }
}