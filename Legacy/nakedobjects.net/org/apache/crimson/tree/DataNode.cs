// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.DataNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.w3c.dom;
using System.ComponentModel;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/CharacterData;")]
  [JavaFlags(1056)]
  public abstract class DataNode : NodeBase, CharacterData
  {
    [JavaFlags(0)]
    public char[] data;
    [JavaFlags(8)]
    public static DataNode.NodeListImpl childNodes;

    [JavaFlags(0)]
    public DataNode()
    {
    }

    [JavaFlags(0)]
    public DataNode(char[] buf, int offset, int len)
    {
      int length = len;
      this.data = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) buf, offset, (object) this.data, 0, len);
    }

    [JavaFlags(0)]
    public DataNode(string s)
    {
      if (s != null)
      {
        int length = StringImpl.length(s);
        this.data = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
        StringImpl.getChars(s, 0, this.data.Length, this.data, 0);
      }
      else
      {
        int length = 0;
        this.data = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      }
    }

    public virtual char[] getText() => this.data;

    public virtual void setText(char[] buf) => this.data = buf;

    public override string ToString() => this.data != null ? StringImpl.createString(this.data) : (string) null;

    public virtual string getData() => this.ToString();

    public virtual void setData(string data)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      if (data == null)
      {
        int length = 0;
        if (length < 0)
          throw new NegativeArraySizeException();
        this.setText(new char[length]);
      }
      else
        this.setText(StringImpl.toCharArray(data));
    }

    public override int getLength() => this.data == null ? 0 : this.data.Length;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual string substringData(int offset, int count)
    {
      if (offset < 0 || offset > this.data.Length || count < 0)
        throw new DomEx((short) 1);
      count = Math.min(count, this.data.Length - offset);
      return StringImpl.createString(this.data, offset, count);
    }

    public virtual void appendData(string newData)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      int num = StringImpl.length(newData);
      int length = num + this.data.Length;
      char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) this.data, 0, (object) chArray, 0, this.data.Length);
      StringImpl.getChars(newData, 0, num, chArray, this.data.Length);
      this.data = chArray;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void insertData(int offset, string newData)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      if (offset < 0 || offset > this.data.Length)
        throw new DomEx((short) 1);
      int num = StringImpl.length(newData);
      int length = num + this.data.Length;
      char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) this.data, 0, (object) chArray, 0, offset);
      StringImpl.getChars(newData, 0, num, chArray, offset);
      java.lang.System.arraycopy((object) this.data, offset, (object) chArray, offset + num, this.data.Length - offset);
      this.data = chArray;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void deleteData(int offset, int count)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      if (offset < 0 || offset >= this.data.Length || count < 0)
        throw new DomEx((short) 1);
      count = Math.min(count, this.data.Length - offset);
      int length = this.data.Length - count;
      char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) this.data, 0, (object) chArray, 0, offset);
      java.lang.System.arraycopy((object) this.data, offset + count, (object) chArray, offset, chArray.Length - offset);
      this.data = chArray;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void replaceData(int offset, int count, string arg)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      if (offset < 0 || offset >= this.data.Length || count < 0)
        throw new DomEx((short) 1);
      if (offset + count >= this.data.Length)
      {
        this.deleteData(offset, count);
        this.appendData(arg);
      }
      else if (StringImpl.length(arg) == count)
      {
        StringImpl.getChars(arg, 0, StringImpl.length(arg), this.data, offset);
      }
      else
      {
        int length = this.data.Length + (StringImpl.length(arg) - count);
        char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) this.data, 0, (object) chArray, 0, offset);
        StringImpl.getChars(arg, 0, StringImpl.length(arg), chArray, offset);
        java.lang.System.arraycopy((object) this.data, offset + count, (object) chArray, offset + StringImpl.length(arg), this.data.Length - (offset + count));
        this.data = chArray;
      }
    }

    public override NodeList getChildNodes() => (NodeList) DataNode.childNodes;

    public override string getNodeValue() => this.getData();

    public override void setNodeValue(string value) => this.setData(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DataNode()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(56)]
    [JavaInterfaces("1;org/w3c/dom/NodeList;")]
    public sealed class NodeListImpl : NodeList
    {
      public virtual Node item(int i) => (Node) null;

      public virtual int getLength() => 0;

      [JavaFlags(0)]
      public NodeListImpl()
      {
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        DataNode.NodeListImpl nodeListImpl = this;
        ObjectImpl.clone((object) nodeListImpl);
        return ((object) nodeListImpl).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
