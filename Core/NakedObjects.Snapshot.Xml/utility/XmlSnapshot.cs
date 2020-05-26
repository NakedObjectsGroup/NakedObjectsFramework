// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Snapshot.Xml.Utility {
    [NotMapped]
    public class XmlSnapshot : IXmlSnapshot {
        private static readonly ILog Log = LogManager.GetLogger(typeof(XmlSnapshot));
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly Place rootPlace;

        private bool topLevelElementWritten;

        //  Start a snapshot at the root object, using own namespace manager.
        public XmlSnapshot(object obj, INakedObjectManager nakedObjectManager, IMetamodelManager metamodelManager) : this(obj, new XmlSchema(), nakedObjectManager, metamodelManager) { }

        // Start a snapshot at the root object, using supplied namespace manager.
        public XmlSnapshot(object obj, XmlSchema schema, INakedObjectManager nakedObjectManager, IMetamodelManager metamodelManager) {
            this.nakedObjectManager = nakedObjectManager;
            this.metamodelManager = metamodelManager;

            var rootObjectAdapter = nakedObjectManager.CreateAdapter(obj, null, null);

            Schema = schema;

            try {
                XmlDocument = new XDocument();
                XsdDocument = new XDocument();

                XsdElement = XsMetaModel.CreateXsSchemaElement(XsdDocument);

                rootPlace = AppendXml(rootObjectAdapter);
            }
            catch (ArgumentException e) {
                throw new NakedObjectSystemException(Log.LogAndReturn("Unable to build snapshot"), e);
            }
        }

        public XDocument XmlDocument { get; }
        //  The root element of GetXmlDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XmlElement { get; private set; }

        public XDocument XsdDocument { get; }
        //  The root element of GetXsdDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XsdElement { get; }
        public XmlSchema Schema { get; }

        #region IXmlSnapshot Members

        public string Xml {
            get {
                var element = XmlElement;
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb)) {
                    element.WriteTo(writer);
                }

                return sb.ToString();
            }
        }

        public string Xsd {
            get {
                var element = XsdElement;
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb)) {
                    element.WriteTo(writer);
                }

                return sb.ToString();
            }
        }

        //  The name of the <code>xsi:schemaLocation</code> in the XML document.
        //  
        //  Taken from the <code>fullyQualifiedClassName</code> (which also is used
        //  as the basis for the <code>targetNamespace</code>.
        //  
        //  Populated in AppendXml(nakedObjectAdapter).
        public string SchemaLocationFileName { get; private set; }

        public string TransformedXml(string transform) => TransformXml(transform);

        public string TransformedXsd(string transform) => TransformXsd(transform);

        public void Include(string path) {
            Include(path, null);
        }

        public void Include(string path, string annotation) {
            // tokenize into successive fields
            var fieldNames = path.Split('/').Select(tok => tok.ToLower()).ToList();
            IncludeField(rootPlace, fieldNames, annotation);
        }

        #endregion

        // Start a snapshot at the root object, using own namespace manager.

        // Creates an XElement representing this object, and appends it as the root
        // element of the Document.
        // 
        // The Document must not yet have a root element Additionally, the supplied
        // schemaManager must be populated with any application-level namespaces
        // referenced in the document that the parentElement resides within.
        // (Normally this is achieved simply by using AppendXml passing in a new
        // schemaManager - see ToXml() or XmlSnapshot).

        private Place AppendXml(INakedObjectAdapter nakedObjectAdapter) {
            var fullyQualifiedClassName = nakedObjectAdapter.Spec.FullName;

            Schema.SetUri(fullyQualifiedClassName); // derive URI from fully qualified name

            var place = ObjectToElement(nakedObjectAdapter);

            var element = place.XmlElement;
            var xsElementElement = element.Annotation<XElement>();

            XmlDocument.Add(element);

            XsdElement.Add(xsElementElement);

            Schema.SetTargetNamespace(XsdDocument, fullyQualifiedClassName);

            var schemaLocationFileName = fullyQualifiedClassName + ".xsd";
            Schema.AssignSchema(XmlDocument, fullyQualifiedClassName, schemaLocationFileName);

            XmlElement = element;
            SchemaLocationFileName = schemaLocationFileName;

            return place;
        }

        // Creates an XElement representing this object, and appends it to the
        // supplied parentElement, provided that an element for the object is not
        // already appended.
        // 
        // The method uses the OID to determine if an object's element is already
        // present. If the object is not yet persistent, then the hashCode is used
        // instead.
        // 
        // The parentElement must have an owner document, and should define the nof
        // namespace. Additionally, the supplied schemaManager must be populated
        // with any application-level namespaces referenced in the document that the
        // parentElement resides within. (Normally this is achieved simply by using
        // appendXml passing in a rootElement and a new schemaManager - see
        // ToXml() or XmlSnapshot).

        private XElement AppendXml(Place parentPlace, INakedObjectAdapter childObjectAdapter) {
            var parentElement = parentPlace.XmlElement;
            var parentXsElement = parentElement.Annotation<XElement>();

            if (parentElement.Document != XmlDocument) {
                throw new ArgumentException(Log.LogAndReturn("parent XML XElement must have snapshot's XML document as its owner"));
            }

            var childPlace = ObjectToElement(childObjectAdapter);
            var childElement = childPlace.XmlElement;
            var childXsElement = childElement.Annotation<XElement>();

            childElement = MergeTree(parentElement, childElement);

            Schema.AddXsElementIfNotPresent(parentXsElement, childXsElement);

            return childElement;
        }

        private bool AppendXmlThenIncludeRemaining(Place parentPlace, INakedObjectAdapter referencedObjectAdapter, IList<string> fieldNames,
                                                   string annotation) {
            var referencedElement = AppendXml(parentPlace, referencedObjectAdapter);
            var referencedPlace = new Place(referencedObjectAdapter, referencedElement);

            var includedField = IncludeField(referencedPlace, fieldNames, annotation);

            return includedField;
        }

        private static IEnumerable<XElement> ElementsUnder(XElement parentElement, string localName) {
            return parentElement.Descendants().Where(element => localName.Equals("*") || element.Name.LocalName.Equals(localName));
        }

        public INakedObjectAdapter GetObject() => rootPlace.NakedObjectAdapter;

        //  return true if able to navigate the complete vector of field names
        //                  successfully; false if a field could not be located or it turned
        //                  out to be a value.

        private bool IncludeField(Place place, IList<string> fieldNames, string annotation) {
            var nakedObjectAdapter = place.NakedObjectAdapter;
            var xmlElement = place.XmlElement;

            // we use a copy of the path so that we can safely traverse collections
            // without side-effects
            fieldNames = fieldNames.ToList();

            // see if we have any fields to process
            if (!fieldNames.Any()) {
                return true;
            }

            // take the first field name from the list, and remove
            var fieldName = fieldNames.First();
            fieldNames.Remove(fieldName);

            // locate the field in the object's class
            var nos = (IObjectSpec) nakedObjectAdapter.Spec;
            var field = nos.Properties.SingleOrDefault(p => p.Id.ToLower() == fieldName);

            if (field == null) {
                return false;
            }

            // locate the corresponding XML element
            // (the corresponding XSD element will later be attached to xmlElement
            // as its userData)
            var xmlFieldElements = ElementsUnder(xmlElement, field.Id).ToArray();
            var fieldCount = xmlFieldElements.Length;
            if (fieldCount != 1) {
                return false;
            }

            var xmlFieldElement = xmlFieldElements.First();

            if (!fieldNames.Any() && annotation != null) {
                // nothing left in the path, so we will apply the annotation now
                NofMetaModel.SetAnnotationAttribute(xmlFieldElement, annotation);
            }

            var fieldPlace = new Place(nakedObjectAdapter, xmlFieldElement);

            if (field.ReturnSpec.IsParseable) {
                return false;
            }

            var oneToOneAssociation = field as IOneToOneAssociationSpec;
            if (oneToOneAssociation != null) {
                var referencedObjectAdapter = oneToOneAssociation.GetNakedObject(fieldPlace.NakedObjectAdapter);

                if (referencedObjectAdapter == null) {
                    return true; // not a failure if the reference was null
                }

                var appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObjectAdapter, fieldNames, annotation);

                return appendedXml;
            }

            var oneToManyAssociation = field as IOneToManyAssociationSpec;
            if (oneToManyAssociation != null) {
                var collection = oneToManyAssociation.GetNakedObject(fieldPlace.NakedObjectAdapter);

                var collectionAsEnumerable = collection.GetAsEnumerable(nakedObjectManager).ToArray();

                var allFieldsNavigated = true;

                foreach (var referencedObject in collectionAsEnumerable) {
                    var appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObject, fieldNames, annotation);

                    allFieldsNavigated = allFieldsNavigated && appendedXml;
                }

                return allFieldsNavigated;
            }

            return false; // fall through, shouldn't get here but just in case.
        }

        private static string DoLog(string label, object obj) => (label ?? "?") + "='" + (obj == null ? "(null)" : obj.ToString()) + "'";

        //  Merges the tree of Elements whose root is <code>childElement</code>
        //  underneath the <code>parentElement</code>.
        //  
        //  If the <code>parentElement</code> already has an element that matches
        //  the <code>childElement</code>, then recursively attaches the
        //  grandchildren instead.
        //  
        //  The element returned will be either the supplied
        //  <code>childElement</code>, or an existing child element if one already
        //  existed under <code>parentElement</code>.

        private static XElement MergeTree(XElement parentElement, XElement childElement) {
            var childElementOid = NofMetaModel.GetAttribute(childElement, "oid");

            if (childElementOid != null) {
                // before we add the child element, check to see if it is already
                // there
                var existingChildElements = ElementsUnder(parentElement, childElement.Name.LocalName);
                foreach (var possibleMatchingElement in existingChildElements) {
                    var possibleMatchOid = NofMetaModel.GetAttribute(possibleMatchingElement, "oid");
                    if (possibleMatchOid == null || !possibleMatchOid.Equals(childElementOid)) {
                        continue;
                    }

                    // match: transfer the children of the child (grandchildren) to the
                    // already existing matching child
                    var existingChildElement = possibleMatchingElement;
                    var grandchildrenElements = ElementsUnder(childElement, "*");
                    foreach (var grandchildElement in grandchildrenElements) {
                        grandchildElement.Remove();

                        MergeTree(existingChildElement, grandchildElement);
                    }

                    return existingChildElement;
                }
            }

            parentElement.Add(childElement);
            return childElement;
        }

        public Place ObjectToElement(INakedObjectAdapter nakedObjectAdapter) {
            var nos = (IObjectSpec) nakedObjectAdapter.Spec;

            var element = Schema.CreateElement(XmlDocument, nos.ShortName, nos.FullName, nos.SingularName, nos.PluralName);
            NofMetaModel.AppendNofTitle(element, nakedObjectAdapter.TitleString());

            var xsElement = Schema.CreateXsElementForNofClass(XsdDocument, element, topLevelElementWritten);

            // hack: every element in the XSD schema apart from first needs minimum cardinality setting.
            topLevelElementWritten = true;

            var place = new Place(nakedObjectAdapter, element);

            NofMetaModel.SetAttributesForClass(element, OidOrHashCode(nakedObjectAdapter));

            var fields = nos.Properties;

            var seenFields = new List<string>();

            foreach (var field in fields) {
                var fieldName = field.Id;

                // Skip field if we have seen the name already
                // This is a workaround for getLastActivity(). This method exists
                // in AbstractNakedObject, but is not (at some level) being picked up
                // by the dot-net reflector as a property. On the other hand it does
                // exist as a field in the meta model (NakedObjectSpecification).
                //
                // Now, to re-expose the lastactivity field for .Net, a deriveLastActivity()
                // has been added to BusinessObject. This caused another field ofthe
                // same name, ultimately breaking the XSD.

                if (seenFields.Contains(fieldName)) {
                    continue;
                }

                seenFields.Add(fieldName);

                XNamespace ns = Schema.GetUri();

                var xmlFieldElement = new XElement(ns + fieldName);

                XElement xsdFieldElement;
                var oneToOneAssociation = field as IOneToOneAssociationSpec;
                var oneToManyAssociation = field as IOneToManyAssociationSpec;

                if (field.ReturnSpec.IsParseable && oneToOneAssociation != null) {
                    var fieldNos = field.ReturnSpec;
                    // skip fields of type XmlValue
                    if (fieldNos?.FullName != null && fieldNos.FullName.EndsWith("XmlValue")) {
                        continue;
                    }

                    var xmlValueElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        var value = oneToOneAssociation.GetNakedObject(nakedObjectAdapter);

                        // a null value would be a programming error, but we protect
                        // against it anyway
                        if (value == null) {
                            continue;
                        }

                        var valueNos = value.Spec;

                        // XML
                        NofMetaModel.SetAttributesForValue(xmlValueElement, valueNos.ShortName);

                        var notEmpty = value.TitleString().Length > 0;
                        if (notEmpty) {
                            var valueStr = value.TitleString();
                            xmlValueElement.Add(new XText(valueStr));
                        }
                        else {
                            NofMetaModel.SetIsEmptyAttribute(xmlValueElement, true);
                        }
                    }
                    catch (Exception) {
                        Log.Warn("objectToElement(NO): " + DoLog("field", fieldName) + ": getField() threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofValue(xsElement, xmlValueElement);
                }
                else if (oneToOneAssociation != null) {
                    var xmlReferenceElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        var referencedNakedObjectAdapter = oneToOneAssociation.GetNakedObject(nakedObjectAdapter);
                        var fullyQualifiedClassName = field.ReturnSpec.FullName;

                        // XML
                        NofMetaModel.SetAttributesForReference(xmlReferenceElement, Schema.Prefix, fullyQualifiedClassName);

                        if (referencedNakedObjectAdapter != null) {
                            NofMetaModel.AppendNofTitle(xmlReferenceElement, referencedNakedObjectAdapter.TitleString());
                        }
                        else {
                            NofMetaModel.SetIsEmptyAttribute(xmlReferenceElement, true);
                        }
                    }
                    catch (Exception) {
                        Log.Warn("objectToElement(NO): " + DoLog("field", fieldName) + ": getAssociation() threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofReference(xsElement, xmlReferenceElement, oneToOneAssociation.ReturnSpec.FullName);
                }
                else if (oneToManyAssociation != null) {
                    var xmlCollectionElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        var collection = oneToManyAssociation.GetNakedObject(nakedObjectAdapter);
                        var facet = collection.GetTypeOfFacetFromSpec();

                        var referencedTypeNos = facet.GetValueSpec(collection, metamodelManager.Metamodel);
                        var fullyQualifiedClassName = referencedTypeNos.FullName;

                        // XML
                        NofMetaModel.SetNofCollection(xmlCollectionElement, Schema.Prefix, fullyQualifiedClassName, collection, nakedObjectManager);
                    }
                    catch (Exception) {
                        Log.Warn("objectToElement(NO): " + DoLog("field", fieldName) + ": get(obj) threw exception - skipping XML generation");
                    }

                    // XSD
                    xsdFieldElement = Schema.CreateXsElementForNofCollection(xsElement, xmlCollectionElement, oneToManyAssociation.ReturnSpec.FullName);
                }
                else {
                    continue;
                }

                if (xsdFieldElement != null) {
                    xmlFieldElement.AddAnnotation(xsdFieldElement);
                }

                // XML
                MergeTree(element, xmlFieldElement);

                // XSD
                if (xsdFieldElement != null) {
                    Schema.AddFieldXsElement(xsElement, xsdFieldElement);
                }
            }

            return place;
        }

        private static string OidOrHashCode(INakedObjectAdapter nakedObjectAdapter) {
            var oid = nakedObjectAdapter.Oid;
            if (oid == null) {
                return "" + nakedObjectAdapter.GetHashCode();
            }

            return oid.ToString();
        }

        private static string Transform(XDocument xDoc, string transform) {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb)) {
                var compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(XmlReader.Create(new StringReader(transform)));
                compiledTransform.Transform(xDoc.CreateReader(), writer);
            }

            return sb.ToString();
        }

        public string TransformXml(string transform) => Transform(XmlDocument, transform);

        public string TransformXsd(string transform) => Transform(XsdDocument, transform);
    }
}