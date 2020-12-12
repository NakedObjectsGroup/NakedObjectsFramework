// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
using System;

namespace NakedFunctions {
    /// <summary>
    ///     When applied to an int or a double parameter on a function, indicates that this parameter should not be
    ///     offered to the user ina  dialog, but should be injected with a random value, equivalent to having called 
    ///      Next() or NextDouble(), respectively, on System.Random().
    ///     
    ///     Applying this attribute to a parameter that is not of type int or double will result in a reflection error.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RandomAttribute : Attribute { }
}