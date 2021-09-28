// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.EmptyNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class EmptyNodeSpecification : NodeSpecification
  {
    public override int canOpen(Content content) => 2;

    [JavaFlags(4)]
    public override View createNodeView(Content content, ViewAxis axis) => (View) null;

    public override bool canDisplay(Content content) => false;

    public override string getName() => "Empty tree node";
  }
}
