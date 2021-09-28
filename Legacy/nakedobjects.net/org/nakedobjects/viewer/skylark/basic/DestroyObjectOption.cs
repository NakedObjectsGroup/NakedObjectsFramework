// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.DestroyObjectOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class DestroyObjectOption : AbstractUserAction
  {
    public DestroyObjectOption()
      : base("Destroy Object", UserAction.EXPLORATION)
    {
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      NakedObject @object = ((ObjectContent) view.getContent()).getObject();
      NakedObjectPersistor objectPersistor = NakedObjects.getObjectPersistor();
      objectPersistor.startTransaction();
      objectPersistor.destroyObject(@object);
      objectPersistor.endTransaction();
      workspace.removeViewsFor(@object);
    }
  }
}
