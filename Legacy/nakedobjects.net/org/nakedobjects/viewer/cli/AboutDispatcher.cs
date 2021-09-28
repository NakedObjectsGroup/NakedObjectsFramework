// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.AboutDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class AboutDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (AboutNakedObjects.getApplicationName() != null)
        view.display(AboutNakedObjects.getApplicationName());
      if (AboutNakedObjects.getApplicationCopyrightNotice() != null)
        view.display(AboutNakedObjects.getApplicationCopyrightNotice());
      if (AboutNakedObjects.getApplicationVersion() != null)
        view.display(new StringBuffer().append("Version ").append(AboutNakedObjects.getApplicationVersion()).ToString());
      if (AboutNakedObjects.getApplicationVersion() != null)
        view.display(AboutNakedObjects.getFrameworkName());
      if (AboutNakedObjects.getFrameworkCopyrightNotice() != null)
        view.display(AboutNakedObjects.getFrameworkCopyrightNotice());
      if (AboutNakedObjects.getFrameworkVersion() != null)
        view.display(AboutNakedObjects.getFrameworkVersion());
      if (AboutNakedObjects.getFrameworkBuild() == null)
        return;
      view.display(new StringBuffer().append("Build ").append(AboutNakedObjects.getFrameworkBuild()).ToString());
    }

    public virtual string getHelp() => "Displays details about the Naked Objects Framework";

    public virtual string getNames() => "about version ver";

    public virtual bool isAvailable(Context context) => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AboutDispatcher aboutDispatcher = this;
      ObjectImpl.clone((object) aboutDispatcher);
      return ((object) aboutDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
