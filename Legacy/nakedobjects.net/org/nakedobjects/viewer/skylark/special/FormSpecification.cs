// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.FormSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class FormSpecification : AbstractCompositeViewSpecification
  {
    public FormSpecification() => this.builder = (CompositeViewBuilder) new WindowDecorator((CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) new FormSpecification.FormSubviews())));

    public override bool canDisplay(Content content) => content.isObject();

    public override string getName() => "Standard Form";

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
    [JavaFlags(42)]
    private class FormSubviews : SubviewSpec
    {
      private static readonly ViewSpecification internalList;

      public virtual View createSubview(Content content, ViewAxis axis)
      {
        ViewFactory viewFactory = Skylark.getViewFactory();
        ViewSpecification viewSpecification;
        switch (content)
        {
          case OneToManyField _:
            viewSpecification = FormSpecification.FormSubviews.internalList;
            break;
          case ValueContent _:
            viewSpecification = viewFactory.getValueFieldSpecification((ValueContent) content);
            break;
          case ObjectContent _:
            viewSpecification = viewFactory.getIconizedSubViewSpecification(content);
            break;
          default:
            throw new NakedObjectRuntimeException();
        }
        return viewSpecification.createView(content, axis);
      }

      public virtual View decorateSubview(View view) => (View) FieldLabel.createInstance(view);

      [JavaFlags(2)]
      public FormSubviews()
      {
      }

      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32778)]
      static FormSubviews()
      {
        // ISSUE: unable to decompile the method.
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        FormSpecification.FormSubviews formSubviews = this;
        ObjectImpl.clone((object) formSubviews);
        return ((object) formSubviews).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
