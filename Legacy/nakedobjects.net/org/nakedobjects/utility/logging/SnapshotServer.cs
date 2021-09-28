// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SnapshotServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using java.net;
using org.apache.log4j;
using org.nakedobjects.utility.configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.utility.logging
{
  public class SnapshotServer
  {
    private static readonly org.apache.log4j.Logger LOG;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      BasicConfigurator.configure();
      PropertiesConfiguration propertiesConfiguration = new PropertiesConfiguration((ConfigurationLoader) new PropertiesFileLoader("nakedobjects.properties", true));
      string str = "nakedobjects.snapshotserver.";
      int integer = propertiesConfiguration.getInteger(new StringBuffer().append(str).append("port").ToString(), 9289);
      string directoryPath = propertiesConfiguration.getString(new StringBuffer().append(str).append("directory").ToString(), ".");
      string baseFileName = propertiesConfiguration.getString(new StringBuffer().append(str).append("filename").ToString(), "log-snapshot-");
      string extension = propertiesConfiguration.getString(new StringBuffer().append(str).append("extension").ToString(), "txt");
      ServerSocket serverSocket;
      try
      {
        serverSocket = new ServerSocket(integer);
      }
      catch (IOException ex)
      {
        SnapshotServer.LOG.error((object) "failed to start server", (Throwable) ex);
        Utilities.cleanupAfterMainReturns();
        return;
      }
      while (true)
      {
        try
        {
          Socket socket = serverSocket.accept();
          if (SnapshotServer.LOG.isInfoEnabled())
            SnapshotServer.LOG.info((object) new StringBuffer().append("receiving log from ").append(socket.getInetAddress().getHostName()).ToString());
          BufferedReader bufferedReader = new BufferedReader((Reader) new InputStreamReader(socket.getInputStream(), "8859_1"));
          string message = bufferedReader.readLine();
          SnapshotWriter snapshotWriter = new SnapshotWriter(directoryPath, baseFileName, extension, message);
          string details;
          while ((details = bufferedReader.readLine()) != null)
            snapshotWriter.appendLog(details);
          socket.close();
          bufferedReader.close();
        }
        catch (IOException ex)
        {
          SnapshotServer.LOG.error((object) "failed to log", (Throwable) ex);
        }
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SnapshotServer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SnapshotServer snapshotServer = this;
      ObjectImpl.clone((object) snapshotServer);
      return ((object) snapshotServer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
