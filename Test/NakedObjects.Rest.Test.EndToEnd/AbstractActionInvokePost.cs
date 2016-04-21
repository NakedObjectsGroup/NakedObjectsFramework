// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    public class AbstractActionInvokePost : AbstractAction {
        public void DoAnActionReturnsVoidWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionInvoke(Domain.AnActionReturnsVoidWithParameters, parms.ToString(), Methods.Post);
        }

        public void DoSyntacticallyMalformedParameters() {
            var parm1 = new JObject(new JProperty("value", 101));
            var parm2 = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
            var parms = new JObject(new JProperty("parm1", parm1), new JProperty("param2", parm2)); //i.e. mis-named parameter
            TestActionInvoke(Domain.AnActionReturnsVoidWithParameters, parms.ToString(), Methods.Post, Codes.SyntacticallyInvalid);

            parms = new JObject(new JProperty("parm1", parm1), new JProperty("param2", parm2), new JProperty("parm3", parm2)); //i.e. additional parameter
            TestActionInvoke(Domain.AnActionReturnsVoidWithParameters, parms.ToString(), Methods.Post, Codes.SyntacticallyInvalid);

            // invalid json 
            string invalidParms = @"{""parm1"" : {""value"" : 101} ""parm2"" : {""value"" : " + JsonRep.MostSimple1AsRef() + "    }} "; // missing , 
            parms = new JObject(new JProperty("parm1", parm1), new JProperty("param2", parm2), new JProperty("parm3", parm2)); //i.e. additional parameter
            TestActionInvoke(Domain.AnActionReturnsVoidWithParameters, parms.ToString(), Methods.Post, Codes.SyntacticallyInvalid);
        }

        public void DoSemanticallyMalformedParameters() {
            var parm1 = new JObject(new JProperty("value", "foo")); //i.e. string where it should be integer
            var parm2 = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
            var parms = new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2));
            TestActionInvoke(Domain.AnActionReturnsVoidWithParameters, parms.ToString(), Methods.Post, Codes.SyntacticallyInvalid);
        }

        public void DoADisabledAction() {
            TestActionInvoke("ADisabledAction", JsonRep.Empty(), Methods.Post, Codes.Forbidden);
        }

        public void DoADisabledCollectionAction() {
            TestActionInvoke("ADisabledCollectionAction", JsonRep.Empty(), Methods.Post, Codes.Forbidden);
        }

        public void DoAnAction() {
            TestActionInvoke("AnAction", JsonRep.Empty(), Methods.Post);
        }

        public void DoAttemptInvokePutActionWithPost() {
            // currently allowed - may need revisiting
            TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Post);
        }

        public void DoAttemptInvokeGetActionWithPost() {
            // currently allowed - may need revisiting
            TestActionInvoke("AnActionAnnotatedQueryOnly", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsCollection() {
            TestActionInvoke("AnActionReturnsCollection", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsCollectionEmpty() {
            TestActionInvoke("AnActionReturnsCollectionEmpty", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsCollectionNull() {
            TestActionInvoke("AnActionReturnsCollectionNull", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsCollectionWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionInvoke("AnActionReturnsCollectionWithParameters", parms.ToString(), Methods.Post);
        }

        public void DoAnActionReturnsCollectionWithScalarParameters() {
            JObject parms = Parm1Is100Parm2IsFred();
            TestActionInvoke("AnActionReturnsCollectionWithScalarParameters", parms.ToString(), Methods.Post);
        }

        public void DoAnActionReturnsNull() {
            TestActionInvoke("AnActionReturnsNull", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsObjectWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionInvoke("AnActionReturnsObjectWithParameters", parms.ToString(), Methods.Post);
        }

        public void DoAnActionReturnsScalar() {
            TestActionInvoke("AnActionReturnsScalar", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsScalarEmpty() {
            TestActionInvoke("AnActionReturnsScalarEmpty", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsScalarNull() {
            TestActionInvoke("AnActionReturnsScalarNull", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionReturnsScalarWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionInvoke("AnActionReturnsScalarWithParameters", parms.ToString(), Methods.Post);
        }

        public void DoAnActionReturnsVoid() {
            TestActionInvoke("AnActionReturnsVoid", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionWithDateTimeParm() {
            var parm = new JObject(new JProperty("value", "2012-08-01"));
            var parms = new JObject(new JProperty("parm", parm));
            TestActionInvoke("AnActionWithDateTimeParm", parms.ToString(), Methods.Post);
        }

        public void DoAnActionWithOptionalParm() {
            //Test with valid parm
            var parm = new JObject(new JProperty("value", "FOO"));
            var parms = new JObject(new JProperty("parm", parm));
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post);

            //Test without parm
            TestActionInvoke("AnActionWithOptionalParm", JsonRep.Empty(), Methods.Post);
        }

        public void DoAnActionWithReferenceParameter() {
            var parm = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
            var parms = new JObject(new JProperty("parm2", parm)); //"parm2" is correct!
            TestActionInvoke("AnActionWithReferenceParameter", parms.ToString(), Methods.Post);
        }

        public void DoAnActionWithValueParameter() {
            var parm = new JObject(new JProperty("value", "1"));
            var parms = new JObject(new JProperty("parm1", parm));
            TestActionInvoke("AnActionWithValueParameter", parms.ToString(), Methods.Post);
        }

        public void DoAnActionWithValueParameterWithChoices() {
            var parm = new JObject(new JProperty("value", "1"));
            var parms = new JObject(new JProperty("parm3", parm));
            TestActionInvoke("AnActionWithValueParameterWithChoices", parms.ToString(), Methods.Post);
        }

        public void DoAnError() {
            TestActionInvoke("AnError", JsonRep.Empty(), Methods.Post, Codes.ServerException);
        }

        public void DoAnErrorCollection() {
            TestActionInvoke("AnErrorCollection", JsonRep.Empty(), Methods.Post, Codes.ServerException);
        }

        public void DoParameterValidateOnlyGood() {
            var parm = new JObject(new JProperty("value", "foo"));
            var parms = new JObject(new JProperty("parm", parm), JsonRep.ValidateOnly());
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.SucceededValidation);
        }

        public void DoParameterValidateOnlyBad() {
            var parm = new JObject(new JProperty("value", "123"));
            var parms = new JObject(new JProperty("parm", parm), JsonRep.ValidateOnly());
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.ValidationFailed);
        }

        public void DoAttemptInvokePostActionWithPut() {
            TestActionInvoke("AnAction", JsonRep.Empty(), Methods.Put, Codes.MethodNotValid);
        }

        public void DoAttemptInvokePostActionWithGet() {
            TestActionInvoke("AnAction", JsonRep.Empty(), Methods.Get, Codes.MethodNotValid);
        }

        public void DoAttemptInvalidParameters() {
            //Test first with valid parm
            var parm = new JObject(new JProperty("value", "FOO"));
            var parms = new JObject(new JProperty("parm", parm));
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post);

            //Test with invalid parm -  fails Regex[A-Z]
            parm = new JObject(new JProperty("value", "123"));
            parms = new JObject(new JProperty("parm", parm));
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.ValidationFailed);

            //Test with invalid parm -  >101 chars
            parm = new JObject(new JProperty("value", "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            parms = new JObject(new JProperty("parm", parm));
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.ValidationFailed);
        }

        public void DoAttemptInvalidJson() {

            const string invalidJson1 = @"{""parm"":""value"": ""123"" }}"; // missing '{'
            const string invalidJson2 = @"{""parm"" {""value"": ""123"" }}"; // missing ':'
            const string invalidJson3 = @"{""parm"": {""value"":  }}"; // missing value

            TestActionInvoke("AnActionWithOptionalParm", invalidJson1, Methods.Post, Codes.SyntacticallyInvalid);
            TestActionInvoke("AnActionWithOptionalParm", invalidJson2, Methods.Post, Codes.SyntacticallyInvalid);
            TestActionInvoke("AnActionWithOptionalParm", invalidJson3, Methods.Post, Codes.SyntacticallyInvalid);
        }

        public void DoValidateOnlyParameters() {
            //Test first with valid parm
            var parm = new JObject(new JProperty("value", "FOO"));
            var parms = new JObject(new JProperty("parm", parm), JsonRep.ValidateOnly());
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.SucceededValidation);

            //Test with invalid parm -  fails Regex[A-Z]
            parm = new JObject(new JProperty("value", "123"));
            parms = new JObject(new JProperty("parm", parm), JsonRep.ValidateOnly());
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.ValidationFailed);

            //Test with invalid parm -  >101 chars
            parm = new JObject(new JProperty("value", "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            parms = new JObject(new JProperty("parm", parm), JsonRep.ValidateOnly());
            TestActionInvoke("AnActionWithOptionalParm", parms.ToString(), Methods.Post, Codes.ValidationFailed);
        }
    }
}