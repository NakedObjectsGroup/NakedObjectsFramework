// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Facade;
using NakedObjects.Facade.Facade;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Model {
    public class FileValue : IValue {
        private readonly IAttachmentFacade value;

        public FileValue(string data, string type, string name) => value = new JsonFileAdapter(data, type, name);

        #region IValue Members

        public object GetValue(IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) => value;

        #endregion
    }
}