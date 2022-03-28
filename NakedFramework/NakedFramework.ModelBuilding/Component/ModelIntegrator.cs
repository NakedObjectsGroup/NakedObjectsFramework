// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Configuration;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.ModelBuilding.Component;

public class ModelIntegrator : IModelIntegrator {
    private readonly ICoreConfiguration coreConfiguration;
    private readonly ILogger<ModelIntegrator> logger;
    private readonly IMenuFactory menuFactory;
    private readonly IMetamodelBuilder metamodelBuilder;
    private readonly IAllServiceList serviceList;
    private readonly HashSet<(IRemovableFacet facet, ISpecificationBuilder spec)> toRemove = new();

    public ModelIntegrator(IMetamodelBuilder metamodelBuilder,
                           IMenuFactory menuFactory,
                           ILogger<ModelIntegrator> logger,
                           ICoreConfiguration coreConfiguration,
                           IAllServiceList serviceList) {
        this.metamodelBuilder = metamodelBuilder;
        this.menuFactory = menuFactory;
        this.logger = logger;
        this.coreConfiguration = coreConfiguration;
        this.serviceList = serviceList;
    }

    private void AddToRemove(IRemovableFacet facet, ISpecificationBuilder spec) {
        lock (toRemove) {
            toRemove.Add((facet, spec));
        }
    }

    private void Execute((ITypeSpecImmutable spec, IIntegrationFacet facet) t) {
        t.facet.Execute(metamodelBuilder);
        AddToRemove(t.facet, t.spec);
    }


    public void Integrate() {
        var services = serviceList.Services;
        PopulateAssociatedActions(services, metamodelBuilder);

        // new way of doing things =- introduce integration facets

        (ITypeSpecImmutable spec, IIntegrationFacet facet)[] integrationFacets = metamodelBuilder.AllSpecifications.Select(s => (s, s.GetFacet<IIntegrationFacet>())).Where(t => t.Item2 is not null).ToArray();

        integrationFacets.ForEach(Execute);

        metamodelBuilder.AllSpecifications.OfType<IObjectSpecBuilder>().AsParallel().ForEach(PopulateLocalCollectionContributedActions);

        metamodelBuilder.AllSpecifications.OfType<ITypeSpecBuilder>().AsParallel().ForEach(spec => spec.CompleteIntegration());

        //Menus installed once rest of metamodel has been built:
        InstallMainMenus(metamodelBuilder);
        InstallObjectMenus(metamodelBuilder);

        // remove any contributed action facets after used by PopulateAssociatedActions and Menu Install

        toRemove.ForEach(tr => tr.facet.Remove(tr.spec));

        ValidateModel(metamodelBuilder);
    }

    private static IMenuActionImmutable[] GetMenuActions(IMenuItemImmutable item) =>
        item switch {
            IMenuActionImmutable actionImmutable => new[] { actionImmutable },
            IMenuImmutable menu => menu.MenuItems.SelectMany(GetMenuActions).ToArray(),
            _ => Array.Empty<IMenuActionImmutable>()
        };

    private static FacetUtils.ActionHolder ToActionHolder(IMenuActionImmutable a) => new(a);
    private static FacetUtils.ActionHolder ToActionHolder(IActionSpecImmutable a) => new(a);

    private void ValidateModel(IMetamodelBuilder builder) {
        foreach (var menu in metamodelBuilder.MainMenus.AsParallel()) {
            menu.MenuItems.SelectMany(GetMenuActions).Select(ToActionHolder).ErrorOnDuplicates();
        }

        foreach (var spec in metamodelBuilder.AllSpecifications.AsParallel()) {
            spec.OrderedObjectActions.Union(spec.OrderedContributedActions).Select(ToActionHolder).ErrorOnDuplicates();
        }
    }

    private void PopulateAssociatedActions(Type[] services, IMetamodelBuilder metamodel) {
        var nonServiceSpecs = metamodel.AllSpecifications.OfType<IObjectSpecBuilder>();

        foreach (var spec in nonServiceSpecs) {
            PopulateAssociatedActions(spec, services, metamodel);
        }
    }

    private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services, IMetamodelBuilder metamodel) {
        if (string.IsNullOrWhiteSpace(spec.FullName)) {
            var id = spec.Identifier?.ClassName ?? "unknown";
            logger.LogWarning($"Specification with id : {id} has null or empty name");
        }
        PopulateContributedActions(spec, services, metamodel);
    }

    private void InstallMainMenus(IMetamodelBuilder metamodel) {
        var menus = coreConfiguration.MainMenus?.Invoke(menuFactory);
        // Unlike other things specified in objectReflectorConfiguration, this one can't be checked when ObjectReflectorConfiguration is constructed.
        // Allows developer to deliberately not specify any menus
        if (menus is not null) {
            if (!menus.Any()) {
                //Catches accidental non-specification of menus
                throw new ReflectionException(logger.LogAndReturn("No MainMenus specified."));
            }

            foreach (var menu in menus.OfType<IMenuImmutable>()) {
                metamodel.AddMainMenu(menu);
            }
        }
    }

    private static void InstallObjectMenus(IMetamodelBuilder metamodel) {
        IEnumerable<(IMenuFacet f, ITypeSpecImmutable s)> menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => (s.GetFacet<IMenuFacet>(), s));
        menuFacets.ForEach(mf => mf.f.CreateMenu(metamodel, mf.s));
    }

    private static bool IsContributedTo(IContributedActionIntegrationFacet integrationFacet, IObjectSpecImmutable parmSpec, IObjectSpecImmutable contributeeSpec) =>
        integrationFacet.IsContributedTo(contributeeSpec) && contributeeSpec.IsOfType(parmSpec);

    private static bool IsContributedTo(IContributedActionIntegrationFacet integrationFacet, IActionSpecImmutable actionSpec, IObjectSpecImmutable objectSpec) =>
        actionSpec.Parameters.Any(p => IsContributedTo(integrationFacet, p.Specification, objectSpec));

    private static bool IsContributedToCollectionOf(IContributedActionIntegrationFacet integrationFacet, IObjectSpecImmutable objectSpec) =>
        integrationFacet.IsContributedToCollectionOf(objectSpec);

    private  void PopulateContributedActions(IObjectSpecBuilder objectSpec, Type[] services, IMetamodel metamodel) {
        var (contribActions, collContribActions, finderActions) = services.AsParallel().Select(serviceType => {
            var serviceSpecification = (ITypeSpecBuilder)metamodel.GetSpecification(serviceType);
            var serviceActions = serviceSpecification.UnorderedObjectActions.Where(sa => sa is not null).ToArray();

            var matchingActionsForObject = new List<IActionSpecImmutable>();
            var matchingActionsForCollection = new List<IActionSpecImmutable>();
            var matchingFinderActions = new List<IActionSpecImmutable>();

            foreach (var actionSpec in serviceActions) {
                var contributedActionFacet = actionSpec.GetFacet<IContributedActionIntegrationFacet>();

                if (contributedActionFacet is not null && serviceType != objectSpec.Type) {
                    if (IsContributedTo(contributedActionFacet, actionSpec, objectSpec)) {
                        matchingActionsForObject.Add(actionSpec);
                    }

                    if (IsContributedToCollectionOf(contributedActionFacet, objectSpec)) {
                        matchingActionsForCollection.Add(actionSpec);
                    }
                    AddToRemove(contributedActionFacet, actionSpec);
                }

                if (actionSpec.IsFinderMethodFor(objectSpec)) {
                    matchingFinderActions.Add(actionSpec);
                }
            }

            return (matchingActionsForObject, matchingActionsForCollection, matchingFinderActions);
        }).Aggregate((new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>()),
                     (a, t) => {
                         var (contrib, collContrib, finder) = a;
                         var (ca, cca, fa) = t;
                         contrib.AddRange(ca);
                         collContrib.AddRange(cca);
                         finder.AddRange(fa);
                         return a;
                     });

        objectSpec.AddContributedActions(contribActions, services);
        objectSpec.AddCollectionContributedActions(collContribActions);
        objectSpec.AddFinderActions(finderActions);
    }

    private void PopulateLocalCollectionContributedActions(IObjectSpecBuilder objectSpec) { 
        bool CollectionContributed(IActionSpecImmutable actionSpec, IContributedToLocalCollectionFacet contributedToLocalCollectionFacet, IObjectSpecImmutable elementSpec, string id) {
            if (contributedToLocalCollectionFacet is not null) {
                AddToRemove(contributedToLocalCollectionFacet, actionSpec);
                return contributedToLocalCollectionFacet.IsContributedToLocalCollectionOf(elementSpec, id);
            }

            return false;
        }

        static bool DirectlyContributed(IMemberOrderFacet memberOrderFacet, string id) => string.Equals(memberOrderFacet?.Name, id, StringComparison.CurrentCultureIgnoreCase);

        foreach (var collSpec in objectSpec.UnorderedFields.OfType<IOneToManyAssociationSpecBuilder>()) {
            var allActions = objectSpec.UnorderedObjectActions.Union(objectSpec.UnorderedContributedActions);
            var id = collSpec.Identifier.MemberName;

            var toAdd = allActions.Select(a => (a, a.GetFacet<IContributedToLocalCollectionFacet>(), a.GetFacet<IMemberOrderFacet>())).Where(t => DirectlyContributed(t.Item3, id) || CollectionContributed(t.a, t.Item2, collSpec.ElementSpec, id)).Select(t => t.a.Identifier.MemberName);

            collSpec.AddLocalContributedActions(toAdd.ToArray());
        }
    }
}