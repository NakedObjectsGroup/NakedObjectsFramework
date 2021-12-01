// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Core.Util; 

public static class AdapterUtils {
    /// <summary>
    ///     Safe (returns null if INakedObjectAdapter is null) getter
    /// </summary>
    public static object GetDomainObject(this INakedObjectAdapter inObjectAdapter) => inObjectAdapter?.Object;

    /// <summary>
    ///     Return spec as object spec if is otherwise null
    /// </summary>
    /// <param name="nakedObjectAdapter"></param>
    /// <returns></returns>
    public static IObjectSpec GetObjectSpec(this INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.Spec as IObjectSpec;

    /// <summary>
    ///     Return spec as service spec if is otherwise null
    /// </summary>
    /// <param name="nakedObjectAdapter"></param>
    /// <returns></returns>
    public static IServiceSpec GetServiceSpec(this INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.Spec as IServiceSpec;

    /// <summary>
    ///     Safe (returns null if INakedObjectAdapter is null) generic getter
    /// </summary>
    public static T GetDomainObject<T>(this INakedObjectAdapter inObjectAdapter) => inObjectAdapter == null ? default : (T) inObjectAdapter.Object;

    public static bool Exists(this INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter?.Object != null;

    public static ICollectionFacet GetCollectionFacetFromSpec(this INakedObjectAdapter objectAdapterRepresentingCollection) {
        var collectionSpec = objectAdapterRepresentingCollection.Spec;
        return collectionSpec.GetFacet<ICollectionFacet>();
    }

    public static IEnumerable<INakedObjectAdapter> GetAsEnumerable(this INakedObjectAdapter objectAdapterRepresentingCollection, INakedObjectManager manager) => objectAdapterRepresentingCollection.GetCollectionFacetFromSpec().AsEnumerable(objectAdapterRepresentingCollection, manager);

    public static IQueryable GetAsQueryable(this INakedObjectAdapter objectAdapterRepresentingCollection) => objectAdapterRepresentingCollection.GetCollectionFacetFromSpec().AsQueryable(objectAdapterRepresentingCollection);

    public static ITypeOfFacet GetTypeOfFacetFromSpec(this INakedObjectAdapter objectAdapterRepresentingCollection) {
        var collectionSpec = objectAdapterRepresentingCollection.Spec;
        return collectionSpec.GetFacet<ITypeOfFacet>();
    }

    public static IActionSpec[] GetActionLeafNodes(this IActionSpec actionSpec) {
        return new[] {actionSpec};
    }

    public static IActionSpec[] GetActionLeafNodes(this INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.Spec.GetActionLeafNodes();

    public static IActionSpec[] GetActionLeafNodes(this ITypeSpec spec) => Enumerable.ToArray(spec.GetActions().SelectMany(GetActionLeafNodes));

    public static IActionSpec GetActionLeafNode(this INakedObjectAdapter nakedObjectAdapter, string actionName) {
        return nakedObjectAdapter.GetActionLeafNodes().Single(x => x.Id == actionName);
    }

    public static IAssociationSpec GetVersionProperty(this INakedObjectAdapter nakedObjectAdapter) {
        var spec = nakedObjectAdapter.Spec as IObjectSpec;

        return spec?.Properties.SingleOrDefault(x => x.ContainsFacet<IConcurrencyCheckFacet>());
    }

    private static DateTime StripMillis(this DateTime fullDateTime) =>
        new(fullDateTime.Year,
            fullDateTime.Month,
            fullDateTime.Day,
            fullDateTime.Hour,
            fullDateTime.Minute,
            fullDateTime.Second);

    public static object GetVersion(this INakedObjectAdapter nakedObjectAdapter) {
        var versionProperty = nakedObjectAdapter.GetVersionProperty();

        if (versionProperty is not null) {
            var version = versionProperty.GetNakedObject(nakedObjectAdapter).GetDomainObject();

            if (version is DateTime dtv) {
                return dtv.StripMillis();
            }

            return version;
        }

        return null;
    }
}