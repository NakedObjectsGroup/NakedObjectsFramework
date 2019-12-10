// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ObjectPut : ObjectAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Object-Put-"; }
        }

        #endregion

        [TestMethod]
        public void MostSimple() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))));
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put);
        }

        [TestMethod]
        public void SyntaticallyMalformedArgument1() {
            var body = new JObject(new JProperty("Iid", new JObject(new JProperty("value", 1)))); // i.e. mis-named property
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void SyntaticallyMalformedArgument2() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("vaalue", 1)))); // mis-named 'value'
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void SyntaticallyMalformedArgument3() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))), new JProperty("Prop2", new JObject(new JProperty("value", 1)))); // additional property
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }


        [TestMethod]
        public void SyntaticallyMalformedArgument4() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1), new JProperty("value2", 1)))); // i.e. additional sub-property
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }


        [TestMethod]
        public void SemanticallyMalformedArgument() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", "foo")))); // i.e. string instead of an int
            Object(Urls.MostSimple1, "MostSimple", body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPutStringIntoInteger() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", "foo"))));
            Object(Urls.MostSimple1, null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPutWithIncorrectPropertyName() {
            var body = new JObject(new JProperty("Identity", new JObject(new JProperty("value", 1))));
            Object(Urls.MostSimple1, null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPutWithSpuriousAdditionalProperty() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))), new JProperty("Number", new JObject(new JProperty("value", 2))));
            Object(Urls.MostSimple1, null, body.ToString(), Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPutImmutableObject() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))));
            Object(Urls.Immutable1, null, body.ToString(), Methods.Put, Codes.MethodNotValid);
        }


        [TestMethod]
        public void AttemptPutNonExistentInstance() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))));
            Object(Urls.NameSpace + @"MostSimple/999", null, body.ToString(), Methods.Put, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptPutNonExistentDomainType() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))));
            Object(Urls.NameSpace + @"MostSilly/1", null, body.ToString(), Methods.Put, Codes.NotFound);
        }

        [TestMethod]
        // no longer fails no sure if an issue - seems no reason to make fail ? 
        public void AttemptPutInstanceOfAbstractType() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 1))));
            Object(Urls.NameSpace + @"WithAction/1", null, body.ToString(), Methods.Put, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptPutWithQueryString() {
            Object(Urls.MostSimple1 + "?Id=1", null, null, Methods.Put, Codes.SyntacticallyInvalid);
        }

        [TestMethod, Ignore]
        public void VerySimple() {
            var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1)))));
            var name = new JProperty("Name", new JObject(new JProperty("value", null)));
            var body = new JObject(ms1, name);
            Object(Urls.VerySimple2, "VerySimple", body.ToString(), Methods.Put);
        }

        [TestMethod]
        public void AttemptWithWrongType() {
            var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.Immutable1)))));
            var name = new JProperty("Name", new JObject(new JProperty("value", null)));
            var body = new JObject(ms1, name);
            Object(Urls.VerySimple2, "VerySimpleFail", body.ToString(), Methods.Put, Codes.ValidationFailed);
        }

        [TestMethod]
        public void ValidateOnlyGood() {
            var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1)))));
            var name = new JProperty("Name", new JObject(new JProperty("value", null)));
            var body = new JObject(ms1, name, JsonRep.ValidateOnly());
            Object(Urls.VerySimple1, null, body.ToString(), Methods.Put, Codes.SucceededValidation);
        }


        [TestMethod]
        public void AttemptValidateOnlyWrongType() {
            var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.Immutable1)))));
            var name = new JProperty("Name", new JObject(new JProperty("value", null)));
            var body = new JObject(ms1, name, JsonRep.ValidateOnly());
            Object(Urls.VerySimple2, "VerySimpleFail", body.ToString(), Methods.Put, Codes.ValidationFailed);
        }
    }
}