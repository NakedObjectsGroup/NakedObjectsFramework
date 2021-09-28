// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.AbstractObjectStoreTransaction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.transaction;
using System.ComponentModel;

namespace org.nakedobjects.@object.persistence.objectstore
{
  [JavaInterfaces("1;org/nakedobjects/object/transaction/Transaction;")]
  public abstract class AbstractObjectStoreTransaction : Transaction
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly Vector commands;
    private bool complete;
    private readonly NakedObjectStore objectStore;
    private readonly Vector toNotify;

    public AbstractObjectStoreTransaction(NakedObjectStore objectStore)
    {
      this.commands = new Vector();
      this.toNotify = new Vector();
      this.objectStore = objectStore;
      if (!AbstractObjectStoreTransaction.LOG.isDebugEnabled())
        return;
      AbstractObjectStoreTransaction.LOG.debug((object) new StringBuffer().append("new transaction ").append((object) this).ToString());
    }

    public virtual void abort()
    {
      if (AbstractObjectStoreTransaction.LOG.isInfoEnabled())
        AbstractObjectStoreTransaction.LOG.info((object) new StringBuffer().append("abort transaction ").append((object) this).ToString());
      this.complete = !this.complete ? true : throw new TransactionException("Transaction already complete; cannot abort");
    }

    [JavaFlags(4)]
    public virtual void includeCommand(PersistenceCommand command)
    {
      if (AbstractObjectStoreTransaction.LOG.isInfoEnabled())
        AbstractObjectStoreTransaction.LOG.info((object) new StringBuffer().append("add command ").append((object) command).ToString());
      this.commands.addElement((object) command);
    }

    [JavaFlags(0)]
    public virtual void addNotify(NakedObject @object)
    {
      if (AbstractObjectStoreTransaction.LOG.isDebugEnabled())
        AbstractObjectStoreTransaction.LOG.debug((object) new StringBuffer().append("add notification for ").append((object) @object).ToString());
      this.toNotify.addElement((object) @object);
    }

    [JavaFlags(4)]
    public virtual bool alreadyHasCommand(Class commandClass, NakedObject onObject) => this.getCommand(commandClass, onObject) != null;

    [JavaFlags(4)]
    public virtual bool alreadyHasCreate(NakedObject onObject) => this.alreadyHasCommand(Class.FromType(typeof (CreateObjectCommand)), onObject);

    [JavaFlags(4)]
    public virtual bool alreadyHasDestroy(NakedObject onObject) => this.alreadyHasCommand(Class.FromType(typeof (DestroyObjectCommand)), onObject);

    [JavaFlags(4)]
    public virtual bool alreadyHasSave(NakedObject onObject) => this.alreadyHasCommand(Class.FromType(typeof (SaveObjectCommand)), onObject);

    public virtual void commit()
    {
      if (AbstractObjectStoreTransaction.LOG.isInfoEnabled())
        AbstractObjectStoreTransaction.LOG.info((object) new StringBuffer().append("commit transaction ").append((object) this).ToString());
      if (this.complete)
        throw new TransactionException("Transaction already complete; cannot commit");
      this.executeCommands();
      this.objectStore.endTransaction();
      this.complete = true;
    }

    [JavaFlags(4)]
    public virtual void executeCommands()
    {
      int length = this.commands.size();
      PersistenceCommand[] commands = length >= 0 ? new PersistenceCommand[length] : throw new NegativeArraySizeException();
      this.commands.copyInto((object[]) commands);
      if (commands.Length > 0)
        this.objectStore.execute(commands);
      this.commands.removeAllElements();
    }

    [JavaFlags(4)]
    public virtual PersistenceCommand getCommand(
      Class commandClass,
      NakedObject onObject)
    {
      Enumeration enumeration = this.commands.elements();
      while (enumeration.hasMoreElements())
      {
        PersistenceCommand persistenceCommand = (PersistenceCommand) enumeration.nextElement();
        if (persistenceCommand.onObject().Equals((object) onObject) && commandClass.isAssignableFrom(ObjectImpl.getClass((object) persistenceCommand)))
          return persistenceCommand;
      }
      return (PersistenceCommand) null;
    }

    [JavaFlags(4)]
    public virtual Vector getCommands() => this.commands;

    [JavaFlags(4)]
    public virtual void removeCommand(Class commandClass, NakedObject onObject) => this.commands.removeElement((object) this.getCommand(commandClass, onObject));

    [JavaFlags(4)]
    public virtual void removeCreate(NakedObject onObject) => this.removeCommand(Class.FromType(typeof (CreateObjectCommand)), onObject);

    [JavaFlags(4)]
    public virtual void removeSave(NakedObject onObject) => this.removeCommand(Class.FromType(typeof (SaveObjectCommand)), onObject);

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("complete", this.complete);
      toString.append("commands", this.commands.size());
      return toString.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static AbstractObjectStoreTransaction()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractObjectStoreTransaction storeTransaction = this;
      ObjectImpl.clone((object) storeTransaction);
      return ((object) storeTransaction).MemberwiseClone();
    }

    public abstract void addCommand(PersistenceCommand command);
  }
}
