// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Set;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.Access;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class CollectionFieldMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {
        private static readonly ILog Log;
        private static readonly string[] FixedPrefixes;

        static CollectionFieldMethodsFacetFactory() {
            Log = LogManager.GetLogger(typeof (CollectionFieldMethodsFacetFactory));

            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.ClearPrefix,
                PrefixesAndRecognisedMethods.ModifyPrefix
            };
        }

        public CollectionFieldMethodsFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.CollectionsOnly) { }

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder collection) {
            string capitalizedName = property.Name;
            Type type = property.DeclaringType;

           
            var facets = new List<IFacet> {new PropertyAccessorFacetViaAccessor(property, collection)};

            AddSetFacet(facets, property, collection);

       
            AddHideForSessionFacetNone(facets, collection);
            AddDisableFacetAlways(facets, collection);
            FindDefaultHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], collection);        
            FindAndRemoveHideMethod(facets, methodRemover, type, MethodType.Object, capitalizedName, collection);        
            return FacetUtils.AddFacets(facets);
        }

       
        private static void AddSetFacet(ICollection<IFacet> collectionFacets, PropertyInfo property, IFacetHolder collection) {
            if (CollectionUtils.IsSet(property.PropertyType)) {
                collectionFacets.Add(new IsASetFacet(collection));
            }
        }

        public bool IsCollectionAccessor(MethodInfo method) {
            Type methodReturnType = method.ReturnType;
            return CollectionUtils.IsCollection(methodReturnType);
        }

        private static IList<Type> BuildCollectionTypes(IEnumerable<PropertyInfo> properties) {
            IList<Type> types = new List<Type>();

            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null &&
                    CollectionUtils.IsCollection(property.PropertyType) &&
                    !CollectionUtils.IsBlobOrClob(property.PropertyType) &&
                    property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                    !CollectionUtils.IsQueryable(property.PropertyType)) {
                    types.Add(property.PropertyType);
                }
            }
            return types;
        }

        public override void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            var propertiesToRemove = new List<PropertyInfo>();
            IList<Type> collectionTypes = BuildCollectionTypes(candidates);
            foreach (PropertyInfo property in candidates) {
                foreach (Type returnType in collectionTypes) {
                    if (property.GetGetMethod() != null && property.PropertyType == returnType) {
                        propertiesToRemove.Add(property);
                        methodListToAppendTo.Add(property);
                        break;
                    }
                }
            }

            foreach (PropertyInfo property in propertiesToRemove) {
                candidates.Remove(property);
            }
        }
    }
}