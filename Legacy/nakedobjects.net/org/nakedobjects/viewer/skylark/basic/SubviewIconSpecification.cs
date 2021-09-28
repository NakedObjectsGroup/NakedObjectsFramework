// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.SubviewIconSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class SubviewIconSpecification : IconSpecification
  {
    public override View createView(Content content, ViewAxis axis)
    {
      NakedObjectSpecification specification = content.getSpecification();
      IconView iconView = new IconView(content, (ViewSpecification) this, axis, Style.NORMAL);
      switch (content)
      {
        case ObjectParameter _ when ((ObjectParameter) content).getOptions() != null && ((ObjectParameter) content).getOptions().Length > 0:
          return (View) new ObjectBorder((View) new OptionBorder((View) iconView));
        case OneToOneField _:
        case ObjectParameter _ when specification.isLookup():
          return (View) new ObjectBorder((View) new LookupBorder((View) iconView));
        default:
          return (View) new ObjectBorder((View) iconView);
      }
    }
  }
}
