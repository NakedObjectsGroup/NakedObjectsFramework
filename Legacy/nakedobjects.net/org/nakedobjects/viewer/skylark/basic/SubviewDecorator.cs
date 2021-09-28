// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.SubviewDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class SubviewDecorator : AbstractBuilderDecorator
  {
    private bool isReplaceable;

    public SubviewDecorator(CompositeViewBuilder design)
      : this(design, true)
    {
    }

    public SubviewDecorator(CompositeViewBuilder design, bool isReplaceable)
      : base(design)
    {
      this.isReplaceable = isReplaceable;
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return (View) new Identifier((View) new SimpleBorder(this.wrappedBuilder.createCompositeView(content, specification, axis)));
    }

    public override bool isOpen() => true;

    public override bool isSubView() => true;

    public override bool isReplaceable() => this.isReplaceable;
  }
}
