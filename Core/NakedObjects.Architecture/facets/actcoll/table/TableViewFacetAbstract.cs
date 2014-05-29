// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Actcoll.Table {
    public abstract class TableViewFacetAbstract : MultipleValueFacetAbstract, ITableViewFacet {
        protected TableViewFacetAbstract(bool title, string[] columns, IFacetHolder holder)
            : base(Type, holder) {
            Title = title;
            Columns = columns;
        }

        public static Type Type {
            get { return typeof (ITableViewFacet); }
        }

        public bool Title { get; set; }
        public string[] Columns { get; private set; }
    }
}