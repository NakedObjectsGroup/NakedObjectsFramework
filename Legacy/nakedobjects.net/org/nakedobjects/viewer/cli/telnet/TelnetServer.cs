// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.telnet.TelnetServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.controller;
using System.ComponentModel;

namespace org.nakedobjects.viewer.cli.telnet
{
  public class TelnetServer
  {
    private static readonly org.apache.log4j.Logger LOG;
    private ServerSocket socket;
    private readonly NakedClass[] classes;

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
      new CommandLineController().setResources(classes);
      TelnetServer telnetServer = new TelnetServer(classes);
    }

    public TelnetServer(NakedClass[] classes) => this.classes = classes;

    public virtual void run()
    {
      try
      {
        this.socket = new ServerSocket(2323);
        if (TelnetServer.LOG.isInfoEnabled())
          TelnetServer.LOG.info((object) new StringBuffer().append("Server started on ").append((object) this.socket).ToString());
        while (true)
        {
          Socket connection = this.socket.accept();
          CommandLineController commandLineController = new CommandLineController();
          commandLineController.setResources(this.classes);
          commandLineController.setInput((Input) new TelnetInput(connection));
          commandLineController.setView((View) new TelnetView(connection));
          commandLineController.init();
        }
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException("Failed to start up server", (Throwable) ex);
      }
    }

    public virtual void shutdown()
    {
      if (this.socket == null)
        return;
      try
      {
        this.socket.close();
      }
      catch (IOException ex)
      {
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TelnetServer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TelnetServer telnetServer = this;
      ObjectImpl.clone((object) telnetServer);
      return ((object) telnetServer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
