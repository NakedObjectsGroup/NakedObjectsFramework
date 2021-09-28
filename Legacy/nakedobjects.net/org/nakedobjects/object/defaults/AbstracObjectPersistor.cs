// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.AbstracObjectPersistor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectPersistor;")]
  public abstract class AbstracObjectPersistor : NakedObjectPersistor
  {
    public abstract void abortTransaction();

    public virtual TypedNakedCollection allInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      NakedObject[] instances = this.getInstances(specification, includeSubclasses);
      return (TypedNakedCollection) new InstanceCollectionVector(specification, instances);
    }

    public virtual NakedObject createPersistentInstance(
      NakedObjectSpecification specification)
    {
      NakedObject transientInstance = this.createTransientInstance(specification);
      this.makePersistent(transientInstance);
      return transientInstance;
    }

    public virtual NakedObject createPersistentInstance(string className) => this.createPersistentInstance(NakedObjects.getSpecificationLoader().loadSpecification(className));

    public virtual NakedObject createTransientInstance(
      NakedObjectSpecification specification)
    {
      return NakedObjects.getObjectLoader().createTransientInstance(specification);
    }

    public virtual NakedObject createTransientInstance(string className) => this.createTransientInstance(NakedObjects.getSpecificationLoader().loadSpecification(className));

    [JavaThrownExceptions("1;org/nakedobjects/object/UnsupportedFindException;")]
    public virtual TypedNakedCollection findInstances(InstancesCriteria criteria)
    {
      NakedObject[] instances = criteria != null ? this.getInstances(criteria) : throw new NullPointerException();
      return (TypedNakedCollection) new InstanceCollectionVector(criteria.getSpecification(), instances);
    }

    [JavaFlags(1028)]
    public abstract NakedObject[] getInstances(
      NakedObjectSpecification cls,
      bool includeSubclasses);

    [JavaFlags(1028)]
    public abstract NakedObject[] getInstances(InstancesCriteria criteria);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstracObjectPersistor abstracObjectPersistor = this;
      ObjectImpl.clone((object) abstracObjectPersistor);
      return ((object) abstracObjectPersistor).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void addObjectChangedListener(DirtyObjectSet listener);

    public abstract void debugData(DebugString debug);

    public abstract void destroyObject(NakedObject @object);

    public abstract void endTransaction();

    public abstract string getDebugTitle();

    public abstract NakedClass getNakedClass(NakedObjectSpecification specification);

    public abstract NakedObject getObject(Oid oid, NakedObjectSpecification spec);

    public abstract bool hasInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses);

    public abstract void init();

    public abstract void makePersistent(NakedObject @object);

    public abstract int numberOfInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses);

    public abstract void objectChanged(NakedObject @object);

    public abstract void refresh(NakedReference root, Hashtable nonRefreshOids);

    public abstract void reload(NakedObject @object);

    public abstract void reset();

    public abstract void resolveField(NakedObject @object, NakedObjectField field);

    public abstract void resolveImmediately(NakedObject @object);

    public abstract void saveChanges();

    public abstract void shutdown();

    public abstract void startTransaction();
  }
}
