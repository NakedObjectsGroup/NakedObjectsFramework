// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedReference
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/object/Naked;")]
  public interface NakedReference : Naked
  {
    void checkLock(Version version);

    void debugClearResolved();

    void destroyed();

    new string getIconName();

    ResolveState getResolveState();

    Version getVersion();

    Persistable persistable();

    void setOptimisticLock(Version version);

    void persistedAs(Oid oid);

    void changeState(ResolveState newState);

    Naked execute(Action action, Naked[] parameters);

    Consent isValid(Action action, Naked[] parameters);

    Consent isAvailable(Action action);

    Consent isVisible(Action action);

    ActionParameterSet getParameters(Action action);
  }
}
