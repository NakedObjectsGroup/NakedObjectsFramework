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
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.ModelBuilding.Component {
    public class ModelIntegrator : IModelIntegrator {
        private readonly ICoreConfiguration coreConfiguration;
        private readonly ILogger<ModelIntegrator> logger;
        private readonly IMenuFactory menuFactory;
        private readonly IMetamodelBuilder metamodelBuilder;
        private readonly IAllServiceList serviceList;

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

        public void Integrate() {
            var services = serviceList.Services;
            PopulateAssociatedActions(services, metamodelBuilder);

            // new way of doing things =- introduce integration facets

            var integrationFacets = metamodelBuilder.AllSpecifications.Select(s => s.GetFacet<IIntegrationFacet>()).Where(f => f is not null).ToArray();

            integrationFacets.ForEach(f => f.Execute(metamodelBuilder));
            integrationFacets.ForEach(f => f.Remove());

            metamodelBuilder.AllSpecifications.OfType<ITypeSpecBuilder>().ForEach(spec => spec.CompleteIntegration());

            //Menus installed once rest of metamodel has been built:
            InstallMainMenus(metamodelBuilder);
            InstallObjectMenus(metamodelBuilder);

            ValidateModel(metamodelBuilder);
        }

        private static IMenuActionImmutable[] GetMenuActions(IMenuItemImmutable item) =>
            item switch {
                IMenuActionImmutable actionImmutable => new[] {actionImmutable},
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
                spec.ObjectActions.Union(spec.ContributedActions).Select(ToActionHolder).ErrorOnDuplicates();
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

            if (FasterTypeUtils.IsSystem(spec.FullName) && !spec.IsCollection) {
                return;
            }

            if (FasterTypeUtils.IsNakedObjects(spec.FullName)) {
                return;
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
            var menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private static void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services, IMetamodel metamodel) {
            var (contribActions, collContribActions, finderActions) = services.AsParallel().Select(serviceType => {
                var serviceSpecification = (IServiceSpecImmutable) metamodel.GetSpecification(serviceType);
                var serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();
                var matchingActionsForCollection = new List<IActionSpecImmutable>();
                var matchingFinderActions = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions) {
                    if (serviceType != spec.Type) {
                        if (sa.IsContributedTo(spec)) {
                            matchingActionsForObject.Add(sa);
                        }

                        if (sa.IsContributedToCollectionOf(spec)) {
                            matchingActionsForCollection.Add(sa);
                        }
                    }

                    if (sa.IsFinderMethodFor(spec)) {
                        matchingFinderActions.Add(sa);
                    }
                }

                return (matchingActionsForObject, matchingActionsForCollection, matchingFinderActions.OrderBy(a => a, new MemberOrderComparator<IActionSpecImmutable>()).ToList());
            }).Aggregate((new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>(), new List<IActionSpecImmutable>()),
                         (a, t) => {
                             var (contrib, collContrib, finder) = a;
                             var (ca, cca, fa) = t;
                             contrib.AddRange(ca);
                             collContrib.AddRange(cca);
                             finder.AddRange(fa);
                             return a;
                         });

            var groupedContribActions = contribActions.GroupBy(i => i.OwnerSpec.Type, i => i, (service, actions) => new {service, actions}).OrderBy(a => Array.IndexOf(services, a.service)).SelectMany(a => a.actions).ToList();

            spec.AddContributedActions(groupedContribActions);
            spec.AddCollectionContributedActions(collContribActions);
            spec.AddFinderActions(finderActions);
        }
    }
}