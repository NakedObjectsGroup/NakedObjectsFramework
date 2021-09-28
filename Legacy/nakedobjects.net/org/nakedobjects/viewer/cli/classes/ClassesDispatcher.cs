// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.classes.ClassesDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.cli.classes
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ClassesDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.getResourceAgent().list(view, (string[]) null);

    public virtual string getHelp() => "Lists the application's classes";

    public virtual string getNames() => "classes cc";

    public virtual bool isAvailable(Context context) => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClassesDispatcher classesDispatcher = this;
      ObjectImpl.clone((object) classesDispatcher);
      return ((object) classesDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
