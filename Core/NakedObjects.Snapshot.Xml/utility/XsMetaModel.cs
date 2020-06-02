// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Common.Logging;
using NakedObjects.Core.Util;

namespace NakedObjects.Snapshot.Xml.Utility {
    public static class XsMetaModel {
        #region xmlns

        // Namespace prefix for W3OrgXmlnsUri.
        // The NamespaceManager  will not allow any namespace to use this prefix.

        public const string W3OrgXmlnsPrefix = "xmlns";

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(XsMetaModel));

        // Creates an &lt;xs:schema&gt; element for the document
        // to the provided element, attaching to root of supplied Xsd doc.
        // 
        // In addition:
        //  - the elementFormDefault is set
        //  - the NOF namespace is set
        //  - the <code>xs:import</code> element referencing the NOF namespace is added  as a child

        public static XElement CreateXsSchemaElement(XDocument xsdDoc) {
            if (xsdDoc.Root != null) {
                throw new ArgumentException(Log.LogAndReturn("XSD document already has content"));
            }

            var xsSchemaElement = CreateXsElement(xsdDoc, "schema");

            xsSchemaElement.SetAttributeValue("elementFormDefault", "qualified");

            NofMetaModel.AddNamespace(xsSchemaElement);

            xsdDoc.Add(xsSchemaElement);
            var xsImportElement = CreateXsElement(xsdDoc, "import");
            xsImportElement.SetAttributeValue("namespace", NofMetaModel.Nof.NamespaceName);
            xsImportElement.SetAttributeValue("schemaLocation", NofMetaModel.DefaultNofSchemaLocation);

            xsSchemaElement.Add(xsImportElement);

            return xsSchemaElement;
        }

        public static XElement CreateXsElementElement(XDocument xsdDoc, string className) => CreateXsElementElement(xsdDoc, className, true);

        public static XElement CreateXsElementElement(XDocument xsdDoc, string className, bool includeCardinality) {
            var xsElementElement = CreateXsElement(xsdDoc, "element");
            xsElementElement.SetAttributeValue("name", className);
            if (includeCardinality) {
                SetXsCardinality(xsElementElement, 0, int.MaxValue);
            }

            return xsElementElement;
        }

        // Creates an element in the XS namespace, adding the definition of the namespace to the
        // root element of the document if required,

        public static XElement CreateXsElement(XDocument xsdDoc, string localName) {
            var element = new XElement(Xs + localName);
            Helper.RootElementFor(element).SetAttributeValue(XNamespace.Xmlns + W3OrgXsPrefix, Xs.NamespaceName);

            return element;
        }

//	private XElement addAnyToSequence( XElement xsSequenceElement) {
//		XElement xsAnyElement = createXsElement(docFor(xsSequenceElement), "any");
//		xsAnyElement.setAttribute("namespace", "##other");
//		xsAnyElement.setAttribute("minOccurs", "0");
//		xsAnyElement.setAttribute("maxOccurs", "unbounded");
//		xsAnyElement.setAttribute("processContents", "lax");
//		xsSequenceElement.appendChild(xsAnyElement);
//		return xsSequenceElement;
//	}

        // Creates an xs:attribute ref="nof:xxx" element, and appends to specified owning element.

        public static XElement AddXsNofAttribute(XElement parentXsElement, string nofAttributeRef) => AddXsNofAttribute(parentXsElement, nofAttributeRef, null);

        // Adds <code>xs:attribute ref="nof:xxx" fixed="yyy"</code> element, and appends to
        // specified parent XSD element.

        public static XElement AddXsNofAttribute(XElement parentXsElement, string nofAttributeRef, string fixedValue) => AddXsNofAttribute(parentXsElement, nofAttributeRef, fixedValue, true);

        // Adds <code>xs:attribute ref="nof:xxx" default="yyy"</code> element, and appends to
        // specified parent XSD element.
        // 
        // The last parameter determines whether to use <code>fixed="yyy"</code> rather than
        // <code>default="yyy"</code>.

        public static XElement AddXsNofAttribute(XElement parentXsElement, string nofAttributeRef, string value, bool useFixed) {
            var xsNofAttributeElement = CreateXsElement(Helper.DocFor(parentXsElement), "attribute");
            xsNofAttributeElement.SetAttributeValue("ref", NofMetaModel.NofMetamodelNsPrefix + ":" + nofAttributeRef);
            parentXsElement.Add(xsNofAttributeElement);
            if (value != null) {
                xsNofAttributeElement.SetAttributeValue(useFixed ? "fixed" : "default", value);
            }

            return parentXsElement;
        }

        // Adds <code>xs:attribute ref="nof:feature" fixed="(feature)"</code> element as child to
        // supplied XSD element, presumed to be an <xs:complexType</code>.

        public static XElement AddXsNofFeatureAttributeElements(XElement parentXsElement, string feature) {
            var xsNofFeatureAttributeElement = CreateXsElement(Helper.DocFor(parentXsElement), "attribute");
            xsNofFeatureAttributeElement.SetAttributeValue("ref", "nof:feature");
            xsNofFeatureAttributeElement.SetAttributeValue("fixed", feature);
            parentXsElement.Add(xsNofFeatureAttributeElement);
            return xsNofFeatureAttributeElement;
        }

        // returns child <code>xs:complexType</code> element allowing mixed content 
        // for supplied parent XSD element, creating and appending if necessary.
        // 
        // The supplied element is presumed to be one for which <code>xs:complexType</code>
        // is valid as a child (eg <code>xs:element</code>).

        public static XElement ComplexTypeFor(XElement parentXsElement) => ComplexTypeFor(parentXsElement, true);

        // returns child <code>xs:complexType</code> element, optionally allowing mixed
        // content, for supplied parent XSD element, creating and appending if necessary.
        // 
        // The supplied element is presumed to be one for which <code>xs:complexType</code>
        // is valid as a child (eg <code>xs:element</code>).

        public static XElement ComplexTypeFor(XElement parentXsElement, bool mixed) {
            var el = ChildXsElement(parentXsElement, "complexType");
            if (mixed) {
                el.SetAttributeValue("mixed", "true");
            }

            return el;
        }

        // returns child <code>xs:sequence</code> element for supplied parent XSD
        // element, creating and appending if necessary.
        // 
        // The supplied element is presumed to be one for which <code>xs:simpleContent</code>
        // is valid as a child (eg <code>xs:complexType</code>).

        public static XElement SequenceFor(XElement parentXsElement) => ChildXsElement(parentXsElement, "sequence");

        public static XElement SequenceForComplexTypeFor(XElement parentXsElement) => SequenceFor(ComplexTypeFor(parentXsElement));

        // Returns the <code>xs:choice</code> or <code>xs:sequence</code> element under
        // the supplied XSD element, or null if neither can be found.
        // ReSharper disable PossibleMultipleEnumeration

        public static XElement ChoiceOrSequenceFor(XElement parentXsElement) {
            var choiceNodeList = parentXsElement.Descendants(Xs + "choice");
            if (choiceNodeList.Any()) {
                return choiceNodeList.First();
            }

            return parentXsElement.Descendants(Xs + "sequence").FirstOrDefault();
        }


        public static XElement ChildXsElement(XElement parentXsElement, string localName) {
            var nodeList = parentXsElement.Descendants(Xs + localName);
            if (nodeList.Any()) {
                return nodeList.First();
            }

            var childXsElement = CreateXsElement(Helper.DocFor(parentXsElement), localName);
            parentXsElement.Add(childXsElement);

            return childXsElement;
        }

        // ReSharper restore PossibleMultipleEnumeration

        // Sets the <code>minOccurs</code> and <code>maxOccurs</code> attributes for
        // provided <code>element</code> (presumed to be an XSD element for which these
        // attributes makes sense.

        public static XElement SetXsCardinality(XElement xsElement, int minOccurs, int maxOccurs) {
            if (maxOccurs >= 0) {
                xsElement.SetAttributeValue("minOccurs", "" + minOccurs);
            }

            if (maxOccurs >= 0) {
                if (maxOccurs == int.MaxValue) {
                    xsElement.SetAttributeValue("maxOccurs", "unbounded");
                }
                else {
                    xsElement.SetAttributeValue("maxOccurs", "" + maxOccurs);
                }
            }

            return xsElement;
        }

        #region xs

        // Namespace prefix for XML schema.

        private const string W3OrgXsUri = "http://www.w3.org/2001/XMLSchema";

        // Namespace prefix for W3OrgXsUri.
        //  
        // The NamespaceManager will not allow any namespace to use this prefix.

        public const string W3OrgXsPrefix = "xs";

        public static readonly XNamespace Xs = W3OrgXsUri;

        #endregion

        #region xsi

        // Namespace prefix for XML schema instance.

        private const string W3OrgXsiUri = "http://www.w3.org/2001/XMLSchema-instance";

        // Namespace prefix for W3OrgXsiUri.
        //  
        // The NamespaceManager will not allow any namespace to use this prefix.

        public const string W3OrgXsiPrefix = "xsi";

        public static readonly XNamespace Xsi = W3OrgXsiUri;

        #endregion
    }
}