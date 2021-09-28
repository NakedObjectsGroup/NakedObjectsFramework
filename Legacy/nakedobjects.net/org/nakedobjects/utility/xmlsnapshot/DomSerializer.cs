// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.DomSerializer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaInterface]
  public interface DomSerializer
  {
    string serialize(Element domElement);

    [JavaThrownExceptions("1;java/io/IOException;")]
    void serializeTo(Element domElement, OutputStream os);

    [JavaThrownExceptions("1;java/io/IOException;")]
    void serializeTo(Element domElement, Writer w);
  }
}
