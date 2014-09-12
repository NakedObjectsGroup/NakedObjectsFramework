// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Web.Mvc;
using System.Web.Security;
//using Microsoft.IdentityModel.Protocols.WSFederation;
//using Microsoft.IdentityModel.Web;
using NakedObjects.Web.Mvc.Controllers;

namespace NakedObjects.Mvc.App.Controllers {
    /// <summary>
    /// Account controller for Windows Identity Foundation. Based on code in 
    /// "Programming Windows Identity Foundation" Bertocci, Microsoft Press (c) 2011.  
    /// Uncomment method bodies and references to use (will only compile if WIF runtime installed) 
    /// </summary>
    [HandleError]
    public class WifAccountController : NakedObjectsController {

        
        public WifAccountController(INakedObjectsFramework framework) : base(framework) {
            
        }

        public ActionResult LogOn(string returnUrl) {
            return LogOnCommon(returnUrl);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Logon() {
            return LogOnCommon(null);
        }

        public ActionResult LogOff() {
            // uncomment before use 
            /*WSFederationAuthenticationModule fam = FederatedAuthentication.WSFederationAuthenticationModule;

            try {
                FormsAuthentication.SignOut();
            }
            finally {
                fam.SignOut(true);
            }*/
            // end uncomment 

            // original code 
            // var signOutRequest = new SignOutRequestMessage(new Uri(fam.Issuer), fam.Realm);
            // return Redirect(signOutRequest.WriteQueryString());
            return RedirectToAction("Index", "System");
        }

        private ActionResult LogOnCommon(string returnUrl) {
            // uncomment before use 
            /*if (!Request.IsAuthenticated) {
                string federatedSignInRedirectUrl = GetFederatedSignInRedirectUrl(returnUrl);
                return Redirect(federatedSignInRedirectUrl);
            }

            string effectiveReturnUrl = returnUrl;

            if (string.IsNullOrEmpty(effectiveReturnUrl)) {
                effectiveReturnUrl = GetContextFromRequest();
            }

            if (!string.IsNullOrEmpty(effectiveReturnUrl)) {
                return Redirect(effectiveReturnUrl);
            }*/
            // end uncomment 

            return RedirectToAction("Index", "System");
        }

        private static string GetFederatedSignInRedirectUrl(string returnUrl) {
            // uncomment before use 
            /*WSFederationAuthenticationModule fam = FederatedAuthentication.WSFederationAuthenticationModule;

            var signInRequest = new SignInRequestMessage(new Uri(fam.Issuer), fam.Realm, fam.Reply) {
                                                                                                        AuthenticationType = fam.AuthenticationType,
                                                                                                        Context = returnUrl,
                                                                                                        Freshness = fam.Freshness,
                                                                                                        HomeRealm = fam.HomeRealm
                                                                                                    };

            return signInRequest.WriteQueryString();*/
            // end uncomment 

            // remove before use
            return string.Empty;
        }

        private string GetContextFromRequest() {
            // uncomment before use 
            /*Uri requestBaseUrl = WSFederationMessage.GetBaseUrl(Request.Url);

            WSFederationMessage message = WSFederationMessage.CreateFromNameValueCollection(requestBaseUrl, Request.Form);

            return message != null ? message.Context : string.Empty;*/
            // end uncomment 

            // remove before use
            return string.Empty;
        }
    }
}