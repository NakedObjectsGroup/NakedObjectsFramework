// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Xml;
using System.Xml.Linq;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Security.Wif {
    public class NakedObjectsClaimsAuthorizationManager : ClaimsAuthorizationManager {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(NakedObjectsClaimsAuthorizationManager));

        private readonly IDictionary<IIdentifier, IDictionary<CheckType, AccessRequest>> rules = new Dictionary<IIdentifier, IDictionary<CheckType, AccessRequest>>();

        public NakedObjectsClaimsAuthorizationManager(object config) {
            IEnumerable<XElement> nodes = ToXElementList(config as XmlNodeList);
            var requests = from node in nodes
                           let cls = node.Attribute("fullname").Value
                           from element in node.Descendants("member")
                           select new AccessRequest(cls, element);


            foreach (AccessRequest r in requests) {
                if (rules.ContainsKey(r.Identifier)) {
                    IDictionary<CheckType, AccessRequest> subRules = rules[r.Identifier];

                    if (subRules.ContainsKey(r.CheckType)) {
                        LOG.ErrorFormat("Error in ClaimsAuthorizationManager config. Duplicate key [{0}][{1}]", r.Identifier, r.CheckType);
                    }
                    else {
                        subRules.Add(r.CheckType, r);
                    }
                }
                else {
                    rules.Add(r.Identifier, new Dictionary<CheckType, AccessRequest> {{r.CheckType, r}});
                }
            }
        }

        private static XElement GetXElement(XmlNode node) {
            var xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter()) {
                node.WriteTo(xmlWriter);
            }
            return xDoc.Root;
        }

        private static IEnumerable<XElement> ToXElementList(XmlNodeList xmlNodeList) {
            return xmlNodeList.Cast<XmlNode>().Select(GetXElement);
        }


        public override bool CheckAccess(AuthorizationContext context) {
            IIdentifier identifier = IdentifierImpl.FromIdentityString(context.Resource.Single().Value);
            var checkType = (CheckType) int.Parse(context.Action.Single().Value);

            if (rules.ContainsKey(identifier)) {
                if (rules[identifier].ContainsKey(checkType)) {
                    return rules[identifier][checkType].AccessAllowed(context);
                }
            }
            // default to not allowed 
            return false;
        }

        #region Nested type: AccessRequest

        private class AccessRequest {
            private readonly IList<Claim> requiredClaims;
            private readonly string checkType;

            public AccessRequest(string cls, XElement xElement) {
                var nameAttr = xElement.Attribute("name");
                string name = nameAttr == null ? "" : nameAttr.Value;
                Identifier = new IdentifierImpl(cls, name);
                checkType = xElement.Attribute("type").Value;
                requiredClaims = xElement.Descendants("claim").Select(c => new Claim(c.Attribute("type").Value, c.Attribute("value").Value)).ToList();
            }


            public CheckType CheckType {
                get {
                    if (checkType == Enum.GetName(typeof (CheckType), CheckType.ViewField)) {
                        return CheckType.ViewField;
                    }
                    if (checkType == Enum.GetName(typeof (CheckType), CheckType.EditField)) {
                        return CheckType.EditField;
                    }
                    if (checkType == Enum.GetName(typeof (CheckType), CheckType.Action)) {
                        return CheckType.Action;
                    }
                    throw new UnexpectedCallException(string.Format("Unexpected value of CheckType enum {0}", checkType));
                }
            }

            public IIdentifier Identifier { get; private set; }

            private static bool AreSame(Claim claim1, Claim claim2) {
                return claim1.Type == claim2.Type && claim1.Value == claim2.Value;
            }

            public bool AccessAllowed(AuthorizationContext context) {
                var claims = context.Principal.Identities.First().Claims;
                return requiredClaims.All(rc => claims.Any(c => AreSame(rc, c)));
            }
        }

        #endregion
    }
}