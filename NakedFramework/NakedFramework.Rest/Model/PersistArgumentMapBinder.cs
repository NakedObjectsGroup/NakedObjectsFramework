// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NakedObjects.Rest.Model {
    public class PersistArgumentMapBinder : IModelBinder {
        #region IModelBinder Members

        public Task BindModelAsync(ModelBindingContext bindingContext) =>
            ModelBinderUtils.BindModelOnSuccessOrFail(bindingContext,
                async () => ModelBinderUtils.CreatePersistArgMap(await ModelBinderUtils.DeserializeJsonContent(bindingContext), true),
                ModelBinderUtils.CreateMalformedArguments<PersistArgumentMap>);

        #endregion
    }
}