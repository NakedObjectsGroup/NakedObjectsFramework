// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.NodeEx
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("2;org/w3c/dom/Node;org/apache/crimson/tree/XmlWritable;")]
  [JavaInterface]
  public interface NodeEx : Node, XmlWritable
  {
    string getInheritedAttribute(string name);

    string getLanguage();

    int getIndexOf(Node maybeChild);

    void setReadonly(bool deep);

    bool isReadonly();
  }
}
