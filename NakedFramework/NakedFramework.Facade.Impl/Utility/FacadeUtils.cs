// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Facade.Error;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Utility {
    public static class FacadeUtils {
        public static INakedObjectAdapter WrappedAdapter(this IObjectFacade objectFacade) => ((ObjectFacade) objectFacade)?.WrappedNakedObject;

        public static IActionParameterSpec WrappedSpec(this IActionParameterFacade actionParameterFacade) => ((ActionParameterFacade) actionParameterFacade)?.WrappedSpec;

        public static NakedObjectsFacadeException Map(Exception e) =>
            e switch {
                FindObjectException => new ObjectResourceNotFoundNOSException(e.Message, e),
                InvalidEntryException => new BadRequestNOSException(NakedObjects.Resources.NakedObjects.InvalidArguments, e),
                ArgumentException => new BadRequestNOSException(NakedObjects.Resources.NakedObjects.InvalidArguments, e),
                TargetParameterCountException => new BadRequestNOSException("Missing arguments", e), // todo i18n
                InvokeException when e.InnerException != null => Map(e.InnerException), // recurse on inner exception
                _ => new GeneralErrorNOSException(e)
            };

        public static IActionSpec[] GetActionsFromSpec(ITypeSpec spec) =>
            spec.GetActionLeafNodes().ToArray();
    }
}