// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.SaveTransientObjectBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class SaveTransientObjectBorder : ButtonBorder
  {
    private static readonly Logger LOG;

    private static Consent canSave(View view)
    {
      Action action1 = view.getContent().getSpecification().getObjectAction(Action.USER, "save") ?? view.getContent().getSpecification().getObjectAction(Action.USER, "persist");
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

    public SaveTransientObjectBorder(View view)
    {
      int length = 3;
      ButtonAction[] actions = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      actions[0] = (ButtonAction) new SaveTransientObjectBorder.SaveAction();
      actions[1] = (ButtonAction) new SaveTransientObjectBorder.SaveAndCloseAction();
      actions[2] = (ButtonAction) new SaveTransientObjectBorder.CloseAction();
      // ISSUE: explicit constructor call
      base.\u002Ector(actions, view);
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
        else if (SaveTransientObjectBorder.hasInvalidFields(view1))
          return true;
      }
      return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SaveTransientObjectBorder()
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

      public override void execute(Workspace workspace, View view, Location at) => SaveTransientObjectBorder.close(workspace, view);
    }

    [JavaFlags(42)]
    private class SaveAction : AbstractButtonAction
    {
      public SaveAction()
        : base("Save")
      {
      }

      public override Consent disabled(View view) => SaveTransientObjectBorder.canSave(view);

      public override void execute(Workspace workspace, View view, Location at)
      {
        if (SaveTransientObjectBorder.hasInvalidFields(view))
          return;
        SaveTransientObjectBorder.save(view);
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

      public override Consent disabled(View view) => SaveTransientObjectBorder.canSave(view);

      public override void execute(Workspace workspace, View view, Location at)
      {
        if (SaveTransientObjectBorder.hasInvalidFields(view))
          return;
        SaveTransientObjectBorder.save(view);
        SaveTransientObjectBorder.close(workspace, view);
      }
    }
  }
}
