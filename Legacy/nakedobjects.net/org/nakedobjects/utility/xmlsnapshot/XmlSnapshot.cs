// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.XmlSnapshot
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using javax.xml.parsers;
using org.nakedobjects.@object;
using org.w3c.dom;
using System.ComponentModel;

namespace org.nakedobjects.utility.xmlsnapshot
{
  public sealed class XmlSnapshot
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly string[] SKIP_FIELDS;
    private readonly bool addOids;
    private readonly Helper helper;
    private readonly NofMetaModel nofMeta;
    private readonly Place rootPlace;
    private readonly XmlSchema schema;
    private string schemaLocationFileName;
    private bool topLevelElementWritten;
    private readonly Document xmlDocument;
    private Element xmlElement;
    private readonly Document xsdDocument;
    private readonly Element xsdElement;
    private readonly XsMetaModel xsMeta;

    public XmlSnapshot(NakedObject rootObject)
      : this(rootObject, false)
    {
    }

    public XmlSnapshot(NakedObject rootObject, bool addOids)
      : this(rootObject, new XmlSchema(), addOids)
    {
    }

    public XmlSnapshot(NakedObject rootObject, XmlSchema schema, bool addOids)
    {
      this.topLevelElementWritten = false;
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append(".ctor(").append(this.log("rootObj", rootObject)).append(this.andlog(nameof (schema), (object) schema)).append(this.andlog(nameof (addOids), (object) new StringBuffer().append("").append(true).ToString())).append(")").ToString());
      this.addOids = addOids;
      this.nofMeta = new NofMetaModel();
      this.xsMeta = new XsMetaModel();
      this.helper = new Helper();
      this.schema = schema;
      DocumentBuilderFactory documentBuilderFactory = DocumentBuilderFactory.newInstance();
      documentBuilderFactory.setNamespaceAware(true);
      try
      {
        DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
        this.xmlDocument = documentBuilder.newDocument();
        this.xsdDocument = documentBuilder.newDocument();
        this.xsdElement = this.xsMeta.createXsSchemaElement(this.xsdDocument);
        this.rootPlace = this.appendXml(rootObject);
      }
      catch (ParserConfigurationException ex)
      {
        XmlSnapshot.LOG.error((object) "unable to build snapshot", (Throwable) ex);
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
    }

    private string andlog(string label, NakedObject @object) => new StringBuffer().append(", ").append(this.log(label, @object)).ToString();

    private string andlog(string label, object @object) => new StringBuffer().append(", ").append(this.log(label, @object)).ToString();

    private Place appendXml(NakedObject @object)
    {
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("appendXml(").append(this.log("obj", @object)).append("')").ToString());
      string fullName = @object.getSpecification().getFullName();
      this.schema.setUri(fullName);
      Place element = this.objectToElement(@object);
      Element xmlElement = element.getXmlElement();
      Element xsdElement = element.getXsdElement();
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(NO): add as element to XML doc");
      this.getXmlDocument().appendChild((Node) xmlElement);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(NO): add as xs:element to xs:schema of the XSD document");
      this.getXsdElement().appendChild((Node) xsdElement);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(NO): set target name in XSD, derived from FQCN of obj");
      this.schema.setTargetNamespace(this.getXsdDocument(), fullName);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(NO): set schema location file name to XSD, derived from FQCN of obj");
      string schemaLocationFileName = new StringBuffer().append(fullName).append(".xsd").ToString();
      this.schema.assignSchema(this.getXmlDocument(), fullName, schemaLocationFileName);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(NO): copy into snapshot obj");
      this.setXmlElement(xmlElement);
      this.setSchemaLocationFileName(schemaLocationFileName);
      return element;
    }

    private Element appendXml(Place parentPlace, NakedObject childObject)
    {
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("appendXml(").append(this.log(nameof (parentPlace), (object) parentPlace)).append(this.andlog("childObj", childObject)).append(")").ToString());
      Element xmlElement1 = parentPlace.getXmlElement();
      Element xsdElement1 = parentPlace.getXsdElement();
      if (xmlElement1.getOwnerDocument() != this.getXmlDocument())
        throw new IllegalArgumentException("parent XML Element must have snapshot's XML document as its owner");
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("appendXml(Pl, NO): invoking objectToElement() for ").append(this.log("childObj", childObject)).ToString());
      Place element1 = this.objectToElement(childObject);
      Element xmlElement2 = element1.getXmlElement();
      Element xsdElement2 = element1.getXsdElement();
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(Pl, NO): invoking mergeTree of parent with child");
      Element element2 = this.mergeTree(xmlElement1, xmlElement2);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXml(Pl, NO): adding XS Element to schema if required");
      this.schema.addXsElementIfNotPresent(xsdElement1, xsdElement2);
      return element2;
    }

    private bool appendXmlThenIncludeRemaining(
      Place parentPlace,
      NakedObject referencedObject,
      Vector fieldNames,
      string annotation)
    {
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("appendXmlThenIncludeRemaining(: ").append(this.log(nameof (parentPlace), (object) parentPlace)).append(this.andlog("referencedObj", referencedObject)).append(this.andlog(nameof (fieldNames), (object) fieldNames)).append(this.andlog(nameof (annotation), (object) annotation)).append(")").ToString());
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "appendXmlThenIncludeRemaining(..): invoking appendXml(parentPlace, referencedObject)");
      Element element = this.appendXml(parentPlace, referencedObject);
      bool flag = this.includeField(new Place(referencedObject, element), fieldNames, annotation);
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("appendXmlThenIncludeRemaining(..): invoked includeField(referencedPlace, fieldNames)").append(this.andlog("returned", (object) new StringBuffer().append("").append(flag).ToString())).ToString());
      return flag;
    }

    private Vector elementsUnder(Element parentElement, string localName)
    {
      Vector vector = new Vector();
      NodeList childNodes = parentElement.getChildNodes();
      for (int index = 0; index < childNodes.getLength(); ++index)
      {
        Node node = childNodes.item(index);
        if (node is Element)
        {
          Element element = (Element) node;
          if (StringImpl.equals(localName, (object) "*") || StringImpl.equals(element.getLocalName(), (object) localName))
            vector.addElement((object) element);
        }
      }
      return vector;
    }

    public virtual NakedObject getObject() => this.rootPlace.getObject();

    public virtual XmlSchema getSchema() => this.getSchema();

    public virtual string getSchemaLocationFileName() => this.schemaLocationFileName;

    public virtual Document getXmlDocument() => this.xmlDocument;

    public virtual Element getXmlElement() => this.xmlElement;

    public virtual Document getXsdDocument() => this.xsdDocument;

    public virtual Element getXsdElement() => this.xsdElement;

    public virtual void include(string path) => this.include(path, (string) null);

    public virtual void include(string path, string annotation)
    {
      Vector fieldNames = new Vector();
      StringTokenizer stringTokenizer = new StringTokenizer(path, "/");
      while (stringTokenizer.hasMoreTokens())
      {
        string lowerCase = StringImpl.toLowerCase(stringTokenizer.nextToken());
        if (XmlSnapshot.LOG.isDebugEnabled())
          XmlSnapshot.LOG.debug((object) new StringBuffer().append("include(..): ").append(this.log("token", (object) lowerCase)).ToString());
        fieldNames.addElement((object) lowerCase);
      }
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("include(..): ").append(this.log("fieldNames", (object) fieldNames)).ToString());
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) "include(..): invoking includeField");
      this.includeField(this.rootPlace, fieldNames, annotation);
    }

    private bool includeField(Place place, Vector fieldNames, string annotation)
    {
      bool flag1 = XmlSnapshot.LOG.isDebugEnabled();
      bool flag2 = XmlSnapshot.LOG.isInfoEnabled();
      if (flag1)
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(: ").append(this.log(nameof (place), (object) place)).append(this.andlog(nameof (fieldNames), (object) fieldNames)).append(this.andlog(nameof (annotation), (object) annotation)).append(")").ToString());
      NakedObject @object = place.getObject();
      Element xmlElement = place.getXmlElement();
      Vector vector1 = fieldNames;
      fieldNames = new Vector();
      Enumeration enumeration = vector1.elements();
      while (enumeration.hasMoreElements())
        fieldNames.addElement(enumeration.nextElement());
      if (fieldNames.size() == 0)
        return true;
      string name = \u003CVerifierFix\u003E.genCastToString(fieldNames.elementAt(0));
      fieldNames.removeElementAt(0);
      if (flag1)
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(Pl, Vec, Str):").append(this.log("processing field", (object) name)).append(this.andlog("left", (object) new StringBuffer().append("").append(fieldNames.size()).ToString())).ToString());
      NakedObjectSpecification specification = @object.getSpecification();
      NakedObjectField field1;
      try
      {
        field1 = specification.getField(name);
      }
      catch (NakedObjectSpecificationException ex)
      {
        if (flag2)
          XmlSnapshot.LOG.info((object) "includeField(Pl, Vec, Str): could not locate field, skipping");
        return false;
      }
      if (flag1)
        XmlSnapshot.LOG.debug((object) "includeField(Pl, Vec, Str): locating corresponding XML element");
      Vector vector2 = this.elementsUnder(xmlElement, field1.getId());
      if (vector2.size() != 1)
      {
        if (flag2)
          XmlSnapshot.LOG.info((object) new StringBuffer().append("includeField(Pl, Vec, Str): could not locate ").append(this.log("field", (object) field1.getId())).append(this.andlog("xmlFieldElements.size", (object) new StringBuffer().append("").append(vector2.size()).ToString())).ToString());
        return false;
      }
      Element element = (Element) vector2.elementAt(0);
      if (fieldNames.size() == 0 && annotation != null)
        this.nofMeta.setAnnotationAttribute(element, annotation);
      Place parentPlace = new Place(@object, element);
      if (field1.isValue())
      {
        if (flag1)
          XmlSnapshot.LOG.debug((object) "includeField(Pl, Vec, Str): field is value; done");
        return false;
      }
      switch (field1)
      {
        case OneToOneAssociation _:
          if (flag1)
            XmlSnapshot.LOG.debug((object) "includeField(Pl, Vec, Str): field is 1->1");
          OneToOneAssociation field2 = (OneToOneAssociation) field1;
          NakedObject association = parentPlace.getObject().getAssociation(field2);
          if (association == null)
            return true;
          bool flag3 = this.appendXmlThenIncludeRemaining(parentPlace, association, fieldNames, annotation);
          if (flag1)
            XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(Pl, Vec, Str): 1->1: invoked appendXmlThenIncludeRemaining for ").append(this.log("referencedObj", association)).append(this.andlog("returned", (object) new StringBuffer().append("").append(flag3).ToString())).ToString());
          return flag3;
        case OneToManyAssociation _:
          if (flag1)
            XmlSnapshot.LOG.debug((object) "includeField(Pl, Vec, Str): field is 1->M");
          OneToManyAssociation toManyAssociation = (OneToManyAssociation) field1;
          InternalCollection field3 = (InternalCollection) parentPlace.getObject().getField((NakedObjectField) toManyAssociation);
          if (flag1)
            XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(Pl, Vec, Str): 1->M: ").append(this.log("collection.size", (object) new StringBuffer().append("").append(field3.size()).ToString())).ToString());
          bool flag4 = true;
          for (int index = 0; index < field3.size(); ++index)
          {
            NakedObject nakedObject = field3.elementAt(index);
            bool flag5 = this.appendXmlThenIncludeRemaining(parentPlace, nakedObject, fieldNames, annotation);
            if (flag1)
              XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(Pl, Vec, Str): 1->M: + invoked appendXmlThenIncludeRemaining for ").append(this.log("referencedObj", nakedObject)).append(this.andlog("returned", (object) new StringBuffer().append("").append(flag5).ToString())).ToString());
            flag4 = flag4 && flag5;
          }
          if (flag1)
            XmlSnapshot.LOG.debug((object) new StringBuffer().append("includeField(Pl, Vec, Str): ").append(this.log("returning", (object) new StringBuffer().append("").append(flag4).ToString())).ToString());
          return flag4;
        default:
          return false;
      }
    }

    private string log(string label, NakedObject @object) => this.log(label, @object != null ? (object) new StringBuffer().append(@object.titleString()).append("[").append(this.oidOrHashCode(@object)).append("]").ToString() : (object) "(null)");

    private string log(string label, object @object) => new StringBuffer().append(label != null ? label : "?").append("='").append(@object != null ? @object.ToString() : "(null)").append("'").ToString();

    private Element mergeTree(Element parentElement, Element childElement)
    {
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("mergeTree(").append(this.log("parent", (object) parentElement)).append(this.andlog("child", (object) childElement)).ToString());
      string attribute1 = this.nofMeta.getAttribute(childElement, "oid");
      if (XmlSnapshot.LOG.isDebugEnabled())
        XmlSnapshot.LOG.debug((object) new StringBuffer().append("mergeTree(El,El): ").append(this.log("childOid", (object) attribute1)).ToString());
      if (attribute1 != null)
      {
        if (XmlSnapshot.LOG.isDebugEnabled())
          XmlSnapshot.LOG.debug((object) "mergeTree(El,El): check if child already there");
        Enumeration enumeration1 = this.elementsUnder(parentElement, childElement.getLocalName()).elements();
        while (enumeration1.hasMoreElements())
        {
          Element element = (Element) enumeration1.nextElement();
          string attribute2 = this.nofMeta.getAttribute(element, "oid");
          if (attribute2 != null && StringImpl.equals(attribute2, (object) attribute1))
          {
            if (XmlSnapshot.LOG.isDebugEnabled())
              XmlSnapshot.LOG.debug((object) "mergeTree(El,El): child already there; merging grandchildren");
            Element parentElement1 = element;
            Enumeration enumeration2 = this.elementsUnder(childElement, "*").elements();
            while (enumeration2.hasMoreElements())
            {
              Element childElement1 = (Element) enumeration2.nextElement();
              childElement.removeChild((Node) childElement1);
              if (XmlSnapshot.LOG.isDebugEnabled())
                XmlSnapshot.LOG.debug((object) new StringBuffer().append("mergeTree(El,El): merging ").append(this.log("grandchild", (object) childElement1)).ToString());
              this.mergeTree(parentElement1, childElement1);
            }
            return parentElement1;
          }
        }
      }
      parentElement.appendChild((Node) childElement);
      return childElement;
    }

    private Hashtable extensionsFor(NakedObjectSpecification nos)
    {
      Hashtable hashtable = new Hashtable();
      Class[] extensions = nos.getExtensions();
      for (int index = 0; index < extensions.Length; ++index)
      {
        object extension = nos.getExtension(extensions[index]);
        hashtable.put((object) extensions[index], extension);
      }
      return hashtable;
    }

    private Hashtable extensionsFor(OneToOneAssociation oneToOneAssoc)
    {
      Hashtable hashtable = new Hashtable();
      Class[] extensions = ((NakedObjectField) oneToOneAssoc).getExtensions();
      for (int index = 0; index < extensions.Length; ++index)
      {
        object extension = oneToOneAssoc.getExtension(extensions[index]);
        hashtable.put((object) extensions[index], extension);
      }
      return hashtable;
    }

    private Hashtable extensionsFor(OneToManyAssociation oneToManyAssoc)
    {
      Hashtable hashtable = new Hashtable();
      Class[] extensions = ((NakedObjectField) oneToManyAssoc).getExtensions();
      for (int index = 0; index < extensions.Length; ++index)
      {
        object extension = oneToManyAssoc.getExtension(extensions[index]);
        hashtable.put((object) extensions[index], extension);
      }
      return hashtable;
    }

    [JavaFlags(0)]
    public virtual Place objectToElement(NakedObject @object)
    {
      // ISSUE: unable to decompile the method.
    }

    private string oidOrHashCode(NakedObject @object)
    {
      Oid oid = @object.getOid();
      if (oid != null)
        return oid.ToString();
      StringBuffer stringBuffer = new StringBuffer().append("");
      NakedObject nakedObject = @object;
      int num = !(nakedObject is string) ? ObjectImpl.hashCode((object) nakedObject) : StringImpl.hashCode((string) nakedObject);
      return stringBuffer.append(num).ToString();
    }

    private void setSchemaLocationFileName(string schemaLocationFileName) => this.schemaLocationFileName = schemaLocationFileName;

    private void setXmlElement(Element xmlElement) => this.xmlElement = xmlElement;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XmlSnapshot()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XmlSnapshot xmlSnapshot = this;
      ObjectImpl.clone((object) xmlSnapshot);
      return ((object) xmlSnapshot).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
