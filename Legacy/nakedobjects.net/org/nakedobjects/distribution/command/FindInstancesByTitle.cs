// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.FindInstancesByTitle
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.distribution.command
{
  public class FindInstancesByTitle : AbstractRequest
  {
    private readonly string name;
    private readonly string title;
    private readonly bool includeSubclasses;

    public FindInstancesByTitle(Session session, TitleCriteria criteria)
      : base(session)
    {
      this.name = criteria.getSpecification().getFullName();
      this.title = criteria.getRequiredTitle();
      this.includeSubclasses = criteria.includeSubclasses();
    }

    public override void execute(Distribution distribution)
    {
      TitleCriteria titleCriteria = new TitleCriteria(NakedObjects.getSpecificationLoader().loadSpecification(this.name), this.title, this.includeSubclasses);
      this.setResponse((object) distribution.findInstances(this.session, (InstancesCriteria) titleCriteria));
    }

    public virtual ObjectData[] getInstances() => (ObjectData[]) this.response;
  }
}
