// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.text.InstallCommandLineConsole
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.controller;
using org.nakedobjects.viewer.cli.debug;

namespace org.nakedobjects.viewer.cli.text
{
  public class InstallCommandLineConsole
  {
    public static void installClient()
    {
      NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (ApplicationContext)));
      TypedNakedCollection typedNakedCollection = NakedObjects.getObjectPersistor().allInstances(specification, true);
      Vector vector = typedNakedCollection.size() != 0 ? ((ApplicationContext) typedNakedCollection.elementAt(0).getObject()).getClasses() : throw new NakedObjectRuntimeException("No application context found");
      int length = vector.size();
      NakedClass[] classes = length >= 0 ? new NakedClass[length] : throw new NegativeArraySizeException();
      int num1 = 0;
      Enumeration enumeration = vector.elements();
      while (enumeration.hasMoreElements())
      {
        NakedClass[] nakedClassArray = classes;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        NakedClass nakedClass = (NakedClass) enumeration.nextElement();
        nakedClassArray[index] = nakedClass;
      }
      CommandLineController commandLineController = new CommandLineController();
      commandLineController.setResources(classes);
      try
      {
        PrintStream printer = new PrintStream((OutputStream) new FileOutputStream("commandline.log"));
        commandLineController.setInput((Input) new DebugInput(printer, (Input) new ConsoleInput()));
        commandLineController.setView((View) new DebugView(printer, (View) new ConsoleView()));
        commandLineController.init();
      }
      catch (IOException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InstallCommandLineConsole commandLineConsole = this;
      ObjectImpl.clone((object) commandLineConsole);
      return ((object) commandLineConsole).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
