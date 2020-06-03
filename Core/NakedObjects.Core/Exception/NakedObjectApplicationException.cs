// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Core {
    /// <summary>
    ///     A NakedObjectApplicationException represents exception that has occurred within the domain code, or as a result
    ///     of the domain code.  These indicate that the application developer need to fix their code.
    /// </summary>
    public abstract class NakedObjectApplicationException : NakedObjectException {
        protected NakedObjectApplicationException(string messsage) : base(messsage) { }
        protected NakedObjectApplicationException(string messsage, Exception cause) : base(messsage, cause) { }
    }
}