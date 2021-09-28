// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.TwoPassPersistAlgorithm
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
  public class TwoPassPersistAlgorithm : PersistAlgorithm
  {
    private static readonly org.apache.log4j.Logger LOG;
    private OidGenerator oidGenerator;

    [JavaFlags(50)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    private Oid createOid(Naked @object)
    {
      Oid oid = this.oidGenerator.next(@object);
      if (TwoPassPersistAlgorithm.LOG.isDebugEnabled())
        TwoPassPersistAlgorithm.LOG.debug((object) new StringBuffer().append("createOid ").append((object) oid).ToString());
      return oid;
    }

    public virtual void init()
    {
      Assert.assertNotNull("oid generator required", (object) this.oidGenerator);
      this.oidGenerator.init();
    }

    public virtual void makePersistent(NakedObject @object, PersistedObjectAdder persistor)
    {
      if (@object.getResolveState().isPersistent() || @object.persistable() == Persistable.TRANSIENT)
        return;
      if (TwoPassPersistAlgorithm.LOG.isInfoEnabled())
        TwoPassPersistAlgorithm.LOG.info((object) new StringBuffer().append("persist ").append((object) @object).ToString());
      NakedObjects.getObjectLoader().madePersistent((NakedReference) @object, this.createOid((Naked) @object));
      NakedObjectField[] fields = @object.getFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        NakedObjectField field1 = fields[index];
        if (!field1.isDerived() && !field1.isValue() && !(field1 is OneToManyAssociation))
        {
          object field2 = (object) @object.getField(field1);
          if (field2 != null)
          {
            if (!(field2 is NakedObject))
              throw new NakedObjectRuntimeException();
            this.makePersistent((NakedObject) field2, persistor);
          }
        }
      }
      for (int index1 = 0; index1 < fields.Length; ++index1)
      {
        NakedObjectField field3 = fields[index1];
        if (!field3.isDerived() && !field3.isValue() && field3 is OneToManyAssociation)
        {
          InternalCollection field4 = (InternalCollection) @object.getField(field3);
          this.makePersistent(field4, persistor);
          field4.changeState(ResolveState.RESOLVED);
          for (int index2 = 0; index2 < field4.size(); ++index2)
            this.makePersistent(field4.elementAt(index2), persistor);
        }
      }
      persistor.createObject(@object);
    }

    public virtual void makePersistent(
      InternalCollection collection,
      PersistedObjectAdder persistor)
    {
      if (collection.getResolveState().isPersistent() || collection.persistable() == Persistable.TRANSIENT)
        return;
      if (TwoPassPersistAlgorithm.LOG.isInfoEnabled())
        TwoPassPersistAlgorithm.LOG.info((object) new StringBuffer().append("persist ").append((object) collection).ToString());
      if (collection.getResolveState() == ResolveState.TRANSIENT)
        collection.changeState(ResolveState.RESOLVED);
      NakedObjects.getObjectLoader().madePersistent((NakedReference) collection, this.createOid((Naked) collection));
      for (int index = 0; index < collection.size(); ++index)
        this.makePersistent(collection.elementAt(index), persistor);
    }

    public virtual string name() => "Two pass,  bottom up persistence walker";

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

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TwoPassPersistAlgorithm()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TwoPassPersistAlgorithm persistAlgorithm = this;
      ObjectImpl.clone((object) persistAlgorithm);
      return ((object) persistAlgorithm).MemberwiseClone();
    }
  }
}
