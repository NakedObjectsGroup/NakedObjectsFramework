// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.CharacterData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterfaces("1;org/w3c/dom/Node;")]
  [JavaInterface]
  public interface CharacterData : Node
  {
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    string getData();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setData(string data);

    int getLength();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    string substringData(int offset, int count);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void appendData(string arg);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void insertData(int offset, string arg);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void deleteData(int offset, int count);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void replaceData(int offset, int count, string arg);
  }
}
