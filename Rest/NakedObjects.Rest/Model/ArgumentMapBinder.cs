// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NakedObjects.Rest.Model {
    public class ArgumentMapBinder : IModelBinder {
        #region IModelBinder Members

        public Task BindModelAsync(ModelBindingContext bindingContext) {
            if (new HttpMethod(bindingContext.HttpContext.Request.Method) == HttpMethod.Get) {
                return BindFromQuery(bindingContext);
            }

            return BindFromBody(bindingContext);
        }

        #endregion

        private static Task BindFromBody(ModelBindingContext bindingContext) {
            return ModelBinderUtils.BindModelOnSuccessOrFail(bindingContext,
                async () => ModelBinderUtils.CreateArgumentMap(await ModelBinderUtils.DeserializeJsonContent(bindingContext), true),
                () => new Task<ArgumentMap>(ModelBinderUtils.CreateArgumentMapForMalformedArgs<ArgumentMap>));
        }

        private static Task BindFromQuery(ModelBindingContext bindingContext) {
            return ModelBinderUtils.BindModelOnSuccessOrFail(bindingContext,
                async () => ModelBinderUtils.CreateSimpleArgumentMap(bindingContext.HttpContext.Request.QueryString.ToString()) ??
                            ModelBinderUtils.CreateArgumentMap(await ModelBinderUtils.DeserializeQueryString(bindingContext), false),
                () => new Task<ArgumentMap>(ModelBinderUtils.CreateArgumentMapForMalformedArgs<ArgumentMap>));
        }
    }
}