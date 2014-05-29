// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace RestfulObjects.Mvc.Model {
    public class SingleValueArgumentUrlBinder : AbstractModelBinder {
        public override bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext) {
            BindModelOnSuccessOrFail(bindingContext,
                                     () => ModelBinderUtils.CreateSingleValueArgument(DeserializeQueryString(actionContext)),
                                     ModelBinderUtils.CreateSingleValueArgumentForMalformedArgs);
            return true;
        }
    }
}