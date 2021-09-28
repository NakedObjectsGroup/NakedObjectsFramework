// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.TypeComparator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Comparator;")]
  public class TypeComparator : Comparator
  {
    private string type;

    public virtual void init(NakedObject element) => this.type = element.getSpecification().getShortName();

    public virtual int compare(NakedObject sortedElement) => StringImpl.compareTo(sortedElement.getSpecification().getShortName(), this.type);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TypeComparator typeComparator = this;
      ObjectImpl.clone((object) typeComparator);
      return ((object) typeComparator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
