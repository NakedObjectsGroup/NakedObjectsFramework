// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.UserAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterface]
  public interface UserAction
  {
    static readonly Action.Type USER;
    static readonly Action.Type DEBUG;
    static readonly Action.Type EXPLORATION;

    Action.Type getType();

    Consent disabled(View view);

    void execute(Workspace workspace, View view, Location at);

    string getDescription(View view);

    string getHelp(View view);

    string getName(View view);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static UserAction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
