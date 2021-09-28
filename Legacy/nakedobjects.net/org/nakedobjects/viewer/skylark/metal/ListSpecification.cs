// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ListSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public class ListSpecification : AbstractCompositeViewSpecification, SubviewSpec
  {
    public ListSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true));

    public override View createView(Content content, ViewAxis axis)
    {
      WindowBorder container = new WindowBorder(base.createView(content, axis), true);
      container.setFocusManager((FocusManager) new SubviewFocusManager(container));
      return (View) container;
    }

    public virtual View createSubview(Content content, ViewAxis axis) => Skylark.getViewFactory().getIconizedSubViewSpecification(content).createView(content, axis);

    public override string getName() => "List";

    public override bool canDisplay(Content content) => content.isCollection();
  }
}
