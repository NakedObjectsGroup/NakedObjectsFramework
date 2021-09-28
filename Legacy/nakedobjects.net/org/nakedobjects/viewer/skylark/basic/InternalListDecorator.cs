// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.InternalListDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class InternalListDecorator : AbstractBuilderDecorator
  {
    public InternalListDecorator(CompositeViewBuilder design)
      : base(design)
    {
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return (View) new InternalCollectionBorder(this.wrappedBuilder.createCompositeView(content, specification, axis));
    }

    public override bool isReplaceable() => true;

    public override bool isSubView() => true;
  }
}
