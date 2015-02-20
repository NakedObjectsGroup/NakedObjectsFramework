// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Services;
using Snapshot.Xml.Test.Two;
using TransformFull = Snapshot.Xml.Test.One.TransformFull;

namespace Snapshot.Xml.Test {
    //[Named("")]
    public class TransformRepository : AbstractFactoryAndRepository {
        // 'fact' shortcut to add a factory method, 
        // 'alli' for an all-instances method
        // 'find' for a method to find a single object by query
        // 'list' for a method to return a list of objects matching a query

        public TransformFull TransformFull() {
            var obj = NewTransientInstance<TransformFull>();

            obj.FieldOne = 1;
            obj.FieldTwo = "";
            obj.FieldThree = 3;
            obj.FieldFour = "";

            return obj;
        }

        public Two.TransformFull TransformWithSubObject() {
            var subObj = NewTransientInstance<TransformSubObject>();

            subObj.FieldThree = 3;
            subObj.FieldFour = "";

            var obj = NewTransientInstance<Two.TransformFull>();

            obj.FieldOne = 1;
            obj.FieldTwo = "";
            obj.Content = subObj;

            return obj;
        }

        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion
    }
}