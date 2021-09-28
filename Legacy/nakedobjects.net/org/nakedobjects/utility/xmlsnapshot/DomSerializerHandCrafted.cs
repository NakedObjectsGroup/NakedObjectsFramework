// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.DomSerializerHandCrafted
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaInterfaces("1;org/nakedobjects/utility/xmlsnapshot/DomSerializer;")]
  public sealed class DomSerializerHandCrafted : DomSerializer
  {
    public virtual string serialize(Element domElement)
    {
      StringBuffer buf = new StringBuffer();
      this.serializeToBuffer(domElement, buf);
      return buf.ToString();
    }

    private void serializeToBuffer(Element el, StringBuffer buf)
    {
      buf.append("<").append(el.getTagName());
      NamedNodeMap attributes = el.getAttributes();
      for (int index = 0; index < attributes.getLength(); ++index)
      {
        Node node = attributes.item(index);
        if (node.getNodeType() == (short) 2)
        {
          Attr attr = (Attr) node;
          buf.append(" ").append(attr.getName()).append("=\"").append(attr.getValue()).append("\"");
        }
      }
      buf.append(">");
      NodeList childNodes = el.getChildNodes();
      for (int index = 0; index < childNodes.getLength(); ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 3)
        {
          Text text = (Text) node;
          buf.append(text.getData());
        }
        else if (node.getNodeType() == (short) 1)
          this.serializeToBuffer((Element) node, buf);
      }
      buf.append("</").append(el.getTagName()).append(">");
    }

    public virtual void serializeTo(Element domElement, OutputStream os)
    {
      StringBuffer buf = new StringBuffer();
      this.serializeToBuffer(domElement, buf);
      PrintStream printStream = new PrintStream(os);
      printStream.println(buf.ToString());
      printStream.flush();
    }

    public virtual void serializeTo(Element domElement, Writer w)
    {
      StringBuffer buf = new StringBuffer();
      this.serializeToBuffer(domElement, buf);
      PrintWriter printWriter = new PrintWriter(w);
      printWriter.println(buf.ToString());
      printWriter.flush();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DomSerializerHandCrafted serializerHandCrafted = this;
      ObjectImpl.clone((object) serializerHandCrafted);
      return ((object) serializerHandCrafted).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
