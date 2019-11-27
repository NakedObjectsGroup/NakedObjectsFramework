// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Model {
    public class ArgumentMapBinder : IModelBinder {
        #region IModelBinder Members

        public Task BindModelAsync(ModelBindingContext bindingContext) {
            return BindModelOnSuccessOrFail(bindingContext,
                async () => ModelBinderUtils.CreateArgumentMap(await DeserializeJsonContent(bindingContext)),
                () => new Task<ArgumentMap>(ModelBinderUtils.CreateArgumentMapForMalformedArgs));
        }

        #endregion

        protected static async Task BindModelOnSuccessOrFail(ModelBindingContext bindingContext, Func<Task<ArgumentMap>> parseFunc, Func<Task<ArgumentMap>> failFunc) {
            try {
                try {
                    bindingContext.Result = ModelBindingResult.Success(await parseFunc());
                }
                catch (Exception e) {
                    LogManager.GetLogger<ArgumentMapBinder>().ErrorFormat("Parsing of request arguments failed: {0}", e.Message);
                    bindingContext.Result = ModelBindingResult.Success(await failFunc());
                }
            }
            catch (Exception e) {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        protected static async Task<JObject> DeserializeJsonContent(ModelBindingContext bindingContext) {
            if (bindingContext.HttpContext.Request.ContentLength > 0) {
                return await ModelBinderUtils.DeserializeJsonStreamAsync(bindingContext.HttpContext.Request.Body);
            }

            return new JObject();
        }
    }
}