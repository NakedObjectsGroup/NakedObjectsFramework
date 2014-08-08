// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof;
using NakedObjects.Reflector.DotNet.Facets.Collections.Modify;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    public class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsPropertiesAndCollections) { }

        private bool ProcessArray(Type type, IFacetHolder holder) {
            holder.AddFacet(new DotNetArrayFacet(holder, type.GetElementType()));
            holder.AddFacet(new TypeOfFacetInferredFromArray(type.GetElementType(), holder, Reflector));
            return true;
        }

        private bool ProcessGenericEnumerable(Type type, IFacetHolder holder) {
            var typeOfFacet = holder.GetFacet<ITypeOfFacet>();
            bool isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable 
            bool isQueryable = CollectionUtils.IsGenericQueryable(type);
            bool isSet = CollectionUtils.IsSet(type);
            Type collectionElementType;
            if (typeOfFacet != null) {
                collectionElementType = typeOfFacet.Value;
            }
            else {
                collectionElementType = CollectionUtils.ElementType(type);
                holder.AddFacet(new TypeOfFacetInferredFromGenerics(collectionElementType, holder, Reflector));
            }

            Type facetType = isQueryable ? typeof (DotNetGenericIQueryableFacet<>) : (isCollection ? typeof (DotNetGenericCollectionFacet<>) : typeof (DotNetGenericIEnumerableFacet<>));

            Type genericFacet = facetType.GetGenericTypeDefinition();
            Type genericCollectionFacetType = genericFacet.MakeGenericType(CollectionUtils.ElementType(type));
            var facet = (IFacet) Activator.CreateInstance(genericCollectionFacetType, holder, collectionElementType, isSet);
            holder.AddFacet(facet);

            return true;
        }


        private bool ProcessCollection(IFacetHolder holder) {
            var typeOfFacet = holder.GetFacet<ITypeOfFacet>();
            Type collectionElementType;
            if (typeOfFacet != null) {
                collectionElementType = typeOfFacet.Value;
            }
            else {
                collectionElementType = typeof (object);
                holder.AddFacet(new TypeOfFacetDefaultToObject(holder, Reflector));
            }
            holder.AddFacet(new DotNetCollectionFacet(holder, collectionElementType));
            return true;
        }


        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (CollectionUtils.IsGenericEnumerable(type)) {
                return ProcessGenericEnumerable(type, holder);
            }
            if (type.IsArray) {
                return ProcessArray(type, holder);
            }
            if (CollectionUtils.IsCollectionButNotArray(type)) {
                return ProcessCollection(holder);
            }

            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            if (CollectionUtils.IsCollectionButNotArray(property.PropertyType)) {
                holder.AddFacet(new CollectionResetFacet(property, holder));
                return true;
            }
            return base.Process(property, methodRemover, holder);
        }
    }
}