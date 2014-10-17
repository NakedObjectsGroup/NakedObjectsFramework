// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Whether the action should be invoked locally, remotely,
    ///     or on the default location depending on its persistence state
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the action method using <see cref="ExecutedAttribute" />
    /// </para>
    public interface IExecutedControlMethodFacet : IFacet {
        Where ExecutedWhere(MethodInfo method);
        void AddMethodExecutedWhere(MethodInfo method, Where where);
    }
}