// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestClassImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestClass;")]
  public class TestClassImpl : AbstractTestObject, TestClass
  {
    private NakedClass nakedClass;

    public TestClassImpl(NakedClass cls, TestObjectFactory factory)
      : base(factory)
    {
      this.nakedClass = cls;
    }

    public virtual TestObject findInstance(string title)
    {
      NakedObjectSpecification specification = ((NakedClass) this.getForNaked().getObject()).forObjectType();
      TypedNakedCollection instances = NakedObjects.getObjectPersistor().findInstances((InstancesCriteria) new TitleCriteria(specification, title, true));
      return instances.size() != 0 ? this.factory.createTestObject(instances.elementAt(0)) : throw new IllegalActionError(new StringBuffer().append("No instance found with title ").append(title).ToString());
    }

    [JavaFlags(17)]
    public override sealed Naked getForNaked() => (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient((object) this.nakedClass);

    public override string getTitle() => this.nakedClass.getSingularName();

    public virtual TestCollection instances()
    {
      NakedCollection instances = (NakedCollection) NakedObjects.getObjectPersistor().allInstances(this.nakedClass.forObjectType(), false);
      return instances.size() != 0 ? this.factory.createTestCollection(instances) : throw new IllegalActionError("Find must find at least one object");
    }

    public virtual TestObject newInstance() => this.factory.createTestObject(this.newInstance(this.nakedClass));

    private NakedObject newInstance(NakedClass cls)
    {
      NakedObjectPersistor objectPersistor = NakedObjects.getObjectPersistor();
      NakedObject @object;
      try
      {
        @object = objectPersistor.createTransientInstance(cls.forObjectType());
        objectPersistor.startTransaction();
        objectPersistor.makePersistent(@object);
        objectPersistor.saveChanges();
        objectPersistor.endTransaction();
      }
      catch (NotPersistableException ex)
      {
        NakedError nakedError = (NakedError) new Error(new StringBuffer().append("Failed to create instance of ").append(cls.forObjectType().getFullName()).ToString(), (Throwable) ex);
        @object = NakedObjects.getObjectLoader().createAdapterForTransient((object) nakedError);
        ((PrintStream) System.@out).println(new StringBuffer().append("Failed to create instance of ").append(cls.forObjectType().getFullName()).ToString());
        ((Throwable) ex).printStackTrace();
      }
      return @object;
    }

    public override void setForNaked(Naked @object) => throw new NakedObjectRuntimeException();

    [JavaFlags(4)]
    public override Action getAction(string name, NakedObjectSpecification[] parameterClasses) => this.nakedClass.forObjectType().getClassAction(Action.USER, this.simpleName(name), parameterClasses);
  }
}
