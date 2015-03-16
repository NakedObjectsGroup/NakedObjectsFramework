// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Snapshot.Xml.Utility {
    public class XmlSnapshot : IXmlSnapshot {
        private static readonly ILog Log = LogManager.GetLogger(typeof (XmlSnapshot));
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly Place rootPlace;
        private bool topLevelElementWritten;
        //  Start a snapshot at the root object, using own namespace manager.
        public XmlSnapshot(object obj, INakedObjectManager nakedObjectManager, IMetamodelManager metamodelManager) : this(obj, new XmlSchema(), nakedObjectManager, metamodelManager) {}
        // Start a snapshot at the root object, using supplied namespace manager.
        public XmlSnapshot(object obj, XmlSchema schema, INakedObjectManager nakedObjectManager, IMetamodelManager metamodelManager) {
            this.nakedObjectManager = nakedObjectManager;
            this.metamodelManager = metamodelManager;

            INakedObjectAdapter rootObjectAdapter = nakedObjectManager.CreateAdapter(obj, null, null);
            Log.Debug(".ctor(" + DoLog("rootObj", rootObjectAdapter) + AndLog("schema", schema) + AndLog("addOids", "" + true) + ")");

            Schema = schema;

            try {
                XmlDocument = new XDocument();
                XsdDocument = new XDocument();

                XsdElement = XsMetaModel.CreateXsSchemaElement(XsdDocument);

                rootPlace = AppendXml(rootObjectAdapter);
            }
            catch (ArgumentException e) {
                Log.Error("unable to build snapshot", e);
                throw new NakedObjectSystemException(e);
            }
        }

        public XDocument XmlDocument { get; private set; }
        //  The root element of GetXmlDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XmlElement { get; private set; }
        public XDocument XsdDocument { get; private set; }
        //  The root element of GetXsdDocument(). Returns <code>null</code>
        //  until the snapshot has actually been built.

        public XElement XsdElement { get; private set; }
        public XmlSchema Schema { get; private set; }

        #region IXmlSnapshot Members

        public string Xml {
            get {
                XElement element = XmlElement;
                var sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb)) {
                    element.WriteTo(writer);
                }
                return sb.ToString();
            }
        }

        public string Xsd {
            get {
                XElement element = XsdElement;
                var sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb)) {
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

        public string TransformedXml(string transform) {
            return TransformXml(transform);
        }

        public string TransformedXsd(string transform) {
            return TransformXsd(transform);
        }

        public void Include(string path) {
            Include(path, null);
        }

        public void Include(string path, string annotation) {
            // tokenize into successive fields
            List<string> fieldNames = path.Split('/').Select(tok => tok.ToLower()).ToList();

            fieldNames.ForEach(s => Log.Debug("include(..): " + DoLog("token", s)));

            Log.Debug("include(..): " + DoLog("fieldNames", fieldNames));

            // navigate first field, from the root.
            Log.Debug("include(..): invoking includeField");
            IncludeField(rootPlace, fieldNames, annotation);
        }

        #endregion

        // Start a snapshot at the root object, using own namespace manager.

        private static string AndLog(string label, INakedObjectAdapter nakedObjectAdapter) {
            return ", " + DoLog(label, nakedObjectAdapter);
        }

        private static string AndLog(string label, Object nakedObject) {
            return ", " + DoLog(label, nakedObject);
        }

        // Creates an XElement representing this object, and appends it as the root
        // element of the Document.
        // 
        // The Document must not yet have a root element Additionally, the supplied
        // schemaManager must be populated with any application-level namespaces
        // referenced in the document that the parentElement resides within.
        // (Normally this is achieved simply by using AppendXml passing in a new
        // schemaManager - see ToXml() or XmlSnapshot).

        private Place AppendXml(INakedObjectAdapter nakedObjectAdapter) {
            Log.Debug("appendXml(" + DoLog("obj", nakedObjectAdapter) + "')");

            string fullyQualifiedClassName = nakedObjectAdapter.Spec.FullName;

            Schema.SetUri(fullyQualifiedClassName); // derive URI from fully qualified name

            Place place = ObjectToElement(nakedObjectAdapter);

            XElement element = place.XmlElement;
            var xsElementElement = element.Annotation<XElement>();

            Log.Debug("appendXml(NO): add as element to XML doc");
            XmlDocument.Add(element);

            Log.Debug("appendXml(NO): add as xs:element to xs:schema of the XSD document");
            XsdElement.Add(xsElementElement);

            Log.Debug("appendXml(NO): set target name in XSD, derived from FQCN of obj");
            Schema.SetTargetNamespace(XsdDocument, fullyQualifiedClassName);

            Log.Debug("appendXml(NO): set schema location file name to XSD, derived from FQCN of obj");
            string schemaLocationFileName = fullyQualifiedClassName + ".xsd";
            Schema.AssignSchema(XmlDocument, fullyQualifiedClassName, schemaLocationFileName);

            Log.Debug("appendXml(NO): copy into snapshot obj");
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
            Log.Debug("appendXml(" + DoLog("parentPlace", parentPlace) + AndLog("childObj", childObjectAdapter) + ")");

            XElement parentElement = parentPlace.XmlElement;
            var parentXsElement = parentElement.Annotation<XElement>();

            if (parentElement.Document != XmlDocument) {
                throw new ArgumentException("parent XML XElement must have snapshot's XML document as its owner");
            }

            Log.Debug("appendXml(Pl, NO): invoking objectToElement() for " + DoLog("childObj", childObjectAdapter));
            Place childPlace = ObjectToElement(childObjectAdapter);
            XElement childElement = childPlace.XmlElement;
            var childXsElement = childElement.Annotation<XElement>();

            Log.Debug("appendXml(Pl, NO): invoking mergeTree of parent with child");
            childElement = MergeTree(parentElement, childElement);

            Log.Debug("appendXml(Pl, NO): adding XS XElement to schema if required");
            Schema.AddXsElementIfNotPresent(parentXsElement, childXsElement);

            return childElement;
        }

        private bool AppendXmlThenIncludeRemaining(Place parentPlace, INakedObjectAdapter referencedObjectAdapter, IList<string> fieldNames,
                                                   string annotation) {
            Log.Debug("appendXmlThenIncludeRemaining(: " + DoLog("parentPlace", parentPlace)
                      + AndLog("referencedObj", referencedObjectAdapter) + AndLog("fieldNames", fieldNames) + AndLog("annotation", annotation)
                      + ")");

            Log.Debug("appendXmlThenIncludeRemaining(..): invoking appendXml(parentPlace, referencedObjectAdapter)");

            XElement referencedElement = AppendXml(parentPlace, referencedObjectAdapter);
            var referencedPlace = new Place(referencedObjectAdapter, referencedElement);

            bool includedField = IncludeField(referencedPlace, fieldNames, annotation);

            Log.Debug("appendXmlThenIncludeRemaining(..): invoked includeField(referencedPlace, fieldNames)"
                      + AndLog("returned", "" + includedField));

            return includedField;
        }

        private static IEnumerable<XElement> ElementsUnder(XElement parentElement, string localName) {
            return parentElement.Descendants().Where(element => localName.Equals("*") || element.Name.LocalName.Equals(localName));
        }

        public INakedObjectAdapter GetObject() {
            return rootPlace.NakedObjectAdapter;
        }

        //  return true if able to navigate the complete vector of field names
        //                  successfully; false if a field could not be located or it turned
        //                  out to be a value.

        private bool IncludeField(Place place, IList<string> fieldNames, string annotation) {
            Log.DebugFormat("includeField(: {0})", DoLog("place", place) + AndLog("fieldNames", fieldNames) + AndLog("annotation", annotation));

            INakedObjectAdapter nakedObjectAdapter = place.NakedObjectAdapter;
            XElement xmlElement = place.XmlElement;

            // we use a copy of the path so that we can safely traverse collections
            // without side-effects
            fieldNames = fieldNames.ToList();

            // see if we have any fields to process
            if (!fieldNames.Any()) {
                return true;
            }

            // take the first field name from the list, and remove
            string fieldName = fieldNames.First();
            fieldNames.Remove(fieldName);

            Log.Debug("includeField(Pl, Vec, Str):" + DoLog("processing field", fieldName) + AndLog("left", "" + fieldNames.Count()));

            // locate the field in the object's class
            var nos = (IObjectSpec) nakedObjectAdapter.Spec;
            IAssociationSpec field = nos.Properties.SingleOrDefault(p => p.Id.ToLower() == fieldName);

            if (field == null) {
                Log.Info("includeField(Pl, Vec, Str): could not locate field, skipping");
                return false;
            }

            // locate the corresponding XML element
            // (the corresponding XSD element will later be attached to xmlElement
            // as its userData)
            Log.Debug("includeField(Pl, Vec, Str): locating corresponding XML element");
            XElement[] xmlFieldElements = ElementsUnder(xmlElement, field.Id).ToArray();
            int fieldCount = xmlFieldElements.Count();
            if (fieldCount != 1) {
                Log.Info("includeField(Pl, Vec, Str): could not locate " + DoLog("field", field.Id) + AndLog("xmlFieldElements.size", "" + fieldCount));
                return false;
            }
            XElement xmlFieldElement = xmlFieldElements.First();

            if (!fieldNames.Any() && annotation != null) {
                // nothing left in the path, so we will apply the annotation now
                NofMetaModel.SetAnnotationAttribute(xmlFieldElement, annotation);
            }

            var fieldPlace = new Place(nakedObjectAdapter, xmlFieldElement);

            if (field.ReturnSpec.IsParseable) {
                Log.Debug("includeField(Pl, Vec, Str): field is value; done");
                return false;
            }
            var oneToOneAssociation = field as IOneToOneAssociationSpec;
            if (oneToOneAssociation != null) {
                Log.Debug("includeField(Pl, Vec, Str): field is 1->1");

                INakedObjectAdapter referencedObjectAdapter = oneToOneAssociation.GetNakedObject(fieldPlace.NakedObjectAdapter);

                if (referencedObjectAdapter == null) {
                    return true; // not a failure if the reference was null
                }

                bool appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObjectAdapter, fieldNames, annotation);

                Log.Debug("includeField(Pl, Vec, Str): 1->1: invoked appendXmlThenIncludeRemaining for " + DoLog("referencedObj", referencedObjectAdapter) + AndLog("returned", "" + appendedXml));

                return appendedXml;
            }
            var oneToManyAssociation = field as IOneToManyAssociationSpec;
            if (oneToManyAssociation != null) {
                Log.Debug("includeField(Pl, Vec, Str): field is 1->M");

                INakedObjectAdapter collection = oneToManyAssociation.GetNakedObject(fieldPlace.NakedObjectAdapter);

                INakedObjectAdapter[] collectionAsEnumerable = collection.GetAsEnumerable(nakedObjectManager).ToArray();

                Log.Debug("includeField(Pl, Vec, Str): 1->M: " + DoLog("collection.size", "" + collectionAsEnumerable.Count()));
                bool allFieldsNavigated = true;

                foreach (INakedObjectAdapter referencedObject in collectionAsEnumerable) {
                    bool appendedXml = AppendXmlThenIncludeRemaining(fieldPlace, referencedObject, fieldNames, annotation);

                    Log.Debug("includeField(Pl, Vec, Str): 1->M: + invoked appendXmlThenIncludeRemaining for " + DoLog("referencedObj", referencedObject) + AndLog("returned", "" + appendedXml));

                    allFieldsNavigated = allFieldsNavigated && appendedXml;
                }
                Log.Debug("includeField(Pl, Vec, Str): " + DoLog("returning", "" + allFieldsNavigated));

                return allFieldsNavigated;
            }

            return false; // fall through, shouldn't get here but just in case.
        }

        private static string DoLog(string label, INakedObjectAdapter nakedObjectAdapter) {
            return DoLog(label, (nakedObjectAdapter == null ? "(null)" : nakedObjectAdapter.TitleString() + "[" + OidOrHashCode(nakedObjectAdapter) + "]"));
        }

        private static string DoLog(string label, object obj) {
            return (label ?? "?") + "='" + (obj == null ? "(null)" : obj.ToString()) + "'";
        }

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
            Log.Debug("mergeTree(" + DoLog("parent", parentElement) + AndLog("child", childElement));

            string childElementOid = NofMetaModel.GetAttribute(childElement, "oid");

            Log.Debug("mergeTree(El,El): " + DoLog("childOid", childElementOid));
            if (childElementOid != null) {
                // before we add the child element, check to see if it is already
                // there
                Log.Debug("mergeTree(El,El): check if child already there");
                IEnumerable<XElement> existingChildElements = ElementsUnder(parentElement, childElement.Name.LocalName);
                foreach (XElement possibleMatchingElement in existingChildElements) {
                    string possibleMatchOid = NofMetaModel.GetAttribute(possibleMatchingElement, "oid");
                    if (possibleMatchOid == null || !possibleMatchOid.Equals(childElementOid)) {
                        continue;
                    }

                    Log.Debug("mergeTree(El,El): child already there; merging grandchildren");

                    // match: transfer the children of the child (grandchildren) to the
                    // already existing matching child
                    XElement existingChildElement = possibleMatchingElement;
                    IEnumerable<XElement> grandchildrenElements = ElementsUnder(childElement, "*");
                    foreach (XElement grandchildElement in grandchildrenElements) {
                        grandchildElement.Remove();

                        Log.Debug("mergeTree(El,El): merging " + DoLog("grandchild", grandchildElement));

                        MergeTree(existingChildElement, grandchildElement);
                    }
                    return existingChildElement;
                }
            }

            parentElement.Add(childElement);
            return childElement;
        }

        public Place ObjectToElement(INakedObjectAdapter nakedObjectAdapter) {
            Log.Debug("objectToElement(" + DoLog("object", nakedObjectAdapter) + ")");

            var nos = (IObjectSpec) nakedObjectAdapter.Spec;

            Log.Debug("objectToElement(NO): create element and nof:title");
            XElement element = Schema.CreateElement(XmlDocument, nos.ShortName, nos.FullName, nos.SingularName, nos.PluralName);
            NofMetaModel.AppendNofTitle(element, nakedObjectAdapter.TitleString());

            Log.Debug("objectToElement(NO): create XS element for NOF class");
            XElement xsElement = Schema.CreateXsElementForNofClass(XsdDocument, element, topLevelElementWritten);

            // hack: every element in the XSD schema apart from first needs minimum cardinality setting.
            topLevelElementWritten = true;

            var place = new Place(nakedObjectAdapter, element);

            NofMetaModel.SetAttributesForClass(element, OidOrHashCode(nakedObjectAdapter));

            IAssociationSpec[] fields = nos.Properties;
            Log.Debug("objectToElement(NO): processing fields");

            var seenFields = new List<string>();

            foreach (IAssociationSpec field in fields) {
                string fieldName = field.Id;

                Log.Debug("objectToElement(NO): " + DoLog("field", fieldName));

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
                    Log.Debug("objectToElement(NO): " + DoLog("field", fieldName) + " SKIPPED");
                    continue;
                }
                seenFields.Add(fieldName);

                XNamespace ns = Schema.GetUri();

                var xmlFieldElement = new XElement(ns + fieldName);

                XElement xsdFieldElement;
                var oneToOneAssociation = field as IOneToOneAssociationSpec;
                var oneToManyAssociation = field as IOneToManyAssociationSpec;

                if (field.ReturnSpec.IsParseable && oneToOneAssociation != null) {
                    Log.Debug("objectToElement(NO): " + DoLog("field", fieldName) + " is value");

                    IObjectSpec fieldNos = field.ReturnSpec;
                    // skip fields of type XmlValue
                    if (fieldNos != null &&
                        fieldNos.FullName != null &&
                        fieldNos.FullName.EndsWith("XmlValue")) {
                        continue;
                    }

                    XElement xmlValueElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        INakedObjectAdapter value = oneToOneAssociation.GetNakedObject(nakedObjectAdapter);

                        // a null value would be a programming error, but we protect
                        // against it anyway
                        if (value == null) {
                            continue;
                        }

                        ITypeSpec valueNos = value.Spec;

                        // XML
                        NofMetaModel.SetAttributesForValue(xmlValueElement, valueNos.ShortName);

                        bool notEmpty = (value.TitleString().Length > 0);
                        if (notEmpty) {
                            string valueStr = value.TitleString();
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
                    Log.Debug("objectToElement(NO): " + DoLog("field", fieldName) + " is IOneToOneAssociation");

                    XElement xmlReferenceElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        INakedObjectAdapter referencedNakedObjectAdapter = oneToOneAssociation.GetNakedObject(nakedObjectAdapter);
                        string fullyQualifiedClassName = field.ReturnSpec.FullName;

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
                    Log.Debug("objectToElement(NO): " + DoLog("field", fieldName) + " is IOneToManyAssociation");

                    XElement xmlCollectionElement = xmlFieldElement; // more meaningful locally scoped name

                    try {
                        INakedObjectAdapter collection = oneToManyAssociation.GetNakedObject(nakedObjectAdapter);
                        ITypeOfFacet facet = collection.GetTypeOfFacetFromSpec();

                        IObjectSpecImmutable referencedTypeNos = facet.GetValueSpec(collection, metamodelManager.Metamodel);
                        string fullyQualifiedClassName = referencedTypeNos.FullName;

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
                    Log.Info("objectToElement(NO): " + DoLog("field", fieldName) + " is unknown type; ignored");
                    continue;
                }

                if (xsdFieldElement != null) {
                    xmlFieldElement.AddAnnotation(xsdFieldElement);
                }

                // XML
                Log.Debug("objectToElement(NO): invoking mergeTree for field");
                MergeTree(element, xmlFieldElement);

                // XSD
                if (xsdFieldElement != null) {
                    Log.Debug("objectToElement(NO): adding XS element for field to schema");
                    Schema.AddFieldXsElement(xsElement, xsdFieldElement);
                }
            }

            return place;
        }

        private static string OidOrHashCode(INakedObjectAdapter nakedObjectAdapter) {
            IOid oid = nakedObjectAdapter.Oid;
            if (oid == null) {
                return "" + nakedObjectAdapter.GetHashCode();
            }
            return oid.ToString();
        }

        private static string Transform(XDocument xDoc, string transform) {
            var sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb)) {
                var compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(XmlReader.Create(new StringReader(transform)));
                compiledTransform.Transform(xDoc.CreateReader(), writer);
            }

            return sb.ToString();
        }

        public string TransformXml(string transform) {
            return Transform(XmlDocument, transform);
        }

        public string TransformXsd(string transform) {
            return Transform(XsdDocument, transform);
        }
    }
}