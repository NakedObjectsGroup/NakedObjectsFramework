// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.AddToFieldAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.cli.field
{
  public class AddToFieldAgent : AbstractFieldAgent
  {
    public AddToFieldAgent(string name, NakedObject @object, NakedObjectField field)
      : base(name, @object, field)
    {
    }

    public override string getName() => "Add To";

    public override string getPrompt() => new StringBuffer().append(this.name).append(" (add to)").ToString();

    [JavaFlags(0)]
    public virtual void addReferenceToCollection(Context context, View view, NakedObject associate)
    {
      OneToManyAssociation field = (OneToManyAssociation) this.field;
      Consent add = field.validToAdd(this.@object, associate);
      if (add.isVetoed())
      {
        view.error(new StringBuffer().append("Can't add reference: ").append(add.getReason()).ToString());
      }
      else
      {
        field.addElement(this.@object, associate);
        context.removeAgent();
      }
    }

    public override bool isValueEntry() => false;
  }
}
