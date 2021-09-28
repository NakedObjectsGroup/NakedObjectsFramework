// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ListSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public class ListSpecification : AbstractCompositeViewSpecification, SubviewSpec
  {
    public ListSpecification() => this.builder = (CompositeViewBuilder) new WindowDecorator((CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true)));

    public virtual View createSubview(Content content, ViewAxis axis) => Skylark.getViewFactory().getIconizedSubViewSpecification(content).createView(content, axis);

    public override string getName() => "Standard List";

    public override bool canDisplay(Content content) => content.isCollection();
  }
}
