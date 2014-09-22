// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Services;

namespace RestfulObjects.Test.Data {
    //[DisplayName("")]
    public class RestDataRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        // 'fact' shortcut to add a factory method, 
        // 'alli' for an all-instances method
        // 'find' for a method to find a single object by query
        // 'list' for a method to return a list of objects matching a query

        public MostSimple AzContributedAction(WithActionObject withAction) {
            return Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
        }

        public MostSimple AzContributedActionOnBaseClass(WithAction withAction) {
            return Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
        }

        public MostSimple AzContributedActionWithRefParm(WithActionObject withAction, WithActionObject withOtherAction) {
            return Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
        }

        public MostSimple AzContributedActionWithValueParm(WithActionObject withAction, string parm) {
            return Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
        }

        public MostSimple CreateTransientMostSimple() {
            return Container.NewTransientInstance<MostSimple>();
        }

        public WithValue CreateTransientWithValue() {
            var withValue = Container.NewTransientInstance<WithValue>();

            withValue.AValue = 102;
            withValue.AChoicesValue = 3;
            withValue.AConditionalChoicesValue = 3;
            withValue.ADisabledValue = 103;
            withValue.AStringValue = "one hundred four";
            withValue.AHiddenValue = 105;

            return withValue;
        }

        public WithReference CreateTransientWithReference() {
            var wr = Container.NewTransientInstance<WithReference>();

            wr.AReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            //wr.AHiddenReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            wr.AChoicesReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            wr.AConditionalChoicesReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            wr.ADisabledReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            wr.AnEagerReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            wr.AnAutoCompleteReference = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);

            return wr;
        }

        public WithCollection CreateTransientWithCollection() {
            var twc = Container.NewTransientInstance<WithCollection>();
            var ms1 = Container.Instances<MostSimple>().Single(ms => ms.Id == 1);
            var ms2 = Container.Instances<MostSimple>().Single(ms => ms.Id == 2);

            twc.ACollection.Add(ms1);
            twc.ACollection.Add(ms2);

            twc.ASet.Add(ms1);
            twc.ASet.Add(ms2);

            twc.ACollectionViewModels.Add(new MostSimpleViewModel() {Id = 1});
            twc.ACollectionViewModels.Add(new MostSimpleViewModel() {Id = 2});

            twc.ADisabledCollection.Add(ms1);
            twc.ADisabledCollection.Add(ms2);

            twc.AHiddenCollection.Add(ms1);
            twc.AHiddenCollection.Add(ms2);

            twc.AnEagerCollection.Add(ms1);
            twc.AnEagerCollection.Add(ms2);

            return twc;
        }
    }
}