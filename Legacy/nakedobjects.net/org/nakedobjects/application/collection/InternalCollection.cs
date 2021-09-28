// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.collection.InternalCollection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using junit.framework;

namespace org.nakedobjects.application.collection
{
  public class InternalCollection
  {
    private Vector elements;
    private string type;

    public InternalCollection(string type)
    {
      this.elements = new Vector();
      this.type = type;
    }

    public virtual void add(object @object)
    {
      if (@object == null)
        throw new NullPointerException("Cannot add null");
      this.elements.addElement(@object);
    }

    public virtual bool contains(object @object) => @object != null ? this.elements.contains(@object) : throw new IllegalArgumentException("null is not a valid element for a collection");

    public virtual object elementAt(int index) => this.elements.elementAt(index);

    public virtual Enumeration elements() => this.elements.elements();

    public virtual string getType() => this.type;

    public virtual bool isEmpty() => this.size() == 0;

    public virtual void init(object[] initElements)
    {
      Assert.assertEquals("Collection not empty", 0, this.elements.size());
      for (int index = 0; index < initElements.Length; ++index)
        this.elements.addElement(initElements[index]);
    }

    public virtual void remove(object @object)
    {
      if (@object == null)
        throw new NullPointerException("Cannot remove null");
      this.elements.removeElement(@object);
    }

    public virtual void removeAllElements() => this.elements.removeAllElements();

    public virtual int size() => this.elements.size();

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append("InternalCollectionVector");
      stringBuffer.append(" [");
      stringBuffer.append(",size=");
      stringBuffer.append(this.size());
      stringBuffer.append("]");
      stringBuffer.append(new StringBuffer().append("  ").append(StringImpl.toUpperCase(Long.toHexString((long) this.GetHashCode()))).ToString());
      return stringBuffer.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InternalCollection internalCollection = this;
      ObjectImpl.clone((object) internalCollection);
      return ((object) internalCollection).MemberwiseClone();
    }
  }
}
