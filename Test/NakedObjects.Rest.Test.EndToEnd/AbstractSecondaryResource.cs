// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    //Abstraction of test classes for Home, User, Version, Service
    [TestClass]
    public abstract class AbstractSecondaryResource {
        protected abstract string Filename();

        protected abstract string ResourceUrl();

        protected abstract string ProfileType();


        public abstract void GetResource();

        protected void DoGetResource() {
            Helpers.TestResponse(ResourceUrl(), Filename());
        }

        public abstract void WithGenericAcceptHeader();

        protected void DoWithGenericAcceptHeader() {
            Helpers.TestResponse(ResourceUrl(), Filename(), null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }

        public abstract void WithProfileAcceptHeader();

        protected void DoWithProfileAcceptHeader() {
            Helpers.TestResponse(ResourceUrl(), Filename(), null, Methods.Get, Codes.Succeeded, ProfileType());
        }

        public abstract void AttemptWithInvalidProfileAcceptHeader();

        protected void DoAttemptWithInvalidProfileAcceptHeader() {
            Helpers.TestResponse(ResourceUrl(), null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
        }

        public abstract void AttemptPost();

        protected void DoAttemptPost() {
            Helpers.TestResponse(ResourceUrl(), null, JsonRep.Empty(), Methods.Post, Codes.MethodNotValid);
        }

        public abstract void AttemptPut();

        protected void DoAttemptPut() {
            Helpers.TestResponse(ResourceUrl(), null, JsonRep.Empty(), Methods.Put, Codes.MethodNotValid);
        }

        public abstract void AttemptDelete();

        protected void DoAttemptDelete() {
            Helpers.TestResponse(ResourceUrl(), null, null, Methods.Delete, Codes.MethodNotValid);
        }

        public abstract void WithFormalDomainModel();

        protected void DoWithFormalDomainModel() {
            string url = ResourceUrl() + JsonRep.FormalDomainModeAsQueryString;
            Helpers.TestResponse(url, Filename() + "-WithFormalDomainModel");
        }

        public abstract void WithSimpleDomainModel();

        protected void DoWithSimpleDomainModel() {
            string url = ResourceUrl() + JsonRep.SimpleDomainModeAsQueryString;
            Helpers.TestResponse(url, Filename() + "-WithSimpleDomainModel");
        }

        public abstract void AttemptWithDomainModelMalformed();

        protected void DoAttemptWithDomainModelMalformed() {
            string url = ResourceUrl() + JsonRep.DomainModeQueryStringMalformed;
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }
    }
}