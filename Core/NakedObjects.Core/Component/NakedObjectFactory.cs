// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class NakedObjectFactory {
        private bool isInitialized;
        private ILifecycleManager lifecycleManager;
        private ILoggerFactory loggerFactory;
        private IMetamodelManager metamodelManager;
        private INakedObjectManager nakedObjectManager;
        private IObjectPersistor persistor;
        private ISession session;

        // ReSharper disable ParameterHidesMember
        public void Initialize(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager nakedObjectManager, ILoggerFactory loggerFactory) {
            // ReSharper restore ParameterHidesMember
            this.metamodelManager = metamodelManager ?? throw new InitialisationException($"{nameof(metamodelManager)} is null");
            this.session = session ?? throw new InitialisationException($"{nameof(session)} is null");
            this.lifecycleManager = lifecycleManager ?? throw new InitialisationException($"{nameof(lifecycleManager)} is null");
            this.persistor = persistor ?? throw new InitialisationException($"{nameof(persistor)} is null");
            this.nakedObjectManager = nakedObjectManager ?? throw new InitialisationException($"{nameof(nakedObjectManager)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            isInitialized = true;
        }

        public INakedObjectAdapter CreateAdapter(object obj, IOid oid) {
            Assert.AssertTrue(isInitialized);
            return new NakedObjectAdapter(metamodelManager, session, persistor, lifecycleManager, nakedObjectManager, obj, oid, loggerFactory, loggerFactory.CreateLogger<NakedObjectAdapter>());
        }
    }
}