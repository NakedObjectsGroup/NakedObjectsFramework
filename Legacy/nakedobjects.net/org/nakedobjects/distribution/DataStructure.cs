// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.DataStructure
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
  public class DataStructure
  {
    private readonly Hashtable cache;

    public virtual NakedObjectField[] getFields(
      NakedObjectSpecification specification)
    {
      NakedObjectField[] nakedObjectFieldArray = (NakedObjectField[]) this.cache.get((object) specification);
      if (nakedObjectFieldArray == null)
      {
        nakedObjectFieldArray = this.loadFields(specification);
        this.cache.put((object) specification, (object) nakedObjectFieldArray);
      }
      return nakedObjectFieldArray;
    }

    private NakedObjectField[] loadFields(NakedObjectSpecification specification)
    {
      NakedObjectField[] fields = specification.getFields();
      Vector vector = new Vector(fields.Length);
label_8:
      for (int index1 = 0; index1 < fields.Length; ++index1)
      {
        string id = fields[index1].getId();
        for (int index2 = 0; index2 < vector.size(); ++index2)
        {
          if (StringImpl.compareTo(((NakedObjectMember) vector.elementAt(index2)).getId(), id) > 0)
          {
            vector.insertElementAt((object) fields[index1], index2);
            goto label_8;
          }
        }
        vector.addElement((object) fields[index1]);
      }
      int length = fields.Length;
      NakedObjectField[] nakedObjectFieldArray = length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
      vector.copyInto((object[]) nakedObjectFieldArray);
      return nakedObjectFieldArray;
    }

    public DataStructure() => this.cache = new Hashtable();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DataStructure dataStructure = this;
      ObjectImpl.clone((object) dataStructure);
      return ((object) dataStructure).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
