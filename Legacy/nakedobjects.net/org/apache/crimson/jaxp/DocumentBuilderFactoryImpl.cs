// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.jaxp.DocumentBuilderFactoryImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using javax.xml.parsers;

namespace org.apache.crimson.jaxp
{
  public class DocumentBuilderFactoryImpl : DocumentBuilderFactory
  {
    [JavaThrownExceptions("1;javax/xml/parsers/ParserConfigurationException;")]
    public override DocumentBuilder newDocumentBuilder() => (DocumentBuilder) new DocumentBuilderImpl((DocumentBuilderFactory) this);

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public override void setAttribute(string name, object value) => throw new IllegalArgumentException("No attributes are implemented");

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public override object getAttribute(string name) => throw new IllegalArgumentException("No attributes are implemented");
  }
}
