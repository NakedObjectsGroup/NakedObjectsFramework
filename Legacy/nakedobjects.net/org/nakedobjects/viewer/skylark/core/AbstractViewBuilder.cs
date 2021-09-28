// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractViewBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/CompositeViewBuilder;")]
  public abstract class AbstractViewBuilder : CompositeViewBuilder
  {
    private CompositeViewBuilder reference;

    public virtual void build(View view)
    {
    }

    public virtual ViewAxis createViewAxis() => (ViewAxis) null;

    public virtual View decorateSubview(View subview) => subview;

    public virtual CompositeViewBuilder getReference() => this.reference;

    public virtual Size getRequiredSize(View view) => throw new NotImplementedException();

    public virtual bool isOpen() => false;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => false;

    public virtual void layout(View view, Size maximumSize)
    {
    }

    public virtual void setReference(CompositeViewBuilder design) => this.reference = design;

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractViewBuilder abstractViewBuilder = this;
      ObjectImpl.clone((object) abstractViewBuilder);
      return ((object) abstractViewBuilder).MemberwiseClone();
    }

    public abstract View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis);
  }
}
