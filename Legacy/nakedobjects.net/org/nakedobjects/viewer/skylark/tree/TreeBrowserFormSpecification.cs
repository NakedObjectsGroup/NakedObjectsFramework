// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.TreeBrowserFormSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class TreeBrowserFormSpecification : AbstractCompositeViewSpecification
  {
    public TreeBrowserFormSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) new TreeBrowserFormSpecification.DataFormSubviews()));

    public override bool canDisplay(Content content) => content.isObject();

    public override View createView(Content content, ViewAxis axis)
    {
      this.resolveObject(content);
      ScrollBorder scrollBorder = new ScrollBorder(base.createView(content, (ViewAxis) new LabelAxis()));
      scrollBorder.setFocusManager((FocusManager) new SubviewFocusManager((View) scrollBorder));
      return (View) scrollBorder;
    }

    public override string getName() => "Tree Browser Form";

    [JavaFlags(42)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
    private class DataFormSubviews : SubviewSpec
    {
      public virtual View createSubview(Content content, ViewAxis axis)
      {
        ViewFactory viewFactory = Skylark.getViewFactory();
        switch (content)
        {
          case OneToOneField _:
            return viewFactory.getIconizedSubViewSpecification(content).createView(content, axis);
          case ValueField _:
            return viewFactory.getValueFieldSpecification((ValueContent) content).createView(content, axis);
          default:
            return (View) null;
        }
      }

      public virtual View decorateSubview(View view) => (View) FieldLabel.createInstance(view);

      [JavaFlags(2)]
      public DataFormSubviews()
      {
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TreeBrowserFormSpecification.DataFormSubviews dataFormSubviews = this;
        ObjectImpl.clone((object) dataFormSubviews);
        return ((object) dataFormSubviews).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
