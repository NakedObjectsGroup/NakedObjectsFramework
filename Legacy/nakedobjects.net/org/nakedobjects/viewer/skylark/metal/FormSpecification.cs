// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.FormSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public class FormSpecification : AbstractCompositeViewSpecification, SubviewSpec
  {
    private static readonly ViewSpecification internalList;

    public FormSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) this));

    public override bool canDisplay(Content content) => content.isObject();

    public virtual View createSubview(Content content, ViewAxis axis)
    {
      ViewFactory viewFactory = Skylark.getViewFactory();
      ViewSpecification viewSpecification;
      switch (content)
      {
        case OneToManyField _:
          viewSpecification = FormSpecification.internalList;
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

    public override View createView(Content content, ViewAxis axis)
    {
      this.resolveObject(content);
      WindowBorder container = new WindowBorder((View) new IconBorder(base.createView(content, (ViewAxis) new LabelAxis())), true);
      container.setFocusManager((FocusManager) new SubviewFocusManager(container));
      return (View) container;
    }

    public override View decorateSubview(View view) => (View) FieldLabel.createInstance(view);

    public override string getName() => "Form";

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static FormSpecification()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
