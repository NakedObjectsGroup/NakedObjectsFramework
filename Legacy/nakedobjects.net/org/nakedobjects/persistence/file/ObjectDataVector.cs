// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.ObjectDataVector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;

namespace org.nakedobjects.persistence.file
{
  public class ObjectDataVector
  {
    [JavaFlags(0)]
    public Vector elements;

    public virtual void addElement(ObjectData instanceData) => this.elements.addElement((object) instanceData);

    public virtual int size() => this.elements.size();

    public virtual ObjectData element(int i) => (ObjectData) this.elements.elementAt(i);

    public virtual bool contains(ObjectData data) => this.elements.contains((object) data);

    public ObjectDataVector() => this.elements = new Vector();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ObjectDataVector objectDataVector = this;
      ObjectImpl.clone((object) objectDataVector);
      return ((object) objectDataVector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
