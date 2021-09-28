// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.XmlSchema
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  public sealed class XmlSchema
  {
    private string prefix;
    private string uriBase;
    private string uri;
    private readonly NofMetaModel nofMeta;
    private readonly XsMetaModel xsMeta;
    private readonly Helper helper;
    public const string DEFAULT_PREFIX = "app";

    public XmlSchema()
      : this("http://www.nakedobjects.org/ns/app/", "app")
    {
    }

    public XmlSchema(string uriBase, string prefix)
    {
      this.nofMeta = new NofMetaModel();
      this.xsMeta = new XsMetaModel();
      this.helper = new Helper();
      uriBase = new Helper().trailingSlash(uriBase);
      if (StringImpl.equals("http://www.w3.org/2000/xmlns/", (object) uriBase))
        throw new IllegalArgumentException("Namespace URI reserved for w3.org XMLNS namespace");
      if (StringImpl.equals("xmlns", (object) prefix))
        throw new IllegalArgumentException("Namespace prefix reserved for w3.org XMLNS namespace.");
      if (StringImpl.equals("http://www.w3.org/2001/XMLSchema", (object) uriBase))
        throw new IllegalArgumentException("Namespace URI reserved for w3.org XML schema namespace.");
      if (StringImpl.equals("xs", (object) prefix))
        throw new IllegalArgumentException("Namespace prefix reserved for w3.org XML schema namespace.");
      if (StringImpl.equals("http://www.w3.org/2001/XMLSchema-instance", (object) uriBase))
        throw new IllegalArgumentException("Namespace URI reserved for w3.org XML schema-instance namespace.");
      if (StringImpl.equals("xsi", (object) prefix))
        throw new IllegalArgumentException("Namespace prefix reserved for w3.org XML schema-instance namespace.");
      if (StringImpl.equals("http://www.nakedobjects.org/ns/0.1/metamodel", (object) uriBase))
        throw new IllegalArgumentException("Namespace URI reserved for NOF metamodel namespace.");
      if (StringImpl.equals("nof", (object) prefix))
        throw new IllegalArgumentException("Namespace prefix reserved for NOF metamodel namespace.");
      this.uriBase = uriBase;
      this.prefix = prefix;
    }

    public virtual string getUriBase() => this.uriBase;

    [JavaFlags(0)]
    public virtual void setUri(string fullyQualifiedClassName) => this.uri = this.uri == null ? new StringBuffer().append(this.getUriBase()).append(this.helper.packageNameFor(fullyQualifiedClassName)).append("/").append(this.helper.classNameFor(fullyQualifiedClassName)).ToString() : throw new IllegalStateException("URI has already been specified.");

    public virtual string getUri() => this.uri != null ? this.uri : throw new IllegalStateException("URI has not been specified.");

    public virtual string getPrefix() => this.prefix;

    [JavaFlags(0)]
    public virtual Element createElement(
      Document doc,
      string localName,
      string fullyQualifiedClassName,
      string singularName,
      string pluralName)
    {
      Element elementNs = doc.createElementNS(this.getUri(), new StringBuffer().append(this.getPrefix()).append(":").append(localName).ToString());
      elementNs.setAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", "nof:fqn", fullyQualifiedClassName);
      elementNs.setAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", "nof:singular", singularName);
      elementNs.setAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", "nof:plural", pluralName);
      this.nofMeta.addNamespace(elementNs);
      this.addNamespace(elementNs, this.getPrefix(), this.getUri());
      return elementNs;
    }

    [JavaFlags(0)]
    public virtual void setTargetNamespace(Document xsdDoc, string fullyQualifiedClassName)
    {
      Element documentElement = xsdDoc.getDocumentElement();
      if (documentElement == null)
        throw new IllegalArgumentException("XSD Document must have <xs:schema> element attached");
      documentElement.setAttribute("targetNamespace", this.getUri());
      this.addNamespace(documentElement, this.getPrefix(), this.getUri());
    }

    [JavaFlags(0)]
    public virtual Element createXsElementForNofClass(
      Document xsdDoc,
      Element element,
      bool addCardinality,
      Hashtable extensions)
    {
      string localName = element.getLocalName();
      Element xsElementElement = this.xsMeta.createXsElementElement(xsdDoc, localName, addCardinality);
      Element parentXsElement = this.xsMeta.complexTypeFor(xsElementElement);
      Element element1 = this.xsMeta.sequenceFor(parentXsElement);
      Element xsElement = this.xsMeta.createXsElement(this.helper.docFor(element1), nameof (element));
      xsElement.setAttribute("ref", new StringBuffer().append("nof").append(":").append("title").ToString());
      element1.appendChild((Node) xsElement);
      this.xsMeta.setXsCardinality(xsElement, 0, 1);
      this.addXsElementForAppExtensions(element1, extensions);
      this.xsMeta.addXsNofFeatureAttributeElements(parentXsElement, "class");
      this.xsMeta.addXsNofAttribute(parentXsElement, "oid");
      this.xsMeta.addXsNofAttribute(parentXsElement, "fqn");
      this.xsMeta.addXsNofAttribute(parentXsElement, "singular");
      this.xsMeta.addXsNofAttribute(parentXsElement, "plural");
      this.xsMeta.addXsNofAttribute(parentXsElement, "annotation");
      Place.setXsdElement(element, xsElementElement);
      return xsElementElement;
    }

    [JavaFlags(0)]
    public virtual void addXsElementForAppExtensions(
      Element parentXsElementElement,
      Hashtable extensions)
    {
      if (extensions.size() == 0)
        return;
      this.addExtensionElements(this.addExtensionsElement(parentXsElementElement), extensions);
    }

    private Element addExtensionsElement(Element parentXsElement)
    {
      Element xsElementElement = this.xsMeta.createXsElementElement(this.helper.docFor(parentXsElement), "nof-extensions");
      parentXsElement.appendChild((Node) xsElementElement);
      return this.xsMeta.sequenceFor(this.xsMeta.complexTypeFor(xsElementElement));
    }

    private string shortName(string className)
    {
      int num = StringImpl.lastIndexOf(className, 46);
      return num < 0 ? className : StringImpl.substring(className, num + 1);
    }

    [JavaFlags(0)]
    public virtual Element createXsElementForNofValue(
      Element parentXsElementElement,
      Element xmlValueElement,
      Hashtable extensions)
    {
      string attributeNs = xmlValueElement.getAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", "datatype");
      string localName = xmlValueElement.getLocalName();
      Element element = this.xsMeta.sequenceFor(this.xsMeta.complexTypeFor(parentXsElementElement));
      Element xsElementElement = this.xsMeta.createXsElementElement(this.helper.docFor(element), localName);
      element.appendChild((Node) xsElementElement);
      Element parentXsElement = this.xsMeta.complexTypeFor(xsElementElement);
      this.addXsElementForAppExtensions(this.xsMeta.sequenceFor(parentXsElement), extensions);
      this.xsMeta.addXsNofFeatureAttributeElements(parentXsElement, "value");
      this.xsMeta.addXsNofAttribute(parentXsElement, "datatype", attributeNs);
      this.xsMeta.addXsNofAttribute(parentXsElement, "isEmpty");
      this.xsMeta.addXsNofAttribute(parentXsElement, "annotation");
      return xsElementElement;
    }

    private void addExtensionElements(Element parentElement, Hashtable extensions)
    {
      Enumeration enumeration = extensions.keys();
      while (enumeration.hasMoreElements())
      {
        Class @class = (Class) enumeration.nextElement();
        object obj = extensions.get((object) @class);
        Element xsElementElement = this.xsMeta.createXsElementElement(this.helper.docFor(parentElement), new StringBuffer().append("x-").append(this.shortName(@class.getName())).ToString());
        xsElementElement.setAttribute("default", obj.ToString());
        xsElementElement.setAttribute("minOccurs", "0");
        parentElement.appendChild((Node) xsElementElement);
      }
    }

    [JavaFlags(0)]
    public virtual Element createXsElementForNofReference(
      Element parentXsElementElement,
      Element xmlReferenceElement,
      string referencedClassName,
      Hashtable extensions)
    {
      string localName = xmlReferenceElement.getLocalName();
      Element element1 = this.xsMeta.sequenceFor(this.xsMeta.complexTypeFor(parentXsElementElement));
      Element xsElementElement = this.xsMeta.createXsElementElement(this.helper.docFor(element1), localName);
      element1.appendChild((Node) xsElementElement);
      Element parentXsElement = this.xsMeta.complexTypeFor(xsElementElement);
      Element element2 = this.xsMeta.sequenceFor(parentXsElement);
      Element xsElement = this.xsMeta.createXsElement(this.helper.docFor(element2), "element");
      xsElement.setAttribute("ref", new StringBuffer().append("nof").append(":").append("title").ToString());
      element2.appendChild((Node) xsElement);
      this.xsMeta.setXsCardinality(xsElement, 0, 1);
      this.addXsElementForAppExtensions(element2, extensions);
      this.xsMeta.setXsCardinality(this.xsMeta.sequenceFor(element2), 0, 1);
      this.xsMeta.addXsNofFeatureAttributeElements(parentXsElement, "reference");
      this.xsMeta.addXsNofAttribute(parentXsElement, "type", new StringBuffer().append("app:").append(referencedClassName).ToString(), false);
      this.xsMeta.addXsNofAttribute(parentXsElement, "isEmpty");
      this.xsMeta.addXsNofAttribute(parentXsElement, "annotation");
      return xsElementElement;
    }

    [JavaFlags(0)]
    public virtual Element createXsElementForNofCollection(
      Element parentXsElementElement,
      Element xmlCollectionElement,
      string referencedClassName,
      Hashtable extensions)
    {
      string localName = xmlCollectionElement.getLocalName();
      Element element1 = this.xsMeta.sequenceFor(this.xsMeta.complexTypeFor(parentXsElementElement));
      Element xsElementElement = this.xsMeta.createXsElementElement(this.helper.docFor(element1), localName);
      element1.appendChild((Node) xsElementElement);
      Element parentXsElement = this.xsMeta.complexTypeFor(xsElementElement);
      Element element2 = this.xsMeta.sequenceFor(parentXsElement);
      Element xsElement = this.xsMeta.createXsElement(this.helper.docFor(element2), "element");
      xsElement.setAttribute("ref", new StringBuffer().append("nof").append(":").append("oids").ToString());
      element2.appendChild((Node) xsElement);
      this.xsMeta.setXsCardinality(xsElement, 0, 1);
      this.addXsElementForAppExtensions(element2, extensions);
      this.xsMeta.addXsNofFeatureAttributeElements(parentXsElement, "collection");
      this.xsMeta.addXsNofAttribute(parentXsElement, "type", new StringBuffer().append("app:").append(referencedClassName).ToString(), false);
      this.xsMeta.addXsNofAttribute(parentXsElement, "size");
      this.xsMeta.addXsNofAttribute(parentXsElement, "annotation");
      return xsElementElement;
    }

    [JavaFlags(0)]
    public virtual void assignSchema(
      Document xmlDoc,
      string fullyQualifiedClassName,
      string schemaLocationFileName)
    {
      string str = new StringBuffer().append(this.getUri()).append(" ").append(schemaLocationFileName).ToString();
      Element documentElement = xmlDoc.getDocumentElement();
      this.addNamespace(documentElement, "xsi", "http://www.w3.org/2001/XMLSchema-instance");
      documentElement.setAttributeNS("http://www.w3.org/2001/XMLSchema-instance", "xsi:schemaLocation", str);
    }

    [JavaFlags(0)]
    public virtual void addFieldXsElement(Element xsElement, Element xsFieldElement)
    {
      if (xsFieldElement == null)
        return;
      this.xsMeta.sequenceForComplexTypeFor(xsElement).appendChild((Node) xsFieldElement);
    }

    private void addNamespace(Element element, string prefix, string nsUri)
    {
      string attributeNs = this.helper.rootElementFor(element).getAttributeNS("http://www.w3.org/2000/xmlns/", prefix);
      if (attributeNs != null && StringImpl.equals(attributeNs, (object) nsUri))
        return;
      this.helper.rootElementFor(element).setAttributeNS("http://www.w3.org/2000/xmlns/", new StringBuffer().append("xmlns").append(":").append(prefix).ToString(), nsUri);
    }

    [JavaFlags(0)]
    public virtual Element addXsElementIfNotPresent(
      Element parentXsElement,
      Element childXsElement)
    {
      Element element1 = this.xsMeta.choiceOrSequenceFor(this.xsMeta.complexTypeFor(parentXsElement));
      if (element1 == null)
        throw new IllegalArgumentException("Unable to locate complexType/sequence or complexType/choice under supplied parent XSD element");
      string str = ((Attr) childXsElement.getAttributes().getNamedItem("name")).getValue();
      NodeList elementsByTagNameNs = element1.getElementsByTagNameNS("*", childXsElement.getLocalName());
      for (int index = 0; index < elementsByTagNameNs.getLength(); ++index)
      {
        Element element2 = (Element) elementsByTagNameNs.item(index);
        Attr namedItem = (Attr) element2.getAttributes().getNamedItem("name");
        if (namedItem != null && StringImpl.equals(namedItem.getValue(), (object) str))
          return element2;
      }
      element1.appendChild((Node) childXsElement);
      return childXsElement;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XmlSchema xmlSchema = this;
      ObjectImpl.clone((object) xmlSchema);
      return ((object) xmlSchema).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
