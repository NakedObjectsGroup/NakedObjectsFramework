// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Xat.Interface;

namespace NakedFramework.Xat.TestObjects {
    internal class TestService : TestHasActions, ITestService {
        public TestService(INakedObjectAdapter service, ITestObjectFactory factory) : base(factory) => NakedObject = service;

        #region ITestService Members

        public override string Title => NakedObject.TitleString();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}