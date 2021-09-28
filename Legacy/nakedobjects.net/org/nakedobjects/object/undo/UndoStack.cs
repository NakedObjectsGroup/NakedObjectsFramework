// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.undo.UndoStack
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object.undo;

namespace org.nakedobjects.@object.undo
{
  public class UndoStack
  {
    private Vector commands;

    public virtual void add(Command command)
    {
      this.commands.addElement((object) command);
      command.execute();
    }

    public virtual void undoLastCommand()
    {
      Command command = (Command) this.commands.lastElement();
      command.undo();
      this.commands.removeElement((object) command);
    }

    public virtual string descriptionOfUndo() => ((Command) this.commands.lastElement()).getDescription();

    public virtual bool isEmpty() => this.commands.isEmpty();

    public virtual string getNameOfUndo() => ((Command) this.commands.lastElement()).getName();

    public UndoStack() => this.commands = new Vector();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      UndoStack undoStack = this;
      ObjectImpl.clone((object) undoStack);
      return ((object) undoStack).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
