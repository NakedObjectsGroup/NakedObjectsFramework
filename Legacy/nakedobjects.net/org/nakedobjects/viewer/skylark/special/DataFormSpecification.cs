// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.DataFormSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark.special
{
  public class DataFormSpecification : AbstractCompositeViewSpecification
  {
    public DataFormSpecification() => this.builder = (CompositeViewBuilder) new WindowDecorator((CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) new DataFormSpecification.DataFormSubviews())));

    public override bool canDisplay(Content content) => content.isObject();

    public override View createView(Content content, ViewAxis axis)
    {
      this.resolveObject(content);
      return base.createView(content, (ViewAxis) new LabelAxis());
    }

    public override string getName() => "Data Form";

    [JavaFlags(42)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
    private class DataFormSubviews : SubviewSpec
    {
      public virtual View createSubview(Content content, ViewAxis axis)
      {
        ViewFactory viewFactory = Skylark.getViewFactory();
        return content is ValueContent ? viewFactory.getValueFieldSpecification((ValueContent) content).createView(content, axis) : (View) null;
      }

      public virtual View decorateSubview(View view) => view;

      public virtual bool isContentShown(Content content) => content is ObjectContent && ((AbstractContent) content).getSpecification().isValue();

      [JavaFlags(2)]
      public DataFormSubviews()
      {
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        DataFormSpecification.DataFormSubviews dataFormSubviews = this;
        ObjectImpl.clone((object) dataFormSubviews);
        return ((object) dataFormSubviews).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
