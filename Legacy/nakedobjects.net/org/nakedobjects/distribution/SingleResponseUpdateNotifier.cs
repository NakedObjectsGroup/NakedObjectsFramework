// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.SingleResponseUpdateNotifier
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  [JavaInterfaces("2;org/nakedobjects/object/DirtyObjectSet;org/nakedobjects/utility/DebugInfo;")]
  public class SingleResponseUpdateNotifier : DirtyObjectSet, DebugInfo
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Vector updates;

    public SingleResponseUpdateNotifier()
    {
      this.updates = new Vector();
      if (!SingleResponseUpdateNotifier.LOG.isInfoEnabled() && !SingleResponseUpdateNotifier.LOG.isDebugEnabled())
        return;
      string str = new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" creating SingleResponseUpdateNotifier").ToString();
      SingleResponseUpdateNotifier.LOG.info((object) str);
      SingleResponseUpdateNotifier.LOG.debug((object) str, (Throwable) new Exception("This is the stack trace of where SingleResponseUpdateNotifier was created:"));
    }

    public virtual void addDirty(NakedObject @object)
    {
      if (this.updates.contains((object) @object))
        return;
      string str = (string) null;
      if (@object.getOid() != null)
        str = @object.getOid().ToString();
      if (SingleResponseUpdateNotifier.LOG.isDebugEnabled())
        SingleResponseUpdateNotifier.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" update of OID:").append(str).append(" from UpdateNotifier. Object:").append((object) @object).ToString());
      this.updates.addElement((object) @object);
    }

    public virtual NakedObject[] getUpdates()
    {
      int num1 = this.updates.size();
      if (SingleResponseUpdateNotifier.LOG.isDebugEnabled())
      {
        StringBuffer stringBuffer = new StringBuffer();
        stringBuffer.append(new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("  ").append(num1).append(" updates for request").append(" from UpdateNotifier hash:").append(this.GetHashCode()).ToString()).append("\n").append(this.updateList());
        SingleResponseUpdateNotifier.LOG.debug((object) stringBuffer);
      }
      int length = num1;
      NakedObject[] nakedObjectArray1 = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      int num2 = 0;
      Enumeration enumeration = this.updates.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject1 = (NakedObject) enumeration.nextElement();
        NakedObject[] nakedObjectArray2 = nakedObjectArray1;
        int num3;
        num2 = (num3 = num2) + 1;
        int index = num3;
        NakedObject nakedObject2 = nakedObject1;
        nakedObjectArray2[index] = nakedObject2;
      }
      this.updates.removeAllElements();
      return nakedObjectArray1;
    }

    public virtual void clearUpdates() => this.updates.removeAllElements();

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("updates", (object) this.updates);
      return toString.ToString();
    }

    public virtual string updateList()
    {
      int length = this.updates.size();
      NakedObject[] nakedObjectArray1 = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      int num1 = 0;
      Enumeration enumeration = this.updates.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject1 = (NakedObject) enumeration.nextElement();
        NakedObject[] nakedObjectArray2 = nakedObjectArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        NakedObject nakedObject2 = nakedObject1;
        nakedObjectArray2[index] = nakedObject2;
      }
      NakedObject[] nakedObjectArray3 = nakedObjectArray1;
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("  Updates from UpdateNotifier ").ToString()).append("\n");
      string str = "";
      for (int index = 0; index < nakedObjectArray3.Length; ++index)
      {
        if (nakedObjectArray3[index].getOid() != null)
          str = nakedObjectArray3[index].getOid().ToString();
        stringBuffer.append(new StringBuffer().append("object OID:").append(str).append(" ObjectHash:").append(Long.toHexString((long) nakedObjectArray3[index].GetHashCode())).append(" current state: ").append(nakedObjectArray3[index].getResolveState().ToString()).ToString()).append("\n");
      }
      return stringBuffer.ToString();
    }

    public virtual void removeUpdateFor(NakedObject @object) => this.updates.removeElement((object) @object);

    public virtual void debugData(DebugString str)
    {
      int num1 = 1;
      Enumeration enumeration = this.updates.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        DebugString debugString = str;
        int num2;
        num1 = (num2 = num1) + 1;
        int number = num2;
        debugString.append(number, 3);
        str.append((object) nakedObject);
      }
    }

    public virtual string getDebugTitle() => nameof (SingleResponseUpdateNotifier);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SingleResponseUpdateNotifier()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SingleResponseUpdateNotifier responseUpdateNotifier = this;
      ObjectImpl.clone((object) responseUpdateNotifier);
      return ((object) responseUpdateNotifier).MemberwiseClone();
    }
  }
}
