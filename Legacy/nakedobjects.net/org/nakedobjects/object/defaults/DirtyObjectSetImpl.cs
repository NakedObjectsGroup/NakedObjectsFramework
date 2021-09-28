// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.DirtyObjectSetImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/DirtyObjectSet;")]
  public class DirtyObjectSetImpl : DirtyObjectSet
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Vector changes;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addDirty(NakedObject @object)
    {
      if (DirtyObjectSetImpl.LOG.isDebugEnabled())
        DirtyObjectSetImpl.LOG.debug((object) new StringBuffer().append("mark as dirty ").append((object) @object).ToString());
      if (this.changes.contains((object) @object))
        return;
      this.changes.addElement((object) @object);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Enumeration dirtyObjects()
    {
      Enumeration enumeration = this.changes.elements();
      if (this.changes.size() > 0 && DirtyObjectSetImpl.LOG.isDebugEnabled())
        DirtyObjectSetImpl.LOG.debug((object) new StringBuffer().append("dirty objects ").append((object) this.changes).ToString());
      this.changes = new Vector();
      return enumeration;
    }

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
      if (DirtyObjectSetImpl.LOG.isInfoEnabled())
        DirtyObjectSetImpl.LOG.info((object) new StringBuffer().append("  shutting down ").append((object) this).ToString());
      this.changes.removeAllElements();
      this.changes = (Vector) null;
    }

    public override string ToString() => new org.nakedobjects.utility.ToString((object) this).append("changes", (object) this.changes).ToString();

    public DirtyObjectSetImpl() => this.changes = new Vector();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DirtyObjectSetImpl()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DirtyObjectSetImpl dirtyObjectSetImpl = this;
      ObjectImpl.clone((object) dirtyObjectSetImpl);
      return ((object) dirtyObjectSetImpl).MemberwiseClone();
    }
  }
}
