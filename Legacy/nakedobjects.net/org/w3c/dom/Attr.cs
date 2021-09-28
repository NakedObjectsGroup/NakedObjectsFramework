// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.Attr
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  [JavaInterfaces("1;org/w3c/dom/Node;")]
  public interface Attr : Node
  {
    string getName();

    bool getSpecified();

    string getValue();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setValue(string value);

    Element getOwnerElement();
  }
}
