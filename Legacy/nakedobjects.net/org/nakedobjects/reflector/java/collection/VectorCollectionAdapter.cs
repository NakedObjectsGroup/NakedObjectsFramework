// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.collection.VectorCollectionAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.reflector.java.collection
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalCollection;")]
  public class VectorCollectionAdapter : AbstractNakedReference, InternalCollection
  {
    private readonly Vector collection;
    private readonly NakedObjectSpecification elementSpecification;

    public VectorCollectionAdapter(Vector vector, NakedObjectSpecification spec)
    {
      Assert.assertNotNull("Vector", (object) vector);
      Assert.assertNotNull("Element specification", (object) spec);
      this.collection = vector;
      this.elementSpecification = spec;
    }

    private NakedObject adapter(object element) => NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(element);

    public virtual bool contains(NakedObject @object) => this.collection.contains(@object.getObject());

    public override void destroyed()
    {
    }

    public virtual NakedObject elementAt(int index) => this.adapter(this.collection.elementAt(index));

    public virtual Enumeration elements() => (Enumeration) new VectorCollectionAdapter.\u0031(this, this.collection.elements());

    public virtual NakedObjectSpecification getElementSpecification() => this.elementSpecification;

    public override object getObject() => (object) this.collection;

    public virtual void init(object[] initElements)
    {
      Assert.assertEquals("Collection not empty", 0, this.collection.size());
      for (int index = 0; index < initElements.Length; ++index)
        this.collection.addElement(initElements[index]);
    }

    public virtual bool isAggregated() => false;

    public virtual NakedObject parent() => (NakedObject) null;

    public override Persistable persistable() => Persistable.TRANSIENT;

    public virtual int size() => this.collection.size();

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
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private VectorCollectionAdapter this\u00240;
      [JavaFlags(16)]
      public readonly Enumeration elements_\u003E;

      public virtual bool hasMoreElements() => this.elements_\u003E.hasMoreElements();

      public virtual object nextElement() => (object) this.this\u00240.adapter(this.elements_\u003E.nextElement());

      public \u0031(VectorCollectionAdapter _param1, [In] Enumeration obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.elements_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        VectorCollectionAdapter.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
