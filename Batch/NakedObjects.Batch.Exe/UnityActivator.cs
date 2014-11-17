// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Batch.Exe {
    public static class UnityActivator {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() {
            UnityConfig.GetConfiguredContainer().Resolve<IReflector>().Reflect();
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown() {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}