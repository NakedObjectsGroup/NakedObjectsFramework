// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.FindFirstOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class FindFirstOption : AbstractUserAction
  {
    public FindFirstOption()
      : base("Find First Matching Object")
    {
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      NakedCollection instances = (NakedCollection) NakedObjects.getObjectPersistor().findInstances((InstancesCriteria) new PatternObjectCriteria(((ObjectContent) view.getContent()).getObject(), true));
      Naked @object = instances.size() < 1 ? (Naked) instances : (Naked) instances.elements().nextElement();
      View subviewFor = workspace.createSubviewFor(@object, false);
      subviewFor.setLocation(at);
      workspace.addView(subviewFor);
    }

    public override string getDescription(View view) => "Find just the first matching object";
  }
}
