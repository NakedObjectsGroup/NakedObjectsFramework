// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ClientSideTransaction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution
{
  public class ClientSideTransaction
  {
    private readonly Vector entries;

    private void add(ClientSideTransaction.Entry entry)
    {
      if (this.entries.contains((object) entry))
        return;
      this.entries.addElement((object) entry);
    }

    public virtual void addDestroyObject(NakedObject @object) => this.add(new ClientSideTransaction.Entry(@object, 3));

    public virtual void addMakePersistent(NakedObject @object) => this.add(new ClientSideTransaction.Entry(@object, 1));

    public virtual void addObjectChanged(NakedObject @object) => this.add(new ClientSideTransaction.Entry(@object, 2));

    public virtual ClientSideTransaction.Entry[] getEntries()
    {
      int length = this.entries.size();
      ClientSideTransaction.Entry[] entryArray = length >= 0 ? new ClientSideTransaction.Entry[length] : throw new NegativeArraySizeException();
      this.entries.copyInto((object[]) entryArray);
      return entryArray;
    }

    public virtual bool isEmpty() => this.entries.size() == 0;

    public virtual void rollback()
    {
    }

    public ClientSideTransaction() => this.entries = new Vector();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ClientSideTransaction clientSideTransaction = this;
      ObjectImpl.clone((object) clientSideTransaction);
      return ((object) clientSideTransaction).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(41)]
    public class Entry
    {
      private NakedObject @object;
      private int type;

      public Entry(NakedObject @object, int type)
      {
        this.@object = @object;
        this.type = type;
      }

      public override bool Equals(object obj) => obj == this || obj is ClientSideTransaction.Entry && ((ClientSideTransaction.Entry) obj).type == this.type && ((ClientSideTransaction.Entry) obj).@object.Equals((object) this.@object);

      public override int GetHashCode()
      {
        int num1 = 37 * (37 * 17 + this.type);
        NakedObject nakedObject = this.@object;
        int num2 = !(nakedObject is string) ? ObjectImpl.hashCode((object) nakedObject) : StringImpl.hashCode((string) nakedObject);
        return num1 + num2;
      }

      public virtual NakedObject getObject() => this.@object;

      public virtual int getType() => this.type;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ClientSideTransaction.Entry entry = this;
        ObjectImpl.clone((object) entry);
        return ((object) entry).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
