// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ObjectActionButtonBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class ObjectActionButtonBorder : ButtonBorder
  {
    private static readonly Logger LOG;

    private static Consent canSave(View view)
    {
      Action action1 = view.getContent().getNaked().getSpecification().getObjectAction(Action.USER, "save") ?? view.getContent().getNaked().getSpecification().getObjectAction(Action.USER, "persist");
      if (action1 == null)
        return (Consent) Allow.DEFAULT;
      NakedObject naked = (NakedObject) view.getContent().getNaked();
      Action action2 = action1;
      int length = 0;
      Naked[] parameters = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      return naked.isValid(action2, parameters);
    }

    private static void close(Workspace workspace, View view) => view.dispose();

    private static NakedObject save(View view)
    {
      // ISSUE: unable to decompile the method.
    }

    public ObjectActionButtonBorder(View view)
      : base(ObjectActionButtonBorder.getButtonActionsForContent(view.getContent()), view)
    {
    }

    private static ButtonAction[] getButtonActionsForContent(Content content)
    {
      List list1 = (List) new ArrayList();
      if (content.isPersistable() && content.isTransient())
      {
        list1.add((object) new ObjectActionButtonBorder.SaveAction());
        list1.add((object) new ObjectActionButtonBorder.SaveAndCloseAction());
        list1.add((object) new ObjectActionButtonBorder.CloseAction());
      }
      if (content is ObjectContent)
      {
        ObjectContent objectContent = (ObjectContent) content;
        UserActionSet options = new UserActionSet(false, false, Action.USER);
        objectContent.contentButtonOptions(options);
        foreach (UserAction menuOption in options.getMenuOptions())
          list1.add((object) new ObjectActionButtonBorder.ButtonActionWrapper(menuOption));
      }
      List list2 = list1;
      int length = 0;
      ButtonAction[] buttonActionArray = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      return (ButtonAction[]) list2.toArray((object[]) buttonActionArray);
    }

    private static bool hasInvalidFields(View view)
    {
      View[] subviews = view.getSubviews();
      if (subviews == null || subviews.Length == 0)
        return false;
      for (int index = 0; index < subviews.Length; ++index)
      {
        View view1 = subviews[index];
        if (view1.getContent().isValue() && view1.canChangeValue())
        {
          view1.editComplete();
          if (view1.getState().isInvalid())
            return true;
        }
        else if (ObjectActionButtonBorder.hasInvalidFields(view1))
          return true;
      }
      return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ObjectActionButtonBorder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(42)]
    private class CloseAction : AbstractButtonAction
    {
      public CloseAction()
        : base("Discard")
      {
      }

      public override void execute(Workspace workspace, View view, Location at) => ObjectActionButtonBorder.close(workspace, view);
    }

    [JavaFlags(42)]
    private class SaveAction : AbstractButtonAction
    {
      public SaveAction()
        : base("Save")
      {
      }

      public override Consent disabled(View view) => ObjectActionButtonBorder.canSave(view);

      public override void execute(Workspace workspace, View view, Location at)
      {
        if (ObjectActionButtonBorder.hasInvalidFields(view))
          return;
        ObjectActionButtonBorder.save(view);
        View view1 = view.getSpecification().createView(view.getContent(), (ViewAxis) null);
        workspace.replaceView(view, view1);
      }
    }

    [JavaFlags(42)]
    private class SaveAndCloseAction : AbstractButtonAction
    {
      public SaveAndCloseAction()
        : base("Save & Close")
      {
      }

      public override Consent disabled(View view) => ObjectActionButtonBorder.canSave(view);

      public override void execute(Workspace workspace, View view, Location at)
      {
        if (ObjectActionButtonBorder.hasInvalidFields(view))
          return;
        ObjectActionButtonBorder.save(view);
        ObjectActionButtonBorder.close(workspace, view);
      }
    }

    [JavaFlags(42)]
    private class ButtonActionWrapper : AbstractButtonAction
    {
      private UserAction wrapped;

      public ButtonActionWrapper(UserAction wrapped)
        : this(wrapped, false)
      {
      }

      public ButtonActionWrapper(UserAction wrapped, bool defaultButton)
        : base(nameof (ButtonActionWrapper), defaultButton)
      {
        this.wrapped = wrapped;
      }

      public override Consent disabled(View view) => this.wrapped.disabled(view);

      public override string getDescription(View view) => this.wrapped.getDescription(view);

      public override string getHelp(View view) => this.wrapped.getHelp(view);

      public override string getName(View view) => this.wrapped.getName(view);

      public override Action.Type getType() => this.wrapped.getType();

      public override void execute(Workspace workspace, View view, Location at) => this.wrapped.execute(workspace, view, at);
    }
  }
}
