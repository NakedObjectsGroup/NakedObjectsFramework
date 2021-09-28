// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.TextNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/Text;")]
  public class TextNode : DataNode, Text
  {
    public TextNode()
    {
    }

    public TextNode(char[] buf, int offset, int len)
      : base(buf, offset, len)
    {
    }

    public TextNode(string s)
      : base(s)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      int num = 0;
      int index = 0;
      if (this.data == null)
      {
        ((PrintStream) System.err).println("Null text data??");
      }
      else
      {
        for (; index < this.data.Length; ++index)
        {
          switch (this.data[index])
          {
            case '&':
              writer.write(this.data, num, index - num);
              num = index + 1;
              writer.write("&amp;");
              break;
            case '<':
              writer.write(this.data, num, index - num);
              num = index + 1;
              writer.write("&lt;");
              break;
            case '>':
              writer.write(this.data, num, index - num);
              num = index + 1;
              writer.write("&gt;");
              break;
          }
        }
        writer.write(this.data, num, index - num);
      }
    }

    public virtual void joinNextText()
    {
      Node nextSibling = this.getNextSibling();
      if (nextSibling == null || nextSibling.getNodeType() != (short) 3)
        return;
      this.getParentNode().removeChild(nextSibling);
      char[] text = ((DataNode) nextSibling).getText();
      int length = this.data.Length + text.Length;
      char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      System.arraycopy((object) this.data, 0, (object) chArray, 0, this.data.Length);
      System.arraycopy((object) text, 0, (object) chArray, this.data.Length, text.Length);
      this.data = chArray;
    }

    public override short getNodeType() => 3;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Text splitText(int offset)
    {
      // ISSUE: unable to decompile the method.
    }

    public override Node cloneNode(bool deep)
    {
      TextNode textNode = new TextNode(this.data, 0, this.data.Length);
      textNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      return (Node) textNode;
    }

    public override string getNodeName() => "#text";
  }
}
