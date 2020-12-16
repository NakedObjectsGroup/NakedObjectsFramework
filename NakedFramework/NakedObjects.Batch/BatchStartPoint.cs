// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Async;

namespace NakedObjects.Batch {
    public class BatchStartPoint : IBatchStartPoint {
        public IAsyncService AsyncService { private get; set; }

        #region IBatchStartPoint Members

        public async void Execute() {
            await AsyncService.RunAsync
                (domainObjectContainer => { });
        }

        #endregion
    }
}