// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ClearOneToOneAssociationOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ClearOneToOneAssociationOption : AbstractUserAction
  {
    public ClearOneToOneAssociationOption()
      : base("Clear association")
    {
    }

    public override Consent disabled(View view) => ((OneToOneField) view.getContent()).canClear();

    public override void execute(Workspace frame, View view, Location at)
    {
      ((OneToOneField) view.getContent()).clear();
      view.getParent().invalidateContent();
    }
  }
}
