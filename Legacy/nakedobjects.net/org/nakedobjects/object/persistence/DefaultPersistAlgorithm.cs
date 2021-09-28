// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.DefaultPersistAlgorithm
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/PersistAlgorithm;")]
  public class DefaultPersistAlgorithm : PersistAlgorithm
  {
    private static readonly org.apache.log4j.Logger LOG;
    private OidGenerator oidGenerator;

    [JavaFlags(52)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Oid createOid(Naked @object)
    {
      Oid oid = this.oidGenerator.next(@object);
      if (DefaultPersistAlgorithm.LOG.isDebugEnabled())
        DefaultPersistAlgorithm.LOG.debug((object) new StringBuffer().append("createOid ").append((object) oid).ToString());
      return oid;
    }

    public virtual void init()
    {
      Assert.assertNotNull("oid generator required", (object) this.oidGenerator);
      this.oidGenerator.init();
    }

    public virtual void makePersistent(NakedObject @object, PersistedObjectAdder manager)
    {
      if (@object.getResolveState().isPersistent() || @object.persistable() == Persistable.TRANSIENT)
      {
        if (!DefaultPersistAlgorithm.LOG.isWarnEnabled())
          return;
        DefaultPersistAlgorithm.LOG.warn((object) new StringBuffer().append("can't make object persistent - either already persistent, or transient only: ").append((object) @object).ToString());
      }
      else
      {
        NakedObjectField[] fields = @object.getFields();
        for (int index = 0; index < fields.Length; ++index)
        {
          NakedObjectField nakedObjectField = fields[index];
          if (nakedObjectField.isDerived() || !nakedObjectField.isValue())
            ;
        }
        if (DefaultPersistAlgorithm.LOG.isInfoEnabled())
          DefaultPersistAlgorithm.LOG.info((object) new StringBuffer().append("persist ").append((object) @object).ToString());
        NakedObjects.getObjectLoader().madePersistent((NakedReference) @object, this.createOid((Naked) @object));
        for (int index = 0; index < fields.Length; ++index)
        {
          NakedObjectField field1 = fields[index];
          if (!field1.isDerived() && !field1.isValue())
          {
            if (field1 is OneToManyAssociation)
            {
              this.makePersistent((InternalCollection) @object.getField(field1), manager);
            }
            else
            {
              object field2 = (object) @object.getField(field1);
              if (field2 != null)
              {
                if (!(field2 is NakedObject))
                  throw new NakedObjectRuntimeException(new StringBuffer().append(field2).append(" is not a NakedObject").ToString());
                this.makePersistent((NakedObject) field2, manager);
              }
            }
          }
        }
        manager.createObject(@object);
      }
    }

    [JavaFlags(4)]
    public virtual void makePersistent(InternalCollection collection, PersistedObjectAdder manager)
    {
      if (DefaultPersistAlgorithm.LOG.isInfoEnabled())
        DefaultPersistAlgorithm.LOG.info((object) new StringBuffer().append("persist ").append((object) collection).ToString());
      if (collection.getResolveState() == ResolveState.TRANSIENT)
        collection.changeState(ResolveState.RESOLVED);
      for (int index = 0; index < collection.size(); ++index)
        this.makePersistent(collection.elementAt(index), manager);
    }

    public virtual string name() => "Simple Bottom Up Persistence Walker";

    public virtual OidGenerator OidGenerator
    {
      set => this.oidGenerator = value;
    }

    public virtual void setOidGenerator(OidGenerator oidGenerator) => this.oidGenerator = oidGenerator;

    public virtual void shutdown()
    {
      this.oidGenerator.shutdown();
      this.oidGenerator = (OidGenerator) null;
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      if (this.oidGenerator != null)
        toString.append("oidGenerator", this.oidGenerator.name());
      return toString.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DefaultPersistAlgorithm()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DefaultPersistAlgorithm persistAlgorithm = this;
      ObjectImpl.clone((object) persistAlgorithm);
      return ((object) persistAlgorithm).MemberwiseClone();
    }
  }
}
