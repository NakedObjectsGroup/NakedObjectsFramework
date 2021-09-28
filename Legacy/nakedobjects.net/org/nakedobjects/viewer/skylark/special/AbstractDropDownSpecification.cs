// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.AbstractDropDownSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaFlags(1056)]
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public abstract class AbstractDropDownSpecification : 
    AbstractCompositeViewSpecification,
    SubviewSpec
  {
    public AbstractDropDownSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true), true);

    public virtual View createSubview(Content content, ViewAxis lookupAxis) => (View) new LookupSelection((View) new IconView(content, (ViewSpecification) this, lookupAxis, Style.NORMAL));

    public override View createView(Content content, ViewAxis axis) => (View) new DisposeOverlay((View) new PanelBorder(1, (View) new ScrollBorder((View) new DropDownFocusBorder(base.createView(this.getOptionsContent(content), axis)))));

    [JavaFlags(1028)]
    public abstract Content getOptionsContent(Content content);
  }
}
