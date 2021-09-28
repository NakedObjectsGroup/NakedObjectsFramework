// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.FindFormSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class FindFormSpecification : AbstractCompositeViewSpecification
  {
    [JavaFlags(4)]
    public Hint about;

    public FindFormSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) new FindFormSpecification.DataFormSubviews()));

    public override bool canDisplay(Content content) => content.isObject() && content.getNaked() != null && content.getNaked().getObject() is FastFinder;

    public override View createView(Content content, ViewAxis axis)
    {
      int length = 2;
      ButtonAction[] actions = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      actions[0] = (ButtonAction) new FindFormSpecification.\u0031(this, "Find", true);
      actions[1] = (ButtonAction) new FindFormSpecification.\u0032(this, "Close");
      return (View) new DialogBorder((View) new ButtonBorder(actions, base.createView(content, (ViewAxis) new LabelAxis())), false);
    }

    public override string getName() => "Find Form";

    [JavaFlags(42)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
    private class DataFormSubviews : SubviewSpec
    {
      public virtual View createSubview(Content content, ViewAxis axis)
      {
        ViewFactory viewFactory = Skylark.getViewFactory();
        switch (content)
        {
          case OneToManyField _:
            return (View) null;
          case ValueContent _:
            return viewFactory.getValueFieldSpecification((ValueContent) content).createView(content, axis);
          case ObjectContent _:
            return viewFactory.getIconizedSubViewSpecification(content).createView(content, axis);
          default:
            return (View) null;
        }
      }

      public virtual View decorateSubview(View view) => (View) FieldLabel.createInstance(view);

      [JavaFlags(2)]
      public DataFormSubviews()
      {
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        FindFormSpecification.DataFormSubviews dataFormSubviews = this;
        ObjectImpl.clone((object) dataFormSubviews);
        return ((object) dataFormSubviews).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractButtonAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private FindFormSpecification this\u00240;

      public override Consent disabled(View view)
      {
        NakedObject nakedObject = ((ObjectContent) view.getContent()).getObject();
        Action objectAction = nakedObject.getSpecification().getObjectAction(Action.USER, "Find");
        return nakedObject.isValid(objectAction, (Naked[]) null);
      }

      public override void execute(Workspace workspace, View view, Location at)
      {
        NakedObject nakedObject = ((ObjectContent) view.getContent()).getObject();
        Action objectAction = nakedObject.getSpecification().getObjectAction(Action.USER, "Find");
        Naked @object = nakedObject.execute(objectAction, (Naked[]) null);
        at.move(30, 60);
        workspace.addOpenViewFor(@object, at);
        view.getViewManager().showMessages();
      }

      public \u0031(FindFormSpecification _param1, string dummy0, bool dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractButtonAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private FindFormSpecification this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => view.getView().dispose();

      public \u0032(FindFormSpecification _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
