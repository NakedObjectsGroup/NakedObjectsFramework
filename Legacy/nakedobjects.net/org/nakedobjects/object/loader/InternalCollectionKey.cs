// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.loader.InternalCollectionKey
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.loader;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.loader
{
  [JavaFlags(32)]
  public class InternalCollectionKey
  {
    private readonly NakedObject parent;
    private readonly string fieldName;
    private static Hashtable keyByFieldNameByNakedObjectByThread;

    public static void reset() => InternalCollectionKey.threadLocalKeyByFieldNameByNakedObject().clear();

    [JavaFlags(12)]
    public static Hashtable threadLocalKeyByFieldNameByNakedObject()
    {
      Thread thread = Thread.currentThread();
      Hashtable hashtable = (Hashtable) InternalCollectionKey.keyByFieldNameByNakedObjectByThread.get((object) thread);
      if (hashtable == null)
      {
        hashtable = new Hashtable();
        InternalCollectionKey.keyByFieldNameByNakedObjectByThread.put((object) thread, (object) hashtable);
      }
      return hashtable;
    }

    public static InternalCollectionKey createKey(
      NakedObject parent,
      string fieldName)
    {
      Hashtable hashtable1 = InternalCollectionKey.threadLocalKeyByFieldNameByNakedObject();
      Hashtable hashtable2 = (Hashtable) hashtable1.get((object) parent);
      if (hashtable2 == null)
      {
        hashtable2 = new Hashtable();
        hashtable1.put((object) parent, (object) hashtable2);
      }
      InternalCollectionKey internalCollectionKey = (InternalCollectionKey) hashtable2.get((object) fieldName);
      if (internalCollectionKey == null)
      {
        internalCollectionKey = new InternalCollectionKey(parent, fieldName);
        hashtable2.put((object) fieldName, (object) internalCollectionKey);
      }
      return internalCollectionKey;
    }

    private InternalCollectionKey(NakedObject parent, string fieldName)
    {
      Assert.assertNotNull(nameof (parent), (object) parent);
      Assert.assertNotNull(nameof (fieldName), (object) fieldName);
      this.parent = parent;
      this.fieldName = NameConvertor.simpleName(fieldName);
    }

    public override int GetHashCode()
    {
      NakedObject parent = this.parent;
      return (!(parent is string) ? ObjectImpl.hashCode((object) parent) : StringImpl.hashCode((string) parent)) + StringImpl.hashCode(this.fieldName);
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is InternalCollectionKey))
        return false;
      InternalCollectionKey internalCollectionKey = (InternalCollectionKey) obj;
      return internalCollectionKey.parent.Equals((object) this.parent) && StringImpl.equals(internalCollectionKey.fieldName, (object) this.fieldName);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InternalCollectionKey()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InternalCollectionKey internalCollectionKey = this;
      ObjectImpl.clone((object) internalCollectionKey);
      return ((object) internalCollectionKey).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
