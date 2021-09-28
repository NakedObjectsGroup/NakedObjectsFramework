// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.undo.AssociateCommand
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.undo;

namespace org.nakedobjects.@object.undo
{
  [JavaInterfaces("1;org/nakedobjects/object/undo/Command;")]
  public class AssociateCommand : Command
  {
    private readonly string description;
    private readonly OneToOneAssociation field;
    private readonly NakedObject @object;
    private readonly NakedObject associatedObject;
    private string name;

    public AssociateCommand(
      NakedObject @object,
      NakedObject associatedObject,
      OneToOneAssociation field)
    {
      this.description = new StringBuffer().append("Clear association of ").append(associatedObject.titleString()).ToString();
      this.name = new StringBuffer().append("associate ").append(associatedObject.titleString()).ToString();
      this.@object = @object;
      this.associatedObject = associatedObject;
      this.field = field;
    }

    public virtual string getDescription() => this.description;

    public virtual string getName() => this.name;

    public virtual void undo() => this.@object.clearAssociation((NakedObjectField) this.field, this.associatedObject);

    public virtual void execute() => this.@object.setAssociation((NakedObjectField) this.field, this.associatedObject);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AssociateCommand associateCommand = this;
      ObjectImpl.clone((object) associateCommand);
      return ((object) associateCommand).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
