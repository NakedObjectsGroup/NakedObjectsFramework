// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.SpecImmutable {
    /// <summary>
    ///   This is the immutable or 'static' core of the IObjectSpec.  It is created by the reflector during start-up, but can also be
    ///   serialised/deserialised and hence persisted.  However, it needs to be wrapped as an IObjectSpec at run-time in order to 
    ///   provide various run-time behaviours required of the Spec, which depend upon the run-time framework services.
    /// </summary>
    public interface IObjectSpecImmutable : ITypeSpecImmutable {}

    // Copyright (c) Naked Objects Group Ltd.
}