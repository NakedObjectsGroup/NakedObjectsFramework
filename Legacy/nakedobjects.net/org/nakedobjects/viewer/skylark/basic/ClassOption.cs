// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ClassOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ClassOption
  {
    public static void xxmenuOptions(
      NakedObjectSpecification specificaton,
      UserActionSet menuOptionSet)
    {
      NakedClass nakedClass = NakedObjects.getObjectPersistor().getNakedClass(specificaton);
      NakedObject adapterForTransient = NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient((object) nakedClass);
      foreach (Action classAction in specificaton.getClassActions(Action.USER))
        ClassOption.addOption(adapterForTransient, menuOptionSet, classAction);
      foreach (Action objectAction in adapterForTransient.getSpecification().getObjectActions(Action.USER))
        ClassOption.addOption(adapterForTransient, menuOptionSet, objectAction);
      Action[] classActions = specificaton.getClassActions(Action.EXPLORATION);
      if (classActions.Length > 0)
      {
        for (int index = 0; index < classActions.Length; ++index)
          ClassOption.addOption(adapterForTransient, menuOptionSet, classActions[index]);
      }
      Action[] objectActions = adapterForTransient.getSpecification().getObjectActions(Action.EXPLORATION);
      if (objectActions.Length <= 0)
        return;
      for (int index = 0; index < objectActions.Length; ++index)
        ClassOption.addOption(adapterForTransient, menuOptionSet, objectActions[index]);
    }

    private static void addOption(NakedObject cls, UserActionSet menuOptionSet, Action action)
    {
      AbstractUserAction abstractUserAction = action.getParameterTypes().Length != 0 ? (AbstractUserAction) DialogedObjectOption.createOption(action, (NakedReference) cls) : (AbstractUserAction) ImmediateObjectOption.createOption(action, (NakedReference) cls);
      if (abstractUserAction == null)
        return;
      menuOptionSet.add((UserAction) abstractUserAction);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClassOption classOption = this;
      ObjectImpl.clone((object) classOption);
      return ((object) classOption).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
