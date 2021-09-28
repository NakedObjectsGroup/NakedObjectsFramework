// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.AttributesImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("1;org/xml/sax/Attributes;")]
  public class AttributesImpl : Attributes
  {
    [JavaFlags(0)]
    public int length;
    [JavaFlags(0)]
    public string[] data;

    public AttributesImpl()
    {
      this.length = 0;
      this.data = (string[]) null;
    }

    public AttributesImpl(Attributes atts) => this.setAttributes(atts);

    public virtual int getLength() => this.length;

    public virtual string getURI(int index) => index >= 0 && index < this.length ? this.data[index * 5] : (string) null;

    public virtual string getLocalName(int index) => index >= 0 && index < this.length ? this.data[index * 5 + 1] : (string) null;

    public virtual string getQName(int index) => index >= 0 && index < this.length ? this.data[index * 5 + 2] : (string) null;

    public virtual string getType(int index) => index >= 0 && index < this.length ? this.data[index * 5 + 3] : (string) null;

    public virtual string getValue(int index) => index >= 0 && index < this.length ? this.data[index * 5 + 4] : (string) null;

    public virtual int getIndex(string uri, string localName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index], (object) uri) && StringImpl.equals(this.data[index + 1], (object) localName))
          return index / 5;
      }
      return -1;
    }

    public virtual int getIndex(string qName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index + 2], (object) qName))
          return index / 5;
      }
      return -1;
    }

    public virtual string getType(string uri, string localName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index], (object) uri) && StringImpl.equals(this.data[index + 1], (object) localName))
          return this.data[index + 3];
      }
      return (string) null;
    }

    public virtual string getType(string qName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index + 2], (object) qName))
          return this.data[index + 3];
      }
      return (string) null;
    }

    public virtual string getValue(string uri, string localName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index], (object) uri) && StringImpl.equals(this.data[index + 1], (object) localName))
          return this.data[index + 4];
      }
      return (string) null;
    }

    public virtual string getValue(string qName)
    {
      int num = this.length * 5;
      for (int index = 0; index < num; index += 5)
      {
        if (StringImpl.equals(this.data[index + 2], (object) qName))
          return this.data[index + 4];
      }
      return (string) null;
    }

    public virtual void clear() => this.length = 0;

    public virtual void setAttributes(Attributes atts)
    {
      this.clear();
      this.length = atts.getLength();
      if (this.length <= 0)
        return;
      int length = this.length * 5;
      this.data = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < this.length; ++index)
      {
        this.data[index * 5] = atts.getURI(index);
        this.data[index * 5 + 1] = atts.getLocalName(index);
        this.data[index * 5 + 2] = atts.getQName(index);
        this.data[index * 5 + 3] = atts.getType(index);
        this.data[index * 5 + 4] = atts.getValue(index);
      }
    }

    public virtual void addAttribute(
      string uri,
      string localName,
      string qName,
      string type,
      string value)
    {
      this.ensureCapacity(this.length + 1);
      this.data[this.length * 5] = uri;
      this.data[this.length * 5 + 1] = localName;
      this.data[this.length * 5 + 2] = qName;
      this.data[this.length * 5 + 3] = type;
      this.data[this.length * 5 + 4] = value;
      ++this.length;
    }

    public virtual void setAttribute(
      int index,
      string uri,
      string localName,
      string qName,
      string type,
      string value)
    {
      if (index >= 0 && index < this.length)
      {
        this.data[index * 5] = uri;
        this.data[index * 5 + 1] = localName;
        this.data[index * 5 + 2] = qName;
        this.data[index * 5 + 3] = type;
        this.data[index * 5 + 4] = value;
      }
      else
        this.badIndex(index);
    }

    public virtual void removeAttribute(int index)
    {
      if (index >= 0 && index < this.length)
      {
        this.data[index * 5] = (string) null;
        this.data[index * 5 + 1] = (string) null;
        this.data[index * 5 + 2] = (string) null;
        this.data[index * 5 + 3] = (string) null;
        this.data[index * 5 + 4] = (string) null;
        if (index < this.length - 1)
          System.arraycopy((object) this.data, (index + 1) * 5, (object) this.data, index * 5, (this.length - index - 1) * 5);
        this.length += -1;
      }
      else
        this.badIndex(index);
    }

    public virtual void setURI(int index, string uri)
    {
      if (index >= 0 && index < this.length)
        this.data[index * 5] = uri;
      else
        this.badIndex(index);
    }

    public virtual void setLocalName(int index, string localName)
    {
      if (index >= 0 && index < this.length)
        this.data[index * 5 + 1] = localName;
      else
        this.badIndex(index);
    }

    public virtual void setQName(int index, string qName)
    {
      if (index >= 0 && index < this.length)
        this.data[index * 5 + 2] = qName;
      else
        this.badIndex(index);
    }

    public virtual void setType(int index, string type)
    {
      if (index >= 0 && index < this.length)
        this.data[index * 5 + 3] = type;
      else
        this.badIndex(index);
    }

    public virtual void setValue(int index, string value)
    {
      if (index >= 0 && index < this.length)
        this.data[index * 5 + 4] = value;
      else
        this.badIndex(index);
    }

    private void ensureCapacity(int n)
    {
      if (n <= 0)
        return;
      int num;
      if (this.data == null || this.data.Length == 0)
      {
        num = 25;
      }
      else
      {
        if (this.data.Length >= n * 5)
          return;
        num = this.data.Length;
      }
      while (num < n * 5)
        num *= 2;
      int length = num;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      if (this.length > 0)
        System.arraycopy((object) this.data, 0, (object) strArray, 0, this.length * 5);
      this.data = strArray;
    }

    [JavaThrownExceptions("1;java/lang/ArrayIndexOutOfBoundsException;")]
    private void badIndex(int index) => throw new ArrayIndexOutOfBoundsException(new StringBuffer().append("Attempt to modify attribute at illegal index: ").append(index).ToString());

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AttributesImpl attributesImpl = this;
      ObjectImpl.clone((object) attributesImpl);
      return ((object) attributesImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
