// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.ObjectData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;

namespace org.nakedobjects.persistence.file
{
  public class ObjectData : Data
  {
    private Hashtable fields;

    public ObjectData(NakedObjectSpecification type, SerialOid oid)
      : base(type, oid)
    {
      this.fields = new Hashtable();
    }

    public override string ToString() => new StringBuffer().append("ObjectData[type=").append(this.getClassName()).append(",oid=").append((object) this.getOid()).append(",fields=").append((object) this.fields).append("]").ToString();

    public virtual void set(string fieldName, object oid)
    {
      if (oid == null)
        this.fields.remove((object) fieldName);
      else
        this.fields.put((object) fieldName, oid);
    }

    [JavaFlags(0)]
    public virtual void saveValue(string fieldName, bool isEmpty, string encodedString)
    {
      if (isEmpty)
        this.fields.remove((object) fieldName);
      else
        this.fields.put((object) fieldName, (object) encodedString);
    }

    public virtual void set(string fieldName, string value) => this.fields.put((object) fieldName, (object) value);

    public virtual object get(string fieldName) => this.fields.get((object) fieldName);

    public virtual string value(string fieldName) => \u003CVerifierFix\u003E.genCastToString(this.get(fieldName));

    public virtual string id(string fieldName)
    {
      object obj = this.get(fieldName);
      return obj == null ? (string) null : new StringBuffer().append("").append(((SerialOid) obj).getSerialNo()).ToString();
    }

    [JavaFlags(0)]
    public virtual void initCollection(SerialOid collectionOid, string fieldName) => this.fields.put((object) fieldName, (object) new ReferenceVector(collectionOid));

    [JavaFlags(0)]
    public virtual void addElement(string fieldName, SerialOid elementOid)
    {
      if (!this.fields.containsKey((object) fieldName))
        throw new NakedObjectRuntimeException(new StringBuffer().append("Field ").append(fieldName).append(" not found  in hashtable").ToString());
      ((ReferenceVector) this.fields.get((object) fieldName)).add(elementOid);
    }

    [JavaFlags(0)]
    public virtual ReferenceVector elements(string fieldName) => (ReferenceVector) this.fields.get((object) fieldName);

    public virtual Enumeration fields() => this.fields.keys();

    [JavaFlags(0)]
    public virtual void addAssociation(
      NakedObject fieldContent,
      string fieldName,
      bool ensurePersistent)
    {
      bool flag = fieldContent != null && (fieldContent.getOid() == null || fieldContent.getOid().isNull());
      if (ensurePersistent && flag)
        throw new IllegalStateException(new StringBuffer().append("Cannot save an object that is not persistent: ").append((object) fieldContent).ToString());
      this.set(fieldName, (object) fieldContent?.getOid());
    }

    [JavaFlags(0)]
    public virtual void addInternalCollection(
      InternalCollection collection,
      string fieldName,
      bool ensurePersistent)
    {
      this.initCollection((SerialOid) collection.getOid(), fieldName);
      int num = collection.size();
      for (int index = 0; index < num; ++index)
      {
        NakedObject nakedObject = collection.elementAt(index);
        Oid oid = nakedObject.getOid();
        if (oid == null || oid.isNull())
          throw new IllegalStateException(new StringBuffer().append("Element is not persistent ").append((object) nakedObject).ToString());
        this.addElement(fieldName, (SerialOid) oid);
      }
    }
  }
}
