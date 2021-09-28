// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.collection.InternalCollectionAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.application.collection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.reflector.java.collection
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalCollection;")]
  public class InternalCollectionAdapter : AbstractNakedReference, InternalCollection
  {
    private InternalCollection collection;
    private NakedObjectSpecification elementSpecification;

    public InternalCollectionAdapter(InternalCollection vector, NakedObjectSpecification spec)
    {
      this.collection = vector;
      this.elementSpecification = spec;
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

    public virtual Enumeration elements() => (Enumeration) new InternalCollectionAdapter.\u0031(this, this.collection.elements());

    public virtual NakedObjectSpecification getElementSpecification() => this.elementSpecification == null ? NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (object))) : this.elementSpecification;

    public override object getObject() => (object) this.collection;

    public virtual bool isAggregated() => false;

    public virtual void init(object[] elements) => this.collection.init(elements);

    public virtual NakedObject parent() => (NakedObject) null;

    public override Persistable persistable() => Persistable.TRANSIENT;

    public virtual int size() => this.collection.size();

    public override string titleString() => "vector...";

    public override string ToString()
    {
      // ISSUE: unable to decompile the method.
    }

    [Inner]
    [JavaInterfaces("1;java/util/Enumeration;")]
    [JavaFlags(32)]
    public class \u0031 : Enumeration
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private InternalCollectionAdapter this\u00240;
      [JavaFlags(16)]
      public readonly Enumeration elements_\u003E;

      public virtual bool hasMoreElements() => this.elements_\u003E.hasMoreElements();

      public virtual object nextElement()
      {
        object @object = this.elements_\u003E.nextElement();
        return @object is NakedObject ? @object : (object) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);
      }

      public \u0031(InternalCollectionAdapter _param1, [In] Enumeration obj1)
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
        InternalCollectionAdapter.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
