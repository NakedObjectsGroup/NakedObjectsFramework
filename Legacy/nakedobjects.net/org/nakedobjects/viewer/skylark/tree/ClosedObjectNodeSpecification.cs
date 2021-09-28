// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.ClosedObjectNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaFlags(32)]
  public class ClosedObjectNodeSpecification : NodeSpecification
  {
    private readonly bool showObjectContents;

    public ClosedObjectNodeSpecification(bool showObjectContents) => this.showObjectContents = showObjectContents;

    public override bool canDisplay(Content content) => content.isObject() && content.getNaked() != null;

    public override int canOpen(Content content)
    {
      NakedObject nakedObject = ((ObjectContent) content).getObject();
      NakedObjectField[] visibleFields = nakedObject.getVisibleFields();
      for (int index = 0; index < visibleFields.Length; ++index)
      {
        if (visibleFields[index].isCollection() || this.showObjectContents && visibleFields[index].isObject() && !nakedObject.getSpecification().isLookup())
          return 1;
      }
      return 2;
    }

    [JavaFlags(4)]
    public override View createNodeView(Content content, ViewAxis axis) => (View) new LeafNodeView(content, (ViewSpecification) this, axis);

    public override string getName() => "Object tree node - closed";
  }
}
