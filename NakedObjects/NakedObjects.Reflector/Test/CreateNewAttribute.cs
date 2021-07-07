// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.CompilerServices;
using NakedFramework;

[assembly:InternalsVisibleTo("NakedFramework.Rest.Test.Data")]

namespace NakedObjects {
    /// <summary>
    ///     Applied to a function, indicating that the primary role of the function is to create a new persistent object of the
    ///     specified type
    ///     with the minimum data set, but with the expectation that the user will likely want to immediately then specify
    ///     further properties (see EditAttribute).
    /// </summary>
    internal class CreateNewAttribute : AbstractCreateNewAttribute { }
}