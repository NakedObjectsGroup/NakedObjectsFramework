// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.CDataNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/CDATASection;")]
  [JavaFlags(32)]
  public class CDataNode : TextNode, CDATASection
  {
    public CDataNode()
    {
    }

    public CDataNode(char[] buf, int offset, int len)
      : base(buf, offset, len)
    {
    }

    public CDataNode(string s)
      : base(s)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      writer.write("<![CDATA[");
      for (int index = 0; index < this.data.Length; ++index)
      {
        char ch = this.data[index];
        if (ch == ']' && index + 2 < this.data.Length && this.data[index + 1] == ']' && this.data[index + 2] == '>')
          writer.write("]]]><![CDATA[");
        else
          writer.write((int) ch);
      }
      writer.write("]]>");
    }

    public override short getNodeType() => 4;

    public override Node cloneNode(bool deep)
    {
      CDataNode cdataNode = new CDataNode(this.data, 0, this.data.Length);
      cdataNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      return (Node) cdataNode;
    }

    public override string getNodeName() => "#cdata-section";
  }
}
