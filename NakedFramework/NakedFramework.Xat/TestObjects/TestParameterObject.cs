// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Exception;
using NakedFramework.Xat.Interface;

namespace NakedFramework.Xat.TestObjects {
    internal class TestParameterObject : ITestValue {
        private readonly object domainObject;
        private readonly INakedObjectManager manager;

        public TestParameterObject(INakedObjectManager manager, object domainObject) {
            this.manager = manager;
            this.domainObject = domainObject;
        }

        #region ITestValue Members

        public string Title => NakedObject.TitleString();

        public INakedObjectAdapter NakedObject {
            get => manager.CreateAdapter(domainObject, null, null);
            set => throw new UnexpectedCallException();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}