// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Collections.Modify {
    public class CollectionResetFacet : FacetAbstract, IImperativeFacet, ICollectionResetFacet {
        private readonly PropertyInfo property;

        public CollectionResetFacet(PropertyInfo property, IFacetHolder holder)
            : base(Type, holder) {
            this.property = property;
        }

        public static Type Type {
            get { return typeof (ICollectionResetFacet); }
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return property.GetGetMethod();
        }

        #endregion

        public void Reset(INakedObject inObject) {
            try {
                var collection = (IList) property.GetValue(inObject.GetDomainObject(), null);
                collection.Clear();
                property.SetValue(inObject.GetDomainObject(), collection, null);
            }
            catch (Exception e) {
                throw new ReflectionException(string.Format("Failed to get/set property {0} in {1}", property.Name, inObject.Specification.FullName), e);
            }
        }

        protected override string ToStringValues() {
            return "property=" + property;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}