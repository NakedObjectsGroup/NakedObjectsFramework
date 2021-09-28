// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.Place
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.crimson.tree;
using org.nakedobjects.@object;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaFlags(48)]
  public sealed class Place
  {
    private readonly NakedObject @object;
    private readonly Element element;

    [JavaFlags(0)]
    public Place(NakedObject @object, Element element)
    {
      this.@object = @object;
      this.element = element;
    }

    public virtual Element getXmlElement() => this.element;

    public virtual NakedObject getObject() => this.@object;

    public virtual Element getXsdElement()
    {
      if (!(this.element is ElementNode2))
        return (Element) null;
      object userObject = ((ElementNode2) this.element).getUserObject();
      return userObject == null || !(userObject is Element) ? (Element) null : (Element) userObject;
    }

    [JavaFlags(8)]
    public static void setXsdElement(Element element, Element xsElement)
    {
      if (!(element is ElementNode2))
        return;
      ((ElementNode2) element).setUserObject((object) xsElement);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Place place = this;
      ObjectImpl.clone((object) place);
      return ((object) place).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
