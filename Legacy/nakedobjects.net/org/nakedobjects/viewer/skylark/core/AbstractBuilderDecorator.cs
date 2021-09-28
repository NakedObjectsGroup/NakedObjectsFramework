// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractBuilderDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/CompositeViewBuilder;")]
  public abstract class AbstractBuilderDecorator : CompositeViewBuilder
  {
    [JavaFlags(20)]
    public readonly CompositeViewBuilder wrappedBuilder;

    public AbstractBuilderDecorator(CompositeViewBuilder design)
    {
      this.wrappedBuilder = design;
      this.wrappedBuilder.setReference((CompositeViewBuilder) this);
    }

    public virtual void build(View view) => this.wrappedBuilder.build(view);

    public virtual View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return this.wrappedBuilder.createCompositeView(content, specification, axis);
    }

    public virtual ViewAxis createViewAxis() => this.wrappedBuilder.createViewAxis();

    public virtual View decorateSubview(View subview) => this.wrappedBuilder.decorateSubview(subview);

    public virtual CompositeViewBuilder getReference() => this.wrappedBuilder.getReference();

    public virtual Size getRequiredSize(View view) => this.wrappedBuilder.getRequiredSize(view);

    public virtual bool isOpen() => this.wrappedBuilder.isOpen();

    public virtual bool isReplaceable() => this.wrappedBuilder.isReplaceable();

    public virtual bool isSubView() => this.wrappedBuilder.isSubView();

    public virtual void layout(View view, Size maximumSize) => this.wrappedBuilder.layout(view, new Size());

    public virtual void setReference(CompositeViewBuilder design) => this.wrappedBuilder.setReference(design);

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return new StringBuffer().append((object) this.wrappedBuilder).append("/").append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractBuilderDecorator builderDecorator = this;
      ObjectImpl.clone((object) builderDecorator);
      return ((object) builderDecorator).MemberwiseClone();
    }
  }
}
