// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.DomSerializerCrimson
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.crimson.tree;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaInterfaces("1;org/nakedobjects/utility/xmlsnapshot/DomSerializer;")]
  public class DomSerializerCrimson : DomSerializer
  {
    private static ElementNode2 assertCrimson(Element domElement) => domElement is ElementNode2 ? (ElementNode2) domElement : throw new IllegalArgumentException("Not using Crimson");

    public virtual string serialize(Element domElement)
    {
      CharArrayWriter charArrayWriter = new CharArrayWriter();
      try
      {
        this.serializeTo(domElement, (Writer) charArrayWriter);
        return charArrayWriter.ToString();
      }
      catch (IOException ex)
      {
        return (string) null;
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void serializeTo(Element domElement, OutputStream os)
    {
      OutputStreamWriter outputStreamWriter = new OutputStreamWriter(os);
      this.serializeTo(domElement, (Writer) outputStreamWriter);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void serializeTo(Element domElement, Writer w)
    {
      ElementNode2 elementNode2 = DomSerializerCrimson.assertCrimson(domElement);
      XmlWriteContext context = new XmlWriteContext(w, 2);
      elementNode2.writeXml(context);
      context.getWriter().flush();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DomSerializerCrimson serializerCrimson = this;
      ObjectImpl.clone((object) serializerCrimson);
      return ((object) serializerCrimson).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
