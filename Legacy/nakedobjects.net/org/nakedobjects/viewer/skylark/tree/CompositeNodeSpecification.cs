// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.CompositeNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaInterfaces("2;org/nakedobjects/viewer/skylark/CompositeViewSpecification;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public abstract class CompositeNodeSpecification : 
    NodeSpecification,
    CompositeViewSpecification,
    SubviewSpec
  {
    [JavaFlags(4)]
    public CompositeViewBuilder builder;
    private NodeSpecification collectionLeafNodeSpecification;
    private NodeSpecification objectLeafNodeSpecification;

    public virtual void setCollectionSubNodeSpecification(
      NodeSpecification collectionLeafNodeSpecification)
    {
      this.collectionLeafNodeSpecification = collectionLeafNodeSpecification;
    }

    public virtual void setObjectSubNodeSpecification(NodeSpecification objectLeafNodeSpecification) => this.objectLeafNodeSpecification = objectLeafNodeSpecification;

    [JavaFlags(4)]
    public override View createNodeView(Content content, ViewAxis axis) => this.builder.createCompositeView(content, (CompositeViewSpecification) this, axis);

    public virtual View decorateSubview(View view) => view;

    public virtual CompositeViewBuilder getSubviewBuilder() => this.builder;

    public virtual View createSubview(Content content, ViewAxis axis)
    {
      if (this.collectionLeafNodeSpecification.canDisplay(content))
        return this.collectionLeafNodeSpecification.createView(content, axis);
      return this.objectLeafNodeSpecification.canDisplay(content) ? this.objectLeafNodeSpecification.createView(content, axis) : (View) null;
    }
  }
}
