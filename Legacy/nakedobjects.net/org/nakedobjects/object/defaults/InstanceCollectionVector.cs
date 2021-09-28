// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.InstanceCollectionVector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("2;org/nakedobjects/object/TypedNakedCollection;org/nakedobjects/object/InternalNakedObject;")]
  public class InstanceCollectionVector : 
    AbstractNakedReference,
    TypedNakedCollection,
    InternalNakedObject
  {
    private string name;
    private Vector instances;
    private NakedObjectSpecification instanceSpecification;

    public InstanceCollectionVector(
      NakedObjectSpecification elementSpecification,
      NakedObject[] instances)
    {
      this.instanceSpecification = elementSpecification;
      this.name = elementSpecification.getPluralName();
      int length = instances.Length;
      this.instances = new Vector(length);
      for (int index = 0; index < length; ++index)
        this.instances.addElement((object) instances[index]);
    }

    public InstanceCollectionVector(NakedObjectSpecification elementSpecification, Vector instances)
    {
      this.instanceSpecification = elementSpecification;
      this.name = elementSpecification.getPluralName();
      int num = instances.size();
      this.instances = new Vector(num);
      for (int index = 0; index < num; ++index)
        this.instances.addElement(instances.elementAt(index));
    }

    public virtual NakedObject elementAt(int i) => i >= 0 && i < this.size() ? (NakedObject) this.instances.elementAt(i) : throw new IllegalArgumentException(new StringBuffer().append("No such element: ").append(i).ToString());

    public override string titleString() => new StringBuffer().append(this.name).append(", ").append(this.size()).ToString();

    public virtual int size() => this.instances.size();

    public override void setOid(Oid oid)
    {
    }

    public virtual NakedObjectSpecification getElementSpecification() => this.instanceSpecification;

    public virtual bool contains(NakedObject @object) => false;

    public override void destroyed()
    {
    }

    public virtual Enumeration elements() => this.instances.elements();

    public override string getIconName() => (string) null;

    public override object getObject() => (object) this.instances;

    public virtual void init(object[] initElements)
    {
      Assert.assertEquals("Collection not empty", 0, this.instances.size());
      for (int index = 0; index < initElements.Length; ++index)
        this.instances.addElement(initElements[index]);
    }

    public override Persistable persistable() => Persistable.TRANSIENT;

    public override string ToString()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
