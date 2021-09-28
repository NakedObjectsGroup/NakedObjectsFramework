// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractFieldSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public abstract class AbstractFieldSpecification : ViewSpecification
  {
    [JavaFlags(28)]
    public const int TEXT_WIDTH = 30;

    public virtual bool canDisplay(Content content) => content.isValue();

    public virtual bool isOpen() => false;

    public virtual bool isReplaceable() => true;

    public virtual bool isSubView() => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractFieldSpecification fieldSpecification = this;
      ObjectImpl.clone((object) fieldSpecification);
      return ((object) fieldSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract View createView(Content content, ViewAxis axis);

    public abstract string getName();
  }
}
