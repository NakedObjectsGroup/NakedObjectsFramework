// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.OptionFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class OptionFactory
  {
    public static void addClassMenuOptions(
      NakedObjectSpecification specificaton,
      UserActionSet menuOptionSet)
    {
      NakedClass nakedClass = NakedObjects.getObjectPersistor().getNakedClass(specificaton);
      NakedObject adapterForTransient = NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient((object) nakedClass);
      OptionFactory.menuOptions(adapterForTransient, menuOptionSet, Action.USER);
      OptionFactory.menuOptions(specificaton.getClassActions(Action.USER), (NakedReference) adapterForTransient, menuOptionSet);
      OptionFactory.menuOptions(adapterForTransient, menuOptionSet, Action.EXPLORATION);
      OptionFactory.menuOptions(specificaton.getClassActions(Action.EXPLORATION), (NakedReference) adapterForTransient, menuOptionSet);
      OptionFactory.menuOptions(adapterForTransient, menuOptionSet, Action.DEBUG);
      OptionFactory.menuOptions(specificaton.getClassActions(Action.DEBUG), (NakedReference) adapterForTransient, menuOptionSet);
    }

    public static void addObjectMenuOptions(NakedReference @object, UserActionSet options)
    {
      if (@object == null)
        return;
      if (@object.getObject() is FastFinder)
      {
        options.add((UserAction) new FindFirstOption());
        options.add((UserAction) new FindAllOption());
      }
      else
      {
        Action[] objectActions1 = @object.getSpecification().getObjectActions(Action.USER);
        Action[] objectActions2 = @object.getSpecification().getObjectActions(Action.EXPLORATION);
        Action[] objectActions3 = @object.getSpecification().getObjectActions(Action.DEBUG);
        int length = objectActions1.Length + objectActions2.Length + objectActions3.Length;
        Action[] actions = length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
        System.arraycopy((object) objectActions1, 0, (object) actions, 0, objectActions1.Length);
        System.arraycopy((object) objectActions2, 0, (object) actions, objectActions1.Length, objectActions2.Length);
        System.arraycopy((object) objectActions3, 0, (object) actions, objectActions1.Length + objectActions2.Length, objectActions3.Length);
        OptionFactory.menuOptions(actions, @object, options);
      }
      bool flag = @object.getOid() != null;
      if (@object.getObject() is NakedClass || @object.getObject() is InstanceCollectionVector || !flag)
        return;
      options.add((UserAction) new DestroyObjectOption());
    }

    public static void addObjectButtonOptions(NakedReference @object, UserActionSet options)
    {
      Action[] buttonActions = @object.getSpecification().getButtonActions();
      if (buttonActions == null || buttonActions.Length <= 0)
        return;
      for (int index = 0; index < buttonActions.Length; ++index)
      {
        Action action = buttonActions[index];
        Action[] actions = action.getActions();
        if (actions == null || actions.Length == 0)
        {
          UserAction userAction = OptionFactory.getUserAction(action, @object);
          if (userAction != null)
            options.add(userAction);
        }
      }
    }

    private static UserAction getUserAction(Action action, NakedReference @object) => action.getParameterTypes().Length != 0 ? (UserAction) DialogedObjectOption.createOption(action, @object) : (UserAction) ImmediateObjectOption.createOption(action, @object);

    private static void menuOptions(
      Action[] actions,
      NakedReference @object,
      UserActionSet menuOptionSet)
    {
      for (int index = 0; index < actions.Length; ++index)
      {
        UserAction option;
        if (actions[index].getActions().Length > 0)
        {
          option = (UserAction) new UserActionSet(actions[index].getName(), menuOptionSet);
          OptionFactory.menuOptions(actions[index].getActions(), @object, (UserActionSet) option);
        }
        else
          option = actions[index].getParameterTypes().Length != 0 ? (UserAction) DialogedObjectOption.createOption(actions[index], @object) : (UserAction) ImmediateObjectOption.createOption(actions[index], @object);
        if (option != null)
          menuOptionSet.add(option);
      }
    }

    private static void menuOptions(
      NakedObject @object,
      UserActionSet menuOptionSet,
      Action.Type actionType)
    {
      OptionFactory.menuOptions(@object.getSpecification().getObjectActions(actionType), (NakedReference) @object, menuOptionSet);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      OptionFactory optionFactory = this;
      ObjectImpl.clone((object) optionFactory);
      return ((object) optionFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
