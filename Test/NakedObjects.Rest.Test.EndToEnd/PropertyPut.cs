// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class PropertyPut : PropertyAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Property-Put-"; }
        }

        #endregion

        [TestMethod, Ignore]
        public void Int() {
            var body = new JObject(new JProperty("value", 100));
            WithValue("AValue", "AValue", body.ToString(), Methods.Put, Codes.Succeeded);
        }

        [TestMethod]
        public void AttemptSyntacticallyMalformedArgument() {
            var body = new JObject(new JProperty("value", 100), new JProperty("value2", 100)); //Additional property
            WithValue("AValue", null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptSyntacticallyMalformedArgument2() {
            var body = new JObject(new JProperty("vaalue", 100)); //'value' mis-spelled
            WithValue("AValue", null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod, Ignore]
        public void AttemptSemanticallyMalformedArgument() {
            var body = new JObject(new JProperty("value", "foo")); //string instead of integer
            WithValue("AValue", null, body.ToString(), Methods.Put, Codes.ValidationFailed);
        }

        [TestMethod, Ignore]
        public void String() {
            var body = new JObject(new JProperty("value", ""));
            WithValue("AStringValue", "AStringValue", body.ToString(), Methods.Put, Codes.Succeeded);
        }


        [TestMethod]
        public void AttemptBadlyFormedStringArgument() {
            var body = new JObject(new JProperty("rubbish", "foo"));
            WithValue("AStringValue", null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod, Ignore]
        public void AttemptToUpdateIntWithString() {
            var body = new JObject(new JProperty("value", "foo"));
            WithValue("AValue", null, body.ToString(), Methods.Put, Codes.ValidationFailed);
        }

        [TestMethod, Ignore]
        //[IgnoreAttribute]
        public void DateTime() {
            var body = new JObject(new JProperty("value", "2007-07-13"));
            WithValue("ADateTimeValue", "DateTime", body.ToString(), Methods.Put, Codes.Succeeded);
        }

        [TestMethod, Ignore]
        public void AReference() {
            string simple = Urls.Objects + Urls.MostSimple1;
            var body = new JObject(new JProperty("value", new JObject(new JProperty("href", simple))));
            WithReference("AReference", "AReference", body.ToString(), Methods.Put, Codes.Succeeded);
        }

        [TestMethod, Ignore]
        public void AttemptReferenceArgumentOfWrongType() {
            string scalar = Urls.Objects + Urls.WithScalars1;
            var body = new JObject(new JProperty("value", new JObject(new JProperty("href", scalar))));
            WithReference("AReference", "AttemptBadlyFormedReferenceArgument", body.ToString(), Methods.Put, Codes.ValidationFailed);
        }

        [TestMethod]
        public void AttemptBadlyFormedReferenceArgument() {
            string simple = Urls.Objects + Urls.MostSimple1;
            var body = new JObject(new JProperty("value", new JObject(new JProperty("foo", simple))));
            WithReference("AReference", "AttemptBadlyFormedReferenceArgument", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod, Ignore]
        public void StringValidateOnlyGood() {
            var body = new JObject(new JProperty("value", "foo"), JsonRep.ValidateOnly());
            WithValue("AStringValue", null, body.ToString(), Methods.Put, Codes.SucceededValidation);
        }

        [TestMethod, Ignore]
        public void StringValidateOnlyFailTooLong() {
            var body = new JObject(new JProperty("value", "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"), JsonRep.ValidateOnly());
            WithValue("AStringValue", null, body.ToString(), Methods.Put, Codes.ValidationFailed);
        }

        [TestMethod, Ignore]
        public void StringValidateOnlyFailRegex() {
            var body = new JObject(new JProperty("value", "123"), JsonRep.ValidateOnly());
            WithValue("AStringValue", null, body.ToString(), Methods.Put, Codes.ValidationFailed);
        }
    }
}