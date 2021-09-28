// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.PINode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaFlags(48)]
  [JavaInterfaces("1;org/w3c/dom/ProcessingInstruction;")]
  public sealed class PINode : NodeBase, ProcessingInstruction
  {
    private string target;
    private char[] data;

    public PINode()
    {
    }

    public PINode(string target, string text)
    {
      this.data = StringImpl.toCharArray(text);
      this.target = target;
    }

    [JavaFlags(0)]
    public PINode(string target, char[] buf, int offset, int len)
    {
      int length = len;
      this.data = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      System.arraycopy((object) buf, offset, (object) this.data, 0, len);
      this.target = target;
    }

    public override short getNodeType() => 7;

    public virtual string getTarget() => this.target;

    public virtual void setTarget(string target) => this.target = target;

    public virtual string getData() => StringImpl.createString(this.data);

    public virtual void setData(string data)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      this.data = StringImpl.toCharArray(data);
    }

    public override string getNodeValue() => this.getData();

    public override void setNodeValue(string data) => this.setData(data);

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      writer.write("<?");
      writer.write(this.target);
      if (this.data != null)
      {
        writer.write(32);
        writer.write(this.data);
      }
      writer.write("?>");
    }

    public override Node cloneNode(bool deep)
    {
      PINode piNode = new PINode(this.target, this.data, 0, this.data.Length);
      piNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      return (Node) piNode;
    }

    public override string getNodeName() => this.target;
  }
}
