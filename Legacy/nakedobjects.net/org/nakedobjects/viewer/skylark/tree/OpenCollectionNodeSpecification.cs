// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.OpenCollectionNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class OpenCollectionNodeSpecification : CompositeNodeSpecification
  {
    public override bool canDisplay(Content content) => content.isCollection() && ((CollectionContent) content).getCollection().size() > 0;

    public OpenCollectionNodeSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true));

    public override bool isOpen() => true;

    public override int canOpen(Content content) => 1;

    public override string getName() => "Collection tree node - open";
  }
}
