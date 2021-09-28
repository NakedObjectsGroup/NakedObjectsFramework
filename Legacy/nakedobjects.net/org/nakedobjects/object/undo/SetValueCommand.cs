// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.undo.SetValueCommand
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
  public class SetValueCommand : Command
  {
    private readonly string description;
    private readonly OneToOneAssociation value;
    private NakedObject @object;
    private string oldValue;

    public SetValueCommand(NakedObject @object, OneToOneAssociation value)
    {
      this.@object = @object;
      this.value = value;
      this.description = new StringBuffer().append("reset the value to ").append(this.oldValue).ToString();
    }

    public virtual string getDescription() => this.description;

    public virtual void undo() => NakedObjects.getObjectPersistor().saveChanges();

    public virtual void execute()
    {
    }

    public virtual string getName() => "entry";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SetValueCommand setValueCommand = this;
      ObjectImpl.clone((object) setValueCommand);
      return ((object) setValueCommand).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
