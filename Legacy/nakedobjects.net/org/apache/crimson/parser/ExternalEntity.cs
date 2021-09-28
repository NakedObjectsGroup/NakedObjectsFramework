// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ExternalEntity
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.net;
using org.xml.sax;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class ExternalEntity : EntityDecl
  {
    [JavaFlags(0)]
    public string systemId;
    [JavaFlags(0)]
    public string publicId;
    [JavaFlags(0)]
    public string notation;
    [JavaFlags(0)]
    public string verbatimSystemId;

    public ExternalEntity(Locator l)
    {
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual InputSource getInputSource(EntityResolver r) => r.resolveEntity(this.publicId, this.systemId) ?? Resolver.createInputSource(new URL(this.systemId), false);
  }
}
