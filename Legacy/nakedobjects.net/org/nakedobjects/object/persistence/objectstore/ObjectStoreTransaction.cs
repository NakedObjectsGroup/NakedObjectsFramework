// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.ObjectStoreTransaction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.transaction;
using System.ComponentModel;

namespace org.nakedobjects.@object.persistence.objectstore
{
  public class ObjectStoreTransaction : AbstractObjectStoreTransaction
  {
    private static readonly Logger LOG;

    public ObjectStoreTransaction(NakedObjectStore objectStore)
      : base(objectStore)
    {
    }

    public override void addCommand(PersistenceCommand command)
    {
      if (command == null)
        return;
      NakedObject onObject = command.onObject();
      bool flag = ObjectStoreTransaction.LOG.isInfoEnabled();
      if (command is SaveObjectCommand)
      {
        if (this.alreadyHasCreate(onObject) || this.alreadyHasSave(onObject))
        {
          if (!flag)
            return;
          ObjectStoreTransaction.LOG.info((object) new StringBuffer().append("ignored command ").append((object) command).append(" as object already created/saved").ToString());
          return;
        }
        if (this.alreadyHasDestroy(onObject))
        {
          if (!flag)
            return;
          ObjectStoreTransaction.LOG.info((object) new StringBuffer().append("ignored command ").append((object) command).append(" as object no longer exists").ToString());
          return;
        }
      }
      if (command is DestroyObjectCommand)
      {
        if (this.alreadyHasCreate(onObject))
        {
          this.removeCreate(onObject);
          if (!flag)
            return;
          ObjectStoreTransaction.LOG.info((object) new StringBuffer().append("ignored both create and destroy command ").append((object) command).ToString());
          return;
        }
        if (this.alreadyHasSave(onObject))
        {
          this.removeSave(onObject);
          if (flag)
            ObjectStoreTransaction.LOG.info((object) new StringBuffer().append("removed prior save command ").append((object) command).ToString());
        }
      }
      this.includeCommand(command);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ObjectStoreTransaction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
