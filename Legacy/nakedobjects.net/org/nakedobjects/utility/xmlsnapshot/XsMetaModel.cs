// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.XsMetaModel
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  public sealed class XsMetaModel
  {
    private readonly Helper helper;
    public const string W3_ORG_XMLNS_URI = "http://www.w3.org/2000/xmlns/";
    public const string W3_ORG_XMLNS_PREFIX = "xmlns";
    public const string W3_ORG_XS_URI = "http://www.w3.org/2001/XMLSchema";
    public const string W3_ORG_XS_PREFIX = "xs";
    public const string W3_ORG_XSI_URI = "http://www.w3.org/2001/XMLSchema-instance";
    public const string W3_ORG_XSI_PREFIX = "xsi";
    private readonly NofMetaModel nofMeta;

    public XsMetaModel()
    {
      this.helper = new Helper();
      this.nofMeta = new NofMetaModel();
    }

    [JavaFlags(0)]
    public virtual Element createXsSchemaElement(Document xsdDoc)
    {
      Element element = xsdDoc.getDocumentElement() == null ? this.createXsElement(xsdDoc, "schema") : throw new IllegalArgumentException("XSD document already has content");
      element.setAttribute("elementFormDefault", "qualified");
      this.nofMeta.addNamespace(element);
      xsdDoc.appendChild((Node) element);
      Element xsElement = this.createXsElement(xsdDoc, "import");
      xsElement.setAttribute("namespace", "http://www.nakedobjects.org/ns/0.1/metamodel");
      xsElement.setAttribute("schemaLocation", "nof.xsd");
      element.appendChild((Node) xsElement);
      return element;
    }

    [JavaFlags(0)]
    public virtual Element createXsElementElement(Document xsdDoc, string className) => this.createXsElementElement(xsdDoc, className, true);

    [JavaFlags(0)]
    public virtual Element createXsElementElement(
      Document xsdDoc,
      string className,
      bool includeCardinality)
    {
      Element xsElement = this.createXsElement(xsdDoc, "element");
      xsElement.setAttribute("name", className);
      if (includeCardinality)
        this.setXsCardinality(xsElement, 0, int.MaxValue);
      return xsElement;
    }

    [JavaFlags(0)]
    public virtual Element createXsElement(Document xsdDoc, string localName)
    {
      Element elementNs = xsdDoc.createElementNS("http://www.w3.org/2001/XMLSchema", new StringBuffer().append("xs").append(":").append(localName).ToString());
      this.helper.rootElementFor(elementNs).setAttributeNS("http://www.w3.org/2000/xmlns/", new StringBuffer().append("xmlns").append(":").append("xs").ToString(), "http://www.w3.org/2001/XMLSchema");
      return elementNs;
    }

    [JavaFlags(0)]
    public virtual Element addXsNofAttribute(Element parentXsElement, string nofAttributeRef) => this.addXsNofAttribute(parentXsElement, nofAttributeRef, (string) null);

    [JavaFlags(0)]
    public virtual Element addXsNofAttribute(
      Element parentXsElement,
      string nofAttributeRef,
      string fixedValue)
    {
      return this.addXsNofAttribute(parentXsElement, nofAttributeRef, fixedValue, true);
    }

    [JavaFlags(0)]
    public virtual Element addXsNofAttribute(
      Element parentXsElement,
      string nofAttributeRef,
      string value,
      bool useFixed)
    {
      Element xsElement = this.createXsElement(this.helper.docFor(parentXsElement), "attribute");
      xsElement.setAttribute("ref", new StringBuffer().append("nof").append(":").append(nofAttributeRef).ToString());
      parentXsElement.appendChild((Node) xsElement);
      if (value != null)
      {
        if (useFixed)
          xsElement.setAttribute("fixed", value);
        else
          xsElement.setAttribute("default", value);
      }
      return parentXsElement;
    }

    [JavaFlags(0)]
    public virtual Element addXsNofFeatureAttributeElements(
      Element parentXsElement,
      string feature)
    {
      Element xsElement = this.createXsElement(this.helper.docFor(parentXsElement), "attribute");
      xsElement.setAttribute("ref", "nof:feature");
      xsElement.setAttribute("fixed", feature);
      parentXsElement.appendChild((Node) xsElement);
      return xsElement;
    }

    [JavaFlags(0)]
    public virtual Element complexTypeFor(Element parentXsElement) => this.complexTypeFor(parentXsElement, true);

    [JavaFlags(0)]
    public virtual Element complexTypeFor(Element parentXsElement, bool mixed)
    {
      Element element = this.childXsElement(parentXsElement, "complexType");
      if (mixed)
        element.setAttribute(nameof (mixed), "true");
      return element;
    }

    [JavaFlags(0)]
    public virtual Element sequenceFor(Element parentXsElement) => this.childXsElement(parentXsElement, "sequence");

    [JavaFlags(0)]
    public virtual Element choiceFor(Element parentXsElement) => this.childXsElement(parentXsElement, "choice");

    [JavaFlags(0)]
    public virtual Element sequenceForComplexTypeFor(Element parentXsElement) => this.sequenceFor(this.complexTypeFor(parentXsElement));

    [JavaFlags(0)]
    public virtual Element choiceForComplexTypeFor(Element parentXsElement) => this.choiceFor(this.complexTypeFor(parentXsElement));

    [JavaFlags(0)]
    public virtual Element choiceOrSequenceFor(Element parentXsElement)
    {
      NodeList elementsByTagNameNs1 = parentXsElement.getElementsByTagNameNS("http://www.w3.org/2001/XMLSchema", "choice");
      if (elementsByTagNameNs1.getLength() > 0)
        return (Element) elementsByTagNameNs1.item(0);
      NodeList elementsByTagNameNs2 = parentXsElement.getElementsByTagNameNS("http://www.w3.org/2001/XMLSchema", "sequence");
      return elementsByTagNameNs2.getLength() > 0 ? (Element) elementsByTagNameNs2.item(0) : (Element) null;
    }

    [JavaFlags(0)]
    public virtual Element simpleContentFor(Element parentXsElement) => this.childXsElement(parentXsElement, "simpleContent");

    [JavaFlags(0)]
    public virtual Element extensionFor(Element parentXsElement, string @base)
    {
      Element element = this.childXsElement(parentXsElement, "extension");
      element.setAttribute(nameof (@base), new StringBuffer().append("xs").append(":").append(@base).ToString());
      return element;
    }

    [JavaFlags(0)]
    public virtual Element childXsElement(Element parentXsElement, string localName)
    {
      NodeList elementsByTagNameNs = parentXsElement.getElementsByTagNameNS("http://www.w3.org/2001/XMLSchema", localName);
      if (elementsByTagNameNs.getLength() > 0)
        return (Element) elementsByTagNameNs.item(0);
      Element xsElement = this.createXsElement(this.helper.docFor(parentXsElement), localName);
      parentXsElement.appendChild((Node) xsElement);
      return xsElement;
    }

    [JavaFlags(0)]
    public virtual Element schemaFor(Element xsElement) => xsElement.getOwnerDocument().getDocumentElement();

    [JavaFlags(0)]
    public virtual Element setXsCardinality(Element xsElement, int minOccurs, int maxOccurs)
    {
      if (maxOccurs >= 0)
        xsElement.setAttribute(nameof (minOccurs), new StringBuffer().append("").append(minOccurs).ToString());
      if (maxOccurs >= 0)
      {
        if (maxOccurs == int.MaxValue)
          xsElement.setAttribute(nameof (maxOccurs), "unbounded");
        else
          xsElement.setAttribute(nameof (maxOccurs), new StringBuffer().append("").append(maxOccurs).ToString());
      }
      return xsElement;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XsMetaModel xsMetaModel = this;
      ObjectImpl.clone((object) xsMetaModel);
      return ((object) xsMetaModel).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
