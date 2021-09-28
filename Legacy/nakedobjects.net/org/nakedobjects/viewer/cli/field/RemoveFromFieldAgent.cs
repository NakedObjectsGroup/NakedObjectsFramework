// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.RemoveFromFieldAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.cli.field
{
  public class RemoveFromFieldAgent : AbstractFieldAgent
  {
    public RemoveFromFieldAgent(string name, NakedObject @object, NakedObjectField field)
      : base(name, @object, field)
    {
    }

    public override string getName() => "Remove From";

    public override string getPrompt() => new StringBuffer().append(this.name).append(" (remove from)").ToString();

    [JavaFlags(0)]
    public virtual void removeReferenceFromCollection(
      Context context,
      View view,
      NakedObject associate)
    {
      OneToManyAssociation field = (OneToManyAssociation) this.field;
      Consent remove = field.validToRemove(this.@object, associate);
      if (remove.isVetoed())
      {
        view.error(new StringBuffer().append("Can't remove reference: ").append(remove.getReason()).ToString());
      }
      else
      {
        field.removeElement(this.@object, associate);
        context.removeAgent();
      }
    }

    public override bool isValueEntry() => false;
  }
}
