// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;

namespace NakedObjects.Core.Component {
    public sealed class NakedObjectFactory {
        private bool isInitialized;
        private ILoggerFactory loggerFactory;
        private INakedObjectsFramework framework;

        // ReSharper disable ParameterHidesMember
        public void Initialize(INakedObjectsFramework framework, ILoggerFactory loggerFactory) {
            this.framework = framework;
            // ReSharper restore ParameterHidesMember
          
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            isInitialized = true;
        }

        public INakedObjectAdapter CreateAdapter(object obj, IOid oid) {
            if (!isInitialized) {
                throw new InitialisationException("NakedObjectFactory not initialized");
            }

            return new NakedObjectAdapter(obj, oid, framework, loggerFactory, loggerFactory.CreateLogger<NakedObjectAdapter>());
        }
    }
}