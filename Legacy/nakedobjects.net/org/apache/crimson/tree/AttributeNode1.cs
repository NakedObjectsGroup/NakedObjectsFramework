// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.AttributeNode1
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.apache.crimson.tree
{
  public class AttributeNode1 : AttributeNode
  {
    public AttributeNode1(string name, string value, bool specified, string defaultValue)
      : base((string) null, name, value, specified, defaultValue)
    {
    }

    [JavaFlags(0)]
    public override AttributeNode makeClone()
    {
      AttributeNode attributeNode = (AttributeNode) new AttributeNode1(this.qName, this.getValue(), this.getSpecified(), this.getDefaultValue());
      attributeNode.ownerDocument = this.ownerDocument;
      return attributeNode;
    }

    public override string getPrefix() => (string) null;

    public override string getLocalName() => (string) null;
  }
}
