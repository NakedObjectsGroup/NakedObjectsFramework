// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.ObjectStoreLogger
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.persistence.objectstore
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/objectstore/NakedObjectStore;")]
  public class ObjectStoreLogger : Logger, NakedObjectStore
  {
    private readonly NakedObjectStore decorated;

    public ObjectStoreLogger(NakedObjectStore decorated, string logFileName)
      : base(logFileName, false)
    {
      this.decorated = decorated;
    }

    public ObjectStoreLogger(NakedObjectStore decorated)
      : base((string) null, true)
    {
      this.decorated = decorated;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void abortTransaction()
    {
      this.log("Abort transaction started");
      this.decorated.abortTransaction();
      this.log("Abort transaction complete");
    }

    public virtual CreateObjectCommand createCreateObjectCommand(
      NakedObject @object)
    {
      this.log(new StringBuffer().append("Create object ").append((object) @object).ToString());
      return this.decorated.createCreateObjectCommand(@object);
    }

    public virtual DestroyObjectCommand createDestroyObjectCommand(
      NakedObject @object)
    {
      this.log(new StringBuffer().append("Destroy object ").append((object) @object).ToString());
      return this.decorated.createDestroyObjectCommand(@object);
    }

    public virtual SaveObjectCommand createSaveObjectCommand(NakedObject @object)
    {
      this.log(new StringBuffer().append("Save object ").append((object) @object).ToString());
      return this.decorated.createSaveObjectCommand(@object);
    }

    public virtual Transaction createTransaction()
    {
      this.log("Create transaction");
      return this.decorated.createTransaction();
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void endTransaction()
    {
      this.log("End transaction");
      this.decorated.endTransaction();
    }

    public virtual void debugData(DebugString debug) => this.decorated.debugData(debug);

    public virtual string getDebugTitle() => this.decorated.getDebugTitle();

    [JavaFlags(4)]
    public override Class getDecoratedClass() => ObjectImpl.getClass((object) this.decorated);

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectPerstsistenceException;org/nakedobjects/object/UnsupportedFindException;")]
    public virtual NakedObject[] getInstances(InstancesCriteria criteria)
    {
      this.log(new StringBuffer().append("Get instances matching ").append((object) criteria).ToString());
      return this.decorated.getInstances(criteria);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedObject[] getInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      this.log(new StringBuffer().append("Get instances of ").append(specification.getShortName()).ToString());
      return this.decorated.getInstances(specification, includeSubclasses);
    }

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedClass getNakedClass(string name)
    {
      NakedClass nakedClass = this.decorated.getNakedClass(name);
      this.log(new StringBuffer().append("Get class ").append(name).ToString(), (object) nakedClass);
      return nakedClass;
    }

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedObject getObject(Oid oid, NakedObjectSpecification hint)
    {
      NakedObject nakedObject = this.decorated.getObject(oid, hint);
      this.log(new StringBuffer().append("Get object for ").append((object) oid).append(" (of type ").append(hint.getShortName()).append(")").ToString(), nakedObject.getObject());
      return nakedObject;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual bool hasInstances(NakedObjectSpecification specification, bool includeSubclasses)
    {
      bool flag = this.decorated.hasInstances(specification, includeSubclasses);
      this.log(new StringBuffer().append("Has instances of ").append(specification.getShortName()).ToString(), (object) new StringBuffer().append("").append(flag).ToString());
      return flag;
    }

    [JavaThrownExceptions("3;org/nakedobjects/utility/configuration/ConfigurationException;org/nakedobjects/utility/configuration/ComponentException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void init()
    {
      this.log(new StringBuffer().append("Initialising ").append(this.name()).ToString());
      this.decorated.init();
    }

    public virtual string name() => this.decorated.name();

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual int numberOfInstances(
      NakedObjectSpecification specification,
      bool includedSubclasses)
    {
      int num = this.decorated.numberOfInstances(specification, includedSubclasses);
      this.log(new StringBuffer().append("Number of instances of ").append(specification.getShortName()).ToString(), (object) new StringBuffer().append("").append(num).ToString());
      return num;
    }

    public virtual void reset()
    {
      this.log("Reset");
      this.decorated.reset();
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveField(NakedObject @object, NakedObjectField field)
    {
      this.log(new StringBuffer().append("Resolve eagerly object in field ").append((object) field).append(" of ").append((object) @object).ToString());
      this.decorated.resolveField(@object, field);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveImmediately(NakedObject @object)
    {
      this.log(new StringBuffer().append("Resolve immediately: ").append((object) @object).ToString());
      this.decorated.resolveImmediately(@object);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void execute(PersistenceCommand[] commands)
    {
      this.log("Run transactions");
      for (int index = 0; index < commands.Length; ++index)
        this.log(new StringBuffer().append("  ").append(index).append(" ").append((object) commands[index]).ToString());
      this.decorated.execute(commands);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void shutdown()
    {
      this.log(new StringBuffer().append("Shutting down ").append((object) this.decorated).ToString());
      this.decorated.shutdown();
      this.close();
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void startTransaction()
    {
      this.log("Start transaction");
      this.decorated.startTransaction();
    }
  }
}
