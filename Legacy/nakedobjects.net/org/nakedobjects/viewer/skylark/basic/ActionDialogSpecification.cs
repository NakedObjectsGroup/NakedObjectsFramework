// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ActionDialogSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;
using org.nakedobjects.viewer.skylark.special;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ActionDialogSpecification : AbstractCompositeViewSpecification
  {
    public ActionDialogSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ActionFieldBuilder((SubviewSpec) new ActionDialogSpecification.DialogFormSubviews()));

    public override bool canDisplay(Content content) => content is ActionContent;

    public override View createView(Content content, ViewAxis axis)
    {
      int length = 2;
      ButtonAction[] actions = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      actions[0] = (ButtonAction) new ActionDialogSpecification.ExecuteAndCloseAction();
      actions[1] = (ButtonAction) new ActionDialogSpecification.CancelAction();
      ButtonBorder buttonBorder = new ButtonBorder(actions, (View) new IconBorder(base.createView(content, (ViewAxis) new LabelAxis())));
      DialogBorder dialogBorder = new DialogBorder((View) buttonBorder, false);
      dialogBorder.setFocusManager((FocusManager) new ActionDialogFocusManager(buttonBorder));
      return (View) dialogBorder;
    }

    public override string getName() => "Action Dialog";

    [JavaFlags(41)]
    public class CancelAction : AbstractButtonAction
    {
      public CancelAction()
        : base("Cancel")
      {
      }

      public override void execute(Workspace workspace, View view, Location at) => view.dispose();
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
    [JavaFlags(42)]
    private class DialogFormSubviews : SubviewSpec
    {
      public virtual View createSubview(Content content, ViewAxis axis)
      {
        switch (content)
        {
          case ValueParameter _:
            return Skylark.getViewFactory().getValueFieldSpecification((ValueContent) content).createView(content, axis);
          case ObjectParameter _:
            return Skylark.getViewFactory().getIconizedSubViewSpecification(content).createView(content, axis);
          default:
            return (View) null;
        }
      }

      public virtual View decorateSubview(View view) => view;

      [JavaFlags(2)]
      public DialogFormSubviews()
      {
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ActionDialogSpecification.DialogFormSubviews dialogFormSubviews = this;
        ObjectImpl.clone((object) dialogFormSubviews);
        return ((object) dialogFormSubviews).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(42)]
    private class ExecuteAction : AbstractButtonAction
    {
      public ExecuteAction()
        : this("Apply")
      {
      }

      public ExecuteAction(string name)
        : base(name, true)
      {
      }

      public override Consent disabled(View view)
      {
        View[] subviews = view.getSubviews();
        StringBuffer stringBuffer1 = new StringBuffer();
        StringBuffer stringBuffer2 = new StringBuffer();
        for (int index = 1; index < subviews.Length; ++index)
        {
          View view1 = subviews[index];
          ParameterContent content = (ParameterContent) view1.getContent();
          if (content.isRequired() && (content.getNaked() == null || content.getNaked() is NakedValue && ((NakedValue) content.getNaked()).isEmpty()))
          {
            string parameterName = content.getParameterName();
            if (stringBuffer1.length() > 0)
              stringBuffer1.append(", ");
            stringBuffer1.append(parameterName);
          }
          else if (view1.getState().isInvalid())
          {
            string parameterName = content.getParameterName();
            if (stringBuffer2.length() > 0)
              stringBuffer2.append(", ");
            stringBuffer2.append(parameterName);
          }
        }
        if (stringBuffer1.length() > 0)
          return (Consent) new Veto(new StringBuffer().append("Fields needed: ").append((object) stringBuffer1).ToString());
        return stringBuffer2.length() > 0 ? (Consent) new Veto(new StringBuffer().append("Invalid fields: ").append((object) stringBuffer2).ToString()) : ((ActionContent) view.getContent()).disabled();
      }

      public override void execute(Workspace workspace, View view, Location at) => this.execute(workspace, view, at, false);

      public virtual void execute(
        Workspace workspace,
        View view,
        Location at,
        bool closeViewOnSuccess)
      {
        BackgroundThread.run(view, (BackgroundTask) new ActionDialogSpecification.ExecuteAction.\u0031(this, view, closeViewOnSuccess, at, workspace));
      }

      [JavaFlags(4)]
      public virtual void move(Location at) => at.move(30, 60);

      [Inner]
      [JavaFlags(32)]
      [JavaInterfaces("1;org/nakedobjects/viewer/skylark/core/BackgroundTask;")]
      public class \u0031 : BackgroundTask
      {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaFlags(32770)]
        private ActionDialogSpecification.ExecuteAction this\u00240;
        [JavaFlags(16)]
        public readonly Location at_\u003E;
        [JavaFlags(16)]
        public readonly bool closeViewOnSuccess_\u003E;
        [JavaFlags(16)]
        public readonly View view_\u003E;
        [JavaFlags(16)]
        public readonly Workspace workspace_\u003E;

        public virtual void execute()
        {
          ActionContent content = (ActionContent) this.view_\u003E.getContent();
          View view1 = (View) null;
          if (this.closeViewOnSuccess_\u003E)
          {
            view1 = this.view_\u003E.getParent();
            view1?.removeView(this.view_\u003E);
          }
          Throwable e = (Throwable) null;
          Naked @object = (Naked) null;
          try
          {
            @object = content.execute();
          }
          catch (Exception ex)
          {
            e = ThrowableWrapper.wrapThrowable(ex);
          }
          finally
          {
            view1?.addView(this.view_\u003E);
            if (e == null && this.closeViewOnSuccess_\u003E)
              this.view_\u003E.dispose();
            if (e != null)
              this.view_\u003E.getViewManager().showException(e);
          }
          if (@object != null)
          {
            if (@object is InternalCollection && ((NakedCollection) @object).size() == 0)
            {
              NakedObjects.getMessageBroker().addWarning("No results");
            }
            else
            {
              this.this\u00240.move(this.at_\u003E);
              View view2 = this.workspace_\u003E.addOpenViewFor(@object, this.at_\u003E);
              if (e == null)
                this.view_\u003E.getViewManager().setKeyboardFocus(view2);
            }
          }
          this.view_\u003E.getViewManager().showMessages();
        }

        public virtual string getName() => ((ActionContent) this.view_\u003E.getContent()).getActionName();

        public virtual string getDescription() => new StringBuffer().append("Running action ").append(this.getName()).append(" on  ").append((object) this.view_\u003E.getContent().getNaked()).ToString();

        public \u0031(
          ActionDialogSpecification.ExecuteAction _param1,
          [In] View obj1,
          [In] bool obj2,
          [In] Location obj3,
          [In] Workspace obj4)
        {
          this.this\u00240 = _param1;
          if (_param1 == null)
            ObjectImpl.getClass((object) _param1);
          this.view_\u003E = obj1;
          this.closeViewOnSuccess_\u003E = obj2;
          this.at_\u003E = obj3;
          this.workspace_\u003E = obj4;
        }

        [JavaFlags(4227077)]
        [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone()
        {
          ActionDialogSpecification.ExecuteAction.\u0031 obj = this;
          ObjectImpl.clone((object) obj);
          return ((object) obj).MemberwiseClone();
        }

        [JavaFlags(4227073)]
        public override string ToString() => ObjectImpl.jloToString((object) this);
      }
    }

    [JavaFlags(42)]
    private class ExecuteAndCloseAction : ActionDialogSpecification.ExecuteAction
    {
      public ExecuteAndCloseAction()
        : base("OK")
      {
      }

      public override void execute(Workspace workspace, View view, Location at) => this.execute(workspace, view, at, true);

      [JavaFlags(4)]
      public override void move(Location at)
      {
      }
    }
  }
}
