// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Services;

namespace Snapshot.Xml.Test {
    //[Named("")]
    public class TransformRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        // 'fact' shortcut to add a factory method, 
        // 'alli' for an all-instances method
        // 'find' for a method to find a single object by query
        // 'list' for a method to return a list of objects matching a query

        public One.TransformFull TransformFull() {
            var obj = NewTransientInstance<One.TransformFull>();

            obj.FieldOne = 1;
            obj.FieldTwo = "";
            obj.FieldThree = 3;
            obj.FieldFour = "";

            return obj;
        }

        public Two.TransformFull TransformWithSubObject() {
            var subObj = NewTransientInstance<Two.TransformSubObject>();

            subObj.FieldThree = 3;
            subObj.FieldFour = "";

            var obj = NewTransientInstance<Two.TransformFull>();

            obj.FieldOne = 1;
            obj.FieldTwo = "";
            obj.Content = subObj;

            return obj;
        }
    }
}