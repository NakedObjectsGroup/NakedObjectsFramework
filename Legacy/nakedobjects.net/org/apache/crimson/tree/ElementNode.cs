// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.ElementNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.w3c.dom;
using System;

namespace org.apache.crimson.tree
{
  public class ElementNode : ElementNode2
  {
    public ElementNode()
      : base((string) null, (string) null)
    {
    }

    public ElementNode(string name)
      : base((string) null, name)
    {
    }

    [JavaFlags(0)]
    public override ElementNode2 makeClone()
    {
      ElementNode2 elementNode2 = (ElementNode2) new ElementNode(this.qName);
      if (this.attributes != null)
      {
        elementNode2.attributes = new AttributeSet(this.attributes, true);
        elementNode2.attributes.setOwnerElement((Element) this);
      }
      elementNode2.setIdAttributeName(this.getIdAttributeName());
      elementNode2.setUserObject(this.getUserObject());
      elementNode2.ownerDocument = this.ownerDocument;
      return elementNode2;
    }

    [JavaFlags(4)]
    [Obsolete(null, false)]
    public virtual void setTag(string t) => this.qName = t;

    public override string getPrefix() => (string) null;

    public override string getLocalName() => (string) null;
  }
}
