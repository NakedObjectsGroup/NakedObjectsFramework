// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
            return Container.NewTransientInstance<WithCollection>();
        }
    }
}