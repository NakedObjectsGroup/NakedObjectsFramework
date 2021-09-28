// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.ObjectPersistorLogger
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectPersistor;")]
  public class ObjectPersistorLogger : Logger, NakedObjectPersistor
  {
    private readonly NakedObjectPersistor decorated;

    public ObjectPersistorLogger(NakedObjectPersistor decorated, string logFileName)
      : base(logFileName, false)
    {
      this.decorated = decorated;
    }

    public ObjectPersistorLogger(NakedObjectPersistor decorated)
      : base((string) null, true)
    {
      this.decorated = decorated;
    }

    public ObjectPersistorLogger(NakedObjectPersistor decorated, bool logAlso)
      : base((string) null, logAlso)
    {
      this.decorated = decorated;
    }

    public virtual void abortTransaction()
    {
      this.log("Abort transaction");
      this.decorated.abortTransaction();
    }

    public virtual void addObjectChangedListener(DirtyObjectSet listener)
    {
      this.log(new StringBuffer().append("Adding object changed listener ").append((object) listener).ToString());
      this.decorated.addObjectChangedListener(listener);
    }

    public virtual TypedNakedCollection allInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      this.log(new StringBuffer().append("All instances of ").append(specification.getShortName()).append(!includeSubclasses ? "" : " including subclasses").ToString());
      return this.decorated.allInstances(specification, includeSubclasses);
    }

    public virtual NakedObject createPersistentInstance(
      NakedObjectSpecification specification)
    {
      NakedObject persistentInstance = this.decorated.createPersistentInstance(specification);
      this.log(new StringBuffer().append("Create an instances of ").append(specification.getShortName()).ToString(), persistentInstance.getObject());
      return persistentInstance;
    }

    public virtual NakedObject createPersistentInstance(string className)
    {
      NakedObject persistentInstance = this.decorated.createPersistentInstance(className);
      this.log(new StringBuffer().append("Create an instances of ").append(className).ToString(), persistentInstance.getObject());
      return persistentInstance;
    }

    public virtual NakedObject createTransientInstance(
      NakedObjectSpecification specification)
    {
      NakedObject transientInstance = this.decorated.createTransientInstance(specification);
      this.log(new StringBuffer().append("Create a transient instances of ").append(specification.getShortName()).ToString(), transientInstance.getObject());
      return transientInstance;
    }

    public virtual NakedObject createTransientInstance(string className)
    {
      NakedObject transientInstance = this.decorated.createTransientInstance(className);
      this.log(new StringBuffer().append("Create a transient instances of ").append(className).ToString(), transientInstance.getObject());
      return transientInstance;
    }

    public virtual void destroyObject(NakedObject @object)
    {
      this.log(new StringBuffer().append("Destroy ").append(@object.getObject()).ToString());
      this.decorated.destroyObject(@object);
    }

    public virtual void endTransaction()
    {
      this.log("End transaction");
      this.decorated.endTransaction();
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/UnsupportedFindException;")]
    public virtual TypedNakedCollection findInstances(InstancesCriteria criteria)
    {
      this.log(new StringBuffer().append("Find instances matching ").append((object) criteria).ToString());
      return this.decorated.findInstances(criteria);
    }

    public virtual void debugData(DebugString debug) => this.decorated.debugData(debug);

    public virtual string getDebugTitle() => this.decorated.getDebugTitle();

    [JavaFlags(4)]
    public override Class getDecoratedClass() => ObjectImpl.getClass((object) this.decorated);

    public virtual NakedClass getNakedClass(NakedObjectSpecification specification)
    {
      NakedClass nakedClass = this.decorated.getNakedClass(specification);
      this.log(new StringBuffer().append("Get class ").append(specification.getShortName()).ToString(), (object) nakedClass);
      return nakedClass;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectNotFoundException;")]
    public virtual NakedObject getObject(Oid oid, NakedObjectSpecification hint)
    {
      NakedObject nakedObject = this.decorated.getObject(oid, hint);
      this.log(new StringBuffer().append("Get object for ").append((object) oid).append(" (of type ").append(hint.getShortName()).append(")").ToString(), nakedObject.getObject());
      return nakedObject;
    }

    public virtual bool hasInstances(NakedObjectSpecification specification, bool includeSubclasses)
    {
      bool flag = this.decorated.hasInstances(specification, false);
      this.log(new StringBuffer().append("Has instances of ").append(specification.getShortName()).ToString(), (object) new StringBuffer().append("").append(flag).ToString());
      return flag;
    }

    public virtual void init()
    {
      this.log(new StringBuffer().append("Initialising ").append((object) this.decorated).ToString());
      this.decorated.init();
    }

    public virtual void makePersistent(NakedObject @object)
    {
      this.log(new StringBuffer().append("Make object graph persistent: ").append((object) @object).ToString());
      this.decorated.makePersistent(@object);
    }

    public virtual int numberOfInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      int num = this.decorated.numberOfInstances(specification, false);
      this.log(new StringBuffer().append("Number of instances of ").append(specification.getShortName()).ToString(), (object) new StringBuffer().append("").append(num).ToString());
      return num;
    }

    public virtual void objectChanged(NakedObject @object)
    {
      this.log(new StringBuffer().append("object changed ").append((object) @object).ToString());
      this.decorated.objectChanged(@object);
    }

    public virtual void reload(NakedObject @object)
    {
      this.decorated.reload(@object);
      this.log(new StringBuffer().append("Relead: ").append((object) @object).ToString());
    }

    public virtual void refresh(NakedReference root, Hashtable nonRefreshOids)
    {
      this.decorated.refresh(root, nonRefreshOids);
      this.log(new StringBuffer().append("Refresh: ").append((object) root).ToString());
    }

    public virtual void reset()
    {
      this.log("reset object manager");
      this.decorated.reset();
    }

    public virtual void resolveImmediately(NakedObject @object)
    {
      this.decorated.resolveImmediately(@object);
      this.log(new StringBuffer().append("Resolve immediately: ").append((object) @object).ToString());
    }

    public virtual void resolveField(NakedObject @object, NakedObjectField field)
    {
      this.log(new StringBuffer().append("Resolve eagerly object in field ").append((object) field).append(" of ").append((object) @object).ToString());
      this.decorated.resolveField(@object, field);
    }

    public virtual void saveChanges()
    {
      this.log("Saving changes");
      this.decorated.saveChanges();
    }

    public virtual void shutdown()
    {
      this.log(new StringBuffer().append("Shutting down ").append((object) this.decorated).ToString());
      this.decorated.shutdown();
      this.close();
    }

    public virtual void startTransaction()
    {
      this.log("Start transaction");
      this.decorated.startTransaction();
    }
  }
}
