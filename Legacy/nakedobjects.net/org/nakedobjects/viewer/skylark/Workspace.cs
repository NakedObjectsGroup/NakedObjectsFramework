// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Workspace
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/View;")]
  [JavaInterface]
  public interface Workspace : View
  {
    View addIconFor(Naked naked, Location at);

    View addOpenViewFor(Naked @object, Location at);

    View createSubviewFor(Naked @object, bool asIcon);

    void lower(View view);

    void raise(View view);

    void removeViewsFor(NakedObject @object);

    void filterKeyShortcuts(KeyboardAction keyboardAction);
  }
}
