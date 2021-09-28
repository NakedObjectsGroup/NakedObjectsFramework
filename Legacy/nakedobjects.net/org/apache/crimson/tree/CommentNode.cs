// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.CommentNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/Comment;")]
  public class CommentNode : DataNode, Comment
  {
    public CommentNode()
    {
    }

    public CommentNode(string data)
      : base(data)
    {
    }

    [JavaFlags(0)]
    public CommentNode(char[] buf, int offset, int len)
      : base(buf, offset, len)
    {
    }

    public override short getNodeType() => 8;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      writer.write("<!--");
      if (this.data != null)
      {
        bool flag = false;
        int length = this.data.Length;
        for (int index = 0; index < length; ++index)
        {
          if (this.data[index] == '-')
          {
            if (flag)
            {
              writer.write(32);
            }
            else
            {
              flag = true;
              writer.write(45);
              continue;
            }
          }
          flag = false;
          writer.write((int) this.data[index]);
        }
        if (this.data[this.data.Length - 1] == '-')
          writer.write(32);
      }
      writer.write("-->");
    }

    public override Node cloneNode(bool deep)
    {
      CommentNode commentNode = new CommentNode(this.data, 0, this.data.Length);
      commentNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      return (Node) commentNode;
    }

    public override string getNodeName() => "#comment";
  }
}
