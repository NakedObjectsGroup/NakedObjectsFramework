// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Value;

namespace NakedObjects.Facade.Impl.Utility {
    internal static class FrameworkHelper {
        public static INakedObjectAdapter GetNakedObject(this INakedObjectsFramework framework, object domainObject) => framework.NakedObjectManager.CreateAdapter(domainObject, null, null);

        public static bool IsNotPersistent(this INakedObjectAdapter nakedObject) =>
            nakedObject.ResolveState.IsTransient() && nakedObject.Spec.Persistable == PersistableType.ProgramPersistable ||
            nakedObject.Spec.Persistable == PersistableType.Transient;

        public static bool IsImage(this ITypeSpec spec, INakedObjectsFramework framework) {
            var imageSpec = framework.MetamodelManager.GetSpecification(typeof(Image));
            return spec != null && spec.IsOfType(imageSpec);
        }

        private static bool IsFileAttachment(this ITypeSpec spec, INakedObjectsFramework framework) {
            var fileSpec = framework.MetamodelManager.GetSpecification(typeof(FileAttachment));
            return spec != null && spec.IsOfType(fileSpec);
        }

        public static bool IsFile(this IAssociationSpec assoc, INakedObjectsFramework framework) => assoc.ReturnSpec.IsFile(framework);

        public static bool IsFile(this ITypeSpec spec, INakedObjectsFramework framework) => spec != null && (spec.IsImage(framework) || spec.IsFileAttachment(framework) || spec.ContainsFacet<IArrayValueFacet<byte>>());

        public static bool IsQueryOnly(this IActionSpec action) => action.ReturnSpec.IsQueryable || action.ContainsFacet<IQueryOnlyFacet>();

        public static bool IsIdempotent(this IActionSpec action) => action.ContainsFacet<IIdempotentFacet>();

        public static bool IsViewModelEditView(this INakedObjectAdapter target, ISession session, IObjectPersistor persistor) => target.Spec.ContainsFacet<IViewModelFacet>() && target.Spec.GetFacet<IViewModelFacet>().IsEditView(target, session, persistor);
    }
}