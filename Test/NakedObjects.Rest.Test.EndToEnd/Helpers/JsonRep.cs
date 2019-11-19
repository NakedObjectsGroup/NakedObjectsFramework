// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    /// <summary>
    /// 
    /// </summary>
    public static class JsonRep {
        //Factored out so that we can test this returning a Null instead

        public const string FormalDomainModeAsQueryString = @"?" + XRoDomainModel + "=" + Formal;
        public const string DomainModeQueryStringMalformed = @"?" + XRoDomainModel + "=" + "foo";
        public const string SimpleDomainModeAsQueryString = @"?" + XRoDomainModel + "=" + Simple;
        public const string Href = "href";
        public const string Members = "members";
        public const string DomainType = "domainType";
        public const string XRoDomainModel = "x-ro-domain-model";
        public const string XRoValidateOnly = "x-ro-validate-only";
        public const string Formal = "formal";
        public const string Simple = "simple";

        private static int nextId = 10000;

        public static int NextId {
            get { return ++nextId; }
        }

        public static int CurrentId {
            get { return nextId; }
        }

        public static string Empty() {
            //return (new JObject()).ToString();
            return null;
        }

        public static JObject MostSimple1AsRef() {
            return ObjectAsRef(@"MostSimple/1");
        }

        public static JObject MostSimple2AsRef() {
            return ObjectAsRef(@"MostSimple/2");
        }

        public static JObject MostSimple3AsRef() {
            return ObjectAsRef(@"MostSimple/3");
        }

        public static JObject WithValue1AsRef() {
            return ObjectAsRef(@"WithValue/1");
        }

        private static JObject ObjectAsRef(string oid) {
            return new JObject(new JProperty(Href, Urls.Objects + Urls.NameSpace + oid));
        }

        public static JProperty ValidateOnly() {
            return new JProperty(XRoValidateOnly, true);
        }

        public static JProperty FormalDomainModel() {
            return new JProperty(XRoDomainModel, Formal);
        }

        public static JProperty SimpleDomainModel() {
            return new JProperty(XRoDomainModel, Simple);
        }
    }
}