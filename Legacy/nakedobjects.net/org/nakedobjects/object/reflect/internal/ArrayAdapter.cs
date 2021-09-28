// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.ArrayAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.reflect.@internal;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalCollection;")]
  public class ArrayAdapter : AbstractNakedReference, InternalCollection
  {
    private object[] array;
    private NakedObjectSpecification elementSpecification;

    public ArrayAdapter(object[] array, NakedObjectSpecification spec)
    {
      this.array = array;
      this.elementSpecification = spec;
    }

    private static NakedObject adapter(object element) => NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(element);

    public virtual bool contains(NakedObject @object)
    {
      for (int index = 0; index < this.array.Length; ++index)
      {
        if (this.array[index] == @object)
          return true;
      }
      return false;
    }

    public override void destroyed()
    {
    }

    public virtual NakedObject elementAt(int index) => ArrayAdapter.adapter(this.array[index]);

    public virtual Enumeration elements() => (Enumeration) new ArrayAdapter.\u0031(this);

    public virtual NakedObjectSpecification getElementSpecification() => this.elementSpecification;

    public override object getObject() => (object) this.array;

    public virtual bool isAggregated() => false;

    public virtual void init(object[] elements)
    {
      int length1 = elements.Length;
      int length2 = length1;
      this.array = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
      for (int index = 0; index < length1; ++index)
        this.array[index] = elements[index];
    }

    public virtual NakedObject parent() => (NakedObject) null;

    public override Persistable persistable() => Persistable.TRANSIENT;

    public virtual int size() => this.array.Length;

    public override string titleString() => "Vector";

    public override string ToString()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaInterfaces("1;java/util/Enumeration;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : Enumeration
    {
      [JavaFlags(0)]
      public int count;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ArrayAdapter this\u00240;

      public virtual bool hasMoreElements() => this.count < this.this\u00240.array.Length;

      public virtual object nextElement()
      {
        object[] array = this.this\u00240.array;
        int count;
        this.count = (count = this.count) + 1;
        int index = count;
        return (object) ArrayAdapter.adapter(array[index]);
      }

      public \u0031(ArrayAdapter _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.count = 0;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ArrayAdapter.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
