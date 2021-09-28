// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.TitleComparator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Comparator;")]
  public class TitleComparator : Comparator
  {
    private string title;

    public virtual void init(NakedObject element) => this.title = element.titleString();

    public virtual int compare(NakedObject sortedElement) => StringImpl.compareTo(sortedElement.titleString(), this.title);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TitleComparator titleComparator = this;
      ObjectImpl.clone((object) titleComparator);
      return ((object) titleComparator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
