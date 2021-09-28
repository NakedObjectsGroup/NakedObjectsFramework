// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.DocumentEx
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.w3c.dom;
using System;

namespace org.apache.crimson.tree
{
  [JavaInterface]
  [JavaInterfaces("3;org/w3c/dom/Document;org/apache/crimson/tree/XmlWritable;org/apache/crimson/tree/ElementFactory;")]
  public interface DocumentEx : Document, XmlWritable, ElementFactory
  {
    string getSystemId();

    void setElementFactory(ElementFactory factory);

    ElementFactory getElementFactory();

    [Obsolete(null, false)]
    ElementEx getElementExById(string id);

    Locale getLocale();

    void setLocale(Locale locale);

    Locale chooseLocale(string[] languages);

    void changeNodeOwner(Node node);
  }
}
