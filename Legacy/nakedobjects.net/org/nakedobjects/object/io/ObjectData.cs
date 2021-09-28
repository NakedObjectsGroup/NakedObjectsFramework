// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.ObjectData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.io;
using System.ComponentModel;

namespace org.nakedobjects.@object.io
{
  [JavaFlags(32)]
  public class ObjectData : Data
  {
    private const long serialVersionUID = 7121411963269613347;
    private static readonly Transferable NO_ENTRY;
    private readonly Hashtable fields;

    public ObjectData(Oid oid, string resolveState, string className)
      : base(oid, resolveState, className)
    {
      this.fields = new Hashtable();
    }

    public ObjectData(TransferableReader data)
      : base(data)
    {
      this.fields = new Hashtable();
      int num = data.readInt();
      for (int index = 0; index < num; ++index)
      {
        string str1 = data.readString();
        if (StringImpl.equals(data.readString(), (object) "O"))
        {
          Transferable transferable = data.readObject();
          this.fields.put((object) str1, (object) transferable);
        }
        else
        {
          string str2 = data.readString();
          this.fields.put((object) str1, (object) str2);
        }
      }
    }

    public virtual void addField(string fieldName, object entry)
    {
      if (this.fields.containsKey((object) fieldName))
        throw new IllegalArgumentException(new StringBuffer().append("Field already entered ").append(fieldName).ToString());
      this.fields.put((object) fieldName, entry != null ? entry : (object) ObjectData.NO_ENTRY);
    }

    public override void writeData(TransferableWriter data)
    {
      base.writeData(data);
      data.writeInt(this.fields.size());
      Enumeration enumeration = this.fields.keys();
      while (enumeration.hasMoreElements())
      {
        string @string = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        object obj = this.fields.get((object) @string);
        data.writeString(@string);
        switch (obj)
        {
          case Data _:
          case Null _:
            data.writeString("O");
            data.writeObject((Transferable) obj);
            continue;
          default:
            data.writeString("S");
            data.writeString(\u003CVerifierFix\u003E.genCastToString(obj));
            continue;
        }
      }
    }

    public virtual object getEntry(string fieldName)
    {
      object obj = this.fields.get((object) fieldName);
      return obj == null || ObjectImpl.getClass(obj) == ObjectImpl.getClass((object) ObjectData.NO_ENTRY) ? (object) null : obj;
    }

    public override string ToString() => this.fields.ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ObjectData()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
