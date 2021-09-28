// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.OpenObjectNodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class OpenObjectNodeSpecification : CompositeNodeSpecification
  {
    public OpenObjectNodeSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new ObjectFieldBuilder((SubviewSpec) this));

    public override bool canDisplay(Content content)
    {
      if (content.isObject() && content.getNaked() != null)
      {
        foreach (NakedObjectField visibleField in ((NakedObject) content.getNaked()).getVisibleFields())
        {
          if (visibleField.isCollection())
            return true;
        }
      }
      return false;
    }

    public override int canOpen(Content content) => 1;

    public override bool isOpen() => true;

    public override string getName() => "Object tree node - open";
  }
}
