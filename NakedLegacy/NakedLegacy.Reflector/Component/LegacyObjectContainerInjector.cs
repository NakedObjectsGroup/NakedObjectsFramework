// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedLegacy.Types.Container;

namespace NakedLegacy.Reflector.Component;

public sealed class LegacyObjectContainerInjector : IDomainObjectInjector {
    private IContainer container;
    private bool initialized;

    private void Initialize() {
        if (!initialized) {
            if (Framework == null) {
                throw new NakedObjectSystemException("no Framework");
            }

            container = new Container(Framework);
            initialized = true;
        }
    }

    #region IDomainObjectInjector Members

    public INakedFramework Framework { private get; set; }

    public void InjectInto(object obj) {
        if (obj is IContainerAware containerAware) {
            Initialize();
            if (container == null) {
                throw new NakedObjectSystemException("no container");
            }

            containerAware.Container = container;
        }
    }

    public void InjectIntoInline(object root, object inlineObject) { }

    public void InjectParentIntoChild(object parent, object child) { }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.