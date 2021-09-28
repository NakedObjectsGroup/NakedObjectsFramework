// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalCollectionVectorAdapter
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
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalCollection;")]
  public class InternalCollectionVectorAdapter : AbstractNakedReference, InternalCollection
  {
    private Vector collection;
    private NakedObjectSpecification elementSpecification;

    public InternalCollectionVectorAdapter(Vector vector, Class type)
    {
      this.collection = vector;
      Class cls = type != null ? type : Class.FromType(typeof (object));
      this.elementSpecification = NakedObjects.getSpecificationLoader().loadSpecification(cls);
    }

    public virtual bool contains(NakedObject @object) => this.collection.contains(@object.getObject());

    public override void destroyed()
    {
    }

    public virtual NakedObject elementAt(int index)
    {
      object @object = this.collection.elementAt(index);
      return NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);
    }

    public virtual Enumeration elements() => (Enumeration) new InternalCollectionVectorAdapter.\u0031(this, this.collection.elements());

    public virtual NakedObjectSpecification getElementSpecification() => this.elementSpecification;

    public override string getIconName() => (string) null;

    public override object getObject() => (object) this.collection;

    public virtual void init(object[] initElements)
    {
      Assert.assertEquals("Collection not empty", 0, this.collection.size());
      for (int index = 0; index < initElements.Length; ++index)
        this.collection.addElement(initElements[index]);
    }

    public virtual bool isAggregated() => false;

    public virtual Enumeration oids() => throw new NotImplementedException();

    public virtual NakedObject parent() => (NakedObject) null;

    public override void setOid(Oid oid)
    {
    }

    public virtual void setResolved()
    {
    }

    public virtual int size() => this.collection.size();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("elements", this.elementSpecification.getFullName());
      return toString.ToString();
    }

    public override string titleString() => "Vector";

    [JavaInterfaces("1;java/util/Enumeration;")]
    [Inner]
    [JavaFlags(32)]
    public class \u0031 : Enumeration
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private InternalCollectionVectorAdapter this\u00240;
      [JavaFlags(16)]
      public readonly Enumeration elements_\u003E;

      public virtual bool hasMoreElements() => this.elements_\u003E.hasMoreElements();

      public virtual object nextElement()
      {
        object @object = this.elements_\u003E.nextElement();
        return @object is NakedObject ? @object : (object) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);
      }

      public \u0031(InternalCollectionVectorAdapter _param1, [In] Enumeration obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.elements_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        InternalCollectionVectorAdapter.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
