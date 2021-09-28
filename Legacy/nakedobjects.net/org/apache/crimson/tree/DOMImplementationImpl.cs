// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.DOMImplementationImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.crimson.util;
using org.w3c.dom;
using System.ComponentModel;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/DOMImplementation;")]
  public class DOMImplementationImpl : DOMImplementation
  {
    private static DOMImplementationImpl singleton;

    public static DOMImplementation getDOMImplementation() => (DOMImplementation) DOMImplementationImpl.singleton;

    public virtual bool hasFeature(string feature, string version) => DOMImplementationImpl.hasFeature0(feature, version);

    [JavaFlags(8)]
    public static bool hasFeature0(string feature, string version) => (StringImpl.equalsIgnoreCase("XML", feature) || StringImpl.equalsIgnoreCase("Core", feature)) && (version == null || StringImpl.equals("", (object) version) || StringImpl.equals("2.0", (object) version) || StringImpl.equals("1.0", (object) version));

    public virtual DocumentType createDocumentType(
      string qualifiedName,
      string publicId,
      string systemId)
    {
      if (!XmlNames.isName(qualifiedName))
        throw new DomEx((short) 5);
      if (!XmlNames.isQualifiedName(qualifiedName))
        throw new DomEx((short) 14);
      return (DocumentType) new Doctype(qualifiedName, publicId, systemId, (string) null);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Document createDocument(
      string namespaceURI,
      string qualifiedName,
      DocumentType doctype)
    {
      Document document = (Document) new XmlDocument();
      if (doctype != null)
        document.appendChild((Node) doctype);
      Element elementNs = document.createElementNS(namespaceURI, qualifiedName);
      document.appendChild((Node) elementNs);
      return document;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DOMImplementationImpl()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DOMImplementationImpl implementationImpl = this;
      ObjectImpl.clone((object) implementationImpl);
      return ((object) implementationImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
