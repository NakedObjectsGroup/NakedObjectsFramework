// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.ClosedCollectionNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaFlags(32)]
  public class ClosedCollectionNodeSpecification : NodeSpecification
  {
    public override bool canDisplay(Content content) => content.isCollection() && content.getNaked() != null;

    public override int canOpen(Content content)
    {
      NakedCollection collection = ((CollectionContent) content).getCollection();
      return collection.getResolveState().isGhost() ? 0 : (collection.size() <= 0 ? 2 : 1);
    }

    [JavaFlags(4)]
    public override View createNodeView(Content content, ViewAxis axis) => (View) new LeafNodeView(content, (ViewSpecification) this, axis);

    public override string getName() => "Collection tree node - closed";

    [JavaFlags(0)]
    public ClosedCollectionNodeSpecification()
    {
    }
  }
}
