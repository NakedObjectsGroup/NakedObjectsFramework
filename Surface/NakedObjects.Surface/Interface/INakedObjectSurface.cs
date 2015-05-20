// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Reflection;

namespace NakedObjects.Surface {
    public interface INakedObjectSurface : IScalarPropertyHolder, ISurfaceHolder {
        INakedObjectSpecificationSurface Specification { get; }
        INakedObjectSpecificationSurface ElementSpecification { get; }
        object Object { get; }
        IOidSurface Oid { get; }
        IVersionSurface Version { get; }
        IEnumerable<INakedObjectSurface> ToEnumerable();
        PropertyInfo[] GetKeys();
        INakedObjectSurface Page(int page, int size);
        INakedObjectSurface Select(object[] selection, bool forceEnumerable);
        int Count();
        AttachmentContext GetAttachment();
        object[] GetSelected();
        void Resolve();
        void SetIsNotQueryableState(bool state);
    }
}