﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Menu;
using NakedObjects.Meta.Utils;

namespace NakedFramework.ModelBuilding.Component {
    public class ModelIntegrator : IModelIntegrator {
        private readonly IMetamodelBuilder metamodelBuilder;
        private readonly IMenuFactory menuFactory;
        private readonly ILogger<ModelIntegrator> logger;
        private readonly IObjectReflectorConfiguration objectReflectorConfiguration;
        private readonly IFunctionalReflectorConfiguration functionalReflectorConfiguration;

        public ModelIntegrator(IMetamodelBuilder metamodelBuilder, 
                               IMenuFactory menuFactory,
                               ILogger<ModelIntegrator> logger, 
                               IObjectReflectorConfiguration objectReflectorConfiguration, 
                               IFunctionalReflectorConfiguration functionalReflectorConfiguration) {
            this.metamodelBuilder = metamodelBuilder;
            this.menuFactory = menuFactory;
            this.logger = logger;
            this.objectReflectorConfiguration = objectReflectorConfiguration;
            this.functionalReflectorConfiguration = functionalReflectorConfiguration;
        }

        public void Integrate() {
            var services =  objectReflectorConfiguration.Services.Union(functionalReflectorConfiguration.Services).ToArray();
            PopulateAssociatedActions(services, metamodelBuilder);

            PopulateAssociatedFunctions(metamodelBuilder);

            //Menus installed once rest of metamodel has been built:
            InstallMainMenus(metamodelBuilder);
            InstallObjectMenus(metamodelBuilder);
        }

        private bool IsStatic(ITypeSpecImmutable spec)
        {
            return spec.Type.IsAbstract && spec.Type.IsSealed;
        }

        private bool IsNotStatic(ITypeSpecImmutable spec)
        {
            return !IsStatic(spec);
        }

        private bool IsContributedFunction(IActionSpecImmutable sa, ITypeSpecImmutable ts)
        {
            var f = sa.GetFacet<IContributedFunctionFacet>();
            return f != null && f.IsContributedTo(ts);
        }


        private void PopulateContributedFunctions(ITypeSpecBuilder spec, ITypeSpecImmutable[] functions, IMetamodel metamodel)
        {
            var result = functions.AsParallel().SelectMany(functionsSpec => {

                var serviceActions = functionsSpec.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions)
                {

                    if (IsContributedFunction(sa, spec))
                    {
                        matchingActionsForObject.Add(sa);
                    }
                }

                return matchingActionsForObject;
            }).ToList();

            spec.AddContributedFunctions(result);
        }

        private void PopulateAssociatedFunctions(IMetamodelBuilder metamodel)
        {
            // todo add facet for this 
            var functions = metamodel.AllSpecifications.Where(IsStatic).ToArray();
            var objects = metamodel.AllSpecifications.Where(IsNotStatic).Cast<ITypeSpecBuilder>();

            foreach (var spec in objects)
            {
                PopulateContributedFunctions(spec, functions, metamodel);
            }
        }

        private void PopulateAssociatedActions(Type[] services, IMetamodelBuilder metamodel)
        {
            var nonServiceSpecs = metamodel.AllSpecifications.OfType<IObjectSpecBuilder>();

            foreach (var spec in nonServiceSpecs)
            {
                PopulateAssociatedActions(spec, services, metamodel);
            }
        }

        private void PopulateAssociatedActions(IObjectSpecBuilder spec, Type[] services, IMetamodelBuilder metamodel)
        {
            if (string.IsNullOrWhiteSpace(spec.FullName))
            {
                var id = spec.Identifier?.ClassName ?? "unknown";
                logger.LogWarning($"Specification with id : {id} has null or empty name");
            }

            if (FasterTypeUtils.IsSystem(spec.FullName) && !spec.IsCollection)
            {
                return;
            }

            if (FasterTypeUtils.IsNakedObjects(spec.FullName))
            {
                return;
            }

            PopulateContributedActions(spec, services, metamodel);
        }

        // temp kludge 
        public Func<IMenuFactory, IMenu[]> MainMenus() =>
            mf => {
                var omm = objectReflectorConfiguration.MainMenus;
                var fmm = functionalReflectorConfiguration.MainMenus;
                IMenu[] menus = null;

                if (omm is not null && omm.Any())
                {
                    menus = omm.Select(tuple => {
                        var (type, name, addAll, action) = tuple;
                        var menu = mf.NewMenu(type, addAll, name);
                        action?.Invoke(menu);
                        return menu;
                    }).ToArray();
                }

                if (fmm is not null && fmm.Any())
                {
                    menus ??= new IMenu[] { };
                    menus = menus.Union(fmm.Select(tuple => {
                        var (type, name, addAll, action) = tuple;
                        var menu = mf.NewMenu(type, addAll, name);
                        action?.Invoke(menu);
                        return menu;
                    })).ToArray();
                }


                return menus;
            };


        private void InstallMainMenus(IMetamodelBuilder metamodel)
        {
        

            var menus = MainMenus()?.Invoke(menuFactory);
            // Unlike other things specified in objectReflectorConfiguration, this one can't be checked when ObjectReflectorConfiguration is constructed.
            // Allows developer to deliberately not specify any menus
            if (menus != null)
            {
                if (!menus.Any())
                {
                    //Catches accidental non-specification of menus
                    throw new ReflectionException(logger.LogAndReturn("No MainMenus specified."));
                }

                foreach (var menu in menus.OfType<IMenuImmutable>())
                {
                    metamodel.AddMainMenu(menu);
                }
            }
        }

        private static void InstallObjectMenus(IMetamodelBuilder metamodel)
        {
            var menuFacets = metamodel.AllSpecifications.Where(s => s.ContainsFacet<IMenuFacet>()).Select(s => s.GetFacet<IMenuFacet>());
            menuFacets.ForEach(mf => mf.CreateMenu(metamodel));
        }

        private static void PopulateContributedActions(IObjectSpecBuilder spec, Type[] services, IMetamodel metamodel)
        {
            var (contribActions, collContribActions, finderActions) = services.AsParallel().Select(serviceType => {
                var serviceSpecification = (IServiceSpecImmutable)metamodel.GetSpecification(serviceType);
                var serviceActions = serviceSpecification.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();
                var matchingActionsForCollection = new List<IActionSpecImmutable>();
                var matchingFinderActions = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions)
                {
                    if (serviceType != spec.Type)
                    {
                        if (sa.IsContributedTo(spec))
                        {
                            matchingActionsForObject.Add(sa);
                        }

                        if (sa.IsContributedToCollectionOf(spec))
                        {
                            matchingActionsForCollection.Add(sa);
                        }
                    }

                    if (sa.IsFinderMethodFor(spec))
                    {
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

            var groupedContribActions = contribActions.GroupBy(i => i.OwnerSpec.Type, i => i, (service, actions) => new { service, actions }).OrderBy(a => Array.IndexOf(services, a.service)).SelectMany(a => a.actions).ToList();

            spec.AddContributedActions(groupedContribActions);
            spec.AddCollectionContributedActions(collContribActions);
            spec.AddFinderActions(finderActions);
        }
    }
}