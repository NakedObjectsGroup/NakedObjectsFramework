// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace RestfulObjects.Snapshot.Representations {
    public class OptionalProperty {
        public OptionalProperty(string name, object value) : this(name, value, value == null ? typeof (object) : value.GetType()) {}

        public OptionalProperty(string name, object value, Type propertyType) {
            Name = name;
            Value = value;
            PropertyType = propertyType;
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public Type PropertyType { get; set; }
    }
}