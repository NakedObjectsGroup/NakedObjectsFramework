// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.NofMetaModel
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaFlags(48)]
  public sealed class NofMetaModel
  {
    public const string DEFAULT_NOF_SCHEMA_LOCATION = "nof.xsd";
    public const string DEFAULT_URI_BASE = "http://www.nakedobjects.org/ns/app/";
    public const string NOF_METAMODEL_FEATURE_CLASS = "class";
    public const string NOF_METAMODEL_FEATURE_COLLECTION = "collection";
    public const string NOF_METAMODEL_FEATURE_REFERENCE = "reference";
    public const string NOF_METAMODEL_FEATURE_VALUE = "value";
    public const string NOF_METAMODEL_NS_PREFIX = "nof";
    public const string NOF_METAMODEL_NS_URI = "http://www.nakedobjects.org/ns/0.1/metamodel";
    private readonly Helper helper;

    public NofMetaModel() => this.helper = new Helper();

    [JavaFlags(0)]
    public virtual void addNamespace(Element element) => this.helper.rootElementFor(element).setAttributeNS("http://www.w3.org/2000/xmlns/", new StringBuffer().append("xmlns").append(":").append("nof").ToString(), "http://www.nakedobjects.org/ns/0.1/metamodel");

    [JavaFlags(0)]
    public virtual Element appendElement(Element parentElement, string localName)
    {
      Element elementNs = this.helper.docFor(parentElement).createElementNS("http://www.nakedobjects.org/ns/0.1/metamodel", new StringBuffer().append("nof").append(":").append(localName).ToString());
      parentElement.appendChild((Node) elementNs);
      return elementNs;
    }

    public virtual void appendNofTitle(Element element, string titleStr)
    {
      Document document = this.helper.docFor(element);
      this.appendElement(element, "title").appendChild((Node) document.createTextNode(titleStr));
    }

    [JavaFlags(0)]
    public virtual string getAttribute(Element element, string attributeName) => element.getAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", attributeName);

    [JavaFlags(0)]
    public virtual void setAnnotationAttribute(Element element, string annotation) => this.setAttribute(element, nameof (annotation), new StringBuffer().append("nof").append(":").append(annotation).ToString());

    private void setAttribute(Element element, string attributeName, string attributeValue) => element.setAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", new StringBuffer().append("nof").append(":").append(attributeName).ToString(), attributeValue);

    [JavaFlags(0)]
    public virtual void setAttributesForClass(Element element, string oid)
    {
      this.setAttribute(element, "feature", "class");
      this.setAttribute(element, nameof (oid), oid);
    }

    [JavaFlags(0)]
    public virtual void setAttributesForReference(
      Element element,
      string prefix,
      string fullyQualifiedClassName)
    {
      this.setAttribute(element, "feature", "reference");
      this.setAttribute(element, "type", new StringBuffer().append(prefix).append(":").append(fullyQualifiedClassName).ToString());
    }

    [JavaFlags(0)]
    public virtual void setAttributesForValue(Element element, string datatypeName)
    {
      this.setAttribute(element, "feature", "value");
      this.setAttribute(element, "datatype", new StringBuffer().append("nof").append(":").append(datatypeName).ToString());
    }

    [JavaFlags(0)]
    public virtual void setIsEmptyAttribute(Element element, bool isEmpty) => this.setAttribute(element, nameof (isEmpty), new StringBuffer().append("").append(isEmpty).ToString());

    [JavaFlags(0)]
    public virtual void setNofCollection(
      Element element,
      string prefix,
      string fullyQualifiedClassName,
      InternalCollection collection,
      bool addOids)
    {
      this.setAttribute(element, "feature", nameof (collection));
      this.setAttribute(element, "type", new StringBuffer().append(prefix).append(":").append(fullyQualifiedClassName).ToString());
      if (addOids)
        throw new NakedObjectRuntimeException();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NofMetaModel nofMetaModel = this;
      ObjectImpl.clone((object) nofMetaModel);
      return ((object) nofMetaModel).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
